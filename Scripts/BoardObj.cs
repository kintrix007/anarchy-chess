using AnarchyChess.Scripts.Boards;
using AnarchyChess.Scripts.Moves;
using AnarchyChess.Scripts.Pieces;
using Godot;
using JetBrains.Annotations;

namespace AnarchyChess.Scripts
{
    public class BoardObj : Node
    {
        public override void _Ready()
        {
            GD.Print("Hello, World!");
            GD.Print(Move.MakeRelative(new Pos(0, 0), new Pos(1, 2)));
        }
    }
}