using JetBrains.Annotations;

namespace AnarchyChess.scripts.pieces
{
    public interface IPiece
    {
        int Cost { get; }
        Side Side { get; }
        bool Unmoved { get; set; }

        Move[] Moves([NotNull] Board board, [NotNull] Pos pos);
    }
}
