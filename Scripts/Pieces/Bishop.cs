using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class Bishop : Resource, IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Bishop(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos) => NormalMove(game, pos);

        /// <summary>
        /// The normal moves of the Bishop, i.e. the diagonal lines.
        /// </summary>
        /// <param name="game">The current game</param>
        /// <param name="pos">The position from where to generate the moves</param>
        /// <returns>The possible (unvalidated) moves</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<AppliedMove> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<AppliedMove>();

            var directions = new[] {
                new Pos(1, 1), new Pos(1, -1),
                new Pos(-1, -1), new Pos(-1, 1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplates.RunLine(game.Board, pos, dir).Select(x => x.Take()));
            }

            return moves;
        }
    }
}