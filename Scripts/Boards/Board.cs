using System;
using System.Collections.Generic;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using AnarchyChess.Scripts.Pieces;
using JetBrains.Annotations;
using Resource = Godot.Resource;

namespace AnarchyChess.Scripts.Boards
{
    //TODO Make it iterable
    public class Board : Resource
    {
        /// <summary>
        /// Currently unused. Might be removed in the future.
        /// </summary>
        [NotNull]
        public Dictionary<Side, Pos> KingPositions { get; private set; }

        /// <summary>
        /// Track the scores of each player. Might be removed in the future.
        /// </summary>
        [NotNull]
        public Dictionary<Side, int> Scores { get; private set; }

        /// <summary>
        /// Track what the last move was. Might be replaced with a move list that represents the game.
        /// </summary>
        [CanBeNull]
        public Move LastMove { get; private set; }

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
        public void UnvalidatedMovePiece(Move foldedMove)
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