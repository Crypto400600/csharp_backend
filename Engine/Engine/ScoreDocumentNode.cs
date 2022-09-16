using Priority_Queue;

namespace Engine {
    /// <summary>
    /// ScoreDocumentNode class records the score of a particular document in a queue
    /// </summary>
    public class ScoreDocumentNode : StablePriorityQueueNode {
        public readonly string DocumentId;

        public ScoreDocumentNode(string documentId) {
            this.DocumentId = documentId;
        }
    }
}