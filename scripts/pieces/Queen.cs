using Godot;

namespace AnarchyChess.scripts.pieces
{
    public class Queen : Object, IPiece
    {
        public int Cost => 9;
        public Side Side { get; }
        public bool Unmoved { get; set; }

        public Queen(Side side)
        {
            Side = side;
            Unmoved = true;
        }

        public Move[] Moves(Board board, Pos pos) => throw new System.NotImplementedException();
    }
}
