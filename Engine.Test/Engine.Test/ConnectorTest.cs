using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Engine;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine.Test
{
    public class ConnectorTest
    {
        private readonly ITestOutputHelper _output;

        public ConnectorTest(ITestOutputHelper output){
            _output = output;
        }

        [Fact]
        public void ConnectorCreation()
        {
            var filters = Builders<BsonDocument>.Filter.Empty;
            var documents = Connector.GetDocumentsCollection().Find(filters).ToList();
            var tokens = Connector.GetTokensCollection().Find(filters).ToList();

            Assert.IsType<List<BsonDocument>>(tokens);
            Assert.IsType<List<BsonDocument>>(documents);
            Assert.Empty(tokens);
            Assert.Empty(documents);
        }
    }
}