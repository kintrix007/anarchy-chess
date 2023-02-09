using System;
using System.Collections.Generic;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Moves
{
    /// <summary>
    /// This class describes a single step of a move.
    /// </summary>
    public class MoveStep : Resource
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
        [NotNull, ItemNotNull] public readonly List<Pos> TakeList;

        /// <summary>
        /// Whether on not it is required that this move captures on all the positions on the take list.
        /// For example the Pawn's capturing move can only happen if it actually captures a piece.
        /// </summary>
        public readonly bool MustTake;

        /// <summary>
        /// Describes what piece it gets promoted to after the move.
        /// Is null if it does not promote
        /// </summary>
        [CanBeNull] public readonly Type PromotesTo;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">The origin of the step</param>
        /// <param name="to">The destination of the step</param>
        /// <param name="takeList">The position to capture the pieces on</param>
        /// <param name="mustTake">Whether this step can only happen if it captures all possible pieces</param>
        /// <param name="promotesTo">What piece this step promotes to</param>
        public MoveStep([NotNull] Pos from, [NotNull] Pos to, [NotNull, ItemNotNull] List<Pos> takeList, bool mustTake, [CanBeNull] Type promotesTo)
        {
            From = from;
            To = to;
            TakeList = takeList;
            MustTake = mustTake;
            PromotesTo = promotesTo;
        }

        /// <summary>
        /// Get the relative movement of this move.
        /// This means how much it moves from the origin of the move.
        /// </summary>
        public Pos RelativeMovement => To - From;

        public AppliedMove ToAppliedMove()
        {
            var move = AppliedMove.Absolute(From, To);
            move.AddTake(TakeList.ToArray());
            if (MustTake) move.Must();
            move.PromoteTo(PromotesTo);
            
            return move;
        }
    }
}
