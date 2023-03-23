

using Pieces;

public class Queen : ChessPiece {
	public Queen(bool white, Chessboard board) : base(white, board)
	{

	}

	public override bool canMove(ChessTile destTile, bool changeFlags = true) {
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
		List<ChessTile> tiles_d;
		List<ChessTile> tiles_rc;
		List<ChessTile> tiles_chosen;
		tiles_d = sameDiagonal(destTile);
		tiles_rc = sameRC(destTile);
		if (tiles_d == null && tiles_rc == null) {
			return false;
		}

		if (tiles_d != null && tiles_rc != null) {
			Console.WriteLine("IMPOSSIBLE ABORT EVERYTHING");
			try {
				throw new Exception();
			} catch (Exception e) {
                Console.WriteLine("Exception thrown");
                return false;
			}
		}

		if (tiles_d != null) {
			tiles_chosen = tiles_d;
		} else {
			tiles_chosen = tiles_rc;
		}

		if (tiles_chosen.Count == 0) {
			return true;
		}
		for (int i = 0; i < tiles_chosen.Count; i++) {
			if (tiles_chosen[i].getPiece() != null) {
				return false;
			}
		}
		return true;
	}

	public override List<ChessTile> inPath(ChessTile destTile) {
		List<ChessTile> tiles_d;
		List<ChessTile> tiles_rc;
		List<ChessTile> tiles_chosen;
		tiles_d = sameDiagonal(destTile);
		tiles_rc = sameRC(destTile);
		if (tiles_d == null && tiles_rc == null) {
			return null;
		}

		if (tiles_d != null && tiles_rc != null) {
			Console.WriteLine("IMPOSSIBLE ABORT EVERYTHING");
			try {
				throw new Exception();
			} catch (Exception e) {
                Console.WriteLine("Exception thrown");
                return null;
			}
		}

		if (tiles_d != null) {
			tiles_chosen = tiles_d;
		} else {
			tiles_chosen = tiles_rc;
		}

		if (tiles_chosen.Count == 0) {
			return null;
		}
		for (int i = 0; i < tiles_chosen.Count; i++) {
			if (tiles_chosen[i].getPiece() != null) {
				return null;
			}
		}
		return tiles_chosen;
	}

	public override string toString() {
		if (white) {
			return "Q";
		} else {
			return "q";
		}
	}
}
