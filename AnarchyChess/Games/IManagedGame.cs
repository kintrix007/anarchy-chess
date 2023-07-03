using AnarchyChess.Moves;

namespace AnarchyChess.Games
{
    public interface IManagedGame
    {
        void OnGameCreated(Game game);
        void OnPieceMoved(Game game, MoveStep step);
        void OnPiecePromoted(Game game, MoveStep step, Object piece);
        void OnPieceRemoved(Game game, Pos pos, Object piece);
        void OnPieceAdded(Game game, Pos pos, Object piece);
    }
}
