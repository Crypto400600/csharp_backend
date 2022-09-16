using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    /// <summary>
    /// This provides an interface between the functions and the database
    /// </summary>
    public static class Connector {
        private static MongoClient _client;

        private static string _database = "404Db";
        private static string _mongoUrl =
            $"mongodb://127.0.0.1:27017/{_database}?retryWrites=true&w=majority";
        
        /// <summary>
        /// This generates a client and stores it on the connector class
        /// </summary>
        public static void GenerateDb() {
            _client = new MongoClient(_mongoUrl);
        }

        /// <summary>
        /// Allows the user to set a mongo db url
        /// </summary>
        /// <param name="url">The target cluster url</param>
        public static void SetMongoUri(string url) {
            _mongoUrl = $"{url}/{_database}?retryWrites=true&w=majority";
        } 

        /// <summary>
        /// Sets database to test db 
        /// </summary>
        public static void SetTestMode()
        {
            _database = "404Db_Test";
            _mongoUrl = $"mongodb://127.0.0.1:27017/{_database}?retryWrites=true&w=majority";
        }

        /// <summary>
        /// Returns the database to be used by the collection classes
        /// </summary>
        private static IMongoDatabase GetDb() {
            if (_client == null) {
                _client = new MongoClient(_mongoUrl);
            }

            return _client.GetDatabase(_database);
        }

        /// <summary>
        /// Helper function to get collection
        /// </summary>
        /// <returns>Documents collection</returns>
        public static IMongoCollection<BsonDocument> GetDocumentsCollection() {
            var db = GetDb();
            return db.GetCollection<BsonDocument>("documents");
        }
        
        /// <summary>
        /// Helper function to get collection
        /// </summary>
        /// <returns>Tokens collection</returns>
        public static IMongoCollection<BsonDocument> GetTokensCollection() {
            var db = GetDb();
            return db.GetCollection<BsonDocument>("tokens");
        }
        
        /// <summary>
        /// Helper function to get collection
        /// </summary>
        /// <returns>Saved queries collection</returns>
        public static IMongoCollection<BsonDocument> GetSavedQueriesCollection() {
            var db = GetDb();
            return db.GetCollection<BsonDocument>("savedQueries");
        }
    }
}