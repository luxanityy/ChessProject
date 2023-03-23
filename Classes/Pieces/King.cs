

using Pieces;

public class King : ChessPiece {
	public King(bool white, Chessboard board) : base(white, board)
	{

	}

	public override bool canMove(ChessTile destTile, bool changeFlags = true) {
		bool[] castle = board.getCastle();
		ChessTile[,] b = board.getBoard();
		int sc, lc, kingRow;
		sc = (white) ? 0 : 2;
		lc = (white) ? 1 : 3;
		kingRow = (white) ? 7 : 0;
		if (canAttack(destTile)) {
			if (board.isAttacking(!white, destTile).Count > 0) {
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

		// FIXME:
		// Can fully do without castle[] flags!!
		// Instead of remembering true/false is castling is permanently altered, this
		// can be checked right here
		// instead of castle[sc] -> this.firstMove && b[kingRow,7] == praviRook &&
		// praviRook.firstMove
		// && ofc 1) praviRook = this.white ? board.wsrook : board.bsrook;
		// && ofc 2) firstMove should be added in king and rook constructors as true
		if (!castle[sc] && !castle[lc]) {
			return false;
		}
		if (castle[sc] && (b[kingRow,6] == destTile)) {
			for (int i = 5; i < 7; i++) {
				if (b[kingRow,i].getPiece() != null) {
					return false;
				}
			}
			for (int i = 4; i < 7; i++) {
				if (board.isAttacking(!white, b[kingRow,i]).Count > 0) {
					return false;
				}
			}
			if(changeFlags)
				board.setCastleSuccess(sc);
			return true;
		}
		if (castle[lc] && (b[kingRow,2] == destTile)) {
			for (int i = 1; i < 4; i++) {
				if (b[kingRow,i].getPiece() != null) {
					return false;
				}
			}
			for (int i = 2; i < 5; i++) {
				if (board.isAttacking(!white, b[kingRow,i]).Count > 0) {
					return false;
				}
			}
			if(changeFlags)
				board.setCastleSuccess(lc);
			return true;
		}

		return false;
	}

	public override bool canAttack(ChessTile destTile) {
		ChessTile[,] b = board.getBoard();
		int[] coeff_r = { -1, -1, -1, +0, +0, +1, +1, +1 };
		int[] coeff_c = { -1, +0, +1, -1, +1, -1, +0, +1 };

		for (int i = 0; i < 8; i++) {
			if ((tile.r + coeff_r[i] >= 0) && (tile.r + coeff_r[i] < 8) && (tile.c + coeff_c[i] >= 0)
					&& (tile.c + coeff_c[i] < 8)) {
				if (b[tile.r + coeff_r[i],tile.c + coeff_c[i]] == destTile) {
					return true;
				}
			}
		}
		return false;
	}

	public override List<ChessTile> inPath(ChessTile destTile) {
		return null;
	}

	public override string toString() {
		if (white) {
			return "K";
		} else {
			return "k";
		}
	}
}
