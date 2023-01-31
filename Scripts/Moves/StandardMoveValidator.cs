using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Moves
{
    public class StandardMoveValidator : IMoveValidator
    {
        public bool IsValid(Board board, Move move)
        {
            if (!ValidateBounds(board, move)) return false;
            if (!ValidateOverlap(board, move)) return false;
            if (!ValidateMustTake(board, move)) return false;
            if (!ValidateNoCheck(board, move)) return false;

            return true;
        }

        public static bool ValidateBounds(Board board, Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
            {
                if (move.To.X < 0 || move.To.X >= 8) return false;
                if (move.To.Y < 0 || move.To.Y >= 8) return false;
            }

            return true;
        }

        public static bool ValidateOverlap(Board board, Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
            {
                var movingPiece = board[move.From];
                if (movingPiece == null) return false;

                var destPiece = board[move.To];
                if (destPiece == null) continue;

                if (destPiece.Side == movingPiece.Side) return false;
                if (!move.TakeList.Contains(move.To)) return false;
            }

            return true;
        }

        public static bool ValidateMustTake(Board board, Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
            {
                if (!move.IsMustTake) continue;

                var movingPiece = board[move.From];
                if (movingPiece == null) return false;

                bool takesAll = move.TakeList.All(x => board[x] != null && board[x].Side != movingPiece.Side);
                if (!takesAll) return false;
            }

            return true;
        }

        public static bool ValidateNoCheck(Board board, Move foldedMove)
        {
            var originalPiece = board[foldedMove.From];

            foreach (var move in foldedMove.Unfold())
            {
                //? IDK man, this move then inverse move seems a bit finicky...
                board.UncheckedMovePiece(move);

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var pos = new Pos(x, y);
                        var piece = board[pos];
                        if (piece == null) continue;

                        bool causesCheck = piece.GetMoves(board, pos)
                                                .Any(m => board[m.To] is King k && k.Side == originalPiece.Side);

                        if (causesCheck) return false;
                    }
                }

                board.UncheckedMovePiece(move.Inverse());
            }

            return true;
        }
    }
}
