
using Pieces;

public class Pawn : ChessPiece {

	public Pawn(bool white, Chessboard board) : base(white, board)
	{
		firstMove = true;
	}

	public override bool canMove(ChessTile destTile, bool changeFlags = true) {
		ChessTile[,] b = board.getBoard();
		int coeff = (white) ? -1 : +1;

		if (firstMove) {
			if ((b[tile.r + coeff,tile.c].getPiece()) == null && (b[tile.r + coeff * 2,tile.c].getPiece()) == null) {
				if (b[tile.r + coeff * 2,tile.c] == destTile) {
					return true;
				}
			}
		}

		if ((b[tile.r + coeff,tile.c].getPiece()) == null) {
			if (b[tile.r + coeff,tile.c] == destTile) {
				return true;
			}
		}
		if (tile.c != 0) {
			if ((b[tile.r + coeff,tile.c - 1].getPiece()) != null
					&& (b[tile.r + coeff,tile.c - 1].getPiece().white != white)) {
				if (b[tile.r + coeff,tile.c - 1] == destTile) {
					return true;
				}
			}
		}
		if (tile.c != 7) {
			if ((b[tile.r + coeff,tile.c + 1].getPiece()) != null
					&& (b[tile.r + coeff,tile.c + 1].getPiece().white != white)) {
				if (b[tile.r + coeff,tile.c + 1] == destTile) {
					return true;
				}
			}
		}

		// En-passant handling
		for (int i = 0; i < 8; i++) {
			if (board.getEnPassant()[i]) {
				if (white && tile.r == 3 && ((tile.c == i - 1) || (tile.c == i + 1))) {
					if (b[2,i] == destTile) {
						if(changeFlags)
							board.setEnPassantSuccess();
						return true;
					}
				}
				if (!white && tile.r == 4 && (tile.c == i - 1 || tile.c == i + 1)) {
					if (b[5,i] == destTile) {
						if(changeFlags)
							board.setEnPassantSuccess();
						return true;
					}
				}
			}
		}

		return false;
	}

	public override bool canAttack(ChessTile destTile) {
		ChessTile[,] b = board.getBoard();
		int coeff = (white) ? -1 : +1;

		if (tile.c != 0) {
			if (b[tile.r + coeff,tile.c - 1] == destTile) {
				return true;
			}
		}
		if (tile.c != 7) {
			if (b[tile.r + coeff,tile.c + 1] == destTile) {
				return true;
			}
		}
		return false;
	}

	// Can't block the path if the pawn is attacking you
	public override List<ChessTile> inPath(ChessTile destTile) {
		return null;
	}

	public override string toString() {
		if (white) {
			return "P";
		} else {
			return "p";
		}
	}
}
