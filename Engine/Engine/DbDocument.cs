using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    /// <summary>
    /// This extends the base document class to add extra field poition to describe the position of the document in the list of documents
    /// Also contains helper functions for indexing documents and saving documents to the database 
    /// </summary>
    public class DbDocument : BaseDocument {
        public readonly long Position;
        private static readonly Queue<BaseDocument> ToBeIndexed = new Queue<BaseDocument>();

        private static readonly Semaphore SaveDocSemaphore = new Semaphore(1, 1);
        /// <summary>
        /// Creates a new instance of <see cref="DbDocument"/>
        /// </summary>
        /// <param name="position">Position of document in list of documents</param>
        /// <param name="documentId">Document id</param>
        /// <param name="url">Url of document</param>
        /// <param name="name">Name of document</param>
        private DbDocument(long position, string documentId, string url, string name) : base(name, url, documentId) {
            Position = position;
        }

        /// <summary>
        /// This function adds the document to the indexing queue and saves the document to the database
        /// </summary>
        /// <param name="url">Url of document</param>
        /// <param name="name">Name of document</param>
        public static void IndexDocument(string name, string url) {
            ToBeIndexed.Enqueue(new BaseDocument(name, url));
            SaveToDb();
        }

        /// <summary>
        /// This saves the document to the database
        /// </summary>
        private static async void SaveToDb() {
            SaveDocSemaphore.WaitOne();
            var currentDocument = ToBeIndexed.Dequeue();
            if (currentDocument == null) return;
            var name = currentDocument.Name;
            var url = currentDocument.Url;

            var collection = Connector.GetDocumentsCollection();
            var documentIndex = await collection.CountDocumentsAsync(new BsonDocument());
            var bsonDocument = new BsonDocument {
                {"name", name},
                {"url", url},
                {"position", documentIndex}
            };

            await collection.InsertOneAsync(bsonDocument);

            var getDocFilter = Builders<BsonDocument>.Filter.Eq("name", name);
            var createdDoc = collection.Find(getDocFilter).ToList().Last();

            Console.WriteLine($"Document {name} saved to database");

            SaveDocSemaphore.Release();

            //Calls the indexer function to add the document to the queue of items to be indexed
            Indexer.TryIndex(new DbDocument(documentIndex, createdDoc["_id"].ToString(), url, createdDoc["name"].ToString()));
        }
    }
}