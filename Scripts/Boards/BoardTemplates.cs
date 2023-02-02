namespace AnarchyChess.Scripts.Boards
{
    public static class BoardTemplates
    {
        /// <summary>
        /// The standard template from Chess.
        /// </summary>
        /// <returns>The board with pieces from that template</returns>
        public static Board Standard() =>
            @"r n b q k b n r
p p p p p p p p
- - - - - - - -
- - - - - - - -
- - - - - - - -
- - - - - - - -
P P P P P P P P
R N B Q K B N R"
                .ParseBoard();
    }
}