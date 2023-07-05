using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class Knight : IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Knight(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<MoveBuilder> GetMoveBuilders(Game game, Pos pos)
        {
            return NormalMove(game, pos);
        }

        
        public IEnumerable<MoveBuilder> NormalMove(Game board, Pos pos) =>
            new[] {
                new Pos(1, 2), new Pos(2, 1),
                new Pos(2, -1), new Pos(1, -2),
                new Pos(-1, 2), new Pos(-2, 1),
                new Pos(-2, -1), new Pos(-1, -2),
            }.Select(x => MoveBuilder.Relative(pos, x).Capture());
    }
}
