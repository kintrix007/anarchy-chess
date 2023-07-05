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

        public IEnumerable<MoveBuilder> GetMoveBuilders(Game game, Pos pos)
        {
            return NormalMove(game, pos);
        }

        public IEnumerable<MoveBuilder> NormalMove(Game game, Pos pos)
        {
            var moves = new List<MoveBuilder>();
            moves.AddRange(new Rook(Side).NormalMove(game, pos));
            moves.AddRange(new Bishop(Side).NormalMove(game, pos));

            return moves;
        }
    }
}
