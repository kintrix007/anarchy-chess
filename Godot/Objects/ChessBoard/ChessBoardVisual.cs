using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ChessBoardVisual : TileMap, IManagedGame
    {
        private Node _piecesParent;
        private readonly Tween _tween = new Tween();
        private GameManager _gameManager;

        [NotNull] public readonly Dictionary<Pos, TextureRect> PieceToControl = new Dictionary<Pos, TextureRect>();

        public override void _Ready()
        {
            _piecesParent = GetNode("%Pieces");
            AddChild(_tween);

            // var (board, registry) = BoardTemplates.Standard();
            var registry = new PieceToAscii()
                .Register(typeof(King), 'K')
                .Register(typeof(Pawn), 'P')
                .Register(typeof(Knight), 'N')
                .Register(typeof(Bishop), 'B')
                .Register(typeof(Rook), 'R')
                .Register(typeof(Queen), 'Q')
                .Register(typeof(Knook), 'Ñ');
            
            // var board = "8/PPPPPPPP/4K3/8/8/4k3/pppppppp/8".ParseBoard(registry);
            var board = "ñnbqkbnñ/pppppppp/8/8/8/8/PPPPPPPP/ÑNBQKBNÑ".ParseBoard(registry);
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

            GD.Randomize();
        }

        private float _time = 0;
        private Side _last = Side.White;

        public override void _Process(float delta)
        {
            _time += delta;
            if (_time > 0.5)
            {
                var moves = _gameManager.Game.GetAllMoves(_last).Where(_gameManager.Game.IsValidMove).ToList();
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
            PieceToControl.Clear();
            foreach (var c in _piecesParent.GetChildren())
            {
                var child = (Node)c;
                child.QueueFree();
            }

            foreach (var (pos, piece) in game.Board)
            {
                var tex = new TextureRect();
                tex.Texture = _gameManager.GetTexture(piece);
                tex.RectPosition = MapToWorld(PosToBoardVector2(game, pos));

                _piecesParent.AddChild(tex);
                PieceToControl[pos] = tex;
            }
        }

        public void OnPieceMoved(Game game, MoveStep step)
        {
            var pos = step.To;
            var pieceNode = PieceToControl[step.From];
            PieceToControl[pos] = pieceNode;
            PieceToControl.Remove(step.From);

            var worldPosition = MapToWorld(PosToBoardVector2(game, pos));

            _tween.InterpolateProperty(
                pieceNode, "rect_position",
                null, worldPosition,
                0.3f, Tween.TransitionType.Cubic, Tween.EaseType.Out
            );

            _tween.Start();
        }

        public void OnPiecePromoted(Game game, MoveStep step, Object piece)
        {
            var pos = step.To;
            var pieceNode = PieceToControl[step.From];
            PieceToControl[pos] = pieceNode;
            PieceToControl.Remove(step.From);

            var worldPosition = MapToWorld(PosToBoardVector2(game, pos));

            _tween.InterpolateProperty(
                pieceNode, "rect_position",
                null, worldPosition,
                0.3f, Tween.TransitionType.Cubic, Tween.EaseType.Out
            );

            Task.Delay(300).ContinueWith(_ => pieceNode.Texture = _gameManager.GetTexture((IPiece)piece));

            _tween.Start();
        }
        
        public void OnPieceAdded(Game game, Pos pos, Object piece)
        {
            var tex = new TextureRect();
            tex.Texture = _gameManager.GetTexture((IPiece) piece);
            tex.RectPosition = MapToWorld(PosToBoardVector2(game, pos));

            _piecesParent.AddChild(tex);
            PieceToControl[pos] = tex;
        }

        public void OnPieceRemoved(Game game, Pos pos, Object piece)
        {
            if (!PieceToControl.ContainsKey(pos)) return;
            var boardPiece = PieceToControl[pos];
            boardPiece.QueueFree();
            PieceToControl.Remove(pos);
        }
        
        private static Vector2 PosToBoardVector2([NotNull] Game game, [NotNull] Pos pos)
        {
            var translatedPos = pos.SetY(game.Board.Height - 1 - pos.Y);
            return translatedPos.ToVector2();
        }
    }
}
