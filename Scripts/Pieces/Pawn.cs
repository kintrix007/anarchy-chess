using System.Collections.Generic;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Pieces
{
    public class Pawn : Object, IPiece
    {
        public int Cost => 1;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Pawn(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] Moves(Boards.Board board, Pos pos)
        {
            var moves = new List<Move>();
            int facing = (Side == Side.White ? 1 : -1);
            moves.Add(Move.Relative(pos, new Pos(0, facing)));

            if (MoveCount == 0)
            {
                moves.Add(Move.Relative(pos, new Pos(0, 2 * facing)));
            }

            moves.Add(Move.Relative(pos, new Pos(-1, facing)).Takes().Must());
            moves.Add(Move.Relative(pos, new Pos(1, facing)).Takes().Must());
            
            moves.AddRange(EnPassant(board, pos));

            return moves.ToArray();
        }

        private IEnumerable<Move> EnPassant(Board board, Pos pos)
        {
            var moves = new List<Move>();
            int facing = (Side == Side.White ? 1 : -1);
            if (board.LastMove == null) return moves;

            foreach (bool isLeft in new[] { true, false })
            {
                int direction = isLeft ? -1 : 1;
                var opponentPawnPos = pos.AddX(direction);
                if (!(board[opponentPawnPos] is Pawn p && p.Side != Side)) continue;
                if (p.MoveCount != 1) continue;
                if (!board.LastMove.To.Equals(opponentPawnPos)) continue;
                
                moves.Add(Move.Relative(pos, new Pos(direction, facing))
                              .AddTake(opponentPawnPos).Must());
            }

            return moves;
        }
    }
}
