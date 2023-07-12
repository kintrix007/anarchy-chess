using AnarchyChess.Moves;

namespace AnarchyChess.Games.Chess
{
    public class StandardChessValidator : IMoveValidator
    {
        public bool Validate(Game game, AppliedMove foldedAppliedMove) => IsValidMove(game, foldedAppliedMove);

        public static bool IsValidMove(Game game, AppliedMove move)
        {
            if (!IsPossibleMove(game, move)) return false;
            if (!ValidateBounds(game, move)) return false;
            if (!ValidateOverlap(game, move)) return false;
            if (!ValidateCaptureOnly(game, move)) return false;
            if (!ValidateNoCheck(game, move)) return false;

            return true;
        }

        private static bool IsPossibleMove(Game game, AppliedMove move)
        {
            //TODO: Make this nicer. As it stands, this sucks.
            var pos = game.Board.Pieces()
                .SelectNotNull(x => ReferenceEquals(x.piece, move.Piece) ? x.pos : null!)
                .First();
            var moves = move.Piece.GetMoves(game, pos);
            return moves.Contains(move);
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
                if (!move.CaptureList.Contains(step.To)) return false;
            }

            return true;
        }

        /// <summary>
        /// Validate that if a move can only happen when it captures a piece,
        /// then it really did capture a piece.
        /// </summary>
        public static bool ValidateCaptureOnly(Game game, AppliedMove move)
        {
            if (!move.MustBeACapture) return true;
            var firstPiece = game.Board[move.GetSteps()[0].From];
            if (firstPiece == null) return false;

            var takesAll = move.CaptureList
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
