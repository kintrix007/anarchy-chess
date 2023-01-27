using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Moves
{
    public class Move : Resource
    {
        [NotNull] public readonly Pos From;
        [NotNull] public readonly Pos To;
        [NotNull] [ItemNotNull] public List<Pos> TakeList { get; private set; }
        public bool IsMustTake { get; private set; }
        [CanBeNull] public Move FollowUp { get; private set; }

        public Move([NotNull] Pos from, [NotNull] Pos to)
        {
            From = from;
            To = to;
            TakeList = new List<Pos>();
            IsMustTake = false;
            FollowUp = null;
        }

        [NotNull]
        public Move Takes()
        {
            TakeList.Add(To);
            return this;
        }

        [NotNull]
        public Move Must()
        {
            IsMustTake = true;
            return this;
        }
        
        [NotNull]
        public Move AddFollowUp([NotNull] Move move)
        {
            FollowUp = move;
            return this;
        }

        [NotNull]
        public Move AddTake(Pos to)
        {
            TakeList.Add(to);
            return this;
        }

        [NotNull]
        [ItemNotNull]
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
        public static Move Relative([NotNull] Pos from, [NotNull] Pos offset) => new Move(from, from + offset);

        public override string ToString() => $"Move(to: {To})";
    }
}
