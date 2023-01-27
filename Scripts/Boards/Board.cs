using System;
using System.Collections.Generic;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Boards
{
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

        public void MovePiece(Move move)
        {
            var moves = move.Unfold();
            var movingPieces = new Dictionary<Move, IPiece>();
            moves.ForEach(x => movingPieces[x] = this[x.From]);

            moves.ForEach(x => {
                //TODO Add logic for taking pieces
                this[move.To] = movingPieces[move];
                this[move.From] = null;
            });

            LastMove = move;
        }
    }
}