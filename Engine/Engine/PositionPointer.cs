using System.Collections.Generic;

namespace Engine {
    /// <summary>
    /// This pointer is used for the linear mapping algorithm
    /// It stores the positions of the word in the document and contains helper function to move forward through the array
    /// </summary>
    public class PositionPointer {
        private readonly List<int> _positions;
        private int _index;
        public readonly bool EmptyPointer;
        public readonly bool IsValid;
        
        /// <summary>
        /// Creates a new <see cref="PositionPointer"/>. Useful for consecutive words to signify empty space when a word in the user query doesn't exist in the database
        /// </summary>
        public PositionPointer() {
            EmptyPointer = true;
        }
        
        /// <summary>
        /// This creates a new <see cref="PositionPointer"/>
        /// </summary>
        /// <param name="positions">The positions of the word in the document</param>
        /// <param name="isValid">Used for specifying whether the current pointer is valid for the current document we're scoring</param>
        public PositionPointer(List<int> positions, bool isValid) {
            _positions = positions;
            IsValid = isValid;
        }

        /// <summary>
        /// This returns the current position that we are in.
        /// Useful for linear mapping algorithm
        /// </summary>
        public int CurrentPosition {
            get {
                if (EmptyPointer) {
                    return -1;
                }
                
                if (_index < _positions.Count) {
                    return _positions[_index];
                }

                return -1;
            }
        }
        
        /// <summary>
        /// Moves current position forward
        /// </summary>
        public void MoveForward() {
            _index++;
        }

        /// <summary>
        /// Keeps moving current position forward until it is either greater than the target or we have exhausted the possible positions
        /// </summary>
        /// <param name="target">The target position</param>
        public void MoveForwardUntilGreaterThanOrEqualTo(int target) {
            while (CurrentPosition < target && _index < _positions.Count) {
                MoveForward();
            }
        }
    }
}