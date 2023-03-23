namespace Chess.Common
{
    public enum PieceColor
    {
        White,
        Black
    }
    public enum TileColor
    {
        White,
        Black
    }
    public enum PieceType
    {
        King,
        Queen,
        Bishop,
        Knight,
        Rook,
        Pawn
    }

    public enum PromotionSelection
    {
        Queen, 
        Bishop, 
        Knight, 
        Rook
    }

    public enum Outcome_e
    {
        WIN_C, WIN_TO, DRAW_50, DRAW_STALEMATE, DRAW_INS, DRAW_3, DRAW_TO_INS, LOSE_C, LOSE_TO
    }
}
