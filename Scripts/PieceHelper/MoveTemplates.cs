using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;

namespace AnarchyChess.Scripts.PieceHelper
{
    public static class MoveTemplates
    {
        public static IEnumerable<Move> RunLine(Board board, Pos pos, Pos dir)
        {
            var moves    = new List<Move>();
            var checkPos = pos;

            foreach (var _ in Enumerable.Range(0, 8))
            {
                checkPos += dir;
                var checkPiece = board[checkPos];

                moves.Add(Move.Absolute(pos, checkPos));
                if (checkPiece != null) break;
            }

            return moves;
        }
    }
}