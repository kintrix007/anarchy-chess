using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Boards;
using AnarchyChess.Games.Chess;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Games
{
    /// <summary>
    /// This object represents a game in a given state.
    /// </summary>
    public class Game : Resource
    {
        [Signal]
        public delegate void GameCreated(Game game);

        [Signal]
        public delegate void PieceMoved(Game game, MoveStep step);

        // Cannot use interface types (like `IPiece`) in signal signatures...
        [Signal]
        public delegate void PieceRemoved(Game game, Pos pos, Object piece);

        [Signal]
        public delegate void PieceAdded(Game game, Pos pos, Object piece);

        [Signal]
        public delegate void PiecePromoted(Game game, MoveStep step, Object piece);
        
        public readonly PieceToAscii PieceToAsciiRegistry;

        /// <summary>
        /// The board this game is played on.
        /// </summary>
        public readonly Board Board;

        /// <summary>
        /// The actual score of each side.
        /// </summary>
        
        public Dictionary<Side, int> Scores { get; private set; }

        /// <summary>
        /// The list of moves that happened during this game.
        /// </summary>
        
        public List<HistoryMove> MoveHistory { get; private set; }

        /// <summary>
        /// The move validator of this game.
        /// </summary>
        public readonly IMoveValidator Validator;

        /// <summary>
        /// The last move of the game.
        /// </summary>
        public HistoryMove? LastMove => MoveHistory.Count <= 0 ? null : MoveHistory.Last();

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
        public Game(Board board, PieceToAscii? registry = null,
            IMoveValidator? validator = null)
        {
            Board = board;
            PieceToAsciiRegistry = registry ?? new PieceToAscii();
            Validator = validator ?? new ChessStandardValidator();
            MoveHistory = new List<HistoryMove>();
            Scores = new Dictionary<Side, int> {
                { Side.White, 0 },
                { Side.Black, 0 },
            };
        }

        /// <summary>
        /// Create the game. Only used to send the signal that it has been created.
        /// </summary>
        public void Create()
        {
            EmitSignal(nameof(GameCreated), this);
        }

        /// <summary>
        /// Remove a piece from the board at a given position. After removing it, return it.
        /// </summary>
        /// <param name="pos">The position to remove it at</param>
        /// <returns>The removed piece</returns>
        public IPiece? RemovePiece(Pos pos)
        {
            var piece = Board.RemovePiece(pos);
            if (piece != null) EmitSignal(nameof(PieceRemoved), this, pos, piece);
            return piece;
        }
        
        public void AddPiece(Pos pos, IPiece piece)
        {
            Board.AddPiece(pos, piece);
            EmitSignal(nameof(PieceAdded), this, pos, piece);
        }
        
        /// <summary>
        /// Execute a move in the game.
        /// </summary>
        /// <param name="move">The move to execute</param>
        /// <exception cref="NullReferenceException"></exception>
        public void ApplyMove(AppliedMove move)
        {
            var steps = move.GetSteps();
            
            var taken = move.TakeList.Select(RemovePiece).Where(x => x != null).ToList();
            taken.ForEach(x => Scores[x.Side] += x.Cost);
            
            Board.ApplySteps(steps, (piece, step) => {
                piece.MoveCount++;

                if (step.PromotesTo == null)
                {
                    EmitSignal(nameof(PieceMoved), this, step);
                    return;
                }
                
                var pieceCtor = step.PromotesTo.GetConstructor(new[] { typeof(Side) });
                if (pieceCtor == null) throw new NullReferenceException();
                var promoted = (IPiece)pieceCtor.Invoke(new object[] { piece.Side });

                Board.ReplacePiece(step.To, promoted);
                EmitSignal(nameof(PiecePromoted), this, step, promoted);

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
        public bool IsValidMove(AppliedMove appliedMove) => Validator.Validate(this, appliedMove);

        /// <summary>
        /// As it stands currently, you should not assume a completely accurate clone.
        /// What you can assume is: <br/>
        /// - The board and the pieces are cloned <br/>
        /// - The scores are cloned <br/>
        /// </summary>
        /// <returns>A copy of the game</returns>
        
        public Game Clone()
        {
            var gameClone = new Game(Board.Clone(), registry: PieceToAsciiRegistry, validator: Validator);

            //? This only makes a shallow copy, but since the elements are immutable that's fine.
            gameClone.MoveHistory = MoveHistory.ToList();
            gameClone.Scores[Side.White] = Scores[Side.White];
            gameClone.Scores[Side.Black] = Scores[Side.Black];

            return gameClone;
        }
    }
}
