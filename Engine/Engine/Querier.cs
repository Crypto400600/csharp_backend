using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using java.lang;
using java.util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Priority_Queue;
using Exception = System.Exception;
using Math = System.Math;

namespace Engine {
    /// <summary>
    /// Querier class is used for querying the database with built index for specific words 
    /// </summary>
    public class Querier {
        /// <summary>
        /// _pointersQueue is used to keep track of the particular items we are used for the linear mapping algorithm 
        /// </summary>
        private StablePriorityQueue<TokenPointer> _pointersQueue;
        /// <summary>
        /// _scoresQueue used for storing the score of each document while looping through
        /// </summary>
        private SimplePriorityQueue<ScoreDocumentNode> _scoresQueue;
        /// <summary>
        /// _documentsCount stores the number of documents to be used when obtaining the TFIDF of each document
        /// </summary>
        private long _documentsCount;
        /// <summary>
        /// _resultDocuments is an array of the result documents
        /// </summary>
        private BaseDocument [] _resultDocuments;
        /// <summary>
        /// arrangedPointers returns the pointers the way they are in the query. This is used in the consecutive word algorithm.
        /// </summary>
        private List<TokenPointer> arrangedPointers = new List<TokenPointer>();

        /// <summary>
        /// This is used to get past queries for autocomplete suggestions
        /// </summary>
        /// <param name="query">The current user input</param>
        /// <returns>Returns string of suggestions</returns>
        public static string[] GetPastQueries(string query) {
            var getQueriesFilter = Builders<BsonDocument>.Filter.Regex("query", new BsonRegularExpression($"^{query}"));
            var savedQueriesCollection = Connector.GetSavedQueriesCollection();
            var savedQueries = savedQueriesCollection.Find(getQueriesFilter).Limit(10).SortByDescending(x => x["count"]).ToList();
            var queries = new string[savedQueries.Count];

            for (int i = 0; i < savedQueries.Count; i++) {
                queries[i] = savedQueries[i].ToBsonDocument()["query"].ToString();
            }

            return queries;
        }
        
        /// <summary>
        /// Searches through the index and performs all necessary algorithms to get documents that match user query
        /// </summary>
        /// <param name="query">User query</param>
        /// <returns>Array of <see cref="BaseDocument"/></returns>
        public async Task<BaseDocument []> Search(string query) {
            try {
                var cleanedWords = Utils.CleanAndExtractWords(query);

                DateTime start = DateTime.Now;
                Index searchIndex = new SearchIndex(cleanedWords);
                Console.WriteLine($"Load words {(DateTime.Now - start).TotalMilliseconds}");
                
                if (searchIndex.Tokens.Count != 0) {
                    _documentsCount = await Connector.GetDocumentsCollection().CountDocumentsAsync(new BsonDocument());
                    start = DateTime.Now;
                    GeneratePointers(searchIndex, cleanedWords);
                    LinearMap();
                    Console.WriteLine($"Generate scores {(DateTime.Now - start).TotalMilliseconds}");
                    start = DateTime.Now;
                    await FetchDocumentDetails();
                    Console.WriteLine($"Fetch documents {(DateTime.Now - start).TotalMilliseconds}");

                    Task.Run(() => SaveQueryToDb(query));
                    
                    return _resultDocuments;
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace, e.Message); //TODO: Log to file
            }

            _resultDocuments = new BaseDocument[0];
            return _resultDocuments;
        }

