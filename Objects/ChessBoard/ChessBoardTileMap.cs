using System.Collections.Generic;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Compatibility;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Moves;
using Godot;
using JetBrains.Annotations;

public class ChessBoardTileMap : TileMap
{
    public override void _Ready()
    {
        var game = new Game(BoardTemplates.Standard());
        OnNewBoard(game);
        OnPieceMoved(game, Move.Relative(new Pos("E2"), new Pos(0, 2)));
    }

    [NotNull]
    public readonly Dictionary<Pos, Control> Pieces = new Dictionary<Pos, Control>();

    public void OnPieceMoved([NotNull] Game game, [NotNull] Move move)
    {
        var piece = Pieces[move.From];
        var pos = move.To;
        Pieces[pos] = piece;
        var worldPosition = MapToWorld(PosToBoardVector2(game, pos));
        
        piece.RectPosition = worldPosition;
        Pieces.Remove(move.From);
    }
    
    public void OnNewBoard([NotNull] Game game)
    {
        foreach (var c in GetChildren())
        {
            var child = (Node)c;
            child.QueueFree();
        }
        
        Pieces.Clear();
        foreach (var (pos, piece) in game.Board)
        {
            var tex = new TextureRect();
            tex.Texture = GD.Load<Texture>("res://icon.png");
            tex.RectPosition = MapToWorld(PosToBoardVector2(game, pos));
            
            AddChild(tex);
            Pieces[pos] = tex;
        }
    }

    private static Vector2 PosToBoardVector2([NotNull] Game game, [NotNull] Pos pos)
    {
        var translatedPos = pos.SetY(game.Board.Height - 1 - pos.Y);
        return translatedPos.ToVector2();
    } 
}
