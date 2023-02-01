using AnarchyChess.Scripts.Boards;
using Godot;

namespace AnarchyChess.Scripts
{
    public class BoardObj : Node
    {
        public override void _Ready()
        {
            var board = BoardTemplates.Standard();
            string template = board.DumpTemplate();

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