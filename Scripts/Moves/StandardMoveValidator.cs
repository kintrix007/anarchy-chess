using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Pieces;

namespace AnarchyChess.Scripts.Moves
{
    public class StandardMoveValidator : IMoveValidator
    {
        public bool IsValid(Board board, Moves.Move foldedMove)
        {
            if (!ValidateBounds(board, foldedMove)) return false;
            if (!ValidateOverlap(board, foldedMove)) return false;
            if (!ValidateMustTake(board, foldedMove)) return false;
            if (!ValidateNoCheck(board, foldedMove)) return false;

            return true;
        }

        public static bool ValidateBounds(Board board, Moves.Move foldedMove)
        {
            foreach (var move in foldedMove.Unfold())
            {
                if (move.To.X < 0 || move.To.X >= 8) return false;
                if (move.To.Y < 0 || move.To.Y >= 8) return false;
            }

            return true;
        }

        public static bool ValidateOverlap(Board board, Moves.Move foldedMove)
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

        public static bool ValidateMustTake(Board board, Moves.Move foldedMove)
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

        public static bool ValidateNoCheck(Board board, Moves.Move foldedMove)
        {
            var originalPiece = board[foldedMove.From];

            foreach (var move in foldedMove.Unfold())
            {
                //? IDK man, this move then inverse move seems a bit finicky...
                board.UnvalidatedMovePiece(move);

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

                board.UnvalidatedMovePiece(move.Inverse());
            }

            return true;
        }
    }
}