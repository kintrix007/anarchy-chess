using System;
using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.scripts
{
    public class Move : Resource
    {
        [NotNull]
        public readonly Pos From;
        [NotNull]
        public readonly Pos To;
        [NotNull] [ItemNotNull]
        public readonly Pos[] Take;
        [CanBeNull]
        public Move FollowUp { get; private set; }

        public Move([NotNull] Pos from, [NotNull] Pos to) : this(from, to, new[] { to }) {}

        public Move([NotNull] Pos from, [NotNull] Pos to, [NotNull] [ItemNotNull] Pos[] take)
        {
            From = from;
            To = to;
            Take = take;
            FollowUp = null;
        }

        [NotNull]
        public Move AddFollowUp([NotNull] Move move)
        {
            FollowUp = move;
            return this;
        }

        public List<Move> Unfold()
        {
            var moves = new List<Move>();
            
            var next = this;
            while (next != null)
            {
                moves.Add(next);
                next = next.FollowUp;
            }

            return moves;
        }

        [NotNull]
        public static Move WithOffset([NotNull] Pos from, [NotNull] Pos offset) => new Move(from, from + offset);
        
        public override string ToString() => $"Move(to: {To})";
    }
}
