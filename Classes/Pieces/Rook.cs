
using Pieces;

public class Rook : ChessPiece {
	public Rook(bool white, Chessboard board) : base(white, board)
	{

	}

	public override  bool canMove(ChessTile destTile, bool changeFlags = true) {
		if (!canAttack(destTile)) {
			return false;
		}

		if (destTile.getPiece() == null) {
			return true;
		}

		if (destTile.getPiece().white == white) {
			return false;
		}

		return true;
	}

	public override bool canAttack(ChessTile destTile) {
		List<ChessTile> tiles;
		tiles = sameRC(destTile);
		if (tiles == null) {
			return false;
		}
		// If size is 0 then it can't be blocked
		if (tiles.Count == 0) {
			return true;
		}
		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i].getPiece() != null) {
				return false;
			}
		}
		return true;
	}

	public override List<ChessTile> inPath(ChessTile destTile) {
		List<ChessTile> tiles;
		tiles = sameRC(destTile);
		if (tiles == null) {
			return null;
		}
		// If size is 0 then it can't be blocked
		if (tiles.Count == 0) {
			return null;
		}
		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i].getPiece() != null) {
				return null;
			}
		}
		return tiles;
	}

	public override string toString() {
		if (white) {
			return "R";
		} else {
			return "r";
		}
	}
}
