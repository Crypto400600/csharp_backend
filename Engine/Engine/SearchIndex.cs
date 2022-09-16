using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    /// <summary>
    /// SearchIndex inherits the Index class. It is used for the Querier class.
    /// It instantiates itself with a list of words from the database.
    /// </summary>
    public class SearchIndex : Index {
        /// <summary>
        /// Makes a new instance of <see cref="SearchIndex"/>
        /// </summary>
        /// <param name="words">These are the words that are loaded from the db</param>
        public SearchIndex(string[] words) {
            var filter = Builders<BsonDocument>.Filter.In("word", words);
            var tokensCollection = Connector.GetTokensCollection();
            var dbTokens = tokensCollection.Find(filter).ToList();
            
            foreach (var token in dbTokens) {
                var word = token["word"].ToString();
                var documents = token["documents"].AsBsonArray.ToArray();
                var frequency = token["frequency"].ToInt32();
                Tokens[word] = new Token(word, documents, frequency);
            }
        }
    }
}