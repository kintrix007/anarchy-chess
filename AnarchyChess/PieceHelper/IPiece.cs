using System.Collections.Generic;
using AnarchyChess.Games;
using AnarchyChess.Moves;

namespace AnarchyChess.PieceHelper
{
    /// <summary>
    /// Object representing a piece on the board.
    /// </summary>
    public interface IPiece
    {
        /// <summary>
        /// The cost of this piece.
        /// </summary>
        int Cost { get; }

        /// <summary>
        /// The side on which this piece plays.
        /// </summary>
        Side Side { get; }

        /// <summary>
        /// The amount of times this piece has moved.
        /// </summary>
        int MoveCount { get; set; }

        /// <summary>
        /// Get the moves the piece can make. These still need to be validated.
        /// </summary>
        /// <param name="game">The game being played</param>
        /// <param name="pos">The current position of the piece</param>
        /// <returns>The moves the piece can make</returns>
        IEnumerable<AppliedMove> GetMoves(Game game, Pos pos) {
            return GetMoveBuilders(game, pos).Select(x => x.Build(this));
        }

        IEnumerable<MoveBuilder> GetMoveBuilders(Game game, Pos pos);

        IEnumerable<MoveBuilder> NormalMove(Game game, Pos pos);
    }
}
