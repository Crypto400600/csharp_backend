using System.Collections.Generic;
using Priority_Queue;

namespace Engine {
    /// <summary>
    /// Token Pointer class is used for the linear mapping algorithm
    /// </summary>
    public class TokenPointer : StablePriorityQueueNode {
        private readonly List<TokenItem> _targets;
        public readonly Token Token;
        private int _index;
        public readonly bool EmptyPointer;
        
        public TokenItem Target => _index < _targets.Count ? _targets[_index] : null;

        /// <summary>
        /// Creates an empty <see cref="TokenPointer"/>
        /// </summary>/
        public TokenPointer() {
            EmptyPointer = true;
        }
        /// <summary>
        /// Creates a new instance of token pointer with a target token and target documents
        /// </summary>/
        public TokenPointer(Token targetToken) {
            Token = targetToken;
            _targets = targetToken.Documents;
        }

        /// <summary>
        /// Moves the current index forward by 1
        /// </summary>
        public bool MoveForward() {
            if (_index + 1 < _targets.Count) {
                _index++;
                return true;
            }

            return false;
        }
    }
}