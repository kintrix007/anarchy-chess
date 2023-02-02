using System;
using Godot;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Moves
{
    /// <summary>
    /// Class defining a position in chess. It can be also used as a 2D integer vector.
    /// </summary>
    public class Pos : Object, IEquatable<Pos>
    {
        /// <summary>
        /// The x coordinate on the board corresponding to the labels A to H.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// The y coordinate on the board correspoinding to the labels 1 to 8.
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

        public Pos(char x, int y) : this(LetterToCoord(x), y - 1) {}

        public Pos(string pos) : this(pos[0], int.Parse(pos.Substring(1)))
        {
            if (pos.Length != 2) throw new ArgumentException();
            if (!pos.Substring(1).IsValidInteger()) throw new ArgumentException();
        }

        public static Pos operator+(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y);
        public static Pos operator-(Pos a, Pos b) => a + -b;
        public static Pos operator-(Pos a) => new Pos(-a.X, -a.Y);
        public static bool operator==(Pos a, Pos b) => a?.Equals(b) ?? false;
        public static bool operator!=(Pos a, Pos b) => !(a == b);

        public Pos Abs() => new Pos(Math.Abs(X), Math.Abs(Y));
        public Pos AddX(int x) => new Pos(X + x, Y);
        public Pos AddY(int y) => new Pos(X, Y + y);
        public Pos SetX(int x) => new Pos(x, Y);
        public Pos SetY(int y) => new Pos(X, y);

        private static int LetterToCoord(char ch)
        {
            if (!(('a' <= ch && ch <= 'h') || ('A' <= ch && ch <= 'H'))) throw new ArgumentException();
            return char.ToLower(ch) - 'a';
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pos)obj);
        }

        public bool Equals(Pos other)
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

        public string ToChessString() => $"<{char.ToString((char)(X + 'A'))}{Y + 1}>";

        public override string ToString() => $"Pos({X}, {Y})";
    }
}