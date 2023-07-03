using AnarchyChess.Games;
using AnarchyChess.Pieces;

namespace AnarchyChess.Boards
{
    public static class BoardTemplates
    {
        /// <summary>
        /// The standard template from Chess.
        /// </summary>
        /// <returns>The board with pieces from that template</returns>
        public static (Board, PieceToAscii) Standard()
        {
            var registry = new PieceToAscii()
                .Register(typeof(King), 'K')
                .Register(typeof(Pawn), 'P')
                .Register(typeof(Knight), 'N')
                .Register(typeof(Bishop), 'B')
                .Register(typeof(Rook), 'R')
                .Register(typeof(Queen), 'Q');
            
            var board = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR".ParseBoard(registry);
            return (board, registry);
        }
    }
}
