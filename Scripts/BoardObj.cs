using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using Godot;

namespace AnarchyChess.Scripts
{
    public class BoardObj : Node
    {
        public override void _Ready()
        {
            var board = BoardTemplates.Standard();
            var game = new Game(board);
            
            GD.Print(game.Board.DumpTemplate());

            var horseyPos = new Pos("A1");
            GD.Print("Pos: ", horseyPos);
            var horsey = game.Board[horseyPos];
            GD.Print("is Horsey: ", horsey is Knight);
            var allHorseyMoves = horsey.GetMoves(game, horseyPos).ToList();
            GD.Print(string.Join(", ", allHorseyMoves.Select(x => x.ToString())));
            var firstMove = allHorseyMoves[0];
            
            GD.Print(firstMove);
            GD.Print(StandardMoveValidator.ValidateBounds(game, firstMove));
            GD.Print(StandardMoveValidator.ValidateOverlap(game, firstMove));
            GD.Print(StandardMoveValidator.ValidateMustTake(game, firstMove));
            GD.Print(StandardMoveValidator.ValidateNoCheck(game, firstMove));

            GD.Print(game.Board.DumpTemplate());
            
            var validHorseyMoves = allHorseyMoves.Where(x => StandardMoveValidator.IsValidMove(game, x)).ToList();
            GD.Print(string.Join(", ", validHorseyMoves.Select(x => x.ToString())));

            var firstValidMove = validHorseyMoves.Count <= 0 ? null : validHorseyMoves.First();
            GD.Print(firstValidMove);
            if (firstValidMove != null)
            {
                var ok = game.ApplyMove(firstValidMove);
                GD.Print(ok);
            }
            
            GD.Print(game.Board.DumpTemplate());
            
            return;
            var board1 = BoardTemplates.Standard();
            var template = board1.DumpTemplate();

            var board2 = template.ParseBoard();

            GD.Print(board2.DumpTemplate());
            GD.Print(board1.DumpTemplate() == board2.DumpTemplate());

            GD.Print(board1.DumpTemplate() == @"r n b q k b n r
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
