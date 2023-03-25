

using Chess.Common;
using Pieces;
using System.Media;
using System.Timers;


public class Chessboard {
	private ChessTile[,] tileMatrix;
	public bool whiteToMove;
	public bool gameOver;
	private Outcome_e outcome;

	// Piece pointers
	private ChessPiece whiteKing;
	private ChessPiece blackKing;
	private ChessPiece wsrook;
	private ChessPiece wlrook;
	private ChessPiece bsrook;
	private ChessPiece blrook;

	// Just flags
	private bool[] enPassant;
	private bool enPassantSuccess;
	private bool[] castle;
	private bool[] castleSuccess;
	private int draw50rule;
	private int[] pieceCounter;
	private int whiteTimer;
	private int blackTimer;
	private int timerIncr;
	private System.Timers.Timer timer;
	List<Position> positionQ;

	
    SoundPlayer regularMoveSound = new SoundPlayer(@"wwwroot/Sounds/move-self.wav");
    SoundPlayer captureMoveSound = new SoundPlayer(@"wwwroot/Sounds/capture.wav");


    public Chessboard(int initTime, int incrTime) {
		tileMatrix = new ChessTile[8,8];
		whiteToMove = true;
		this.gameOver = false;
		enPassant = new bool[8];
		for (int i = 0; i < 8; i++) {
			enPassant[i] = false;
		}
		enPassantSuccess = false;
		castle = new bool[4];
		for (int i = 0; i < 4; i++) {
			castle[i] = true; // White short, White long, Black short, Black long
		}
		castleSuccess = new bool[4];
		for (int i = 0; i < 4; i++) {
			castleSuccess[i] = false; // White short, White long, Black short, Black long
		}
		draw50rule = 0;
		pieceCounter = new int[10];
		pieceCounter[0] = 2;
		pieceCounter[1] = 2;
		pieceCounter[2] = 2;
		pieceCounter[3] = 1;
		pieceCounter[4] = 8;
		pieceCounter[5] = 2;
		pieceCounter[6] = 2;
		pieceCounter[7] = 2;
		pieceCounter[8] = 1;
		pieceCounter[9] = 8;
		positionQ = new List<Position>();
		int[] rows = { 8, 7, 6, 5, 4, 3, 2, 1 };
		string[] columns = { "a", "b", "c", "d", "e", "f", "g", "h" };
		ChessPiece[] blackP = { new Rook(false, this), new Knight(false, this), new Bishop(false, this),
				new Queen(false, this), new King(false, this), new Bishop(false, this), new Knight(false, this),
				new Rook(false, this) };
		ChessPiece[] whiteP = { new Rook(true, this), new Knight(true, this), new Bishop(true, this),
				new Queen(true, this), new King(true, this), new Bishop(true, this), new Knight(true, this),
				new Rook(true, this) };

		bool tileWhite = true;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				tileMatrix[i,j] = new ChessTile(columns[j] + rows[i], tileWhite, i, j);
				tileWhite = !tileWhite;
			}
			tileWhite = !tileWhite;
		}

		for (int i = 0; i < 8; i++) {
			tileMatrix[0,i].setPiece(blackP[i]);
		}

		for (int i = 0; i < 8; i++) {
			tileMatrix[1,i].setPiece(new Pawn(false, this));
		}

		for (int i = 0; i < 8; i++) {
			tileMatrix[6,i].setPiece(new Pawn(true, this));
		}

		for (int i = 0; i < 8; i++) {
			tileMatrix[7,i].setPiece(whiteP[i]);
		}

		whiteKing = tileMatrix[7,4].getPiece();
		blackKing = tileMatrix[0,4].getPiece();
		wsrook = tileMatrix[7,7].getPiece();
		wlrook = tileMatrix[7,0].getPiece();
		bsrook = tileMatrix[0,7].getPiece();
		blrook = tileMatrix[0,0].getPiece();
		whiteTimer = initTime*10;
        blackTimer = initTime*10;
		timerIncr = incrTime*10;
        timer = new System.Timers.Timer(100);
        timer.Elapsed += OnTimedEvent;
    }
	public int GetWhiteTimer()
	{
		return whiteTimer;
	}
    public int GetBlackTimer()
    {
        return blackTimer;
    }
    void OnTimedEvent(object source, ElapsedEventArgs e)
	{
        // Check whose turn it is.
        //Console.WriteLine("Black: " + blackTimer);
		if(gameOver)
			timer.Enabled = false;

        if (whiteToMove)
		{
            whiteTimer--;
		}
		else
		{
			blackTimer--;
        }

		if (whiteTimer <= 0)
		{
			timer.Enabled = false;
			if ((pieceCounter[5] == 0) && (pieceCounter[8] == 0) && (pieceCounter[9] == 0) && (pieceCounter[6] + pieceCounter[7] < 2))
			{
				outcome = Outcome_e.DRAW_TO_INS;
			}
			else
			{
				outcome = Outcome_e.LOSE_TO;
			}
			gameOver = true;
			return;
		}

		if (blackTimer <=0)
		{
            timer.Enabled = false;
            if ((pieceCounter[0] == 0) && (pieceCounter[3] == 0) && (pieceCounter[4] == 0) && (pieceCounter[1] + pieceCounter[2] < 2))
            {
                outcome = Outcome_e.DRAW_TO_INS;
            }
            else
            {
                outcome = Outcome_e.WIN_TO;
            }
            gameOver = true;
            return;
        }
	}

        public int[] parseMove(string move) {
		int[] parsed = new int[4];
		parsed[0] = 8 - (move[1] - '0');
		parsed[1] = move[0] - 'a';
		parsed[2] = 8 - (move[3] - '0');
		parsed[3] = move[2] - 'a';
		return parsed;
	}

	public bool move(string move, string promote) {
		ChessPiece p1;
		ChessPiece p2;
		ChessPiece enemyKing;
		int isCheck;
		int[] moveParsed;
		List<ChessPiece> attackers;

		if (!checkMoveString(move)) {
			return false;
		}

		moveParsed = parseMove(move);
		p1 = tileMatrix[moveParsed[0],moveParsed[1]].getPiece();
		p2 = tileMatrix[moveParsed[2],moveParsed[3]].getPiece();

		if (p1 == null) {
			return false;
		}

		if (p1.white != whiteToMove) {
			return false;
		}

		if (!tileMatrix[moveParsed[0],moveParsed[1]].getPiece().canMove(tileMatrix[moveParsed[2],moveParsed[3]])) {
			return false;
		}

		// This does the move
		if (!moveIfLegal(moveParsed[0], moveParsed[1], moveParsed[2], moveParsed[3], false, promote)) {
			return false;
		}

		// En-Passant status handling
		for (int i = 0; i < 8; i++) {
			enPassant[i] = false;
		}
		if (p1.toString() == "P" && moveParsed[0] == 6 && moveParsed[2] == 4) {
			enPassant[moveParsed[1]] = true;
		}
		if (p1.toString() == "p" && moveParsed[0] == 1 && moveParsed[2] == 3) {
			enPassant[moveParsed[1]] = true;
		}

		if (p1 == whiteKing) {
			castle[0] = false;
			castle[1] = false;
		}
		if (p1 == blackKing) {
			castle[2] = false;
			castle[3] = false;
		}

		if (p1 == wlrook) {
			castle[1] = false;
		}
		if (p1 == wsrook) {
			castle[0] = false;
		}
		if (p1 == blrook) {
			castle[3] = false;
		}
		if (p1 == bsrook) {
			castle[2] = false;
		}

		if (p2 != null) {
			if (p2 == wlrook) {
				castle[1] = false;
			}
			if (p2 == wsrook) {
				castle[0] = false;
			}
			if (p2 == blrook) {
				castle[3] = false;
			}
			if (p2 == bsrook) {
				castle[2] = false;
			}
		}

		// Only after successful move you can say that FirstMove is false
		// This variable only used in pawns
		p1.firstMove = false;

		enemyKing = (p1.white) ? blackKing : whiteKing;
		attackers = isAttacking(p1.white, enemyKing.tile);
		isCheck = attackers.Count;

		// Now check for mate
		if (isCheck > 0) {
			if (isCheck > 2) {
				try {
					throw new Exception();
				} catch (Exception e) {
                    Console.WriteLine("Exception");
                    return true;
				}
			}
			// If double check, then the only possible solution for king is to MOVE
			if (isCheck == 2) {
				if (!canKingMove(enemyKing)) {
					gameOver = true;
					outcome = (p1.white) ? Outcome_e.WIN_C : Outcome_e.LOSE_C;
					return true;
				}
			}
			if (isCheck == 1) {
				ChessPiece attacker = attackers[0];
				if (!canBlockCheckOrTake(!attacker.white, attacker.inPath(enemyKing.tile), attacker)
						&& !canKingMove(enemyKing)) {
					gameOver = true;
					outcome = (p1.white) ? Outcome_e.WIN_C : Outcome_e.LOSE_C;
					return true;
				}
			}

			// TODO: Sound for check move
		}

		// Check for stalemate
		if (!hasLegalMove(!p1.white) && (isCheck == 0)) {
			gameOver = true;
			outcome = Outcome_e.DRAW_STALEMATE;
			return true;
		}

		// 50-move draw
		if (p2 != null || p1.toString() == "P" || p1.toString() == "p") {
			draw50rule = 0;
		} else {
			draw50rule++;
		}

		if (draw50rule == 100) {
			gameOver = true;
			outcome = Outcome_e.DRAW_50;
			return true;
		}

		// If the last move was a pawn move, or if the last move was taking a pawn, then
		// delete previous positions safely
		if (p1.toString() == "P" || p1.toString() == "p") {
			positionQ.Clear();
		}
		if (p2 != null) {
			positionQ.Clear();
		}

		if (recordPosition()) {
			gameOver = true;
			outcome = Outcome_e.DRAW_3;
			return true;
		}

		if ((pieceCounter[0] == 0) && (pieceCounter[3] == 0) && (pieceCounter[4] == 0) && (pieceCounter[5] == 0)
				&& (pieceCounter[8] == 0) && (pieceCounter[9] == 0)) {
			bool draw1 = (pieceCounter[1] + pieceCounter[2] < 2) && (pieceCounter[6] + pieceCounter[7] < 2);
			bool draw2 = (pieceCounter[1] == 2) && (pieceCounter[2] == 0) && (pieceCounter[6] == 0)
					&& (pieceCounter[7] == 0);
			bool draw3 = (pieceCounter[1] == 0) && (pieceCounter[2] == 0) && (pieceCounter[6] == 2)
					&& (pieceCounter[7] == 0);
			if (draw1 || draw2 || draw3) {
				gameOver = true;
				outcome = Outcome_e.DRAW_INS;
				return true;
			}
		}

		if (isCheck == 0) {
			if (p2 == null) {
				regularMoveSound.Play();

            } else {
				captureMoveSound.Play();

            }
		}

		if (whiteToMove)
		{
			whiteTimer += timerIncr;
		} else
		{
            blackTimer += timerIncr;
        }
        timer.Enabled = true;

        whiteToMove = !whiteToMove;
		return true;
	}

	private bool checkMoveString(String move) {
		if (move.Length != 4) {
			return false;
		}
		if (!(move[0] == 'a' || move[0] == 'b' || move[0] == 'c' || move[0] == 'd'
				|| move[0] == 'e' || move[0] == 'f' || move[0] == 'g' || move[0] == 'h')) {
			return false;
		}
		if (!(move[1] == '1' || move[1] == '2' || move[1] == '3' || move[1] == '4'
				|| move[1] == '5' || move[1] == '6' || move[1] == '7' || move[1] == '8')) {
			return false;
		}
		if (!(move[2] == 'a' || move[2] == 'b' || move[2] == 'c' || move[2] == 'd'
				|| move[2] == 'e' || move[2] == 'f' || move[2] == 'g' || move[2] == 'h')) {
			return false;
		}
		if (!(move[3] == '1' || move[3] == '2' || move[3] == '3' || move[3] == '4'
				|| move[3] == '5' || move[3] == '6' || move[3] == '7' || move[3] == '8')) {
			return false;
		}
		return true;
	}

	private bool moveIfLegal(int r1, int c1, int r2, int c2, bool alwaysUndo, String rsp) {
		ChessPiece p1;
		ChessPiece p2;
		ChessPiece enPassantPawn = null;
		ChessPiece castleRook = null;
		ChessPiece kingInDanger;
		bool illegal;

		p1 = tileMatrix[r1,c1].getPiece();
		p2 = tileMatrix[r2,c2].getPiece();
		kingInDanger = (p1.white) ? whiteKing : blackKing;

		tileMatrix[r1,c1].removePiece();
		tileMatrix[r2,c2].removePiece();

		// Special case of Piece removal in case of EnPassant - the pawn is not removed
		// from from r2,c2 coordinates, but rather from 3,c2 or 4,c2 depending on the
		// color
		// Additionally, previous removing of r2,c2 won't be problematic, since if
		// enPassant was successful, then there is no piece on r2,c2 in the first place
		if (enPassantSuccess) {
			if (p1.white) {
				enPassantPawn = tileMatrix[3,c2].getPiece();
				tileMatrix[3,c2].removePiece();
			} else {
				enPassantPawn = tileMatrix[4,c2].getPiece();
				tileMatrix[4,c2].removePiece();
			}
		}

		if (castleSuccess[0]) {
			castleRook = tileMatrix[7,7].getPiece();
			tileMatrix[7,7].removePiece();
			tileMatrix[7,5].setPiece(castleRook);
		} else if (castleSuccess[1]) {
			castleRook = tileMatrix[7,0].getPiece();
			tileMatrix[7,0].removePiece();
			tileMatrix[7,3].setPiece(castleRook);
		} else if (castleSuccess[2]) {
			castleRook = tileMatrix[0,7].getPiece();
			tileMatrix[0,7].removePiece();
			tileMatrix[0,5].setPiece(castleRook);
		} else if (castleSuccess[3]) {
			castleRook = tileMatrix[0,0].getPiece();
			tileMatrix[0,0].removePiece();
			tileMatrix[0,3].setPiece(castleRook);
		}

		if ((p1.toString() == "P" && r2 == 0) || (p1.toString() == "p" && r2 == 7)) {
			if (rsp.Equals("N")) {
				tileMatrix[r2,c2].setPiece(new Knight(p1.white, this));
			} else if (rsp.Equals("B")) {
				tileMatrix[r2,c2].setPiece(new Bishop(p1.white, this));
			} else if (rsp.Equals("R")) {
				tileMatrix[r2,c2].setPiece(new Rook(p1.white, this));
			} else {
				tileMatrix[r2,c2].setPiece(new Queen(p1.white, this));
			}
		} else {
			tileMatrix[r2,c2].setPiece(p1);
		}

		// Now check if the new situation on the board is legal! (The one who moved
		// can't be in check)
		// If illegal, undo everything that was done

		illegal = (isAttacking(!p1.white, kingInDanger.tile).Count > 0) ? true : false;

		if (illegal || alwaysUndo) {
			// UNDO Everything
			tileMatrix[r2,c2].removePiece();

			// Pretty sure successful castle can't be illegal (since the only way King ends
			// up in check after castle is if that tile was already in check, which if it
			// was it would prevent castle in canMove function
			if (castleSuccess[0]) {
				tileMatrix[7,5].removePiece();
				tileMatrix[7,7].setPiece(castleRook);
				castleSuccess[0] = false;
			} else if (castleSuccess[1]) {
				tileMatrix[7,3].removePiece();
				tileMatrix[7,0].setPiece(castleRook);
				castleSuccess[1] = false;
			} else if (castleSuccess[2]) {
				tileMatrix[0,5].removePiece();
				tileMatrix[0,7].setPiece(castleRook);
				castleSuccess[2] = false;
			} else if (castleSuccess[3]) {
				tileMatrix[0,3].removePiece();
				tileMatrix[0,0].setPiece(castleRook);
				castleSuccess[3] = false;
			}

			if (enPassantSuccess) {
				if (p1.white) {
					tileMatrix[3,c2].setPiece(enPassantPawn);
				} else {
					tileMatrix[4,c2].setPiece(enPassantPawn);
				}
				enPassantSuccess = false;
			}

			if (p2 != null) {
				tileMatrix[r2,c2].setPiece(p2);
			}
			tileMatrix[r1,c1].setPiece(p1);

			return !illegal;
		}

		enPassantSuccess = false;
		for (int i = 0; i < 4; i++) {
			castleSuccess[i] = false;
		}
		return true;
	}

	public List<ChessPiece> isAttacking(bool isWhiteAttacking, ChessTile destTile) {
		List<ChessPiece> list = new List<ChessPiece>();
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				ChessPiece tmp = tileMatrix[i,j].getPiece();
				if (tmp != null) {
					if (tmp.white == isWhiteAttacking) {
						if (tmp.canAttack(destTile)) {
							list.Add(tmp);
						}
					}
				}
			}
		}
		return list;
	}

	public bool hasLegalMove(bool isWhite) {
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				ChessPiece tmp = tileMatrix[i,j].getPiece();
				if (tmp != null) {
					if (tmp.white == isWhite) {
						for (int k = 0; k < 8; k++) {
							for (int h = 0; h < 8; h++) {
								if (tmp.canMove(tileMatrix[k,h])) {
									if (moveIfLegal(i, j, k, h, true, "Q")) {
										return true;
									}
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

    public bool[,] pieceCanMove(ChessPiece piece)
    {
        bool[,] m = new bool[8,8];
		if (piece == null)
			return m;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                m[i, j] = piece.canMove(tileMatrix[i,j], false) && piece.white == whiteToMove;
            }
        }
        return m;
    }

    public bool canKingMove(ChessPiece king) {
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (king.canMove(tileMatrix[i,j])) {
					if (moveIfLegal(king.tile.r, king.tile.c,i,j,true,"Q"))
					{
                        return true;
                    }		
				}
			}
		}
		return false;
	}

	public bool canBlockCheckOrTake(bool whiteDefending, List<ChessTile> free, ChessPiece attacker) {
		List<ChessTile> whichToMove = new List<ChessTile>();
		bool enPassantPossible = false;
		int enPassantColumn = 0;
		int enPassantRow = (whiteDefending) ? 2 : 5;
		if (free != null) {
			for (int i = 0; i < free.Count; i++) {
				whichToMove.Add(free[i]);
			}
		}
		whichToMove.Add(attacker.tile);

		for (int i = 0; i < 8; i++) {
			if (enPassant[i] == true) {
				enPassantPossible = true;
				enPassantColumn = i;
				break;
			}
		}

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				ChessPiece tmp = tileMatrix[i,j].getPiece();
				if (tmp != null) {
					if (tmp.white == whiteDefending) {
						for (int k = 0; k < whichToMove.Count; k++) {
							ChessTile t = whichToMove[k];
							if (tmp.canMove(t)) {
								if (moveIfLegal(i, j, t.r, t.c, true, "Q")) {
									return true;
								}
							}
						}
						if (enPassantPossible) {
							if (tmp.toString() == "P" || tmp.toString() == "p") {
								if (tmp.canMove(tileMatrix[enPassantRow,enPassantColumn])) {
									if (moveIfLegal(i, j, enPassantRow, enPassantColumn, true, "Q")) {
										return true;
									}
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	public bool isPromotion(string move) {
		int[] moveParsed;
		ChessPiece pawn;

		if (!checkMoveString(move)) {
			return false;
		}
		moveParsed = parseMove(move);
		pawn = tileMatrix[moveParsed[0],moveParsed[1]].getPiece();

		if (pawn == null) {
			return false;
		}

		if ((whiteToMove && pawn.toString() == "P" && pawn.tile.r == 1
				&& pawn.canMove(tileMatrix[moveParsed[2],moveParsed[3]]))
				|| (!whiteToMove && pawn.toString() == "p" && pawn.tile.r == 6
						&& pawn.canMove(tileMatrix[moveParsed[2],moveParsed[3]]))) {
			return true;
		}
		return false;
	}

	public bool recordPosition() {
		string position = "";
		string value;
		Position thisPos;
		bool posFound = false;
		for (int i = 0; i < 10; i++) {
			pieceCounter[i] = 0;
		}

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				ChessPiece tmp = tileMatrix[i,j].getPiece();
				if (tmp == null) {
					position += "-";
				} else {
					value = tmp.toString();
					position += value;
					switch (value) {
					case "R":
						pieceCounter[0]++;
						break;
					case "N":
						pieceCounter[1]++;
						break;
					case "B":
						pieceCounter[2]++;
						break;
					case "Q":
						pieceCounter[3]++;
						break;
					case "P":
						pieceCounter[4]++;
						break;
					case "r":
						pieceCounter[5]++;
						break;
					case "n":
						pieceCounter[6]++;
						break;
					case "b":
						pieceCounter[7]++;
						break;
					case "q":
						pieceCounter[8]++;
						break;
					case "p":
						pieceCounter[9]++;
						break;
					}
				}
			}
		}
		for (int i = 0; i < 8; i++) {
			position += (enPassant[i]) ? 1 : 0;
		}
		for (int i = 0; i < 4; i++) {
			position += (castle[i]) ? 1 : 0;
		}

		for (int i = 0; i < positionQ.Count; i++) {
			if (positionQ[i].isEqual(position)) {
				posFound = true;
				if (positionQ[i].getCnt() == 3) {
					return true;
				}
				break;
			}
		}

		if (!posFound) {
			thisPos = new Position(position);
			positionQ.Add(thisPos);
		}

		return false;
	}

	public ChessTile[,] getBoard() {
		return tileMatrix;
	}

	public bool[] getEnPassant() {
		return enPassant;
	}

	public void setEnPassantSuccess() {
		enPassantSuccess = true;
	}

	public bool[] getCastle() {
		return castle;
	}

	public void setCastleSuccess(int index) {
		castleSuccess[index] = true;
	}

	public bool isGameOver() {
		return gameOver;
	}
	public Outcome_e GetOutcome()
	{
		return outcome;
	}

	public string toString() {
		string odg = "";
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				odg += tileMatrix[i,j] + " ";
			}

			odg += "    ";

			for (int j = 0; j < 8; j++) {
				if (tileMatrix[i,j].getPiece() != null) {
					odg += ((tileMatrix[i,j].getPiece().firstMove) ? 1 : 0) + " ";
				} else {
					odg += "- ";
				}

			}

			odg += "\n";
		}
		odg += "WhiteToMove: " + whiteToMove + "\n";
		odg += "Castle: " + castle[0] + " " + castle[1] + " " + castle[2] + " " + castle[3] + "\n";
		odg += "EnPassant: " + ((enPassant[0]) ? 1 : 0) + ((enPassant[1]) ? 1 : 0) + ((enPassant[2]) ? 1 : 0)
				+ ((enPassant[3]) ? 1 : 0) + ((enPassant[4]) ? 1 : 0) + ((enPassant[5]) ? 1 : 0)
				+ ((enPassant[6]) ? 1 : 0) + ((enPassant[7]) ? 1 : 0) + "\n";
		odg += "50move: " + draw50rule + "\n";
		odg += "Count: " + pieceCounter[0] + "," + pieceCounter[1] + "," + pieceCounter[2] + "," + pieceCounter[3] + ","
				+ pieceCounter[4] + "," + pieceCounter[5] + "," + pieceCounter[6] + "," + pieceCounter[7] + ","
				+ pieceCounter[8] + "," + pieceCounter[9] + "\n";
		odg += "Positions: " + positionQ.Count + "\n";
		for (int i = 0; i < positionQ.Count; i++) {
			odg += positionQ[i].getPosition() + " " + positionQ[i].getCnt() + "\n";
		}
		return odg;
	}
}
