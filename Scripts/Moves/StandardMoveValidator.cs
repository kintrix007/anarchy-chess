using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Moves
{
    public class StandardMoveValidator : IMoveValidator
    {
        public bool Validate(Game game, Move foldedMove)
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
                if (move.To.X < 0 || move.To.X >= 8) return false;
                if (move.To.Y < 0 || move.To.Y >= 8) return false;
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

            foreach (var move in foldedMove.Unfold())
            {
                //? IDK man, this move then inverse move seems a bit finicky...
                game.Board.InternalApplyMove(move);

                for (var y = 0; y < 8; y++)
                {
                    for (var x = 0; x < 8; x++)
                    {
                        var pos   = new Pos(x, y);
                        var piece = game.Board[pos];
                        if (piece == null) continue;

                        var causesCheck = piece.GetMoves(game, pos)
                            .Any(m => game.Board[m.To] is King k && k.Side == originalPiece.Side);

                        if (causesCheck) return false;
                    }
                }

                game.Board.InternalApplyMove(move.Inverse());
            }

            return true;
        }
    }
}