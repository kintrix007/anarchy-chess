using System.Collections.Generic;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

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

        public IEnumerable<Move> GetMoves(Board board, Pos pos) => NormalMove(board, pos);

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Board board, [NotNull] Pos pos)
        {
            var moves = new List<Move>();
            moves.AddRange(Rook.NormalMove(board, pos));
            moves.AddRange(Bishop.NormalMove(board, pos));

            return moves;
        }
    }
}
