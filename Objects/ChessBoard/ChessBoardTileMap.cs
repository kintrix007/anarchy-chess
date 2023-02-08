using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Compatibility;
using AnarchyChess.Scripts.Games;
using AnarchyChess.Scripts.Games.Chess;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.PieceHelper;
using AnarchyChess.Scripts.Pieces;
using JetBrains.Annotations;
using Godot;
using Object = Godot.Object;

namespace AnarchyChess.Objects.ChessBoard
{
    public class ChessBoardTileMap : TileMap, IManagedGame
    {
        private Node _pieces;
        private readonly Tween _tween = new Tween();
        private GameManager _gameManager;

        [NotNull] public readonly Dictionary<Pos, Control> Pieces = new Dictionary<Pos, Control>();

        public override void _Ready()
        {
            _pieces = GetNode("%Pieces");
            AddChild(_tween);

            var (board, registry) = BoardTemplates.Standard();
            var game = new Game(board, registry: registry);
            _gameManager = new GameManager(game)
                .SetDefaultTexturePath("res://icon.png")
                .RegisterPiece(typeof(King), "res://Assets/Pieces/{0}_king.png")
                .RegisterPiece(typeof(Pawn), "res://Assets/Pieces/{0}_pawn.png")
                .RegisterPiece(typeof(Knight), "res://Assets/Pieces/{0}_knight.png")
                .RegisterPiece(typeof(Bishop), "res://Assets/Pieces/{0}_bishop.png")
                .RegisterPiece(typeof(Rook), "res://Assets/Pieces/{0}_rook.png")
                .RegisterPiece(typeof(Queen), "res://Assets/Pieces/{0}_queen.png")
                .RegisterPiece(typeof(Knook), "res://Assets/Pieces/{0}_knook.png")
                .Manage(this);
            game.Create();

            game.Board.RemovePiece(new Pos("A1"));
            game.Board.AddPiece(new Pos("A1"), new Knook(Side.White));
            GD.Randomize();
        }

        private float _time = 0;
        private Side _last = Side.White;

        public override void _Process(float delta)
        {
            _time += delta;
            if (_time > 0.5)
            {
                var moves = _gameManager.Game.GetAllValidMoves(_last).ToList();
                if (ChessMateCheck.IsMate(_gameManager.Game, _last))
                {
                    GD.Print("_ Mate.");
                    SetProcess(false);
                    return;
                }

                var move = moves[Math.Abs((int)GD.Randi()) % moves.Count];
                _last = _last == Side.White ? Side.Black : Side.White;
                _gameManager.Game.ApplyMove(move);
                _time = 0;
            }
        }

        public void OnGameCreated(Game game)
        {
            Pieces.Clear();
            foreach (var c in _pieces.GetChildren())
            {
                var child = (Node)c;
                child.QueueFree();
            }

            foreach (var (pos, piece) in game.Board)
            {
                var tex = new TextureRect();
                tex.Texture = _gameManager.GetTexture(piece);
                tex.RectPosition = MapToWorld(PosToBoardVector2(game, pos));

                _pieces.AddChild(tex);
                Pieces[pos] = tex;
            }
        }

        public void OnPieceMoved(Game game, Move move)
        {
            var pos = move.To;
            var piece = Pieces[move.From];
            Pieces[pos] = piece;
            Pieces.Remove(move.From);

            var worldPosition = MapToWorld(PosToBoardVector2(game, pos));

            _tween.InterpolateProperty(
                piece, "rect_position",
                null, worldPosition,
                0.3f, Tween.TransitionType.Cubic, Tween.EaseType.Out
            );

            _tween.Start();
        }

        public void OnPieceRemoved(Game game, Pos pos, Object piece)
        {
            if (!Pieces.ContainsKey(pos)) return;
            var boardPiece = Pieces[pos];
            boardPiece.QueueFree();
            Pieces.Remove(pos);
        }

        public void OnPieceAdded(Game game, Pos pos, Object piece)
        {
            GD.Print("got here");
            var tex = new TextureRect();
            tex.Texture = _gameManager.GetTexture((IPiece) piece);
            tex.RectPosition = MapToWorld(PosToBoardVector2(game, pos));

            _pieces.AddChild(tex);
            Pieces[pos] = tex;
        }

        private static Vector2 PosToBoardVector2([NotNull] Game game, [NotNull] Pos pos)
        {
            var translatedPos = pos.SetY(game.Board.Height - 1 - pos.Y);
            return translatedPos.ToVector2();
        }
    }
}
