using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public class GameBoard: MonoBehaviour {
		//2do: move enum to game logic or directions stuff
		public enum Direction {
			DIR_LEFT = 0,
			DIR_RIGHT = 1,
			DIR_UP = 2,
			DIR_DOWN = 3
		}

		[System.Serializable]
		public class CellTile {
			public float spawnProb = -1f;
			public BaseTile tile;
		}

		[SerializeField] private int width = 4;
		[SerializeField] private int height = 4;
		[SerializeField] private float stepDelay = 0.1f;

		[SerializeField] private GameObject emptyTile;
		[SerializeField] private CellTile[] tilesSequence;

		private BaseTile[,] gameField; //2do: ?hide gameField array & separate board logic from game logic?
		private bool isShiftAnimationWorking = false;
		private readonly DirectionsSet allowedDirections = new DirectionsSet();

		public static event System.Action<DirectionsSet> onAllowedDirectionsChanged;

		private void Awake() {
			gameField = new BaseTile[width, height];

			foreach (var cell in EachCell()) {
				gameField[cell.x, cell.y] = null;
				UpdateTilePos(GameObject.Instantiate(emptyTile, transform).transform as RectTransform, cell.x, cell.y); //2do: replace
			}
		}

		private void Start() {
			RestartGame();
		}

		public void RestartGame() {
			StopAllCoroutines();
			isShiftAnimationWorking = false;

			//? cast "new game" event?
			ClearField();
			TrySpawnRandomTile();
			TrySpawnRandomTile();
			UnmarkNewTiles();

			CalculateAllowedDirections();
		}

		private void ClearField() {
			foreach (var cell in EachCell()) {
				if (!IsCellFree(cell)) {
					Destroy(gameField[cell.x, cell.y].gameObject);
				}
				gameField[cell.x, cell.y] = null;
			}
		}

		private void TrySpawnRandomTile() {
			var emptyCells = new List<Vector2Int>(width * height);
			foreach (var cell in EachCell()) {
				if (IsCellFree(cell)) {
					emptyCells.Add(cell);
				}
			}

			if (emptyCells.Count == 0) {
				//EndGameFail(); //this can't happen
			} else {
				int index = 0;
				var pos = emptyCells[Random.Range(0, emptyCells.Count)];
				SpawnGameTile(index, pos);
			}
		}

		private BaseTile SpawnGameTile(int index, Vector2Int pos) {
			var tile = gameField[pos.x, pos.y] = GameObject.Instantiate(tilesSequence[index].tile, transform);
			UpdateTilePos(tile, pos);
			tile.InitTile(index);
			return tile;
		}


		//2do: add move components to tiles after creation and control them
		private void UpdateTilePos(BaseTile tile, in Vector2Int pos) => UpdateTilePos(tile.transform as RectTransform, pos.x, pos.y);
		private void UpdateTilePos(RectTransform rectTransform, float x, float y) {
			rectTransform.anchorMin = new Vector2((x + 0) / width, (y + 0) / height);
			rectTransform.anchorMax = new Vector2((x + 1) / width, (y + 1) / height);
		}

		private int GetTileIndex(in Vector2Int pos) => gameField[pos.x, pos.y]?.Index ?? -1;
		private bool IsCellFree(in Vector2Int pos) => gameField[pos.x, pos.y] == null;
		private bool IsTileNew(in Vector2Int pos) => gameField[pos.x, pos.y]?.IsNew == true;
		private bool IsCellInsideField(in Vector2Int pos) => pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
		private bool CanCellBeMoved(in Vector2Int pos, in Vector2Int dir) => !IsCellFree(pos) && IsCellFree(pos + dir);

		private bool CanCellsBeCombined(in Vector2Int cell1, in Vector2Int cell2) {
			return !IsCellFree(cell1) &&
				GetTileIndex(cell1) == GetTileIndex(cell2) &&
				!IsTileNew(cell1) && !IsTileNew(cell2);
		}

		private IEnumerable<Vector2Int> EachCell() => EachCell(Vector2Int.zero, Vector2Int.zero, Vector2Int.one);
		private IEnumerable<Vector2Int> EachCell(Vector2Int startPos, Vector2Int offset, Vector2Int step) {
			for (var traveller = startPos; (traveller.x + offset.x) >= 0 && (traveller.x + offset.x) < width; traveller.x += step.x) {
				for (traveller.y = startPos.y; (traveller.y + offset.y) >= 0 && (traveller.y + offset.y) < height; traveller.y += step.y) {
					yield return traveller;
				}
			}
		}

		private void UnmarkNewTiles() {
			foreach (var cell in EachCell()) {
				gameField[cell.x, cell.y]?.UnmarkNewFlag();
			}
		}


		private void MoveTile(Vector2Int pos, Vector2Int dir) {
			gameField[pos.x + dir.x, pos.y + dir.y] = gameField[pos.x, pos.y];
			gameField[pos.x, pos.y] = null;
			UpdateTilePos(gameField[pos.x + dir.x, pos.y + dir.y], pos + dir);
		}

		private void DestroyTile(Vector2Int pos) {
			Destroy(gameField[pos.x, pos.y].gameObject);
			gameField[pos.x, pos.y] = null;
		}


		public void ShiftBoard(Direction direction) {
			if (!isShiftAnimationWorking) {
				if (allowedDirections.Contains(direction)) {
					Debug.Log($"Shift: {direction}");
					StartCoroutine(ShiftBoardRoutine(direction));
				}
			}
		}

		private IEnumerator ShiftBoardRoutine(Direction direction) {
			isShiftAnimationWorking = true;

			Vector2Int offset = direction switch {
				Direction.DIR_LEFT => new Vector2Int(+1, 0),
				Direction.DIR_RIGHT => new Vector2Int(-1, 0),
				Direction.DIR_DOWN => new Vector2Int(0, +1),
				Direction.DIR_UP => new Vector2Int(0, -1),
				_ => Vector2Int.one,
			};

			var startPos = new Vector2Int(direction == Direction.DIR_RIGHT ? width - 1 : 0, direction == Direction.DIR_UP ? height - 1 : 0);
			var step = new Vector2Int(direction == Direction.DIR_RIGHT ? -1 : +1, direction == Direction.DIR_UP ? -1 : +1);

			bool wasMovement;
			do {
				wasMovement = false;
				bool needDelay = false;
				foreach (var traveller in EachCell(startPos, offset, step)) {
					Vector2Int targetCell = traveller + offset;
					if (CanCellBeMoved(targetCell, -offset)) {
						MoveTile(targetCell, -offset);
						wasMovement = true;
						needDelay = true;
					} else if (CanCellsBeCombined(traveller, targetCell)) {
						int index = GetTileIndex(traveller);
						DestroyTile(traveller);
						DestroyTile(targetCell); //2do: оставить движение для анимации?
						SpawnGameTile(index + 1, traveller);
						wasMovement = true;
						//needDelay = true;
					}
				}

				if (needDelay) {
					yield return new WaitForSecondsRealtime(stepDelay);
				}
			} while (wasMovement);

			TrySpawnRandomTile();

			UnmarkNewTiles();

			CalculateAllowedDirections();

			isShiftAnimationWorking = false;
		}

		private bool IsMoveAllowed(in Vector2Int cell, in Vector2Int dir) {
			Vector2Int targetCell = cell + dir;
			if (IsCellInsideField(targetCell)) {
				if (CanCellBeMoved(cell, dir)) {
					return true;
				};
				return CanCellsBeCombined(cell, targetCell);
			}

			return false;
		}

		private void CalculateAllowedDirections() {
			allowedDirections.Clear();
			foreach (var cell in EachCell()) {
				if (IsMoveAllowed(cell, Vector2Int.left)) {
					allowedDirections.Add(Direction.DIR_LEFT);
				}
				if (IsMoveAllowed(cell, Vector2Int.right)) {
					allowedDirections.Add(Direction.DIR_RIGHT);
				}
				if (IsMoveAllowed(cell, Vector2Int.up)) {
					allowedDirections.Add(Direction.DIR_UP);
				}
				if (IsMoveAllowed(cell, Vector2Int.down)) {
					allowedDirections.Add(Direction.DIR_DOWN);
				}
			}

			onAllowedDirectionsChanged?.Invoke(allowedDirections);
		}

	}
}