using AnarchyChess.Scripts.Moves;
using JetBrains.Annotations;
using Object = Godot.Object;

namespace AnarchyChess.Scripts.Games
{
    public interface IManagedGame
    {
        void OnGameCreated([NotNull] Game game);
        void OnPieceMoved([NotNull] Game game, [NotNull] Move move);
        void OnPieceRemoved([NotNull] Game game, [NotNull] Pos pos, [NotNull] Object piece);
        void OnPieceAdded([NotNull] Game game, [NotNull] Pos pos, [NotNull] Object piece);
    }
}