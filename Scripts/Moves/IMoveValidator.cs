using JetBrains.Annotations;
using AnarchyChess.Scripts.Boards;

namespace AnarchyChess.Scripts.Moves
{
    public interface IMoveValidator
    {
        /// <summary>
        /// Check if a move is a legal move.
        /// This should validate conditions including but not limited to the following: <br/>
        /// - the piece does not get out of bounds of the board <br/>
        /// - the piece does not step on a friendly piece (and overwrite it in the process) <br/>
        /// - the piece is only allowed to step on an opposing piece if it captures on that position <br/>
        /// - the king does not end up in a check after the move <br/>
        /// </summary>
        /// <param name="board">The board the game is played on</param>
        /// <param name="move">The move to validate</param>
        /// <returns>Whether the move is valid</returns>
        bool IsValid([NotNull] Board board, [NotNull] Move move);
    }
}