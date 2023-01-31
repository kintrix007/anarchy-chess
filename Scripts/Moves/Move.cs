using System.Collections.Generic;
using System.Linq;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Moves
{
    /// <summary>
    /// Class defining a move in chess.
    /// </summary>
    public class Move : Resource
    {
        /// The origin of the move.
        [NotNull] public readonly Pos From;

        /// The destination of the move.
        [NotNull] public readonly Pos To;

        ///The list of positions where the pieces will get taken by this move.
        [NotNull] [ItemNotNull] public readonly List<Pos> TakeList;

        /// Whether on not it is required that this move captures on all the positions of the take list.
        /// For example for Il Vaticano this would be true.
        public bool IsMustTake { get; private set; }

        /// A move that has to happen after this move.
        /// For example, for castling the rook's move would be a follow-up.
        [CanBeNull]
        public Move FollowUp { get; private set; }

        /// Get the relative offset from the origin to the destination of the move.
        public Pos Relative => To - From;

        /// <summary>
        /// Create a move with an origin and a destination.
        /// </summary>
        /// <param name="from">The origin of the move</param>
        /// <param name="to">The destination of the move</param>
        public Move([NotNull] Pos from, [NotNull] Pos to)
        {
            From = from;
            To = to;
            TakeList = new List<Pos>();
            IsMustTake = false;
            FollowUp = null;
        }

        /// <summary>
        /// Create a move based on the relative offset of the movement.
        /// </summary>
        /// <param name="from">The origin of the move</param>
        /// <param name="offset">The offset the piece should move by</param>
        /// <returns>The move</returns>
        [NotNull]
        public static Move MakeRelative([NotNull] Pos from, [NotNull] Pos offset) => new Move(from, from + offset);

        /// <summary>
        /// Set this move to also capture the piece at the destination.
        /// </summary>
        /// <returns>This same move</returns>
        [NotNull]
        public Move Take()
        {
            TakeList.Add(To);
            return this;
        }

        /// <summary>
        /// Set this move to only be possible if it captures the required pieces.
        /// </summary>
        /// <returns>This same move</returns>
        [NotNull]
        public Move Must()
        {
            IsMustTake = true;
            return this;
        }

        /// <summary>
        /// Add a follow-up move.
        /// </summary>
        /// <param name="move">The move that needs to happen after this one</param>
        /// <returns>This same move</returns>
        [NotNull]
        public Move AddFollowUp([NotNull] Move move)
        {
            FollowUp = move;
            return this;
        }

        /// <summary>
        /// /// Add a position where this move would capture a piece.
        /// </summary>
        /// <param name="to">Absolute position where this move captures a piece</param>
        /// <returns>This same move</returns>
        [NotNull]
        public Move AddTake([NotNull] Pos to)
        {
            TakeList.Add(to);
            return this;
        }

        /// <summary>
        /// Get the inverse of a move. This reverses the order of the moves if there are follow-ups,
        /// and flips around the movement direction.
        /// </summary>
        /// <returns>The inverse of the move</returns>
        [NotNull]
        public Move Inverse()
        {
            var moveList = Unfold().Select(InvertParams).Reverse().ToList();
            var inverseMove = Fold(moveList);
            return inverseMove;
        }

        private static Move InvertParams(Move move)
        {
            var inverse = new Move(move.To, move.From);
            if (move.IsMustTake) inverse.Must();
            move.TakeList.ForEach(x => inverse.AddTake(x));
            return inverse;
        }

        /// <summary>
        /// Unfold the follow-up moves into a list for ease of use.
        /// </summary>
        /// <returns>The list of this move with its follow-up moves as well</returns>
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

        public static Move Fold(List<Move> moveList)
        {
            for (int i = 1; i < moveList.Count; i++)
            {
                moveList[i - 1].AddFollowUp(moveList[i]);
            }

            return moveList[0];
        }

        public override string ToString() => $"Move({From} -> {To})";
    }
}
