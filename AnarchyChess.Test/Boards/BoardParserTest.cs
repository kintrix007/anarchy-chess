namespace AnarchyChess.Test;
using AnarchyChess.Boards;
using AnarchyChess.Games;
using AnarchyChess.PieceHelper;
using AnarchyChess.Pieces;

public class BoardParserTest
{
    private PieceToAscii _registry;

    [SetUp]
    public void Setup()
    {
        _registry = new PieceToAscii()
                .Register(typeof(King), 'K')
                .Register(typeof(Pawn), 'P')
                .Register(typeof(Knight), 'N')
                .Register(typeof(Bishop), 'B')
                .Register(typeof(Rook), 'R')
                .Register(typeof(Queen), 'Q');
    }

    [TestCaseSource(nameof(EmptyBoardTestSource))]
    public void TestEmptyNByNBoard(string chessString, Board expected)
    {
        var board = chessString.ParseBoard(_registry);
        CollectionAssert.AreEqual(expected, board);
    }

    internal static object[] EmptyBoardTestSource = {
        new object[2] {"8/8/8/8/8/8/8/8", new Board(8, 8)},
        new object[2] {"8/8/8/8", new Board(8, 4)},
        new object[2] {"4/4/4/4", new Board(4, 4)},
        new object[2] {"4/4/4/4/4/4/4", new Board(4, 8)},
        new object[2] {"1/2/3", new Board(3, 3)},
    };

    [TestCaseSource(nameof(BoardTestSource))]
    public void TestNByNBoard(string chessString, Board expected)
    {
        var board = chessString.ParseBoard(_registry);
        CollectionAssert.AreEqual(expected, board);
    }

    internal static object[] BoardTestSource = {
        new object[2] {"P", new List<IPiece>(){new Pawn(Side.White)}.ToBoard(1, 1)},
        new object[2] {"p", new List<IPiece>(){new Pawn(Side.Black)}.ToBoard(1, 1)},
        new object[2] {
            "Pp",
            new List<IPiece>() {
                new Pawn(Side.White), new Pawn(Side.Black),
            }.ToBoard(2, 1)
        },
        new object[2] {
            "PP/pp",
            new List<IPiece>() {
                new Pawn(Side.White), new Pawn(Side.White),
                new Pawn(Side.Black), new Pawn(Side.Black),
            }.ToBoard(2, 2)
        },
        new object[2] {
            "1P/p",
            new List<IPiece?>() {
                null, new Pawn(Side.White),
                new Pawn(Side.Black), null,
            }.ToBoard(2, 2)
        },
        new object[2] {
            "PP/2/pp",
            new List<IPiece?>() {
                new Pawn(Side.White), new Pawn(Side.White),
                null, null,
                new Pawn(Side.Black), new Pawn(Side.Black),
            }.ToBoard(2, 3)
        },
        new object[2] {
            "P1N/3/pn1",
            new List<IPiece?>() {
                new Pawn(Side.White), null, new Knight(Side.White),
                null, null, null,
                new Pawn(Side.Black), new Knight(Side.Black), null,
            }.ToBoard(3, 3)
        },
        new object[2] {
            "1PP1/4/R2r/4/1pp1",
            new List<IPiece?>() {
                null, new Pawn(Side.White), new Knight(Side.White), null,
                null, null, null, null,
                new Rook(Side.White), null, null, new Rook(Side.Black),
                null, null, null, null,
                null, new Pawn(Side.Black), new Pawn(Side.Black), null,
            }.ToBoard(4, 4)
        },
    };
}
