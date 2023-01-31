using System;
using System.Diagnostics.Contracts;
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

        public Pos(char x, int y) : this(LetterToCoord(x), y) {}

        public Pos(string pos) : this(LetterToCoord(pos[0]), int.Parse(pos.Substring(1)))
        {
            Contract.Requires<ArgumentException>(pos.Length == 2);
            Contract.Requires<ArgumentException>(pos.Substring(1).IsValidInteger());
        }

        public static Pos operator+(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y);
        public static Pos operator-(Pos a, Pos b) => a + -b;
        public static Pos operator-(Pos a) => new Pos(-a.X, -a.Y);

        public Pos Abs() => new Pos(Math.Abs(X), Math.Abs(Y));
        public Pos AddX(int x) => new Pos(this.X + x, Y);
        public Pos AddY(int y) => new Pos(X, this.Y + y);
        public Pos SetX(int x) => new Pos(x, this.Y);
        public Pos SetY(int y) => new Pos(this.X, y);

        private static int LetterToCoord(char ch)
        {
            Contract.Requires<ArgumentException>('a' <= ch && ch <= 'h' || 'A' <= ch && ch <= 'H');
            return ch.ToString().ToLower()[0] - 'a';
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