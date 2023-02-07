using System.Collections.Generic;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class King : Resource, IPiece
    {
        public int Cost => int.MaxValue;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public King(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        IEnumerable<Move> IPiece.GetMoves(Game game, Pos pos)
        {
            var moves = new List<Move>();
            moves.AddRange(NormalMove(game, pos));
            moves.AddRange(Castling(game, pos));

            return moves;
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<Move>();
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    moves.Add(Move.Relative(pos, new Pos(x, y)).Take());
                }
            }

            return moves;
        }

        //TODO Make it check so that you cannot castle through a line of attack
        [NotNull, ItemNotNull]
        public static IEnumerable<Move> Castling([NotNull] Game game, [NotNull] Pos pos)
        {
            var piece = game.Board[pos];
            var moves = new List<Move>();
            if (piece.MoveCount > 0) return moves;

            moves.AddRange(_InternalCastle(true, game, pos));
            moves.AddRange(_InternalCastle(false, game, pos));

            return moves;
        }

        //TODO rewrite it in a way that it does not matter where the castlable is.
        //TODO It should just be unmoved and in the same row/column.
        private static IEnumerable<Move> _InternalCastle(bool isLeft, Game game, Pos pos)
        {
            var rookX = isLeft ? 0 : 7;
            var direction = isLeft ? -1 : 1;
            var castlable = game.Board[pos.SetX(rookX)];
            if (!(castlable is ICastlable)) return new List<Move>();
            if (castlable.MoveCount != 0) return new List<Move>();

            for (var x = pos.X + direction; x != rookX; x += direction)
            {
                // Do not add the move if there are any pieces between the rook and the king
                if (game.Board[pos.SetX(x)] != null) return new List<Move>();
            }

            var newKingPos = pos.AddX(2 * direction);
            var move = Move.Absolute(pos, newKingPos)
                .AddFollowUp(Move.Absolute(pos.SetX(rookX), newKingPos.AddX(1 * -direction)));

            return new List<Move> { move };
        }
    }
}