using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;
using JetBrains.Annotations;

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

        public Move[] GetMoves(Board board, Pos pos)
        {
            var moves = new List<Move>();
            moves.AddRange(NormalMove(board, pos));
            moves.AddRange(Castling(board, pos));

            return moves.ToArray();
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Board board, [NotNull] Pos pos)
        {
            var moves = new List<Move>();
            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                moves.Add(Move.MakeRelative(pos, new Pos(x, y)).Take());
            }

            return moves;
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Move> Castling([NotNull] Board board, [NotNull] Pos pos)
        {
            var piece = board[pos];
            var moves = new List<Move>();
            if (piece.MoveCount >= 0) return moves;

            moves.AddRange(_InternalCastle(true, board, pos));
            moves.AddRange(_InternalCastle(false, board, pos));

            return moves;
        }

        private static IEnumerable<Move> _InternalCastle(bool isLeft, Board board, Pos pos)
        {
            int rookX = isLeft ? 0 : 7;
            int direction = isLeft ? -1 : 1;
            var rook = board[pos.SetX(rookX)];
            if (!(rook is Rook) || rook.MoveCount == 0) return new List<Move>();

            for (int x = pos.X + direction; x != rookX; x += direction)
            {
                // Do not add the move if there are any pieces between the rook and the king
                if (board[pos.SetX(x)] != null) return new List<Move>();
            }

            var move = Move.MakeRelative(pos, new Pos(2 * direction, 0))
                           .AddFollowUp(new Move(pos.SetX(rookX), pos.AddX(1 * -direction)));

            return new List<Move> { move };
        }
    }
}
