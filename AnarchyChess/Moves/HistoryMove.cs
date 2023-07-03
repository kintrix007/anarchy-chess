using System.Collections.Generic;

namespace AnarchyChess.Moves
{
    /// <summary>
    /// This class will describe a move in the move history.
    /// It should have a lot more convenience fields,
    /// such as what the moving piece is, what pieces it captured etc...
    /// </summary>
    public class HistoryMove
    {
        public readonly List<MoveStep> Steps;
        
        public HistoryMove(AppliedMove move)
        {
            Steps = move.GetSteps();
        }
    }
}
