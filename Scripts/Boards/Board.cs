using System;
using System.Collections.Generic;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Boards
{
    //TODO Make it iterable
    public class Board : Resource
    {
        [NotNull, ItemCanBeNull] private readonly IPiece[,] _pieces;
        
        //TODO Actually take these values into consideration after making them public.
        private const int Width = 8;
        private const int Height = 8;

        public Board()
        {
            _pieces = new IPiece[Width, Height];
        }

        public IPiece this[[NotNull] Pos pos]
        {
            get => IsInBounds(pos) ? _pieces[pos.X, pos.Y] : null;
            private set => _pieces[pos.X, pos.Y] = value;
        }

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
            return piece;
        }

        /// <summary>
        /// Apply the move with its follow-ups on the board.
        /// </summary>
        /// <param name="foldedMove"></param>
        public void InternalApplyMove([NotNull] Move foldedMove)
        {
            var moveList     = foldedMove.Unfold();
            var movingPieces = new Dictionary<Move, IPiece>();
            moveList.ForEach(move => movingPieces[move] = this[move.From]);

            moveList.ForEach(move => {
                this[move.To] = movingPieces[move];
                this[move.From] = null;
            });
        }

        /// <summary>
        /// Creates a deep copy of this board, including copying the pieces.
        /// </summary>
        /// <returns>A copy of the board</returns>
        /// <exception cref="NullReferenceException"></exception>
        public Board Clone()
        {
            var boardClone = new Board();
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var piece = this[new Pos(x, y)];
                    if (piece == null) continue;
                    
                    var pieceCtor = piece.GetType().GetConstructor(new[] { typeof(Side) });
                    if (pieceCtor == null) throw new NullReferenceException();
                    
                    var pieceClone = (IPiece)pieceCtor.Invoke(new object[] { piece.Side });
                    pieceClone.MoveCount = piece.MoveCount;
                    
                    boardClone[new Pos(x, y)] = pieceClone;
                }
            }

            return boardClone;
        }
    }
}