        /// <summary>
        /// This saves a user query to the database in order for it to be used for autocomplete in the future
        /// </summary>
        /// <param name="query"></param>
        private async void SaveQueryToDb(string query) {
            var getFilter = Builders<BsonDocument>.Filter.Eq("query", query);
            var savedQueriesCollection = Connector.GetSavedQueriesCollection();
            var prevQuery = savedQueriesCollection.Find(getFilter).FirstOrDefault();
            
            if (prevQuery != null) {
                //If the query exists previously increase the count
                var queryBson = prevQuery.ToBsonDocument();
                int count = queryBson["count"].ToInt32();
                var update = Builders<BsonDocument>.Update.Set("count", count + 1);
                await savedQueriesCollection.UpdateOneAsync(getFilter, update);
            }
            else {
                //Else create a new query
                var token = new BsonDocument {
                    {"query", query},
                    {"count", 1},
                };
                
                await savedQueriesCollection.InsertOneAsync(token);
            }
            
            Console.WriteLine($"{query} saved to saved queries collection");
        }
        
        
        /// <summary>
        /// Fetched the scored document details from the database
        /// </summary>
        private async Task FetchDocumentDetails() {
            string[] resultIds = new string[_scoresQueue.Count];
            _resultDocuments = new BaseDocument[_scoresQueue.Count];
            
            List<FilterDefinition<BsonDocument>> filtersList = new List<FilterDefinition<BsonDocument>>();
            
            while (_scoresQueue.Count > 0) {
                var currentDoc = _scoresQueue.Dequeue();
                resultIds[_scoresQueue.Count] = currentDoc.DocumentId;
                _resultDocuments[_scoresQueue.Count] = new BaseDocument(currentDoc.DocumentId);
                
                var getFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(currentDoc.DocumentId));
                filtersList.Add(getFilter);
            }

            var filter = Builders<BsonDocument>.Filter.Or(filtersList);
            var documents = await Connector.GetDocumentsCollection().FindAsync(filter);
            var bson = documents.ToList();

            Dictionary<string, BsonDocument> docIdToValueMapping = new Dictionary<string, BsonDocument>();

            foreach (var doc in bson) {
                var currentId = doc["_id"].ToString();
                docIdToValueMapping[currentId] = doc;
            }

            for (int i = 0; i < resultIds.Length; i++) {
                var id = resultIds[i];
                var values = docIdToValueMapping[id];
                _resultDocuments[i].Name = values["name"].ToString();
                _resultDocuments[i].Url = values["url"].ToString();
            }
        }

        /// <summary>
        /// Generates the pointers user for linear mapping
        /// </summary>
        /// <param name="targetIndex">An instance of <see cref="Index"/></param>
        /// <param name="words">The words we want to generate pointers for</param>
        private void GeneratePointers(Index targetIndex, string [] words) {
            _pointersQueue = new StablePriorityQueue<TokenPointer>(words.Length);
            _scoresQueue = new SimplePriorityQueue<ScoreDocumentNode>();
            
            foreach (var word in words) {
                if (targetIndex.Tokens.ContainsKey(word)) {
                    var pointer = new TokenPointer(targetIndex.Tokens[word]);
                    _pointersQueue.Enqueue(pointer, pointer.Target.DocumentPosition);
                    arrangedPointers.Add(pointer);
                }
                else {
                    arrangedPointers.Add(new TokenPointer());
                }
            }
        }

        
        /// <summary>
        /// This function is used to map through the pointers and generate scores
        /// </summary>
        private void LinearMap() {
            List<TokenPointer> smallestPointers = ExtractSmallest();
            ScorePointers(smallestPointers);
            foreach (var pointer in smallestPointers) {
                if (pointer.MoveForward()) {
                    _pointersQueue.Enqueue(pointer, pointer.Target.DocumentPosition);
                };
            }

            if (_pointersQueue.Count > 0) {
                LinearMap();
            }
        }

        /// <summary>
        /// Gives tf idf scores for each word per document and also calls consecutive words function
        /// </summary>
        /// <param name="pointers">The pointers to be scored</param>
        private void ScorePointers(List<TokenPointer> pointers) {
            double documentScore = 0;

            foreach (var tokenPointer in pointers) {
                var target = tokenPointer.Target;
                double termFrequency = target.Positions.Count;
                double documentsWithTerm = tokenPointer.Token.Frequency;

                double tfIdf = termFrequency * Math.Log(_documentsCount / documentsWithTerm, 2);
                
                documentScore += tfIdf;
            }

            double d = ScoreConsecutiveWords(pointers[0].Target.DocumentId);

            documentScore += d;
            
            string targetDocumentId = pointers[0].Target.DocumentId;
            _scoresQueue.Enqueue(new ScoreDocumentNode(targetDocumentId), (float) documentScore);
        }

