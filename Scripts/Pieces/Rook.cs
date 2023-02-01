using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

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
        
        public IEnumerable<Move> GetMoves(Board board, Pos pos) => NormalMove(board, pos);

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Board board, [NotNull] Pos pos)
        {
            var moves = new List<Move>();

            var directions = new[] {
                new Pos(1, 0), new Pos(0, 1),
                new Pos(-1, 0), new Pos(0, -1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplates.RunLine(board, pos, dir).Select(x => x.Take()));
            }

            return moves;
        }
    }
}
