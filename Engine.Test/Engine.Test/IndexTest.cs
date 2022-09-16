using System.IO;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;
using Engine;

namespace Engine.Test
{
    public class QuerierTest
    {
        private static readonly string _name = "Report for testing";
        private static readonly string _url = "https://res.cloudinary.com/dpgdjfckl/raw/upload/v1629749999/fat_dydkjd.txt";

        private readonly ITestOutputHelper _output;
        FilterDefinition<BsonDocument> _filter = Builders<BsonDocument>.Filter.Eq("name", _name);

        public QuerierTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private void SetupDb()
        {

            Connector.SetTestMode();
            var dColl = Connector.GetDocumentsCollection();
            dColl.DeleteMany(new BsonDocument());
            var docs = dColl.CountDocuments(new BsonDocument());

            _output.WriteLine($"Before setup, no of docs: {docs}");

            DbDocument.IndexDocument(_name, _url);
            Thread.Sleep(10000);
            
            docs = dColl.CountDocuments(new BsonDocument());
            _output.WriteLine($"After setup, no of docs: {docs}");
            var tColl = Connector.GetTokensCollection();
            var tokens = tColl.CountDocuments(new BsonDocument()); 
            _output.WriteLine($"After setup, no of tokens: {tokens}");
        }

        private void TearDown()
        {
            var dColl = Connector.GetDocumentsCollection();
            var docs = dColl.CountDocuments(new BsonDocument());
            _output.WriteLine($"Before teardown, no of docs: {docs}");
            dColl.DeleteMany(new BsonDocument());
            docs = dColl.CountDocuments(_filter);
            
            
            _output.WriteLine($"Ended, no of docs: {docs}");

            var tColl = Connector.GetTokensCollection();
            var tokens = tColl.CountDocuments(new BsonDocument());
            _output.WriteLine($"Before teardown, no of tokens: {tokens}");
            tColl.DeleteMany(new BsonDocument());
            tokens = tColl.CountDocuments(new BsonDocument());
            _output.WriteLine($"Ended, no of tokens: {tokens}");
        }

        [Theory]
        [InlineData("dog")]
        [InlineData("dog fat")]
        public async void SearchTest(string query)
        {
            SetupDb();
            _output.WriteLine("Started search");
            var querier = new Querier();
            var result = await querier.Search(query);
            var doc = Connector.GetDocumentsCollection().AsQueryable().ToList()[0];

            Assert.Single(result);

            if (result.Length > 0) {
                Assert.Equal(doc["_id"].ToString(), result[0].DocumentId);
            }
            TearDown();
        }
    }
}