using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public interface IPiece
    {
        int Cost { get; }
        Side Side { get; }
        int MoveCount { get; set; }

        /// <summary>
        /// Get the moves the piece can make. These still need to be validated.
        /// </summary>
        /// <param name="board">The board the game is played on</param>
        /// <param name="pos">The current position of the piece</param>
        /// <returns>The moves the piece can make</returns>
        [NotNull]
        [ItemNotNull]
        Move[] GetMoves([NotNull] Board board, [NotNull] Pos pos);
    }
}