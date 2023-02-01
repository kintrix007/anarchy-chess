using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Godot;

namespace AnarchyChess.Scripts.Pieces
{
    public class Rook : Object, IPiece, ICastlable
    {
        public int Cost => 5;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Rook(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] GetMoves(Board board, Pos pos)
        {
            var moves = new List<Move>();

            var directions = new[] {
                new Pos(1, 0), new Pos(0, 1),
                new Pos(-1, 0), new Pos(0, -1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplate.RunLine(board, pos, dir).Select(x => x.Take()));
            }

            return moves.ToArray();
        }
    }
}