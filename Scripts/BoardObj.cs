using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Pieces;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts
{
    [Signal]
    public delegate void PieceTaken(int x, int y, IPiece piece);

    public class BoardObj : Node
    {
        [NotNull] public Board Pieces = BoardTemplates.Standard();

        public override void _Ready()
        {
            var king = new King(Side.White);
            // king.Moves(new Pos(0, 0));
        }
    }
}
