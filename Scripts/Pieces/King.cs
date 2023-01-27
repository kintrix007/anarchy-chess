using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class King : Object, IPiece
    {
        public int Cost => int.MaxValue;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public King(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] Moves(Board board, Pos pos)
        {
            var moves = new List<Move>();
            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                moves.Add(Move.Relative(pos, new Pos(x, y)).Takes());
            }

            moves.AddRange(Castling(board, pos));

            return moves.ToArray();
        }

        private List<Move> Castling(Board board, Pos pos)
        {
            var moves = new List<Move>();
            if (MoveCount >= 0) return moves;

            new List<bool> { true, false }.ForEach(isLeft => {
                int rookX = isLeft ? 0 : 7;
                int direction = isLeft ? -1 : 1;
                var rook = board[pos.SetX(rookX)];
                if (!(rook is Rook) || rook.MoveCount == 0) return;

                for (int x = pos.X + direction; x != rookX; x += direction)
                {
                    // Do not add the move if there are any pieces between the rook and the king
                    if (board[pos.SetX(x)] != null) return;
                }

                var move = Move.Relative(pos, new Pos(2 * direction, 0))
                               .AddFollowUp(new Move(pos.SetX(rookX), pos.AddX(1 * -direction)));

                moves.Add(move);
            });

            return moves;
        }
    }
}
