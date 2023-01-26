using System;
using Object = Godot.Object;

namespace AnarchyChess.scripts.pieces
{
    public class Pawn : Object, IPiece
    {
        public int Cost => 1;
        public Side Side { get; }
        public bool Unmoved { get; set; }

        public Pawn(Side side)
        {
            Side = side;
            Unmoved = true;
        }

        public Move[] Moves(Board board, Pos pos) => throw new NotImplementedException();
    }
}
