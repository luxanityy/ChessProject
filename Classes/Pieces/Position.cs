public class Position {

	private string pos;
	private int cnt;

	public Position(string pos) {
		this.pos = pos;
		cnt = 1;
	}

	public string getPosition() {
		return pos;
	}

	public int getCnt() {
		return cnt;
	}

	public bool isEqual(string pos2) {
		if (pos.Equals(pos2)) {
			cnt++;
			return true;
		}
		return false;
	}

}
