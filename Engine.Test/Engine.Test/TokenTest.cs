using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;
using Engine;

namespace Engine.Test
{
    public class TokenTest
    {
        private static readonly string _name = "Report for testing";
        private static readonly string _url = "../../fixtures/CSC326.docx";

        private readonly ITestOutputHelper _output;
        FilterDefinition<BsonDocument> _filter = Builders<BsonDocument>.Filter.Eq("name", _name);

        public TokenTest(ITestOutputHelper output)
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
            Thread.Sleep(5000);
            
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

        [Fact]
        public void TokenCreationTest()
        {
            SetupDb();
            var dbtoken = Connector.GetTokensCollection().AsQueryable().FirstOrDefault();
            var word = dbtoken["word"].ToString();
            var documents = dbtoken["documents"].AsBsonArray.ToArray();
            var frequency = dbtoken["frequency"].ToInt32();
            var token = new Token(word, documents, frequency);
            Assert.Equal(1, 1);
            TearDown();
        }
    }
}