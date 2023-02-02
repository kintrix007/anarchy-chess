using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Games
{
    /// <summary>
    /// This object represents a game in a given state.
    /// </summary>
    public class Game : Resource
    {
        /// <summary>
        /// The board this game is played on.
        /// </summary>
        [NotNull] public readonly Board Board;

        /// <summary>
        /// The actual score of each side.
        /// </summary>
        [NotNull]
        public Dictionary<Side, int> Scores { get; private set; }

        /// <summary>
        /// The list of moves that happened during this game.
        /// </summary>
        [NotNull, ItemNotNull]
        public List<Move> MoveHistory { get; private set; }

        /// <summary>
        /// The move validator of this game.
        /// </summary>
        [NotNull] public readonly IMoveValidator Validator;

        /// <summary>
        /// The last move of the game.
        /// </summary>
        [CanBeNull]
        public Move LastMove => MoveHistory.Count <= 0 ? null : MoveHistory.Last();

        /// <summary>
        /// Create a new game with played on a board and optionally with a move validator.
        /// If a validator is not specified, it will use the standard chess move validator.
        /// </summary>
        /// <param name="board">The board the game is played on</param>
        /// <param name="validator">Validator used to validate the moves</param>
        public Game([NotNull] Board board, [CanBeNull] IMoveValidator validator = null)
        {
            Board = board;
            Validator = validator ?? new StandardMoveValidator();
            MoveHistory = new List<Move>();
            Scores = new Dictionary<Side, int> {
                { Side.White, 0 },
                { Side.Black, 0 },
            };
        }

        /// <summary>
        /// Execute a move to the board.
        /// </summary>
        /// <param name="move">The move to apply</param>
        /// <param name="shouldValidate">Whether the move should be validated</param>
        /// <returns>Whether the move succeeded</returns>
        //? Might be a good idea to consider, instead, throwing an exception if unsuccessful
        public bool ApplyMove([NotNull] Move move, bool shouldValidate = true)
        {
            var movingPiece = Board[move.From];
            if (movingPiece == null) return false;
            if (shouldValidate && !ValidateMove(move)) return false;

            var taken = move.Unfold()
                .SelectMany(x => x.TakeList.Select(p => Board.RemovePiece(p)))
                .Where(x => x != null)
                .ToList();

            taken.ForEach(x => Scores[movingPiece.Side] += x.Cost);
            movingPiece.MoveCount++;
            MoveHistory.Add(move);

            Board.InternalApplyMove(move);
            return true;
        }

        /// <summary>
        /// Validate a move with this game's validator.
        /// </summary>
        /// <param name="move">The move to validate</param>
        /// <returns>Whether the move is valid</returns>
        public bool ValidateMove([NotNull] Move move) => Validator.Validate(this, move);

        /// <summary>
        /// As it stands currently, you should not assume a completely accurate clone.
        /// What you can assume is: <br/>
        /// - The board and the pieces are cloned <br/>
        /// - The scores are cloned <br/>
        /// </summary>
        /// <returns>A copy of the game</returns>
        [NotNull]
        public Game Clone()
        {
            var gameClone = new Game(Board.Clone(), Validator);

            // I think this should make a shallow copy
            gameClone.MoveHistory = MoveHistory.ToList();
            gameClone.Scores[Side.White] = Scores[Side.White];
            gameClone.Scores[Side.Black] = Scores[Side.Black];

            return gameClone;
        }
    }
}