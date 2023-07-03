using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class Knight : Resource, IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Knight(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<AppliedMove> GetMoves(Game game, Pos pos)
        {
            return NormalMove(game, pos);
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<AppliedMove> NormalMove([NotNull] Game board, [NotNull] Pos pos) =>
            new[] {
                new Pos(1, 2), new Pos(2, 1),
                new Pos(2, -1), new Pos(1, -2),
                new Pos(-1, 2), new Pos(-2, 1),
                new Pos(-2, -1), new Pos(-1, -2),
            }.Select(x => AppliedMove.Relative(pos, x).Take());
    }
}