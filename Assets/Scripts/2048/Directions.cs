using UnityEngine;

namespace G2048 {
	public enum Direction {
		DIR_LEFT = 0,
		DIR_RIGHT = 1,
		DIR_UP = 2,
		DIR_DOWN = 3
	}

	public class DirectionsSet {
		private uint mask = 0;

		public void Clear() => mask = 0;

		public void Add(Direction direction) => mask |= 1u << (int)direction;

		public bool Contains(Direction direction) => (mask & 1u << (int)direction) != 0;

		public bool IsEmpty() => mask == 0;

		public static Direction[] GetAllDirections() => new Direction[] { Direction.DIR_LEFT, Direction.DIR_RIGHT, Direction.DIR_UP, Direction.DIR_DOWN };
	}

	public static class DirectionExtension {
		public static Vector2Int ToVec2(this Direction dir) => dir switch {
			Direction.DIR_LEFT	=> Vector2Int.left,
			Direction.DIR_RIGHT	=> Vector2Int.right,
			Direction.DIR_DOWN	=> Vector2Int.down,
			Direction.DIR_UP		=> Vector2Int.up,
			_ => Vector2Int.one,
		};
	}
}