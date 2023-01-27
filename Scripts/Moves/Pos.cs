using System;
using System.Diagnostics.Contracts;
using Godot;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Moves
{
    public class Pos : Object, IEquatable<Pos>
    {
        public readonly int X;
        public readonly int Y;

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

        public Pos AddX(int x) => new Pos(this.X + x, Y);
        public Pos AddY(int y) => new Pos(X, this.Y + y);
        public Pos SetX(int x) => new Pos(x, this.Y);
        public Pos SetY(int y) => new Pos(this.X, y);

        public static Pos operator+(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y);

        public static Pos operator-(Pos a, Pos b) => a + -b;

        public static Pos operator-(Pos a) => new Pos(-a.X, -a.Y);

        public override string ToString() => $"Pos({X}, {Y})";

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pos)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}
