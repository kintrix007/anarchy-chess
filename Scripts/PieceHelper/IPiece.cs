using System.Collections.Generic;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.PieceHelper
{
    public interface IPiece
    {
        int Cost { get; }
        Side Side { get; }
        int MoveCount { get; set; }

        /// <summary>
        ///     Get the moves the piece can make. These still need to be validated.
        /// </summary>
        /// <param name="game">The game being played</param>
        /// <param name="pos">The current position of the piece</param>
        /// <returns>The moves the piece can make</returns>
        [NotNull, ItemNotNull]
        IEnumerable<Move> GetMoves([NotNull] Game game, [NotNull] Pos pos);
    }
}