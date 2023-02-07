using System.Linq;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Games.Chess
{
    public class ChessStandardValidator : IMoveValidator
    {
        public bool Validate(Game game, Move foldedMove) => IsValidMove(game, foldedMove);

        public static bool IsValidMove([NotNull] Game game, [NotNull] Move foldedMove)
        {
            if (!ValidateBounds(game, foldedMove)) return false;
            if (!ValidateOverlap(game, foldedMove)) return false;
            if (!ValidateMustTake(game, foldedMove)) return false;
            if (!ValidateNoCheck(game, foldedMove)) return false;

            return true;
        }

        public static bool ValidateBounds(Game game, Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
            {
                if (move.To.X < 0 || move.To.X >= game.Board.Width) return false;
                if (move.To.Y < 0 || move.To.Y >= game.Board.Height) return false;
            }

            return true;
        }

        public static bool ValidateOverlap(Game game, Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
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

        public static bool ValidateMustTake(Game game, Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
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

        public static bool ValidateNoCheck(Game game, Move foldedMove)
        {
            var originalPiece = game.Board[foldedMove.From];
            if (originalPiece == null) return false;
            var gameClone = game.Clone();

            gameClone.Board.InternalApplyMove(foldedMove);
            var isValid = !ChessMateCheck.IsCheck(gameClone, originalPiece.Side);
            return isValid;
        }
    }
}
