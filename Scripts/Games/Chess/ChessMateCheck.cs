using System.Linq;
using AnarchyChess.Scripts.PieceHelper;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Games.Chess
{
    public static class ChessMateCheck
    {
        public static bool IsMate(Game game, Side side) => !game.GetAllValidMoves(side).Any();

        public static bool IsCheckMate(Game game, Side side) => IsMate(game, side) && IsCheck(game, side);

        public static bool IsStaleMate(Game game, Side side) => IsMate(game, side) && !IsCheck(game, side);

        /// <summary>
        /// Determine whether or not a given side is in check.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public static bool IsCheck(Game game, Side side)
        {
            foreach (var (pos, piece) in game.Board)
            {
                if (piece.Side == side) continue;
                var causesCheck = piece.GetMoves(game, pos)
                    .Any(x => game.Board[x.To] is King k && k.Side == side);

                if (causesCheck) return true;
            }

            return false;
        }
    }
}
