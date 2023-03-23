
using Pieces;

public class ChessTile 
{

	private string name;
	private bool white;
	private ChessPiece piece;
	public int r;
	public int c;

	public bool RightClicked { get; set; } = false;

	public ChessTile(string name, bool white, int r, int c) 
	{
		this.name = name;
		this.white = white;
		this.r = r;
		this.c = c;
	}

	public string getName() 
	{
		return name;
	}

	public bool isWhite() 
	{
		return white;
	}

	public void setPiece(ChessPiece piece) 
	{
		this.piece = piece;
		this.piece.setTile(this);
	}

	public void removePiece() 
	{
		if (this.piece != null) 
		{
			this.piece.setTile(null);
			this.piece = null;
		}
	}

	public ChessPiece getPiece() 
	{
		return piece;
	}

	public string toString() {
		string odg = "";
		odg += (piece == null) ? "-" : piece.toString();
		// odg += name;
		return odg;
	}
}
