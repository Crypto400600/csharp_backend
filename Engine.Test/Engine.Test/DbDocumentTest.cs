using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;
using TikaOnDotNet.TextExtraction;

namespace Engine.Test
{
    public class DbDocumentTest
    {
        private static readonly string _name = "Report for testing";
        private readonly string _url = "../../fixtures/CSC326.docx";
        FilterDefinition<BsonDocument> _filter = Builders<BsonDocument>.Filter.Eq("name", _name);
        
        private readonly ITestOutputHelper _output;

        public DbDocumentTest(ITestOutputHelper output){
            _output = output;
        }
        
        public void SetupDb()
        {
            Connector.SetTestMode();
            DbDocument.IndexDocument(_name, _url);
            Thread.Sleep(5000);
        }
        public void TearDown()
        {
            var dColl = Connector.GetDocumentsCollection();
            var docs = dColl.AsQueryable().ToList();
            _output.WriteLine($"Before teardown, no of docs: {docs.Count}");
            dColl.DeleteMany(new BsonDocument());
            docs = dColl.Find(_filter).ToList();
            _output.WriteLine($"Ended, no of docs: {docs.Count}");
        }
        [Fact]
        public void IndexDocument()
        {
            SetupDb();
            var doc = Connector.GetDocumentsCollection().Find(_filter).SingleOrDefault();
            _output.WriteLine($"Here's your doc: {doc}");
            Assert.NotNull(doc);
            TearDown();
        }
    }
}