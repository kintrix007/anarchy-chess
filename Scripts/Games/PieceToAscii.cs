using System;
using System.Collections.Generic;
using System.Linq;
using AnarchyChess.Scripts.PieceHelper;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts.Games
{
    /**
     * A simple two-way dictionary that maps between piece types and character representations of the
     * pieces. This is always case-insensitive, as lowercase characters represent black, and uppercase
     * characters represent white.
     */
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

        /**
         * Register what piece is represented by what character. This is case-insensitive,
         * as lowercase characters represent black, and uppercase characters represent white.
         */
        public PieceToAscii Register(Type piece, char ascii)
        {
            ascii = char.ToUpper(ascii);
            if (!piece.GetInterfaces().Contains(typeof(IPiece)))
            {
                throw new ArgumentException($"The type must represent a type of piece. '{piece}' does not.");
            }

            _pieceToAsciiRegistry[piece] = ascii;
            _asciiToPieceRegistry[ascii] = piece;
            return this;
        }

        /**
         * Deregister a piece from the two-way dictionary.
         * <param name="piece">The type of the piece to deregister</param>
         */
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

        /**
         * Deregister a piece from the two-way dictionary.
         * <param name="ascii">The character representation for the piece (case-insensitive)</param>
         */
        public PieceToAscii Deregister(char ascii)
        {
            ascii = char.ToUpper(ascii);
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
            ascii = char.ToUpper(ascii);
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
