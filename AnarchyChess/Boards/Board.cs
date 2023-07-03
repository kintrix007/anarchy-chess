using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Boards
{
    /// <summary>
    /// Object to hold all the pieces on their corresponding position.
    /// </summary>
    public class Board : IEnumerable<(Pos pos, IPiece piece)>
    {
        private readonly IPiece?[,] _pieces;

        //TODO Actually take these values into consideration after making them public.
        public readonly int Width;
        public readonly int Height;

        public Board()
        {
            Width = 8;
            Height = 8;
            _pieces = new IPiece[Width, Height];
        }

        /// <summary>
        /// Get the piece on the board at pos or null if it is empty.
        /// </summary>
        /// <param name="pos">The position to get the piece from</param>
        public IPiece? this[Pos pos]
        {
            get => IsInBounds(pos) ? _pieces[pos.X, pos.Y] : null;
            private set => _pieces[pos.X, pos.Y] = value;
        }

        /// <summary>
        /// Get the piece on the board at pos or null if it is empty.
        /// </summary>
        /// <param name="x">The x position to get the piece from</param>
        /// <param name="y">The y position to get the piece from</param>
        public IPiece? this[int x, int y] => this[new Pos(x, y)];

        /// <summary>
        /// Check if a position is in the bounds of this board.
        /// </summary>
        /// <param name="pos">The position to check</param>
        /// <returns>Whether it is in bounds</returns>
        public bool IsInBounds(Pos pos) => pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;

        /// <summary>
        /// Add a piece to the board.
        /// </summary>
        /// <param name="pos">Where to add the piece</param>
        /// <param name="piece">The piece to add</param>
        /// <exception cref="ArgumentException">If there is already a piece at pos</exception>
        public void AddPiece(Pos pos, IPiece piece)
        {
            if (this[pos] != null) throw new ArgumentException($"There is already a piece at {pos}");
            this[pos] = piece;
        }

        /// <summary>
        /// Remove a piece from the board and return it, or return null if there was not a piece.
        /// </summary>
        /// <param name="pos">Where to remove the piece from</param>
        /// <returns>The piece removed</returns>
        public IPiece? RemovePiece(Pos pos)
        {
            var piece = this[pos];
            this[pos] = null;

            return piece;
        }

        public void ReplacePiece(Pos pos, IPiece with)
        {
            if (this[pos] == null) throw new ArgumentException($"There is no piece to replace at {pos}");
            this[pos] = with;
        }
        
        /// <summary>
        /// Apply a move that is already split into steps.<br />
        /// <br />
        /// Now, this one has some quirks as it currently stands, so brace yourself: <br />
        /// <br />
        /// The callback is called after a step is applied.
        /// This is where the move counter could be implemented etc. <br />
        /// <br />
        /// It is allowed for a piece to move on the same square as another one under the following conditions: <br />
        ///   1. At most 2 pieces may stand on the same tile after any step of the move. <br />
        ///   2. After the last step, every piece must stand on a unique tile. <br />
        /// </summary>
        /// <param name="steps">The already split up steps of a move</param>
        /// <param name="afterStepCallback">Function to be called after each step</param>
        /// <exception cref="Exception">If some of the rules got violated</exception>
        public void ApplySteps(List<MoveStep> steps, Action<IPiece, MoveStep> afterStepCallback)
        {
            var tmpPositions = new Dictionary<Pos, IPiece>();

            IPiece GetAt(Pos pos) => tmpPositions.ContainsKey(pos) ? tmpPositions[pos] : this[pos];

            void SetAt(Pos pos, IPiece piece)
            {
                if (this[pos] != null)
                {
                    if (tmpPositions.ContainsKey(pos))
                    {
                        throw new Exception("There are more than 2 overlapping pieces.");
                    }
                    tmpPositions[pos] = this[pos];
                }

                this[pos] = piece;
            }

            void PopPossible()
            {
                var toRemove = new List<Pos>();
                
                foreach (var pair in tmpPositions.Where(pair => this[pair.Key] == null))
                {
                    this[pair.Key] = pair.Value;
                    toRemove.Add(pair.Key);
                }
                
                toRemove.ForEach(x => tmpPositions.Remove(x));
            }
            
            foreach (var step in steps)
            {
                var movingPiece = GetAt(step.From);
                
                SetAt(step.To, movingPiece);
                this[step.From] = null;
                PopPossible();

                afterStepCallback(movingPiece, step);
            }

            if (tmpPositions.Count != 0)
            {
                throw new Exception("There are overlapping pieces at the end of the move.");
            }
        }

        /// <summary>
        /// Creates a deep copy of this board, including copying the pieces.
        /// </summary>
        /// <returns>A copy of the board</returns>
        /// <exception cref="NullReferenceException"></exception>
        public Board Clone()
        {
            var boardClone = new Board();
            foreach (var (pos, piece) in this)
            {
                var pieceCtor = piece.GetType().GetConstructor(new[] { typeof(Side) });
                if (pieceCtor == null) throw new NullReferenceException();

                var pieceClone = (IPiece)pieceCtor.Invoke(new object[] { piece.Side });
                pieceClone.MoveCount = piece.MoveCount;

                boardClone[pos] = pieceClone;
            }

            return boardClone;
        }

        public IEnumerator<(Pos, IPiece)> GetEnumerator()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var pos = new Pos(x, y);
                    var piece = this[pos];
                    if (piece == null) continue;
                    yield return (pos, piece);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
