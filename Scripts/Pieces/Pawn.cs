using System.Collections.Generic;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Pieces
{
    public class Pawn : Object, IPiece, IPromotable
    {
        public int Cost => 1;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Pawn(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] GetMoves(Board board, Pos pos)
        {
            var moves = new List<Move>();
            moves.AddRange(NormalMove(board, pos));
            moves.AddRange(EnPassant(board, pos));

            return moves.ToArray();
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Board board, [NotNull] Pos pos)
        {
            var piece = board[pos];
            var moves = new List<Move>();
            int facing = (piece.Side == Side.White ? 1 : -1);
            moves.Add(Move.MakeRelative(pos, new Pos(0, facing)));

            if (piece.MoveCount == 0)
            {
                moves.Add(Move.MakeRelative(pos, new Pos(0, 2 * facing)));
            }

            moves.Add(Move.MakeRelative(pos, new Pos(-1, facing)).Must().Take());
            moves.Add(Move.MakeRelative(pos, new Pos(1, facing)).Must().Take());

            return moves;
        }

        /// Defined in a way that it works even if the pawn did not start at the pawn base line
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Move> EnPassant([NotNull] Board board, [NotNull] Pos pos)
        {
            var moves = new List<Move>();
            if (board.LastMove == null) return moves;

            moves.AddRange(_InternalEnPassant(true, board, pos));
            moves.AddRange(_InternalEnPassant(true, board, pos));

            return moves;
        }

        private static IEnumerable<Move> _InternalEnPassant(bool isLeft, Board board, Pos pos)
        {
            var piece = board[pos];
            int facing = (piece.Side == Side.White ? 1 : -1);
            int direction = (isLeft ? -1 : 1);
            var opponentPawnPos = pos.AddX(direction);
            var moves = new List<Move>();

            if (!(board[opponentPawnPos] is Pawn p)) return moves;
            if (p.Side == piece.Side) return moves;
            if (p.MoveCount != 1) return moves;
            if (board.LastMove == null) return moves;
            if (board.LastMove.To != opponentPawnPos) return moves;
            if (board.LastMove.Relative.Abs() != new Pos(0, 2)) return moves;

            moves.Add(Move.MakeRelative(pos, new Pos(direction, facing))
                          .AddTake(opponentPawnPos).Must());

            return moves;
        }
    }
}