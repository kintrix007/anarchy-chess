using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class Bishop : Object, IPiece
    //We will now be describing the bisshop uwu
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Bishop(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] Moves(Board board, Pos pos) => throw new System.NotImplementedException();
    }
}
