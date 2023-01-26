using System;
using System.Diagnostics;
using AnarchyChess.scripts.pieces;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.scripts
{
    [Signal]
    public delegate void PieceTaken(int x, int y, IPiece piece);

    public class BoardObj : Node
    {
        [NotNull]
        public Board Pieces = Board.StandardBoard();

        public override void _Ready()
        {
            var king = new King(Side.White);
            // king.Moves(new Pos(0, 0));
        }
    }
}
