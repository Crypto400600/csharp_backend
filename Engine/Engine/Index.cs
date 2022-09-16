using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine {
    /// <summary>
    /// This is a representation of the index
    /// </summary>
    public class Index {
        /// <summary>
        /// This created a key value mapping of the word to a <see cref="Token"/>
        /// </summary>
        public readonly Dictionary<string, Token> Tokens = new Dictionary<string, Token>();

        /// <summary>
        /// This is used to add an instance of a word to the index
        /// It checks if the token already contains an instance of the word.
        /// If it does it just updates the word
        /// Else it creates a new instance of the word and adds that initial position
        /// </summary>
        /// <param name="word">The string word</param>
        /// <param name="dbDocument">The document the word is in</param>
        /// <param name="position">The position of the word in the document</param>
        public void AddWord(string word, DbDocument dbDocument, int position) {
            Token wordItem;
            if (Tokens.ContainsKey(word)) {
                wordItem = Tokens[word];
            }
            else {
                wordItem = new Token(word);
                Tokens[word] = wordItem;
            }

            wordItem.AddItem(dbDocument, position);
        }

        /// <summary>
        /// Saves the tokens in the document to the database
        /// </summary>
        public async Task SaveToDb() {
            var saveActions = Tokens.Select(token => token.Value.SaveSelfToDb()).ToList();

            await Task.WhenAll(saveActions);
        }
    }
}