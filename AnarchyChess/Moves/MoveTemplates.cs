using System;
using System.Collections.Generic;
using System.Linq;

using AnarchyChess.Boards;

namespace AnarchyChess.Moves
{
    public static class MoveTemplates
    {
        /// <summary>
        /// Generate all the possible moves when running a straight line in a given direction.
        /// The running will stop when it encounters a piece or the border of the board.
        /// </summary>
        /// <param name="board">The board to run on</param>
        /// <param name="pos">The starting position</param>
        /// <param name="dir">The direction to run in</param>
        /// <returns>The possible moves in that line</returns>

        public static IEnumerable<MoveBuilder> RunLine(Board board, Pos pos, Pos dir)
        {
            var moves = new List<MoveBuilder>();
            var checkPos = pos;

            foreach (var _ in Enumerable.Range(0, Math.Max(board.Width, board.Height)))
            {
                checkPos += dir;
                if (!board.IsInBounds(checkPos)) break;

                var checkPiece = board[checkPos];
                moves.Add(MoveBuilder.Absolute(pos, checkPos));
                if (checkPiece != null) break;
            }

            return moves;
        }

        public static IEnumerable<MoveBuilder> RunSteps(Board board, Pos pos, Pos by)
        {
            if (by.X == 0 || by.Y == 0 || Math.Abs(by.X) == Math.Abs(by.Y))
            {
                throw new ArgumentException("Only supports horizontal, vertical and 45 degree diagonal moves");
            }

            var dir = new Pos(Math.Sign(by.X), Math.Sign(by.Y));
            var checkPos = pos;
            while (checkPos != pos + by)
            {
                checkPos += dir;
                if (!board.IsInBounds(checkPos)) return new List<MoveBuilder>();

                var checkPiece = board[checkPos];
                if (checkPiece != null) return new List<MoveBuilder>();
            }

            return new List<MoveBuilder>() { MoveBuilder.Relative(pos, by) };
        }
    }
}
