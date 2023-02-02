namespace AnarchyChess.Scripts.Boards
{
    public static class BoardTemplates
    {
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
