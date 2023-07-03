using System.Collections.Generic;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class Queen : Resource, IPiece
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

        [NotNull, ItemNotNull]
        public static IEnumerable<AppliedMove> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<AppliedMove>();
            moves.AddRange(Rook.NormalMove(game, pos));
            moves.AddRange(Bishop.NormalMove(game, pos));

            return moves;
        }
    }
}