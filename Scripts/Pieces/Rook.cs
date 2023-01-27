using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class Rook : Object, IPiece
    {
        public int Cost => 5;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Rook(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] Moves(Board board, Pos pos) => throw new System.NotImplementedException();
    }
}
