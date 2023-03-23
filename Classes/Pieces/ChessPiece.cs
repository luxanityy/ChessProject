
namespace Pieces;
public abstract class ChessPiece {
	public bool white;
	public ChessTile tile;
	public Chessboard board;
	public bool firstMove;

	public ChessPiece(bool white, Chessboard board) {
		this.white = white;
		this.board = board;
	}

	public void setTile(ChessTile tile) {
		this.tile = tile;
	}

	// CanMove returns true if a move can be made - if the piece can be moved on the
	// specified tile
	// It is impossible to move if the path is blocked or if the chosen tile is
	// occupied with friendly piece
	public abstract bool canMove(ChessTile destTile,bool changeFlags = true);

	// CanAttack returns true if the piece is attacking the specified tile
	// It doesn't matter to it what color piece is on the tile, as long as it
	// attacks it and the path is not blocked
	// Also, the pattern is different to pawns and king
	public abstract bool canAttack(ChessTile destTile);

	// Returns all the squares that are representing a path between the piece and a
	// specified tile
	// If the path to the tile doesn't exist or it is already blocked returns null
	// If the path exists and not blocked, returns all squares on which you can put
	// a piece to block the path
	public abstract List<ChessTile> inPath(ChessTile destTile);

	// Returns null if the tiles are not on the same diagonal
	// If they are, returns all the tiles between the tiles (excluding the destTile)
	public List<ChessTile> sameDiagonal(ChessTile destTile) {
		ChessTile[,] b = board.getBoard();
		List<ChessTile> tiles = new List<ChessTile>();
		// If same tile, then nothing possible to do
		if (tile.r == destTile.r && tile.c == destTile.c) {
			return null;
		}

		// If abs. value of differences is not equal, then not the same diagonal
		if (Math.Abs(tile.r - destTile.r) != Math.Abs(tile.c - destTile.c)) {
			return null;
		}

		int movingCoeffR = (tile.r < destTile.r) ? +1 : -1;
		int movingCoeffC = (tile.c < destTile.c) ? +1 : -1;
		int itR = tile.r + movingCoeffR;
		int itC = tile.c + movingCoeffC;

		while (itR != destTile.r && itC != destTile.c) {
			tiles.Add(b[itR,itC]);
			itR += movingCoeffR;
			itC += movingCoeffC;
		}

		return tiles;
	}

	// Returns null if the tiles are not on the same row/column
	// If they are, returns all the tiles between the tiles (excluding the destTile)
	public List<ChessTile> sameRC(ChessTile destTile) {
		ChessTile[,] b = board.getBoard();
		List<ChessTile> tiles = new List<ChessTile>();
		// If same tile, then nothing possible to do
		if (tile.r == destTile.r && tile.c == destTile.c) {
			return null;
		}
		// If both r and c are different, then not on same row or column
		if (tile.r != destTile.r && tile.c != destTile.c) {
			return null;
		}

		if (tile.r == destTile.r) {
			int movingCoeff = (tile.c < destTile.c) ? +1 : -1;
			for (int i = tile.c + movingCoeff; i != destTile.c; i += movingCoeff) {
				tiles.Add(b[tile.r,i]);
			}
			return tiles;
		}

		if (tile.c == destTile.c) {
			int movingCoeff = (tile.r < destTile.r) ? +1 : -1;
			for (int i = tile.r + movingCoeff; i != destTile.r; i += movingCoeff) {
				tiles.Add(b[i,tile.c]);
			}
			return tiles;
		}
		return null;
	}

	public abstract string toString();

}
