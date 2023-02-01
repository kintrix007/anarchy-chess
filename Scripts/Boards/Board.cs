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

        public Board()
        {
            _pieces = new IPiece[8, 8];
        }

        public IPiece this[[NotNull] Pos pos]
        {
            get => _pieces[pos.X, pos.Y];
            private set => _pieces[pos.X, pos.Y] = value;
        }

        public void AddPiece([NotNull] Pos pos, [NotNull] IPiece piece)
        {
            if (this[pos] != null) throw new ArgumentException($"There is already a piece at {pos}");
            this[pos] = piece;
        }

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
    }
}
