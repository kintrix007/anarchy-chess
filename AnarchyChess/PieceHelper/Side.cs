namespace AnarchyChess.PieceHelper
{
    public enum Side { White, Black }

    public static class SideMethods
    {
        public static Side Opposite(this Side side) => side == Side.White ? Side.Black : Side.White;
    }
}