        /// <summary>
        /// This scores the phrases in a particular document
        /// </summary>
        /// <param name="targetDocumentId">The current document we're scoring</param>
        /// <returns>The consecutive score of the document</returns>
        private double ScoreConsecutiveWords(string targetDocumentId) {
            //Total consecutive count
            int consecutiveCount = 0;
            List<PositionPointer> positionPointers = new List<PositionPointer>();
            
            //Fills up position pointers array
            foreach (var pointer in arrangedPointers) {
                if (pointer.EmptyPointer) {
                    positionPointers.Add(new PositionPointer());   
                }
                else {
                    //Add position arrays to pointers array
                    positionPointers.Add(new PositionPointer(pointer.Target.Positions, pointer.Target.DocumentId == targetDocumentId));   
                }
            }

            while (true) {
                bool hasInvalidPointer = false;
                int currentRunOn = 0;
                
                //This is to store the first point at which our run on failed so that we can increase everything behind that
                int firstBreakPoint = 0;
                bool hasSetFirstBreakPoint = false;
                
                for (int i = 0; i < positionPointers.Count; i++) {
                    var positionPointer = positionPointers[i];

                    if (positionPointer.EmptyPointer || !positionPointer.IsValid) {
                        if (currentRunOn > 1) {
                            consecutiveCount += currentRunOn;
                            currentRunOn = 1;
                        }
                        
                        if(!hasSetFirstBreakPoint) {
                            firstBreakPoint = Math.Max(i - 1, 0);
                            hasSetFirstBreakPoint = true;
                        }
                        continue;
                    }
                    
                    //If we have invalid just break out of loop to save resources
                    if (positionPointer.CurrentPosition == -1) {
                        hasInvalidPointer = positionPointer.CurrentPosition == -1;
                        break;
                    }
                    
                    if (positionPointer.CurrentPosition != -1) {
                        //If first item start the run
                        if (i == 0) {
                            currentRunOn = 1;
                        }
                        else {
                            //Else check if previous item is directly before current item
                            var prevPointer = positionPointers[i - 1];
                            if (positionPointer.CurrentPosition - 1 == prevPointer.CurrentPosition) {
                                currentRunOn++;
                            }
                            //Else if current item position is behind previous item
                            //(which it shouldn't be because of the order of the words should match the query)
                            //Then move the current item forward until it's ahead of the previous item
                            else if (positionPointer.CurrentPosition < prevPointer.CurrentPosition) {
                                positionPointer.MoveForwardUntilGreaterThanOrEqualTo(prevPointer.CurrentPosition);
                                if (positionPointer.CurrentPosition - 1 == prevPointer.CurrentPosition) {
                                    currentRunOn++;
                                }    
                            }
                            else {
                                //If we have a runon add it to our consecutive count
                                if (currentRunOn > 1) {
                                    consecutiveCount += currentRunOn;
                                    currentRunOn = 1;
                                }
                                
                                //If we've not set our first breakpoint set it
                                if(!hasSetFirstBreakPoint) {
                                    firstBreakPoint = i - 1;
                                    hasSetFirstBreakPoint = true;
                                }
                            }
                        }
                    }
                }

                //If there's a residual currentRunon i.e there was a runon towards the end of the string just add it to consecutive count
                if (currentRunOn > 1) {
                    consecutiveCount += currentRunOn;
                }

                //If we haven't set our first breakpoint that means we never broke so move both token positions forward
                if (!hasSetFirstBreakPoint) {
                    firstBreakPoint = positionPointers.Count - 1;
                }

                //Moved forward all the words that were before the first breakpoint
                for (int i = 0; i <= firstBreakPoint; i++) {
                    var positionPointer = positionPointers[i];
                    if (!positionPointer.EmptyPointer && positionPointer.IsValid) {
                        positionPointer.MoveForward();
                    }
                }

                if (hasInvalidPointer) {
                    break;
                }
            }

            return consecutiveCount;
        }

        /// <summary>
        /// Used for extracting all the pointers for the current target document.
        /// Since we're using linear mapping this will get the lowest position document
        /// </summary>
        /// <returns>Returns pointers for current document</returns>
        private List<TokenPointer> ExtractSmallest() {
            var smallestPointers = new List<TokenPointer>();

            while (true) {
                smallestPointers.Add(_pointersQueue.Dequeue());

                if (_pointersQueue.Count == 0) {
                    break;
                }

                var leastPointer = smallestPointers[0];
                var leastPointerInQueue = _pointersQueue.First;

                if (leastPointer.Target.DocumentPosition != leastPointerInQueue.Target.DocumentPosition) {
                    break;
                }
            }
            
            return smallestPointers;
        }
    }
}