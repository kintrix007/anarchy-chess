using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
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

        public IEnumerable<Move> GetMoves(Game game, Pos pos) => NormalMove(game, pos);

        [NotNull, ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<Move>();

            var directions = new[] {
                new Pos(1, 1), new Pos(1, -1),
                new Pos(-1, -1), new Pos(-1, 1),
            };

            foreach (var dir in directions)
            {
                moves.AddRange(MoveTemplates.RunLine(game.Board, pos, dir).Select(x => x.Take()));
            }

            return moves;
        }
    }
}