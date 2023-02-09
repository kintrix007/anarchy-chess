using System.Collections.Generic;
using JetBrains.Annotations;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Moves
{
    /// <summary>
    /// This class will describe a move in the move history.
    /// It should have a lot more convenience fields,
    /// such as what the moving piece is, what pieces it captured etc...
    /// </summary>
    public class HistoryMove : Resource
    {
        [NotNull] public readonly List<MoveStep> Steps;
        
        public HistoryMove([NotNull] AppliedMove move)
        {
            Steps = move.GetSteps();
        }
    }
}
