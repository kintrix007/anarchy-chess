using JetBrains.Annotations;

namespace AnarchyChess.scripts
{
    public interface IMoveValidator
    {
        bool IsValid([NotNull] Move move);
    }
}
