using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Pieces
{
    public class Bishop : Object, IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Bishop(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public Move[] GetMoves(Board board, Pos pos)
        {
            var moves = new List<Move>();

            var directions = new[] {
                new Pos(1, 1), new Pos(1, -1),
                new Pos(-1, -1), new Pos(-1, 1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplate.RunLine(board, pos, dir).Select(x => x.Take()));
            }

            return moves.ToArray();
        }
    }
}