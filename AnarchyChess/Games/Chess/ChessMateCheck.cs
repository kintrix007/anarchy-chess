using System.Linq;
using AnarchyChess.PieceHelper;
using AnarchyChess.Pieces;

namespace AnarchyChess.Games.Chess
{
    public static class ChessMateCheck
    {
        /// <summary>
        /// Determine whether a given side got either a stale mate or a check mate.
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <param name="side">Side to check</param>
        /// <returns>Whether it is a _ mate</returns>
        public static bool IsMate(Game game, Side side) => !game.HasValidMove(side);

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
            foreach (var (pos, piece) in game.Board.Pieces())
            {
                if (piece.Side == side) continue;

                var attackedPositions = piece.GetMoves(game, pos)
                    .SelectMany(move => move.CaptureList);
                
                var isInCheck = attackedPositions
                    .Any(x => game.Board[x] is King k && k.Side == side);
                
                if (isInCheck) return true;
            }

            return false;
        }
    }
}
