using System.Collections.Generic;
using MongoDB.Bson;

namespace Engine {
    /// <summary>
    /// This class represents a document under a token
    /// </summary>
    public class TokenItem {
        /// <summary>
        /// Positions contains the particular postion a token exists in a document
        /// </summary>
        public readonly List<int> Positions = new List<int>();
        /// <summary>
        /// DocumentId contains the ID of the particular document 
        /// </summary>
        public readonly string DocumentId;
        /// <summary>
        /// DocumentPosition contains the positions where the token is found
        /// </summary>
        public readonly long DocumentPosition;
        
        /// <summary>
        /// Creates a new instance of <see cref="TokenItem"/>
        /// </summary>
        /// <param name="position">This is the first position of the word in a document</param>
        /// <param name="documentId">This is the id of the document</param>
        /// <param name="documentPosition">This is the position of the document in the documents repository</param>
        public TokenItem(int position, string documentId, long documentPosition) {
            Positions.Add(position);
            DocumentId = documentId;
            DocumentPosition = documentPosition;
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="TokenItem"/>
        /// </summary>
        /// <param name="positions">These are the positions of the item in the document</param>
        /// <param name="documentId">This is the id of the document</param>
        /// <param name="documentPosition">This is the position of the document in the documents repository</param>
        public TokenItem(List<int> positions, string documentId, long documentPosition) {
            Positions = positions;
            DocumentId = documentId;
            DocumentPosition = documentPosition;
        }
        
        /// <summary>
        /// Adds a new position
        /// </summary>
        /// <param name="position">Position of word in document</param>
        public void AddPosition(int position) {
            Positions.Add(position);
        }

        /// <summary>
        /// Converts positions to bson document
        /// </summary>
        /// <returns>Returns <see cref="BsonArray"/> of positions</returns>
        public BsonArray GetBsonPositions() {
            var positionsArray = new BsonArray();

            foreach (var position in Positions) {
                positionsArray.Add(position);
            }

            return positionsArray;
        }
    }
}