using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class Knight : Object, IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Knight(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] Moves(Board board, Pos pos)
        {
            var positions = new[] {
                new Pos(1, 2), new Pos(2, 1),
                new Pos(2, -1), new Pos(1, -2),
                new Pos(-1, 2), new Pos(-2, 1),
                new Pos(-2, -1), new Pos(-1, -2),
            };

            return positions.Select(x => Move.Relative(pos, x).Takes()).ToArray();
        }
    }
}
