using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class Queen : Object, IPiece
    {
        public int Cost => 9;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Queen(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] GetMoves(Board board, Pos pos) => throw new System.NotImplementedException();
    }
}