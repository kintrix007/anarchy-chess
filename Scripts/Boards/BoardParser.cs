using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Boards
{
    public static class BoardParser
    {
        public static readonly Dictionary<char, Type> SymbolToPiece = new Dictionary<char, Type> {
            { 'P', typeof(Pawn) },
            { 'K', typeof(King) },
            { 'B', typeof(Bishop) },
            { 'N', typeof(Knight) },
            { 'R', typeof(Rook) },
            { 'Q', typeof(Queen) },
            { '-', null },
        };

        public static readonly Dictionary<Type, char> PieceToSymbol = new Dictionary<Type, char> {
            { typeof(Pawn), 'P' },
            { typeof(King), 'K' },
            { typeof(Bishop), 'B' },
            { typeof(Knight), 'N' },
            { typeof(Rook), 'R' },
            { typeof(Queen), 'Q' },
        };

        //TODO PLEASE clean up this mess. I beg you.
        public static Board ParseBoard(this string template)
        {
            string[] lines = template.Split('\n');
            if (lines.Length != 8) throw new ArgumentException();

            var pieceStrings = lines.Select(
                l => {
                    var pieces = l
                                 .Split(' ', '\t')
                                 .Where(s => s != "")
                                 .Select(s => s[0])
                                 .ToList();

                    if (pieces.Count != 8) throw new ArgumentException();
                    return pieces;
                }).ToList();

            var board = new Board();
            for (int y = 0; y < pieceStrings.Count; y++)
            {
                for (int x = 0; x < pieceStrings[y].Count; x++)
                {
                    char ch = pieceStrings[pieceStrings.Count - y - 1][x];
                    var pieceClass = SymbolToPiece[char.ToUpper(ch)];
                    if (pieceClass == null) continue;

                    var pieceConstructor = pieceClass.GetConstructor(new[] { typeof(Side) });
                    if (pieceConstructor == null) throw new NullReferenceException();

                    var piece = (IPiece)pieceConstructor.Invoke(new object[]
                        { char.IsUpper(ch) ? Side.White : Side.Black });

                    board.AddPiece(new Pos(x, y), piece);
                }
            }

            return board;
        }

        public static string DumpTemplate(this Board board)
        {
            string[][] pieceStrings = new string[8][];

            for (int y = 0; y < 8; y++)
            {
                pieceStrings[y] = new string[8];
                for (int x = 0; x < 8; x++)
                {
                    var piece = board[new Pos(x, 7 - y)];
                    if (piece == null)
                    {
                        pieceStrings[y][x] = "-";
                        continue;
                    }

                    string str = PieceToSymbol[piece.GetType()].ToString();
                    if (piece.Side == Side.Black) str = str.ToLower();
                    pieceStrings[y][x] = str;
                }
            }

            return string.Join("\n", pieceStrings.Select(x => string.Join(" ", x)));
        }
    }
}