using System.Collections.Generic;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.PieceHelper;

namespace AnarchyChess.Pieces
{
    public class Queen : IPiece
    {
        public int Cost => 9;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Queen(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos) => NormalMove(game, pos);

        
        public IEnumerable<AppliedMove> NormalMove(Game game, Pos pos)
        {
            var moves = new List<AppliedMove>();
            moves.AddRange(new Rook(Side).NormalMove(game, pos));
            moves.AddRange(new Bishop(Side).NormalMove(game, pos));

            return moves;
        }
    }
}
