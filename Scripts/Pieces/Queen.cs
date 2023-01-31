using System.Collections.Generic;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class Queen : Object, IPiece
    {
        public int Cost => 9;
        public Side Side { get; }
        public int MoveCount { get; set; }

        private readonly Rook _rook;
        private readonly Bishop _bishop;

        public Queen(Side side)
        {
            Side = side;
            MoveCount = 0;
            _rook = new Rook(Side);
            _bishop = new Bishop(Side);
        }

        public Move[] GetMoves(Board board, Pos pos)
        {
            var moves = new List<Move>();

            moves.AddRange(_rook.GetMoves(board, pos));
            moves.AddRange(_bishop.GetMoves(board, pos));

            return moves.ToArray();
        }
    }
}