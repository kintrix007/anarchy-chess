using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games.Chess;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Object = Godot.Object;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Games
{
    /// <summary>
    /// This object represents a game in a given state.
    /// </summary>
    public class Game : Resource
    {
        [Godot.Signal]
        public delegate void GameCreated([NotNull] Game game);

        [Godot.Signal]
        public delegate void PieceMoved([NotNull] Game game, [NotNull] MoveStep step);

        // Cannot use interface types (like `IPiece`) in signal signatures...
        [Godot.Signal]
        public delegate void PieceRemoved([NotNull] Game game, [NotNull] Pos pos, [NotNull] Object piece);

        [Godot.Signal]
        public delegate void PieceAdded([NotNull] Game game, [NotNull] Pos pos, [NotNull] Object piece);

        [NotNull] public readonly PieceToAscii PieceToAsciiRegistry;

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
        public List<HistoryMove> MoveHistory { get; private set; }

        /// <summary>
        /// The move validator of this game.
        /// </summary>
        [NotNull] public readonly IMoveValidator Validator;

        /// <summary>
        /// The last move of the game.
        /// </summary>
        [CanBeNull]
        public HistoryMove LastAppliedMove => MoveHistory.Count <= 0 ? null : MoveHistory.Last();

        /// <summary>
        /// Should never be used, but Godot is complaining about the lack of its existence.
        /// </summary>
        /// <exception cref="Exception">Is always thrown</exception>
        public Game() => throw new Exception();

        /// <summary>
        /// Create a new game with played on a board and optionally with a move validator.
        /// If a validator is not specified, it will use the standard chess move validator.
        /// </summary>
        /// <param name="board">The board the game is played on</param>
        /// <param name="registry">The piece to character registry</param>
        /// <param name="validator">Validator used to validate the moves</param>
        public Game([NotNull] Board board, [CanBeNull] PieceToAscii registry = null,
            [CanBeNull] IMoveValidator validator = null)
        {
            Board = board;
            PieceToAsciiRegistry = registry ?? new PieceToAscii();
            Validator = validator ?? new ChessStandardValidator();
            MoveHistory = new List<HistoryMove>();
            Scores = new Dictionary<Side, int> {
                { Side.White, 0 },
                { Side.Black, 0 },
            };

            board.Connect(nameof(Board.PieceAdded), this, nameof(OnPieceAdded));
            board.Connect(nameof(Board.PieceRemoved), this, nameof(OnPieceRemoved));
        }

        /// <summary>
        /// Create the game. Only used to send the signal that it has been created.
        /// </summary>
        public void Create()
        {
            EmitSignal(nameof(GameCreated), this);
        }

        /// <summary>
        /// Execute a move in the game.
        /// </summary>
        /// <param name="move">The move to execute</param>
        /// <exception cref="NullReferenceException"></exception>
        public void ApplyMove([NotNull] AppliedMove move)
        {
            var steps = move.GetSteps();
            var stepTuples = steps.Select(x => (x.From, x.To)).ToList();
            
            Board.ApplySteps(stepTuples, (i, piece) => {
                var step = steps[i];
                piece.MoveCount++;
                var taken = step.TakeList.Select(Board.RemovePiece).Where(x => x != null).ToList();
                taken.ForEach(x => Scores[x.Side] += x.Cost);

                if (step.PromotesTo == null)
                {
                    //? Not sure if this is a good idea, as now we emit the move signal before the piece is actually moved.
                    EmitSignal(nameof(PieceMoved), this, step);
                }
                else
                {
                    var pieceCtor = step.PromotesTo.GetConstructor(new[] { typeof(Side) });
                    if (pieceCtor == null) throw new NullReferenceException();
                    var promoted = (IPiece)pieceCtor.Invoke(new object[] { piece.Side });

                    Board.RemovePiece(step.From);
                    Board.AddPiece(step.To, promoted);
                }
            });
            
            MoveHistory.Add(new HistoryMove(move));
        }

        /// <summary>
        /// Get all the moves a given side can make. These moves remain to be validated.
        /// </summary>
        /// <param name="side">The side to get the moves of</param>
        /// <returns>The moves</returns>
        public IEnumerable<AppliedMove> GetAllMoves(Side side)
        {
            var moves = new List<AppliedMove>();
            foreach (var (pos, piece) in Board)
            {
                if (piece.Side != side) continue;
                moves.AddRange(piece.GetMoves(this, pos));
            }

            return moves;
        }

        /// <summary>
        /// Determine whether a giver side has at least one valid move.
        /// </summary>
        /// <param name="side">The side to check</param>
        /// <returns>Whether there is at least one valid move</returns>
        public bool HasValidMove(Side side) => GetAllMoves(side).Any(IsValidMove);

        /// <summary>
        /// Validate a move with this game's validator.
        /// </summary>
        /// <param name="appliedMove">The move to validate</param>
        /// <returns>Whether the move is valid</returns>
        public bool IsValidMove([NotNull] AppliedMove appliedMove) => Validator.Validate(this, appliedMove);

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
            var gameClone = new Game(Board.Clone(), registry: PieceToAsciiRegistry, validator: Validator);

            //? This only makes a shallow copy, but since the elements are immutable that's fine.
            gameClone.MoveHistory = MoveHistory.ToList();
            gameClone.Scores[Side.White] = Scores[Side.White];
            gameClone.Scores[Side.Black] = Scores[Side.Black];

            return gameClone;
        }

        private void OnPieceAdded([NotNull] Pos pos, [NotNull] Object piece)
        {
            EmitSignal(nameof(PieceAdded), this, pos, piece);
        }

        private void OnPieceRemoved([NotNull] Pos pos, [NotNull] Object piece)
        {
            EmitSignal(nameof(PieceRemoved), this, pos, piece);
        }
    }
}
