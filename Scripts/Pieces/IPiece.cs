using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Pieces
{
    public interface IPiece
    {
        int Cost { get; }
        Side Side { get; }
        int MoveCount { get; set; }

        Move[] Moves([NotNull] Board board, [NotNull] Pos pos);
    }
}
