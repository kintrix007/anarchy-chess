using System;
using System.Collections.Generic;
using AnarchyChess.scripts.pieces;
using Godot;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AnarchyChess.scripts
{
    public class Board : Resource
    {
        [Signal]
        public delegate void Taken([NotNull] Object piece, [NotNull] Pos at);

        [NotNull] [ItemCanBeNull]
        private readonly IPiece[,] _pieces;
        [NotNull]
        public Dictionary<Side, Pos> KingPositions { get; private set; }
        [NotNull]
        public Dictionary<Side, int> Scores { get; private set; }

        public Board()
        {
            _pieces = new IPiece[8, 8];
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
        }

        public static Board StandardBoard()
        {
            var board = new Board();

            board.AddPiece(new Pos(0, 0), new Rook(Side.White));
            board.AddPiece(new Pos(1, 0), new Bishop(Side.White));
            board.AddPiece(new Pos(2, 0), new Knight(Side.White));
            board.AddPiece(new Pos(3, 0), new Queen(Side.White));
            board.AddPiece(new Pos(4, 0), new King(Side.White));
            board.AddPiece(new Pos(5, 0), new Knight(Side.White));
            board.AddPiece(new Pos(6, 0), new Bishop(Side.White));
            board.AddPiece(new Pos(7, 0), new Rook(Side.White));
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(new Pos(i, 1), new Pawn(Side.White));
            }

            board.AddPiece(new Pos(0, 7), new Rook(Side.Black));
            board.AddPiece(new Pos(1, 7), new Bishop(Side.Black));
            board.AddPiece(new Pos(2, 7), new Knight(Side.Black));
            board.AddPiece(new Pos(3, 7), new Queen(Side.Black));
            board.AddPiece(new Pos(4, 7), new King(Side.Black));
            board.AddPiece(new Pos(5, 7), new Knight(Side.Black));
            board.AddPiece(new Pos(6, 7), new Bishop(Side.Black));
            board.AddPiece(new Pos(7, 7), new Rook(Side.Black));
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(new Pos(i, 6), new Pawn(Side.Black));
            }

            return board;
        }
    }
}
