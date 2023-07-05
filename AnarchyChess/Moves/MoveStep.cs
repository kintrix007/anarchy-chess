namespace AnarchyChess.Moves;

//TODO: Make this into a record struct
/// <summary>
/// This class describes a single step of a move.
/// </summary>
public class MoveStep
{
    /// <summary>
    /// The origin of the move.
    /// </summary>
    public readonly Pos From;

    /// <summary>
    /// The destination of the move.
    /// </summary>
    public readonly Pos To;

    /// <summary>
    /// Describes what piece it gets promoted to after the move.
    /// Is null if it does not promote
    /// </summary>
    public readonly Type? PromotesTo;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from">The origin of the step</param>
    /// <param name="to">The destination of the step</param>
    /// <param name="promotesTo">What piece this step promotes to</param>
    public MoveStep(Pos from, Pos to, Type? promotesTo)
    {
        From = from;
        To = to;
        PromotesTo = promotesTo;
    }

    /// <summary>
    /// Get the relative movement of this move.
    /// This means how much it moves from the origin of the move.
    /// </summary>
    public Pos Offset => To - From;
}

