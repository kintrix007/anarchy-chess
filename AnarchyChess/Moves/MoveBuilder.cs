using AnarchyChess.PieceHelper;

namespace AnarchyChess.Moves;

public class MoveBuilder : StepBuilder
{
    public List<Pos> CaptureList;
    public bool MustBeACapture;

    private MoveBuilder(Pos from, Pos to) : base(from, to)
    {
        CaptureList = new();
        MustBeACapture = false;
    }

    public static new MoveBuilder Absolute(Pos from, Pos to)
    {
        return new(from, to);
    }

    public static new MoveBuilder Relative(Pos from, Pos to)
    {
        return new(from, from + to);
    }

    public new MoveBuilder PromoteTo(Type? piece)
    {
        return (MoveBuilder)base.PromoteTo(piece);
    }

    public new MoveBuilder AddFollowUp(StepBuilder builder)
    {
        return (MoveBuilder)base.AddFollowUp(builder);
    }

    public MoveBuilder Must()
    {
        MustBeACapture = true;
        return this;
    }

    public MoveBuilder Capture()
    {
        CaptureList.Add(To);
        return this;
    }

    public MoveBuilder CaptureAt(params Pos[] positions)
    {
        CaptureList.AddRange(positions);
        return this;
    }

    public new MoveBuilder Clone()
    {
        var clone = (MoveBuilder)base.Clone();
        if (MustBeACapture) clone.Must();
        CaptureList.ForEach(x => clone.CaptureAt(x));
        return clone;
    }

    public new AppliedMove Build(IPiece piece) => new(
            piece,
            From,
            To,
            CaptureList,
            MustBeACapture,
            Promotion,
            FollowUp?.Build(piece));
}

public class StepBuilder
{
    public Pos From;
    public Pos To;
    public Type? Promotion;
    public StepBuilder? FollowUp;

    public static StepBuilder Absolute(Pos from, Pos to) => new(from, to);

    public static StepBuilder Relative(Pos from, Pos to) => new(from, from + to);

    public StepBuilder PromoteTo(Type? piece)
    {
        if (piece != null && piece.GetInterfaces().Contains(typeof(IPiece)))
        {
            throw new ArgumentException($"Expected a type that implements IPiece. Got {piece}");
        }
        Promotion = piece;
        return this;
    }

    public StepBuilder AddFollowUp(StepBuilder builder)
    {
        FollowUp = builder;
        return this;
    }

    public StepBuilder Clone()
    {
        var clone = new StepBuilder(From, To);
        if (Promotion != null) clone.PromoteTo(Promotion);
        if (FollowUp != null) clone.AddFollowUp(FollowUp.Clone());
        throw new NotImplementedException();
    }

    public AppliedMove Build(IPiece piece) => new(
            piece,
            From,
            To,
            new(),
            false,
            Promotion,
            FollowUp?.Build(piece));

    protected StepBuilder(Pos from, Pos to)
    {
        From = from;
        To = to;
        Promotion = null;
        FollowUp = null;
    }
}
