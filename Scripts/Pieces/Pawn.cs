using System.Collections.Generic;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class Pawn : Resource, IPiece, IPromotable
    {
        public int Cost => 1;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Pawn(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<Move> GetMoves(Game game, Pos pos)
        {
            var moves = new List<Move>();
            moves.AddRange(NormalMove(game, pos));
            moves.AddRange(EnPassant(game, pos));
            
            foreach (var move in moves)
            {
                if (CheckPromotion(move))
                {
                    move.Promotes();
                }
            }
            
            return moves.ToArray();
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var piece  = game.Board[pos];
            var moves  = new List<Move>();
            if (piece == null) return moves;

            var facing = piece.Side == Side.White ? 1 : -1;
            moves.Add(Move.Relative(pos, new Pos(0, facing)));

            // Can go two tiles if it hasn't moved and there is nothing in front of it.
            if (piece.MoveCount == 0 && game.Board[pos.AddY(facing)] == null)
            {
                moves.Add(Move.Relative(pos, new Pos(0, 2 * facing)));
            }

            moves.Add(Move.Relative(pos, new Pos(-1, facing)).Must().Take());
            moves.Add(Move.Relative(pos, new Pos(1, facing)).Must().Take());

            return moves;
        }

        /// Defined in a way that it works even if the pawn did not start at the pawn base line
        [NotNull, ItemNotNull]
        public static IEnumerable<Move> EnPassant([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<Move>();
            if (game.LastMove == null) return moves;

            moves.AddRange(_InternalEnPassant(true, game, pos));
            moves.AddRange(_InternalEnPassant(false, game, pos));

            return moves;
        }

        private static IEnumerable<Move> _InternalEnPassant(bool isLeft, Game game, Pos pos)
        {
            var piece = game.Board[pos];
            var moves = new List<Move>();
            if (piece == null) return moves;

            var facing = piece.Side == Side.White ? 1 : -1;
            var direction = isLeft ? -1 : 1;
            var opponentPawnPos = pos.AddX(direction);

            if (!(game.Board[opponentPawnPos] is Pawn p)) return moves;
            if (p.Side == piece.Side) return moves;
            if (p.MoveCount != 1) return moves;
            if (game.LastMove == null) return moves;
            if (game.LastMove.To != opponentPawnPos) return moves;
            if (game.LastMove.AsRelative.Abs() != new Pos(0, 2)) return moves;

            moves.Add(Move.Relative(pos, new Pos(direction, facing))
                .AddTake(opponentPawnPos).Must());

            return moves;
        }

        //System for Promoting a pawn.
        //Hazel made this, it is ** garbage **
        private static bool CheckPromotion(Move move)
        {
            if ((move.To.Y == 0) || (move.To.Y == 7))
            {
                return true;
            }

            return false;
        }
    }
}