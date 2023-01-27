using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Boards
{
    public static class BoardTemplates
    {
        public static Board Standard()
        {
            var board = new Board();

            board.AddPiece(new Pos(0, 0), new Rook(Side.White));
            board.AddPiece(new Pos(1, 0), new Bishop(Side.White));
            board.AddPiece(new Pos(2, 0), new Knight(Side.White));
            board.AddPiece(new Pos(3, 0), new Queen(Side.White));
            board.AddPiece(new Pos(4, 0), new King(Side.White));
            board.AddPiece(new Pos(5, 0), new Knight(Side.White));
            board.AddPiece(new Pos(6, 0), new Bishop(Side.White));
            board.AddPiece(new Pos(7, 0), new Rook(Side.White));
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(new Pos(i, 1), new Pawn(Side.White));
            }

            board.AddPiece(new Pos(0, 7), new Rook(Side.Black));
            board.AddPiece(new Pos(1, 7), new Bishop(Side.Black));
            board.AddPiece(new Pos(2, 7), new Knight(Side.Black));
            board.AddPiece(new Pos(3, 7), new Queen(Side.Black));
            board.AddPiece(new Pos(4, 7), new King(Side.Black));
            board.AddPiece(new Pos(5, 7), new Knight(Side.Black));
            board.AddPiece(new Pos(6, 7), new Bishop(Side.Black));
            board.AddPiece(new Pos(7, 7), new Rook(Side.Black));
            for (int i = 0; i < 8; i++)
            {
                board.AddPiece(new Pos(i, 6), new Pawn(Side.Black));
            }

            return board;
        }
    }
}
