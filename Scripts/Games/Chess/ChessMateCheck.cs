using System.Linq;
using AnarchyChess.Scripts.PieceHelper;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Games.Chess
{
    public static class ChessMateCheck
    {
        /// <summary>
        /// Determine whether a given side got either a stale mate or a check mate.
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <param name="side">Side to check</param>
        /// <returns>Whether it is a _ mate</returns>
        public static bool IsMate(Game game, Side side) => !game.GetAllValidMoves(side).Any();

        /// <summary>
        /// Determine whether a given side got a check mate.
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <param name="side">Side to check</param>
        /// <returns>Whether it is a check mate</returns>
        public static bool IsCheckMate(Game game, Side side) => IsMate(game, side) && IsCheck(game, side);

        /// <summary>
        /// Determine whether a given side got a stale mate.
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <param name="side">Side to check</param>
        /// <returns>Whether it is a stale mate</returns>
        public static bool IsStaleMate(Game game, Side side) => IsMate(game, side) && !IsCheck(game, side);

        /// <summary>
        /// Determine whether a given side is in check.
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <param name="side">Side to check</param>
        /// <returns>Whether the side is in check in the game</returns>
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
