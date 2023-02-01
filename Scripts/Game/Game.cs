using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Game
{
    public class Game : Resource
    {
        /// <summary>
        /// The board this game is played on.
        /// </summary>
        [NotNull]
        public Board Board { get; private set; }

        /// <summary>
        /// The actual score of each side.
        /// </summary>
        [NotNull]
        public Dictionary<Side, int> Scores { get; private set; }

        /// <summary>
        /// The list of moves that happened during this game.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public List<Move> MoveList { get; private set; }

        /// <summary>
        /// The last move of the game.
        /// </summary>
        [CanBeNull]
        public Move LastMove => MoveList.Count <= 0 ? null : MoveList.Last();

        public Game() : this(new Board()) {}

        public Game([NotNull] Board board)
        {
            Board = board;
            MoveList = new List<Move>();
            Scores = new Dictionary<Side, int> {
                { Side.White, 0 },
                { Side.Black, 0 },
            };
        }
    }
}