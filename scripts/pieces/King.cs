using System.Collections.Generic;
using Godot;

namespace AnarchyChess.scripts.pieces
{
    public class King : Object, IPiece
    {
        public int Cost => int.MaxValue;
        public Side Side { get; }
        public bool Unmoved { get; set; }

        public King(Side side)
        {
            Side = side;
            Unmoved = true;
        }

        public Move[] Moves(Board board, Pos pos)
        {
            var moves = new List<Move>();
            for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                moves.Add(Move.WithOffset(pos, new Pos(x, y)));
            }

            if (Unmoved)
            {
                
            }

            return moves.ToArray();
        }
    }
}
