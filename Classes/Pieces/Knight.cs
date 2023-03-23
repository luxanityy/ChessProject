

using Pieces;

public class Knight : ChessPiece {
	public Knight(bool white, Chessboard board) : base(white, board)
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
		ChessTile[,] b = board.getBoard();
		int[] coeff_r = { -1, +1, -2, +2, -2, +2, -1, +1 };
		int[] coeff_c = { -2, -2, -1, -1, +1, +1, +2, +2 };

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

	// Nothing can block knights so just return null
	public override List<ChessTile> inPath(ChessTile destTile) {
		return null;
	}

	public override string toString() {
		if (white) {
			return "N";
		} else {
			return "n";
		}
	}
}
