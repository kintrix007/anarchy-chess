using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Games
{
    public class PieceToAscii
    {
        private readonly Dictionary<char, Type> _asciiToPieceRegistry;
        private readonly Dictionary<Type, char> _pieceToAsciiRegistry;

        public PieceToAscii([NotNull] Dictionary<Type, char> pieceToAscii,
            [NotNull] Dictionary<char, Type> asciiToPiece)
        {
            _pieceToAsciiRegistry = pieceToAscii;
            _asciiToPieceRegistry = asciiToPiece;
        }

        public PieceToAscii() : this(new Dictionary<Type, char>(), new Dictionary<char, Type>()) {}
        public PieceToAscii(Dictionary<Type, char> registry) : this(registry, Invert(registry)) {}
        public PieceToAscii(Dictionary<char, Type> registry) : this(Invert(registry), registry) {}

        public PieceToAscii Register(Type piece, char ascii)
        {
            if (!piece.GetInterfaces().Contains(typeof(IPiece)))
            {
                throw new ArgumentException($"The type must represent a type of piece. '{piece}' does not.");
            }

            _pieceToAsciiRegistry[piece] = ascii;
            _asciiToPieceRegistry[ascii] = piece;
            return this;
        }

        public PieceToAscii Deregister(Type piece)
        {
            if (!piece.GetInterfaces().Contains(typeof(IPiece)))
            {
                throw new ArgumentException($"The type must represent a type of piece. '{piece}' does not.");
            }

            if (!_pieceToAsciiRegistry.ContainsKey(piece)) return this;

            var ascii = _pieceToAsciiRegistry[piece];
            _pieceToAsciiRegistry.Remove(piece);
            _asciiToPieceRegistry.Remove(ascii);
            return this;
        }

        public PieceToAscii Deregister(char ascii)
        {
            if (!_asciiToPieceRegistry.ContainsKey(ascii)) return this;

            var piece = _asciiToPieceRegistry[ascii];
            _asciiToPieceRegistry.Remove(ascii);
            _pieceToAsciiRegistry.Remove(piece);
            return this;
        }

        public PieceToAscii SetRegistered(Type piece, char ascii, bool register) =>
            register ? Register(piece, ascii) : Deregister(piece);

        public Type GetType(char ascii)
        {
            if (!_asciiToPieceRegistry.ContainsKey(ascii)) throw new KeyNotFoundException();
            return _asciiToPieceRegistry[ascii];
        }

        public char GetAscii(Type piece)
        {
            if (!piece.GetInterfaces().Contains(typeof(IPiece)))
            {
                throw new ArgumentException($"The type must represent a type of piece. '{piece}' does not.");
            }

            if (!_pieceToAsciiRegistry.ContainsKey(piece)) throw new KeyNotFoundException();
            return _pieceToAsciiRegistry[piece];
        }

        private static Dictionary<V, K> Invert<K, V>(Dictionary<K, V> dictionary)
        {
            var result = new Dictionary<V, K>();
            foreach (var pair in dictionary)
            {
                result[pair.Value] = pair.Key;
            }

            return result;
        }
    }
}
