namespace Engine {
    /// <summary>
    /// BaseDocument class is used to represent a document. 
    /// </summary>
    public class BaseDocument {
        public string Name;
        public string Url;
        public readonly string DocumentId;

        /// <summary>
        /// Constructor for the BaseDocument class
        /// </summary>
        /// <param name="documentId">UUID of the document in the database</param>
        public BaseDocument(string documentId) {
            DocumentId = documentId;
        }

        /// <summary>
        /// Constructor for the BaseDocument class
        /// </summary>
        /// <param name="name">Document Name</param>
        /// <param name="url">Document URL</param>
        public BaseDocument(string name, string url) {
            Name = name;
            Url = url;
        }
        /// <summary>
        /// Constructor for the BaseDocument class
        /// </summary>
        /// <param name="name">Document Name</param>
        /// <param name="url">Document URL</param>
        /// <param name="documentId">UUID of the document in the database</param>
        public BaseDocument(string name, string url, string documentId) : this(name, url) {
            DocumentId = documentId;
        }
    }
}