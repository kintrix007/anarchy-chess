using System;

namespace AnarchyChess.Moves
{
    /// <summary>
    /// Class defining a position in chess. It can be also used as a 2D integer vector.
    /// </summary>
    public class Pos : IEquatable<Pos>
    {
        /// <summary>
        /// The x coordinate on the board corresponding to the labels A to H.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// The y coordinate on the board corresponding to the labels 1 to 8.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Create a new position.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Create a new position from a "chess notation".
        /// For example A1 would be Pos(0, 0), and D3 would be Pos(3, 2).
        /// </summary>
        /// <param name="pos">The position as a string</param>
        /// <exception cref="ArgumentException">If the position is a malformed string</exception>
        public Pos(string pos) : this(pos[0], int.Parse(pos[1..]))
        {
            if (pos.Length != 2) throw new ArgumentException("Position should be described with an letter followed by a digit");
            if (!int.TryParse(pos[1..], out _)) throw new ArgumentException("Invalid digit in position description");
        }

        public static Pos operator+(Pos a, Pos b) => new(a.X + b.X, a.Y + b.Y);
        public static Pos operator-(Pos a, Pos b) => a + -b;
        public static Pos operator-(Pos a) => new(-a.X, -a.Y);
        public static bool operator==(Pos a, Pos b) => a?.Equals(b) ?? false;
        public static bool operator!=(Pos a, Pos b) => !(a == b);

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
        /// Invalid positions such as Pos(-1, -4) will look strange.
        /// </summary>
        /// <returns>The string representation of this position</returns>
        public string ToChessString() => $"<{char.ToString((char)(X + 'A'))}{Y + 1}>";

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pos)obj);
        }

        public bool Equals(Pos? other)
        {
            if (other is null) return false;
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString() => $"Pos({X}, {Y})";

        public Pos Clone() => new(X, Y);

        /* --- Protected --- */

        protected Pos(char x, int y) : this(LetterToCoord(x), y - 1) {}

        /* --- Private --- */

        private static int LetterToCoord(char ch)
        {
            var upper = char.ToUpper(ch);
            if (!('A' <= upper && upper <= 'Z')) throw new ArgumentException("Character must be in range A..Z");
            return upper - 'A';
        }
    }
}
