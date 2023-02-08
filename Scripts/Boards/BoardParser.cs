using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Boards
{
    public static class BoardParser
    {
        //TODO PLEASE clean up this mess. I beg you.
        public static Board ParseBoard(this string template, [NotNull] PieceToAscii registry)
        {
            var board = new Board();

            var x = 0;
            var y = 7;
            foreach (var ch in template)
            {
                if (ch == '/')
                {
                    x = 0;
                    y--;
                    continue;
                }

                if (char.IsDigit(ch))
                {
                    x += int.Parse(ch.ToString());
                    continue;
                }

                var pieceCtor = registry.GetType(ch).GetConstructor(new[] { typeof(Side) });
                if (pieceCtor == null) throw new NullReferenceException();

                var piece = (IPiece)pieceCtor.Invoke(new object[]
                    { char.IsUpper(ch) ? Side.White : Side.Black });
                board.AddPiece(new Pos(x, y), piece);
                x++;
            }

            return board;
        }

        public static string DumpTemplate(this Board board, [NotNull] PieceToAscii registry)
        {
            var pieceStrings = new string[board.Height][];

            for (var y = 0; y < board.Height; y++)
            {
                pieceStrings[y] = new string[board.Width];
                for (var x = 0; x < board.Width; x++)
                {
                    var piece = board[new Pos(x, board.Height - 1 - y)];
                    if (piece == null)
                    {
                        pieceStrings[y][x] = "-";
                        continue;
                    }

                    var str = registry.GetAscii(piece.GetType()).ToString();
                    if (piece.Side == Side.Black) str = str.ToLower();
                    pieceStrings[y][x] = str;
                }
            }

            return string.Join("\n", pieceStrings.Select(x => string.Join(" ", x)));
        }
    }
}
