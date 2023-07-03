using AnarchyChess.Moves;

namespace AnarchyChess.Games
{
    public interface IMoveValidator
    {
        /// <summary>
        /// Check if a move is a legal move. It could have follow-ups which all need to be validated.
        /// This should validate conditions including but not limited to the following: <br/>
        /// - the piece does not get out of bounds of the board <br/>
        /// - the piece does not step on a friendly piece (and overwrite it in the process) <br/>
        /// - the piece is only allowed to step on an opposing piece if it captures on that position <br/>
        /// - the king does not end up in a check after the move <br/>
        /// </summary>
        /// <param name="game">The game being played</param>
        /// <param name="foldedAppliedMove">The move to validate</param>
        /// <returns>Whether the move is valid</returns>
        bool Validate(Game game, AppliedMove foldedAppliedMove);
    }
}
