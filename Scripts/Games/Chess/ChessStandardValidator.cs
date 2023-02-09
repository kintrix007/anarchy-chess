using System.Linq;
using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Games.Chess
{
    public class ChessStandardValidator : IMoveValidator
    {
        public bool Validate(Game game, AppliedMove foldedAppliedMove) => IsValidMove(game, foldedAppliedMove);

        public static bool IsValidMove([NotNull] Game game, [NotNull] AppliedMove foldedAppliedMove)
        {
            if (!ValidateBounds(game, foldedAppliedMove)) return false;
            if (!ValidateOverlap(game, foldedAppliedMove)) return false;
            if (!ValidateMustTake(game, foldedAppliedMove)) return false;
            if (!ValidateNoCheck(game, foldedAppliedMove)) return false;

            return true;
        }

        public static bool ValidateBounds(Game game, AppliedMove foldedAppliedMove)
        {
            foreach (var move in foldedAppliedMove.Unfold())
            {
                if (!game.Board.IsInBounds(move.To)) return false;
            }

            return true;
        }

        public static bool ValidateOverlap(Game game, AppliedMove foldedAppliedMove)
        {
            foreach (var move in foldedAppliedMove.Unfold())
            {
                var movingPiece = game.Board[move.From];
                if (movingPiece == null) return false;

                var destPiece = game.Board[move.To];
                if (destPiece == null) continue;

                if (destPiece.Side == movingPiece.Side) return false;
                if (!move.TakeList.Contains(move.To)) return false;
            }

            return true;
        }

        public static bool ValidateMustTake(Game game, AppliedMove foldedAppliedMove)
        {
            foreach (var move in foldedAppliedMove.Unfold())
            {
                if (!move.IsMustTake) continue;

                var movingPiece = game.Board[move.From];
                if (movingPiece == null) return false;

                var takesAll = move.TakeList.All(
                    x => game.Board[x] != null && game.Board[x].Side != movingPiece.Side
                );

                if (!takesAll) return false;
            }

            return true;
        }

        public static bool ValidateNoCheck(Game game, AppliedMove foldedAppliedMove)
        {
            var originalPiece = game.Board[foldedAppliedMove.From];
            if (originalPiece == null) return false;
            var gameClone = game.Clone();

            gameClone.ApplyMove(foldedAppliedMove);
            var isValid = !ChessMateCheck.IsCheck(gameClone, originalPiece.Side);
            return isValid;
        }
    }
}
