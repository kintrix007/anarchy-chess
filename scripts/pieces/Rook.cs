using Godot;

namespace AnarchyChess.scripts.pieces
{
    public class Rook : Object, IPiece
    {
        public int Cost => 5;
        public Side Side { get; }
        public bool Unmoved { get; set; }

        public Rook(Side side)
        {
            Side = side;
            Unmoved = true;
        }

        public Move[] Moves(Board board, Pos pos) => throw new System.NotImplementedException();
    }
}
