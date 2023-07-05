using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class Bishop : IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Bishop(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos)
        {
            return NormalMove(game, pos).Select(x => x.Build());
        }

        /// <summary>
        /// The normal moves of the Bishop, i.e. the diagonal lines.
        /// </summary>
        /// <param name="game">The current game</param>
        /// <param name="pos">The position from where to generate the moves</param>
        /// <returns>The possible (unvalidated) moves</returns>
        public IEnumerable<MoveBuilder> NormalMove(Game game, Pos pos)
        {
            var moves = new List<MoveBuilder>();

            var directions = new[] {
                new Pos(1, 1), new Pos(1, -1),
                new Pos(-1, -1), new Pos(-1, 1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplates.RunLine(game.Board, pos, dir).Select(x => x.Capture()));
            }

            return moves;
        }
    }
}
