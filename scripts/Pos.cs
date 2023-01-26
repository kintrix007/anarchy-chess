using System;
using System.Diagnostics.Contracts;
using Godot;
using Object = Godot.Object;

namespace AnarchyChess.scripts
{
    public class Pos : Object
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

        public static Pos operator+(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y);

        public static Pos operator-(Pos a, Pos b) => a + -b;

        public static Pos operator-(Pos a) => new Pos(-a.X, -a.Y);

        public override string ToString() => $"Pos({X}, {Y})";

        private static int LetterToCoord(char ch)
        {
            Contract.Requires<ArgumentException>('a' <= ch && ch <= 'h' || 'A' <= ch && ch <= 'H');
            return ch.ToString().ToLower()[0] - 'a';
        }
    }
}
