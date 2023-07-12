namespace AnarchyChess.Moves;

/// <summary>
/// Class defining a position in chess. It can be also used as a 2D integer vector.
/// </summary>
public readonly record struct Pos(int X, int Y) : IEquatable<Pos>
{
    /// <summary>
    /// Create a new position from a "chess notation".
    /// For example A1 would be Pos(0, 0), and D3 would be Pos(3, 2).
    /// </summary>
    /// <param name="pos">The position as a string</param>
    /// <exception cref="FormatException">If the position is a malformed string</exception>
    public Pos(string pos)
        : this(LetterToCoord(pos[0]), int.Parse(pos[1..]) - 1)
    {
        if (pos.Length != 2) throw new ArgumentException("Position should be described with an letter followed by a digit");
    }

    public static int LetterToCoord(char ch)
    {
        var upper = char.ToUpper(ch);
        if (!('A' <= upper && upper <= 'Z')) throw new ArgumentException("Character must be in range A..Z");
        return upper - 'A';
    }

    // ? This may be a terrible idea..?
    public static implicit operator Pos(string pos) => new(pos);

    public static Pos operator +(Pos a, Pos b) => new(a.X + b.X, a.Y + b.Y);
    public static Pos operator -(Pos a, Pos b) => a + -b;
    public static Pos operator -(Pos a) => new(-a.X, -a.Y);

    /// <summary>
    /// Return a new position with the absolute value of X and Y respectively.
    /// </summary>
    /// <returns>A new position</returns>
    public Pos Abs() => new(Math.Abs(X), Math.Abs(Y));

    /// <summary>
    /// Return a new position with X incremented by some value.
    /// </summary>
    /// <param name="x">Increment the X position by this much</param>
    /// <returns>The new position</returns>
    public Pos AddX(int x) => new(X + x, Y);

    /// <summary>
    /// Return a new position with Y incremented by some value.
    /// </summary>
    /// <param name="y">Increment the Y position by this much</param>
    /// <returns>The new position</returns>
    public Pos AddY(int y) => new(X, Y + y);

    /// <summary>
    /// Return a new position with X set to some value, leaving Y unchanged.
    /// </summary>
    /// <param name="x">The new value of X</param>
    /// <returns>The new position</returns>
    public Pos SetX(int x) => new(x, Y);

    /// <summary>
    /// Return a new position with Y set to some value, leaving X unchanged.
    /// </summary>
    /// <param name="y">The new value of Y</param>
    /// <returns>The new position</returns>
    public Pos SetY(int y) => new(X, y);

    /// <summary>
    /// Convert this position to a string in "chess notation".
    /// Positions that are invalid in chess, such as
    /// Pos(-1, -4), will look strange.
    /// </summary>
    /// <returns>The string representation of this position</returns>
    public string ToDebugString()
    {
        var charX = char.ToString((char)(X + 'A'));
        return $"<{charX}{Y + 1}>";
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }

    public override string ToString() => $"Pos({X}, {Y})";
}

