using System;
using AnarchyChess.Scripts.Moves;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Compatibility
{
    public static class Pos2Vector
    {
        public static Vector2 ToVector2([NotNull] this Pos pos) => new Vector2(pos.X, pos.Y);
        
        [NotNull]
        public static Pos ToPos(this Vector2 vec)
        {
            if (!vec.x.IsInteger() || !vec.y.IsInteger())
            {
                throw new ArgumentException("The x and y values must be integers");
            }

            return new Pos((int)Math.Round(vec.x), (int)Math.Round(vec.y));
        }

        private static bool IsInteger(this float num) => num % 1 <= float.Epsilon * 100;
    }
}
