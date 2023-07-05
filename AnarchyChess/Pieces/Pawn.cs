using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces;

//TODO: Rewrite promotion as it is completely a big mess
public class Pawn : IPiece, IPromotable
{
    public int Cost => 1;
    public Side Side { get; }
    public int MoveCount { get; set; }
    public List<Type> Promotions =>
        new() { typeof(Queen), typeof(Rook), typeof(Knight), typeof(Bishop), typeof(Knook) };


    public Pawn(Side side)
    {
        Side = side;
        MoveCount = 0;
    }

    public IEnumerable<MoveBuilder> GetMoveBuilders(Game game, Pos pos)
    {
        var moves = new List<MoveBuilder>();
        moves.AddRange(NormalMove(game, pos));
        moves.AddRange(EnPassant(game, pos));

        var withPromotion = moves
            .SelectMany(x => ShouldBePromotion(x)
                ? Promotions.Select(p => x.Clone().PromoteTo(p))
                : new[] { x }
            );

        return withPromotion;
    }

    public IEnumerable<MoveBuilder> NormalMove(Game game, Pos pos)
    {
        var moves = new List<MoveBuilder>();

        var facing = this.Side == Side.White ? 1 : -1;
        moves.Add(MoveBuilder.Relative(pos, new Pos(0, facing)));

        // Can go two tiles if it hasn't moved and there is nothing in front of it.
        if (this.MoveCount == 0 && game.Board[pos.AddY(facing)] == null)
        {
            moves.Add(MoveBuilder.Relative(pos, new Pos(0, 2 * facing)));
        }

        moves.Add(MoveBuilder.Relative(pos, new Pos(-1, facing)).Must().Capture());
        moves.Add(MoveBuilder.Relative(pos, new Pos(1, facing)).Must().Capture());

        return moves;
    }

    /// Defined in a way that it works even if the pawn did not start at the pawn base line

    public static IEnumerable<MoveBuilder> EnPassant(Game game, Pos pos)
    {
        var moves = new List<MoveBuilder>();
        if (game.LastMove == null) return moves;

        moves.AddRange(InternalEnPassant(true, game, pos));
        moves.AddRange(InternalEnPassant(false, game, pos));

        return moves;
    }

    private static IEnumerable<MoveBuilder> InternalEnPassant(bool isLeft, Game game, Pos pos)
    {
        var piece = game.Board[pos];
        var moves = new List<MoveBuilder>();
        if (piece == null) return moves;

        var facing = piece.Side == Side.White ? 1 : -1;
        var direction = isLeft ? -1 : 1;
        var opponentPawnPos = pos.AddX(direction);

        if (game.Board[opponentPawnPos] is not Pawn p) return moves;
        if (p.Side == piece.Side) return moves;
        if (p.MoveCount != 1) return moves;
        if (game.LastMove == null) return moves;
        if (game.LastMove.Steps.Count != 1) return moves;
        var lastStep = game.LastMove.Steps[0];

        if (lastStep.To != opponentPawnPos) return moves;
        if (lastStep.Offset.Abs() != new Pos(0, 2)) return moves;

        moves.Add(MoveBuilder.Relative(pos, new Pos(direction, facing))
            .Must().CaptureAt(opponentPawnPos));

        return moves;
    }

    private static bool ShouldBePromotion(MoveBuilder appliedMove)
    {
        var steps = appliedMove.Build().GetSteps();
        if (steps.Count != 1) return false;

        var step = steps[0];
        return step.To.Y == 0 || step.To.Y == 7;
    }
}

