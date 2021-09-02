
namespace G2048 {
	public class DirectionsSet {
		private uint mask = 0;

		public void Clear() => mask = 0;

		public void Add(GameBoard.Direction direction) => mask |= 1u << (int)direction;

		public bool Contains(GameBoard.Direction direction) => (mask & 1u << (int)direction) != 0;

		public bool IsEmpty() => mask == 0;
	}
}