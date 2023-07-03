using System.Collections.Generic;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class King : IPiece
    {
        public int Cost => int.MaxValue;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public King(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        IEnumerable<AppliedMove> IPiece.GetMoves(Game game, Pos pos)
        {
            var moves = new List<AppliedMove>();
            moves.AddRange(NormalMove(game, pos));
            moves.AddRange(Castling(game, pos));

            return moves;
        }

        
        public static IEnumerable<AppliedMove> NormalMove(Game game, Pos pos)
        {
            var moves = new List<AppliedMove>();
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    moves.Add(AppliedMove.Relative(pos, new Pos(x, y)).Take());
                }
            }

            return moves;
        }

        //TODO Make it check so that you cannot castle through a line of attack
        
        public static IEnumerable<AppliedMove> Castling(Game game, Pos pos)
        {
            var piece = game.Board[pos];
            var moves = new List<AppliedMove>();
            if (piece.MoveCount > 0) return moves;

            moves.AddRange(_InternalCastle(true, game, pos));
            moves.AddRange(_InternalCastle(false, game, pos));

            return moves;
        }

        //TODO Rewrite it in a way that it does not matter where the castlable is.
        // It should just be unmoved and in the same row/column.
        private static IEnumerable<AppliedMove> _InternalCastle(bool isLeft, Game game, Pos pos)
        {
            var rookX = isLeft ? 0 : 7;
            var direction = isLeft ? -1 : 1;
            var castlable = game.Board[pos.SetX(rookX)];
            if (!(castlable is ICastlable)) return new List<AppliedMove>();
            if (castlable.MoveCount != 0) return new List<AppliedMove>();

            for (var x = pos.X + direction; x != rookX; x += direction)
            {
                // Do not add the move if there are any pieces between the rook and the king
                if (game.Board[pos.SetX(x)] != null) return new List<AppliedMove>();
            }

            var newKingPos = pos.AddX(2 * direction);
            var move = AppliedMove.Absolute(pos, newKingPos)
                .AddFollowUp(AppliedMove.Absolute(pos.SetX(rookX), newKingPos.AddX(1 * -direction)));

            return new List<AppliedMove> { move };
        }
    }
}
