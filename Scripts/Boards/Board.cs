using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Signal = Godot.SignalAttribute;
using Resource = Godot.Resource;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Boards
{
    /// <summary>
    /// Object to hold all the pieces.
    /// </summary>
    public class Board : Resource, IEnumerable<(Pos pos, IPiece piece)>
    {
        [Signal]
        public delegate void PieceAdded([NotNull] Pos pos, [NotNull] Object piece);

        [Signal]
        public delegate void PieceRemoved([NotNull] Pos pos, [NotNull] Object piece);

        [NotNull, ItemCanBeNull] private readonly IPiece[,] _pieces;

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
        [CanBeNull]
        public IPiece this[[NotNull] Pos pos]
        {
            get => IsInBounds(pos) ? _pieces[pos.X, pos.Y] : null;
            private set => _pieces[pos.X, pos.Y] = value;
        }

        /// <summary>
        /// Get the piece on the board at pos or null if it is empty.
        /// </summary>
        /// <param name="x">The x position to get the piece from</param>
        /// <param name="y">The y position to get the piece from</param>
        [CanBeNull]
        public IPiece this[int x, int y] => this[new Pos(x, y)];

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
        public void AddPiece([NotNull] Pos pos, [NotNull] IPiece piece)
        {
            if (this[pos] != null) throw new ArgumentException($"There is already a piece at {pos}");

            EmitSignal(nameof(Game.PieceAdded), pos, (Object)piece);
            this[pos] = piece;
        }

        /// <summary>
        /// Remove a piece from the board and return it, or return null if there was not a piece.
        /// </summary>
        /// <param name="pos">Where to remove the piece from</param>
        /// <returns>The piece removed</returns>
        [CanBeNull]
        public IPiece RemovePiece([NotNull] Pos pos)
        {
            var piece = this[pos];
            this[pos] = null;

            if (piece != null)
            {
                EmitSignal(nameof(Game.PieceRemoved), pos, (Object)piece);
            }

            return piece;
        }

        /// <summary>
        /// Apply a move that is already split into steps.
        /// They need to be split into only the origin and the destination. <br />
        /// <br />
        /// Now, this one has some quirks as it currently stands, so brace yourself: <br />
        /// <br />
        /// The callback is called _before_ a step is applied.
        /// This is where the logic for capturing pieces could be handled. <br />
        /// <br />
        /// It is allowed for a piece to move on the same square as another one under the following conditions: <br />
        ///   1. At most 2 pieces may stand on the same tile after any step of the move. <br />
        ///   2. After the last step, every piece must stand on a unique tile. <br />
        /// </summary>
        /// <param name="steps">The already split up steps of a move</param>
        /// <param name="beforeStepCallback">Function to be called before each step</param>
        /// <exception cref="Exception">If some of the rules got violated</exception>
        public void ApplySteps([NotNull] List<(Pos from, Pos to)> steps, Action<int, IPiece> beforeStepCallback)
        {
            var tmpPositions = new Dictionary<Pos, IPiece>();

            IPiece GetAt(Pos pos)
            {
                if (!tmpPositions.ContainsKey(pos)) return this[pos];
                
                var piece = tmpPositions[pos];
                tmpPositions.Remove(pos);
                return piece;
            }

            void SetAt(Pos pos, IPiece piece)
            {
                if (this[pos] != null)
                {
                    if (tmpPositions[pos] != null) throw new Exception();
                    tmpPositions[pos] = this[pos];
                }

                this[pos] = piece;
            }
            
            foreach (var i in Enumerable.Range(0, steps.Count))
            {
                var (from, to) = steps[i];
                var movingPiece = GetAt(from);
                beforeStepCallback(i, movingPiece);

                SetAt(to, movingPiece);
                this[from] = null;
                SetAt(from, GetAt(from)); // If there is a piece with a tmp pos, then put it on the board.
            }

            if (tmpPositions.Count != 0) throw new Exception();
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
