using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;
using Godot;

namespace AnarchyChess.Scripts.Games
{
    public class GameManager : Resource
    {
        [NotNull] private readonly Dictionary<Type, string> _pieceTextureRegistry = new Dictionary<Type, string>();

        private string _defaultTexturePath = "res://icon.png";

        [NotNull] public readonly Game Game;

        public GameManager(Game game)
        {
            Game = game;
        }

        [NotNull]
        public GameManager SetDefaultTexturePath(string path)
        {
            _defaultTexturePath = path;
            return this;
        }

        [NotNull]
        public GameManager RegisterPiece([NotNull] Type pieceType, string pathTemplate)
        {
            var isPieceType = pieceType.GetInterfaces().Contains(typeof(IPiece));
            if (!isPieceType)
            {
                throw new ArgumentException($"The type must represent a type of piece. '{pieceType}' does not.");
            }

            _pieceTextureRegistry[pieceType] = pathTemplate;
            return this;
        }

        [NotNull]
        public GameManager DeregisterPiece([NotNull] Type pieceType)
        {
            var isPieceType = pieceType.GetInterfaces().Contains(typeof(IPiece));
            if (!isPieceType)
            {
                throw new ArgumentException($"The type must represent a type of piece. '{pieceType}' does not.");
            }

            _pieceTextureRegistry.Remove(pieceType);
            return this;
        }

        [NotNull]
        public GameManager SetRegistered([NotNull] Type pieceType, string pathTemplate, bool register)
        {
            return register ? RegisterPiece(pieceType, pathTemplate) : DeregisterPiece(pieceType);
        }

        [NotNull]
        public Texture GetTexture([NotNull] IPiece piece)
        {
            if (!_pieceTextureRegistry.ContainsKey(piece.GetType())) return GD.Load<Texture>(_defaultTexturePath);

            var template = _pieceTextureRegistry[piece.GetType()];
            var path = string.Format(template, piece.Side == Side.White ? "white" : "black");

            return GD.Load<Texture>(ResourceLoader.Exists(path) ? path : _defaultTexturePath);
        }

        public GameManager Manage(IManagedGame managedGame)
        {
            if (!(managedGame is Godot.Object obj))
            {
                throw new ArgumentException($"Type '{managedGame.GetType()}' is not a Godot Object.");
            }

            ConnectSignal(nameof(Game.GameCreated),  obj, nameof(managedGame.OnGameCreated));
            ConnectSignal(nameof(Game.PieceMoved),   obj, nameof(managedGame.OnPieceMoved));
            ConnectSignal(nameof(Game.PiecePromoted), obj, nameof(managedGame.OnPiecePromoted));
            ConnectSignal(nameof(Game.PieceAdded),   obj, nameof(managedGame.OnPieceAdded));
            ConnectSignal(nameof(Game.PieceRemoved), obj, nameof(managedGame.OnPieceRemoved));
            return this;
        }

        private void ConnectSignal(string signal, Godot.Object obj, string method)
        {
            var ok = Game.Connect(signal, obj, method);
            if (ok != Error.Ok) throw new Exception();
        }
    }
}
