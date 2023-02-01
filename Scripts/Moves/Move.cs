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
        /// <summary>
        /// The origin of the move.
        /// </summary>
        [NotNull] public readonly Pos From;

        /// <summary>
        /// The destination of the move.
        /// </summary>
        [NotNull] public readonly Pos To;

        /// <summary>
        /// The list of positions where the pieces will get taken by this move.
        /// This means that ALL of the pieces on these positions will be captured by this move.
        /// </summary>
        [NotNull] [ItemNotNull] public readonly List<Pos> TakeList;

        /// <summary>
        /// Whether on not it is required that this move captures on all the positions on the take list.
        /// For example the Pawn's capturing move can only happen if it actually captures a piece.
        /// </summary>
        public bool IsMustTake { get; private set; }

        /// <summary>
        /// The follow-up happens right after this move, and they can only happen together.
        /// For example, for castling would be a move that has a follow-up.
        /// The first move would be the king, and the follow-up would be the rook moving in the correct place.
        /// </summary>
        [CanBeNull]
        public Move FollowUp { get; private set; }

        /// <summary>
        /// Get the relative movement of this move.
        /// This means how much it moves from the origin of the move.
        /// </summary>
        public Pos AsRelative => To - From;

        /// <summary>
        /// Create a move based on the origin and the destination of the move.
        /// </summary>
        /// <param name="from">The origin of the move</param>
        /// <param name="to">The destination of the move</param>
        /// <returns></returns>
        [NotNull]
        public static Move Absolute([NotNull] Pos from, [NotNull] Pos to) => new Move(from, to);

        /// <summary>
        /// Create a move based on the origin and the offset of the move.
        /// </summary>
        /// <param name="from">The origin of the move</param>
        /// <param name="offset">The offset the piece should move by</param>
        /// <returns>The move</returns>
        [NotNull]
        public static Move Relative([NotNull] Pos from, [NotNull] Pos offset) => new Move(from, from + offset);

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

        /// <summary>
        /// Create a move with follow-ups from a list of moves.
        /// The already existing follow-ups of these moves will be overridden.
        /// </summary>
        /// <param name="moveList">The list of moves</param>
        /// <returns>A single move with the follow-ups</returns>
        public static Move Fold(List<Move> moveList)
        {
            for (int i = 1; i < moveList.Count; i++)
            {
                moveList[i - 1].AddFollowUp(moveList[i]);
            }

            return moveList[0];
        }

        public override string ToString() => $"Move({From} -> {To})";

        /* --- Protected --- */

        protected Move([NotNull] Pos from, [NotNull] Pos to)
        {
            From = from;
            To = to;
            TakeList = new List<Pos>();
            IsMustTake = false;
            FollowUp = null;
        }

        /* --- Private --- */

        private static Move InvertParams(Move move)
        {
            var inverse = new Move(move.To, move.From);
            if (move.IsMustTake) inverse.Must();
            move.TakeList.ForEach(x => inverse.AddTake(x));
            return inverse;
        }
    }
}