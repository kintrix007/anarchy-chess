using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Moves
{
    /// <summary>
    /// This class describes a move in a game.
    /// It can be comprised of multiple steps, represented as follow-up moves.
    /// It can be broken down into individual steps that need to be executed in order.
    /// </summary>
    public class AppliedMove : Resource
    {
        [NotNull] private readonly Pos _from;
        
        [NotNull] private readonly Pos _to;
        
        [NotNull, ItemNotNull] public readonly List<Pos> TakeList;
        
        public bool MustTake { get; private set; }
        
        [CanBeNull] private Type _promotesTo;
        
        [CanBeNull] private AppliedMove _followUp;

        /// <summary>
        /// Create a move based on the origin and the destination of the move.
        /// </summary>
        /// <param name="from">The origin of the move</param>
        /// <param name="to">The destination of the move</param>
        /// <returns></returns>
        [NotNull]
        public static AppliedMove Absolute([NotNull] Pos from, [NotNull] Pos to) => new AppliedMove(from, to);

        /// <summary>
        /// Create a move based on the origin and the offset of the move.
        /// </summary>
        /// <param name="from">The origin of the move</param>
        /// <param name="offset">The offset the piece should move by</param>
        /// <returns>The move</returns>
        [NotNull]
        public static AppliedMove Relative([NotNull] Pos from, [NotNull] Pos offset) => new AppliedMove(from, from + offset);

        /// <summary>
        /// Set this move to also capture the piece at the destination.
        /// </summary>
        /// <returns>This same move</returns>
        [NotNull]
        public AppliedMove Take(bool isTake = true)
        {
            if (isTake) TakeList.Add(_to);
            else        TakeList.Remove(_to);
            return this;
        }

        /// <summary>
        /// Set this move to only be possible if it captures the required pieces.
        /// </summary>
        /// <returns>This same move</returns>
        [NotNull]
        public AppliedMove Must(bool isMust = true)
        {
            MustTake = isMust;
            return this;
        }

        /// <summary>
        /// Describes what piece it gets promoted to after the move.
        /// It is null if it does not promote.
        /// </summary>
        /// <param name="piece">The piece to promote to</param>
        /// <returns>This same move</returns>
        /// <exception cref="ArgumentException">If the type specified does not represent a piece</exception>
        [NotNull]
        public AppliedMove PromoteTo([CanBeNull] Type piece)
        {
            if (piece != null && !piece.GetInterfaces().Contains(typeof(IPiece)))
            {
                throw new ArgumentException($"The type must represent a type of piece. '{piece}' does not.");
            }
            
            _promotesTo = piece;
            return this;
        }
        
        /// <summary>
        /// Add a follow-up move.
        /// </summary>
        /// <param name="appliedMove">The move that needs to happen after this one</param>
        /// <returns>This same move</returns>
        [NotNull]
        public AppliedMove AddFollowUp([NotNull] AppliedMove appliedMove)
        {
            _followUp = appliedMove;
            if (_followUp.TakeList.Count != 0)
            {
                throw new ArgumentException("The follow-up moves must not specify take lists.");
            }
            return this;
        }

        /// <summary>
        /// /// Add a position where this move would capture a piece.
        /// </summary>
        /// <param name="to">Absolute position where this move captures a piece</param>
        /// <returns>This same move</returns>
        [NotNull]
        public AppliedMove AddTake([NotNull, ItemNotNull] params Pos[] to)
        {
            TakeList.AddRange(to);
            return this;
        }

        /// <summary>
        /// Break down this move into steps.
        /// </summary>
        /// <returns>The steps as a list</returns>
        [NotNull, ItemNotNull]
        public List<MoveStep> GetSteps()
        {
            var steps = new List<MoveStep>();
            
            var move = this;
            while (move != null)
            {
                var step = new MoveStep(move._from, move._to, move._promotesTo);
                steps.Add(step);
                move = move._followUp;
            }

            return steps;
        }
        
        /// <summary>
        /// Create a move with follow-ups from a list of moves.
        /// The already existing follow-ups of these moves will be overridden.
        /// </summary>
        /// <param name="steps">The list of steps comprising the move</param>
        /// <returns>A single move with the follow-ups</returns>
        public static AppliedMove Fold(List<MoveStep> steps)
        {
            var move = steps[0].ToAppliedMove();
            var result = move;
            for (var i = 1; i < steps.Count; i++)
            {
                var followUp = steps[i].ToAppliedMove();
                move.AddFollowUp(followUp);
                move = followUp;
            }

            return result;
        }

        public override string ToString() => $"Move({_from} -> {_to})";

        /* --- Protected --- */

        protected AppliedMove([NotNull] Pos from, [NotNull] Pos to)
        {
            _from = from;
            _to = to;
            TakeList = new List<Pos>();
            MustTake = false;
            _followUp = null;
            _promotesTo = null;
        }

        /// <summary>
        /// Performs a deep copy of this move.
        /// </summary>
        /// <returns></returns>
        public AppliedMove Clone()
        {
            var steps = GetSteps().Select(x => x.Clone()).ToList();
            return AppliedMove.Fold(steps);
        }

        /* --- Private --- */

        private static AppliedMove InvertParams(AppliedMove appliedMove)
        {
            var inverse = new AppliedMove(appliedMove._to, appliedMove._from);
            if (appliedMove.MustTake) inverse.Must();
            appliedMove.TakeList.ForEach(x => inverse.AddTake(x));
            return inverse;
        }
    }
}
