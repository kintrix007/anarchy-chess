using Godot;

namespace AnarchyChess.scripts.pieces
{
    public class Bishop : Object, IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public bool Unmoved { get; set; }

        public Bishop(Side side)
        {
            Side = side;
            Unmoved = true;
        }

        public Move[] Moves(Board board, Pos pos) => throw new System.NotImplementedException();
    }
}
