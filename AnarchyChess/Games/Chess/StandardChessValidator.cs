using System.Linq;
using AnarchyChess.Moves;

namespace AnarchyChess.Games.Chess
{
    public class StandardChessValidator : IMoveValidator
    {
        public bool Validate(Game game, AppliedMove foldedAppliedMove) => IsValidMove(game, foldedAppliedMove);

        public static bool IsValidMove(Game game, AppliedMove move)
        {
            if (!ValidateBounds(game, move)) return false;
            if (!ValidateOverlap(game, move)) return false;
            if (!ValidateMustTake(game, move)) return false;
            if (!ValidateNoCheck(game, move)) return false;

            return true;
        }

        public static bool ValidateBounds(Game game, AppliedMove move)
        {
            foreach (var step in move.GetSteps())
            {
                if (!game.Board.IsInBounds(step.To)) return false;
            }

            return true;
        }

        public static bool ValidateOverlap(Game game, AppliedMove move)
        {
            foreach (var step in move.GetSteps())
            {
                var movingPiece = game.Board[step.From];
                if (movingPiece == null) return false;

                var destPiece = game.Board[step.To];
                if (destPiece == null) continue;

                if (destPiece.Side == movingPiece.Side) return false;
                if (!move.TakeList.Contains(step.To)) return false;
            }

            return true;
        }

        public static bool ValidateMustTake(Game game, AppliedMove move)
        {
            if (!move.MustTake) return true;
            var firstPiece = game.Board[move.GetSteps()[0].From];
            if (firstPiece == null) return false;

            var takesAll = move.TakeList
                .All(x => game.Board[x] != null && game.Board[x]!.Side != firstPiece.Side);

            return takesAll;
        }

        public static bool ValidateNoCheck(Game game, AppliedMove move)
        {
            var firstPiece = game.Board[move.GetSteps()[0].From];
            if (firstPiece == null) return false;
            var gameClone = game.Clone();

            gameClone.ApplyMove(move);
            var isValid = !ChessMateCheck.IsCheck(gameClone, firstPiece.Side);
            return isValid;
        }
    }
}
