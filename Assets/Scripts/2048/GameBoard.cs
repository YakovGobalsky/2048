using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public class GameBoard: MonoBehaviour {
		[System.Serializable]
		public class CellTile {
			public float spawnProb = -1f;
			public BaseTile tile;
		}

		[SerializeField] private int width = 4;
		[SerializeField] private int height = 4;

		[SerializeField] private GameObject emptyTile;
		[SerializeField] private CellTile[] tilesSequence;

		private BaseTile[,] gameField;

		private float randomTileIndexSumProb;

		private void Awake() {
			gameField = new BaseTile[width, height];

			foreach (var cell in EachCell()) {
				gameField[cell.x, cell.y] = null;
				UpdateTilePos(GameObject.Instantiate(emptyTile, transform).transform as RectTransform, cell.x, cell.y); //2do: replace
			}

			randomTileIndexSumProb = 0f;
			for (int a=0; a<tilesSequence.Length; a++) {
				if (tilesSequence[a].spawnProb > 0) {
					randomTileIndexSumProb += tilesSequence[a].spawnProb;
				}
			}
		}

		public void ClearField() {
			foreach (var cell in EachCell()) {
				if (!IsCellFree(cell)) {
					Destroy(gameField[cell.x, cell.y].gameObject);
				}
				gameField[cell.x, cell.y] = null;
			}
		}

		public void TrySpawnRandomTile() {
			var emptyCells = new List<Vector2Int>(width * height);
			foreach (var cell in EachCell()) {
				if (IsCellFree(cell)) {
					emptyCells.Add(cell);
				}
			}

			if (emptyCells.Count == 0) {
				//EndGameFail(); //this can't happen
			} else {
				float targetProb = Random.Range(0f, randomTileIndexSumProb);
				int index = 0;
				while (index < emptyCells.Count && (targetProb > tilesSequence[index].spawnProb)) {
					if (tilesSequence[index].spawnProb > 0f) {
						targetProb -= tilesSequence[index].spawnProb;
					}
					index++;
				}

				if (index == emptyCells.Count) {
					index = 0; //cant happen
				}

				var pos = emptyCells[Random.Range(0, emptyCells.Count)];
				SpawnGameTile(index, pos);
			}
		}

		public BaseTile SpawnGameTile(int index, Vector2Int pos) {
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

		private bool IsCellFree(in Vector2Int pos) => gameField[pos.x, pos.y] == null;
		private bool IsTileNew(in Vector2Int pos) => gameField[pos.x, pos.y]?.IsNew == true;

		public int GetTileIndex(in Vector2Int pos) => gameField[pos.x, pos.y]?.Index ?? -1;
		public bool IsCellInsideField(in Vector2Int pos) => pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
		public bool CanCellBeMoved(in Vector2Int pos, in Vector2Int dir) => !IsCellFree(pos) && IsCellFree(pos + dir);

		public bool CanCellsBeCombined(in Vector2Int cell1, in Vector2Int cell2) {
			return !IsCellFree(cell1) &&
				GetTileIndex(cell1) == GetTileIndex(cell2) &&
				!IsTileNew(cell1) && !IsTileNew(cell2);
		}

		public IEnumerable<Vector2Int> EachCell() {
			for (var traveller = Vector2Int.zero; traveller.x < width; traveller.x++) {
				for (traveller.y = 0; traveller.y < height; traveller.y++) {
					yield return traveller;
				}
			}
		}

		public IEnumerable<Vector2Int> EachCell(Direction direction) {
			var offset = -direction.ToVec2();
			var startPos = new Vector2Int(direction == Direction.DIR_RIGHT ? width - 1 : 0, direction == Direction.DIR_UP ? height - 1 : 0);
			var step = new Vector2Int(direction == Direction.DIR_RIGHT ? -1 : +1, direction == Direction.DIR_UP ? -1 : +1);

			for (var traveller = startPos; (traveller.x + offset.x) >= 0 && (traveller.x + offset.x) < width; traveller.x += step.x) {
				for (traveller.y = startPos.y; (traveller.y + offset.y) >= 0 && (traveller.y + offset.y) < height; traveller.y += step.y) {
					yield return traveller;
				}
			}
		}

		public void UnmarkNewTiles() {
			foreach (var cell in EachCell()) {
				gameField[cell.x, cell.y]?.UnmarkNewFlag();
			}
		}


		public void MoveTile(Vector2Int pos, Vector2Int dir) {
			gameField[pos.x + dir.x, pos.y + dir.y] = gameField[pos.x, pos.y];
			gameField[pos.x, pos.y] = null;
			UpdateTilePos(gameField[pos.x + dir.x, pos.y + dir.y], pos + dir);
		}

		public void DestroyTile(Vector2Int pos) {
			Destroy(gameField[pos.x, pos.y].gameObject);
			gameField[pos.x, pos.y] = null;
		}

	}
}