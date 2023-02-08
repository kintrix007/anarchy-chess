using System.Collections.Generic;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public class Knook : Resource, IPiece
    {
        public int Cost => 7;
        public Side Side { get; }
        public int MoveCount { get; set; }

        public Knook(Side side)
        {
            Side = side;
            MoveCount = 0;
        }
        
        public IEnumerable<Move> GetMoves(Game game, Pos pos) => NormalMove(game, pos);
        
        [NotNull, ItemNotNull]
        public static IEnumerable<Move> NormalMove([NotNull] Game game, [NotNull] Pos pos)
        {
            var moves = new List<Move>();
            moves.AddRange(Rook.NormalMove(game, pos));
            moves.AddRange(Knight.NormalMove(game, pos));

            return moves;
        }
    }
}