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
        /// Describes what piece it gets promoted to after the move.
        /// Is null if it does not promote
        /// </summary>
        [CanBeNull] public readonly Type PromotesTo;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">The origin of the step</param>
        /// <param name="to">The destination of the step</param>
        /// <param name="promotesTo">What piece this step promotes to</param>
        public MoveStep([NotNull] Pos from, [NotNull] Pos to, [CanBeNull] Type promotesTo)
        {
            From = from;
            To = to;
            PromotesTo = promotesTo;
        }

        /// <summary>
        /// Get the relative movement of this move.
        /// This means how much it moves from the origin of the move.
        /// </summary>
        public Pos Offset => To - From;

        /// <summary>
        /// Create a new AppliedMove object with a single step that holds the date of this step.
        /// </summary>
        /// <returns>The new AppliedMove object</returns>
        public AppliedMove ToAppliedMove() => AppliedMove.Absolute(From, To).PromoteTo(PromotesTo);

        /// <summary>
        /// Performs a deep copy of this step.
        /// </summary>
        /// <returns></returns>
        public MoveStep Clone() => new MoveStep(From.Clone(), To.Clone(), PromotesTo);
    }
}
