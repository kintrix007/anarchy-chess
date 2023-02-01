using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts
{
    public class BoardObj : Node
    {
        public override void _Ready()
        {
            var board = BoardTemplates.Standard();
            var template = board.DumpTemplate();

            var board2 = template.ParseBoard();

            GD.Print(board2.DumpTemplate());
            GD.Print(board.DumpTemplate() == board2.DumpTemplate());

            GD.Print(board.DumpTemplate() == @"r n b q k b n r
p p p p p p p p
- - - - - - - -
- - - - - - - -
- - - - - - - -
- - - - - - - -
P P P P P P P P
R N B Q K B N R");
        }
    }
}
