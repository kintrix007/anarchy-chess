using AnarchyChess.Boards;
using AnarchyChess.Games;
using AnarchyChess.Moves;
using AnarchyChess.Pieces;

internal class Program
{
    private static void Main(string[] _)
    {
        var registry = new PieceToAscii()
            .Register(typeof(King), 'K')
            .Register(typeof(Pawn), 'P')
            .Register(typeof(Knight), 'N')
            .Register(typeof(Bishop), 'B')
            .Register(typeof(Rook), 'R')
            .Register(typeof(Queen), 'Q')
            .Register(typeof(Knook), 'Ñ');

        var board = "ñnbqkbnñ/pppppppp/8/8/8/8/PPPPPPPP/ÑNBQKBNÑ".ParseBoard(registry);
        var game = new Game(board, registry);
        Console.WriteLine(game.Board.CreateDebugChessString(registry));

        var move = MoveBuilder.Absolute(new Pos("D2"), new Pos("D7")).Build();
        Console.WriteLine(game.IsValidMove(move));
        game.ApplyMove(move);
        Console.WriteLine(game.Board.CreateDebugChessString(registry));

        // Console.WriteLine("Hello, World!");
    }
}
