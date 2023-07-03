using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class Rook : IPiece, ICastlable
    {
        public int Cost => 5;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Rook(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos) => NormalMove(game, pos);

        
        public static IEnumerable<AppliedMove> NormalMove(Game game, Pos pos)
        {
            var moves = new List<AppliedMove>();

            var directions = new[] {
                new Pos(1, 0), new Pos(0, 1),
                new Pos(-1, 0), new Pos(0, -1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplates.RunLine(game.Board, pos, dir).Select(x => x.Take()));
            }

            return moves;
        }
    }
}
