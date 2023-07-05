using AnarchyChess.PieceHelper;

namespace AnarchyChess.Moves;

/// <summary>
/// This class describes a move in a game.
/// It can be comprised of multiple steps, represented as follow-up moves.
/// It can be broken down into individual steps that need to be executed in order.
/// <br />
/// You should not assume that the first move is made by the piece the move came from.
/// </summary>
public record class AppliedMove(
    IPiece Piece,
    Pos From,
    Pos To,
    List<Pos> CaptureList,
    bool MustBeACapture,
    Type? PromotesTo,
    AppliedMove? FollowUp)
{
    private Pos From { get; init; } = From;
    private Pos To { get; init; } = To;
    private Type? PromotesTo { get; init; } = PromotesTo;
    private AppliedMove? FollowUp { get; init; } = FollowUp;

    /// <summary>
    /// Break down this move into steps.
    /// </summary>
    /// <returns>The steps as a list</returns>
    public List<MoveStep> GetSteps()
    {
        var steps = new List<MoveStep>();

        var move = this;
        while (move != null)
        {
            var step = new MoveStep(move.From, move.To, move.PromotesTo);
            steps.Add(step);
            move = move.FollowUp;
        }

        return steps;
    }
}
