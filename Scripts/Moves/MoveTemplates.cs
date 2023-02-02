using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.PieceHelper
{
    public static class MoveTemplates
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<Move> RunLine([NotNull] Board board, [NotNull] Pos pos, [NotNull] Pos dir)
        {
            var moves = new List<Move>();
            var checkPos = pos;

            foreach (var _ in Enumerable.Range(0, 8))
            {
                checkPos += dir;
                if (!board.IsInBounds(checkPos)) break;
                
                var checkPiece = board[checkPos];
                moves.Add(Move.Absolute(pos, checkPos));
                if (checkPiece != null) break;
            }

            return moves;
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<Move> RunSteps([NotNull] Board board, [NotNull] Pos pos, [NotNull] Pos by)
        {
            if (by.X == 0 || by.Y == 0 || Math.Abs(by.X) == Math.Abs(by.Y))
            {
                throw new ArgumentException("Only supports horizontal, vertical and 45 degree diagonal moves");
            }

            var dir = new Pos(Math.Sign(by.X), Math.Sign(by.Y));
            var checkPos = pos;
            while (checkPos != pos+by)
            {
                checkPos += dir;
                if (!board.IsInBounds(checkPos)) return new List<Move>();
                
                var checkPiece = board[checkPos];
                if (checkPiece != null) return new List<Move>();
            }

            return new List<Move>{ Move.Relative(pos, by) };
        }
    }
}
