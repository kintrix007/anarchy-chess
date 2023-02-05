using System;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using AnarchyChess.Scripts.Pieces;
using Godot;

namespace AnarchyChess.Scripts
{
    public class BoardObj : Node
    {
        public override void _Ready()
        {
            CastlingEnPassantDemo();
        }

        private static void CastlingEnPassantDemo()
        {
            var game = new Game(@"
r n b q k b n r
p p p p - p p p
- - - - - - - -
- - - - P - - -
- - - - - - - -
- - - - - - - -
P P P P - P P P
R - - - K - - R
".Trim().ParseBoard());

            var kingPos = new Pos("E1");
            var king = game.Board[kingPos];
            var moves = king.GetMoves(game, kingPos).Where(game.ValidateMove).ToList();
            GD.Print(game.Board.DumpTemplate());
            GD.Print("------");
            game.ApplyMove(moves[3]);
            GD.Print(game.Board.DumpTemplate());
            GD.Print("------");
            game.ApplyMove(Move.Relative(new Pos("D7"), new Pos(0, -2)));
            GD.Print(game.Board.DumpTemplate());
            GD.Print("------");

            var pawn = game.Board[new Pos("E5")];
            moves = pawn.GetMoves(game, new Pos("E5")).Where(game.ValidateMove).ToList();
            GD.Print(string.Join(", ", moves));

            game.ApplyMove(moves.Last());
            GD.Print(game.Board.DumpTemplate());
            GD.Print("------");
        }

        private static void SimulateRandomGame()
        {
            var game = new Game(BoardTemplates.Standard());

            foreach (var i in Enumerable.Range(0, 100))
            {
                var move = GetRandomMove(game, i % 2 == 0 ? Side.White : Side.Black);
                game.ApplyMove(move);

                GD.Print(game.Board.DumpTemplate());
                GD.Print($"{game.Scores[Side.White]} ------ {game.Scores[Side.Black]}");
            }
        }

        public static Move GetRandomMove(Game game, Side side)
        {
            var rnd = new Random();

            while (true)
            {
                var pos = new Pos(rnd.Next(8), rnd.Next(8));
                var piece = game.Board[pos];
                if (piece == null) continue;
                if (piece.Side != side) continue;

                var moves = piece.GetMoves(game, pos).Where(game.ValidateMove).ToList();
                if (moves.Count == 0) continue;
                return moves[rnd.Next(moves.Count)];
            }
        }

        private static void OtherThingIGuess()
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
        }

        private static void ReallyDontKnow()
        {
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