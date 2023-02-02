using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class Knight : Object, IPiece
    {
        public int Cost => 3;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Knight(Side side)
        {
            Side = side;
            MoveCount = 0;
        }

        public IEnumerable<Move> GetMoves(Game game, Pos pos)
        {
            return NormalMove(game, pos);
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Game board, [NotNull] Pos pos) =>
            new[] {
                new Pos(1, 2), new Pos(2, 1),
                new Pos(2, -1), new Pos(1, -2),
                new Pos(-1, 2), new Pos(-2, 1),
                new Pos(-2, -1), new Pos(-1, -2),
            }.Select(x => Move.Relative(pos, x).Take());
    }
}