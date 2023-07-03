using System.Collections.Generic;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class Knook : IPiece
    {
        public int Cost => 7;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Knook(Side side)
        {
            Side = side;
            MoveCount = 0;
        }
        
        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos) => NormalMove(game, pos);
        
        
        public static IEnumerable<AppliedMove> NormalMove(Game game, Pos pos)
        {
            var moves = new List<AppliedMove>();
            moves.AddRange(Rook.NormalMove(game, pos));
            moves.AddRange(Knight.NormalMove(game, pos));

            return moves;
        }
    }
}
