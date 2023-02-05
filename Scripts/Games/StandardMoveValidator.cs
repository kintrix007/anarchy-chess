using System.Linq;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Games
{
    public class StandardMoveValidator : IMoveValidator
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

            foreach (var move in foldedMove.Unfold().Where(move => game.Board.IsInBounds(move.To)))
            {
                gameClone.Board.InternalApplyMove(move);

                for (var y = 0; y < game.Board.Height; y++)
                {
                    for (var x = 0; x < game.Board.Width; x++)
                    {
                        var pos = new Pos(x, y);
                        var piece = gameClone.Board[pos];
                        if (piece == null) continue;
                        if (piece.Side == originalPiece.Side) continue;

                        var causesCheck = piece.GetMoves(gameClone, pos)
                            .Any(m => gameClone.Board[m.To] is King k && k.Side == originalPiece.Side);

                        if (causesCheck) return false;
                    }
                }
            }

            return true;
        }
    }
}