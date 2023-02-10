using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Type> Promotions =>
            new List<Type>{ typeof(Queen), typeof(Rook), typeof(Knight), typeof(Bishop), typeof(Knook) };


        public Pawn(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos)
        {
            var moves = new List<AppliedMove>();
            moves.AddRange(NormalMove(game, pos));
            moves.AddRange(EnPassant(game, pos));

            //TODO Instead generate all possible promotion moves
            foreach (var move in moves.Where(IsPromotion))
            {
                move.PromoteTo(Promotions[Math.Abs((int)GD.Randi()) % Promotions.Count]);
            }

            return moves.ToArray();
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<AppliedMove> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var piece  = game.Board[pos];
            var moves  = new List<AppliedMove>();
            if (piece == null) return moves;

            var facing = piece.Side == Side.White ? 1 : -1;
            moves.Add(AppliedMove.Relative(pos, new Pos(0, facing)));

            // Can go two tiles if it hasn't moved and there is nothing in front of it.
            if (piece.MoveCount == 0 && game.Board[pos.AddY(facing)] == null)
            {
                moves.Add(AppliedMove.Relative(pos, new Pos(0, 2 * facing)));
            }

            moves.Add(AppliedMove.Relative(pos, new Pos(-1, facing)).Must().Take());
            moves.Add(AppliedMove.Relative(pos, new Pos(1, facing)).Must().Take());

            return moves;
        }

        /// Defined in a way that it works even if the pawn did not start at the pawn base line
        [NotNull, ItemNotNull]
        public static IEnumerable<AppliedMove> EnPassant([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<AppliedMove>();
            if (game.LastMove == null) return moves;

            moves.AddRange(_InternalEnPassant(true, game, pos));
            moves.AddRange(_InternalEnPassant(false, game, pos));

            return moves;
        }

        private static IEnumerable<AppliedMove> _InternalEnPassant(bool isLeft, Game game, Pos pos)
        {
            var piece = game.Board[pos];
            var moves = new List<AppliedMove>();
            if (piece == null) return moves;

            var facing = piece.Side == Side.White ? 1 : -1;
            var direction = isLeft ? -1 : 1;
            var opponentPawnPos = pos.AddX(direction);

            if (!(game.Board[opponentPawnPos] is Pawn p)) return moves;
            if (p.Side == piece.Side) return moves;
            if (p.MoveCount != 1) return moves;
            if (game.LastMove == null) return moves;
            if (game.LastMove.Steps.Count != 1) return moves;
            var lastStep = game.LastMove.Steps[0];

            if (lastStep.To != opponentPawnPos) return moves;
            if (lastStep.Offset.Abs() != new Pos(0, 2)) return moves;

            moves.Add(AppliedMove.Relative(pos, new Pos(direction, facing))
                .AddTake(opponentPawnPos).Must());

            return moves;
        }

        private static bool IsPromotion(AppliedMove appliedMove)
        {
            var steps = appliedMove.GetSteps();
            if (steps.Count != 1) return false;

            var step = steps[0];
            return step.To.Y == 0 || step.To.Y == 7;
        }
    }
}
