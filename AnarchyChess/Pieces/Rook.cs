using System.Collections.Generic;
using System.Linq;

using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class Rook : IPiece, ICastlable
    {
        public int Cost => 5;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Rook(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<MoveBuilder> GetMoveBuilders(Game game, Pos pos)
        {
            return NormalMove(game, pos);
        }

        public IEnumerable<MoveBuilder> NormalMove(Game game, Pos pos)
        {
            var moves = new List<MoveBuilder>();

            var directions = new[] {
                new Pos(1, 0), new Pos(0, 1),
                new Pos(-1, 0), new Pos(0, -1),
            };

            foreach (var dir in directions)
            {
                var line = MoveTemplates
                    .RunLine(game.Board, pos, dir)
                    .Select(x => x.Capture());
                moves.AddRange(line);
            }

            return moves;
        }
    }
}
