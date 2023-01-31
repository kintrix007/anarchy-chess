using System;
using System.Collections.Generic;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Boards
{
    //TODO Make it iterable
    public class Board : Resource
    {
        [Signal]
        public delegate void Taken([NotNull] Object piece, [NotNull] Pos at);

        [NotNull] public Dictionary<Side, Pos> KingPositions { get; private set; }
        [NotNull] public Dictionary<Side, int> Scores { get; private set; }
        [CanBeNull] public Move LastMove { get; private set; }

        [NotNull] [ItemCanBeNull] private readonly IPiece[,] _pieces;

        public Board()
        {
            _pieces = new IPiece[8, 8];
            LastMove = null;

            KingPositions = new Dictionary<Side, Pos>();
            Scores = new Dictionary<Side, int> {
                [Side.White] = 0,
                [Side.Black] = 0,
            };
        }

        public IPiece this[Pos pos]
        {
            get => _pieces[pos.X, pos.Y];
            private set => _pieces[pos.X, pos.Y] = value;
        }

        public void AddPiece(Pos pos, IPiece piece)
        {
            this[pos] = piece;
            Scores[piece.Side] += piece.Cost;

            if (!(piece is King)) return;

            if (KingPositions.ContainsKey(piece.Side))
            {
                throw new Exception($"Side {piece.Side} has multiple kings");
            }

            KingPositions[piece.Side] = pos;
        }

        /// <summary>
        /// Does not modify anything except the piece positions.
        /// </summary>
        /// <param name="foldedMove"></param>
        public void UncheckedMovePiece(Move foldedMove)
        {
            var moveList = foldedMove.Unfold();
            var movingPieces = new Dictionary<Move, IPiece>();
            moveList.ForEach(move => movingPieces[move] = this[move.From]);

            moveList.ForEach(move => {
                this[move.To] = movingPieces[move];
                this[move.From] = null;
            });

            LastMove = foldedMove;
        }
    }
}
