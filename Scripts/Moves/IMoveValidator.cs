using JetBrains.Annotations;
using AnarchyChess.Scripts.Boards;

namespace AnarchyChess.Scripts.Moves
{
    public interface IMoveValidator
    {
        bool IsValid([NotNull] Board board, [NotNull] Move move);
    }
}
