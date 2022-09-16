using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Engine {
    /// <summary>
    /// This is a representation of a token in the index. It stores the word, the documents under a token and the number of times that token exists.
    /// </summary>
    public class Token {
        private readonly string _word;
        public readonly List<TokenItem> Documents = new List<TokenItem>();
        private string _currentDocumentId;
        public int Frequency;

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class 
        /// </summary>
        /// <param name="word">Sets the word that a token is pointing to</param>
        public Token(string word) {
            _word = word;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class 
        /// </summary>
        /// <param name="word">Sets the word that a token is pointing to</param>
        /// <param name="initialDocuments">These are the initial documents a token contains. It is useful when loading a token from the database for querying</param>
        /// <param name="frequency">This is the count of the number of times a token appears across the documents. Useful for querying</param>
        public Token(string word, BsonValue[] initialDocuments, int frequency) {
            _word = word;
            Frequency = frequency;

            foreach (var document in initialDocuments) {
                var documentPositions = BsonSerializer.Deserialize<List<int>>(document["positions"].ToJson());
                var tokenItem = new TokenItem(documentPositions, document["fileId"].ToString(),
                    document["fileIndex"].ToInt32());
                Documents.Add(tokenItem);
            }
        }

        /// <summary>
        /// This adds a new item to the the list of tokens.
        /// It checks if the document has already been included in our list of documents if it has it adds the item else it deletes it
        /// </summary>
        /// <param name="dbDocument">An instance of the <see cref="DbDocument"/> class</param>
        /// <param name="position">The position of the word within the document</param>
        public void AddItem(DbDocument dbDocument, int position) {
            if (_currentDocumentId == dbDocument.DocumentId) {
                var currentDocument = Documents[Documents.Count - 1];
                currentDocument.AddPosition(position);
            }
            else {
                Frequency++;
                _currentDocumentId = dbDocument.DocumentId;
                var newDocument = new TokenItem(position, dbDocument.DocumentId, dbDocument.Position);
                Documents.Add(newDocument);
            }
        }

        /// <summary>
        /// This is used to convert the db document instances to a BSON document which is useful for saving to the db
        /// </summary>
        /// <returns>Returns <see cref="BsonArray"/> of documents</returns>
        private BsonArray GetBsonDocuments() {
            var documentsArray = new BsonArray();
            foreach (var document in Documents) {
                var item = new BsonDocument {
                    {"fileId", document.DocumentId},
                    {"fileIndex", document.DocumentPosition},
                    {"positions", document.GetBsonPositions()},
                };

                documentsArray.Add(item);
            }

            return documentsArray;
        }
        
        /// <summary>
        /// This saves the current token to the database to the database
        /// </summary>
        public async Task SaveSelfToDb() {
            var getFilter = Builders<BsonDocument>.Filter.Eq("word", _word);
            var tokensCollection = Connector.GetTokensCollection();
            
            //Fetch previous instance of token
            var prevToken = tokensCollection.Find(getFilter).FirstOrDefault();
            
            if (prevToken != null) {
                //If it already exists update relevant fields
                var taskBson = prevToken.ToBsonDocument();
                var previousDocuments = BsonSerializer.Deserialize<BsonArray>(taskBson["documents"].ToJson());
                var newDocuments = previousDocuments.AddRange(GetBsonDocuments());
                
                var update = Builders<BsonDocument>.Update.Set("documents", newDocuments).Set("frequency", prevToken["frequency"].ToInt32() + Frequency);

                await tokensCollection.UpdateOneAsync(getFilter, update);
            }
            else {
                //Create a new token
                var token = new BsonDocument {
                    {"word", _word},
                    {"documents", GetBsonDocuments()},
                    {"frequency", Frequency}
                };
                
                await tokensCollection.InsertOneAsync(token);
            }
        }
    }
}