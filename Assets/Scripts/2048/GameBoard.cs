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

		public int MaxIndex => tilesSequence.Length - 1;

		private BaseTile[,] gameField;

		private float randomTileIndexSumProb;
		private Vector2 cellScale;
		private float animationScale = 1f;

		private void Awake() {
			cellScale = new Vector2(1f / width, 1f / height);

			gameField = new BaseTile[width, height];
			foreach (var cell in EachCell()) {
				ClearTile(cell);

				//background cell
				var rctBGCell = GameObject.Instantiate(emptyTile, transform).transform as RectTransform;
				rctBGCell.anchorMin = Vector2.Scale(cell, cellScale);
				rctBGCell.anchorMax = Vector2.Scale(cell + Vector2Int.one, cellScale);
			}

			//summ of all probabilities in tiles sequence
			randomTileIndexSumProb = 0f;
			for (int a=0; a<tilesSequence.Length; a++) {
				if (tilesSequence[a].spawnProb > 0) {
					randomTileIndexSumProb += tilesSequence[a].spawnProb;
				}
			}
		}

		public void SetAnimationScale(float scale) => animationScale = scale;

		public void ClearField() {
			foreach (var cell in EachCell()) {
				if (!IsCellFree(cell)) {
					Destroy(GetTile(cell).gameObject);
				}
				ClearTile(cell);
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
			var tile = GameObject.Instantiate(tilesSequence[index].tile, transform);
			SetTile(pos, tile);
			tile.InitTile(index, pos, cellScale, animationScale);
			return tile;
		}

		public IEnumerable<Vector2Int> EachCell() {
			for (var traveller = Vector2Int.zero; traveller.x < width; traveller.x++) {
				for (traveller.y = 0; traveller.y < height; traveller.y++) {
					yield return traveller;
				}
			}
		}

		public IEnumerable<Vector2Int> EachCellInDirection(Direction direction) {
			var startPos = new Vector2Int(direction == Direction.DIR_RIGHT ? width - 1 : 0, direction == Direction.DIR_UP ? height - 1 : 0);
			var step = new Vector2Int(direction == Direction.DIR_RIGHT ? -1 : +1, direction == Direction.DIR_UP ? -1 : +1);

			for (var traveller = startPos; traveller.x >= 0 && traveller.x < width; traveller.x += step.x) {
				for (traveller.y = startPos.y; traveller.y >= 0 && traveller.y < height; traveller.y += step.y) {
					yield return traveller;
				}
			}
		}

		public void UnmarkNewTiles() {
			foreach (var cell in EachCell()) {
				GetTile(cell)?.UnmarkNewFlag();
			}
		}


		public void MoveTile(Vector2Int pos, Vector2Int dir, float moveTime) {
			GetTile(pos).MoveTo(pos + dir, moveTime);
			SetTile(pos + dir, GetTile(pos));
			ClearTile(pos);
		}

		public void DestroyTile(Vector2Int pos) {
			Destroy(GetTile(pos).gameObject);
			ClearTile(pos);
		}

		public void MoveAndDestroyTile(Vector2Int pos, Vector2Int deathPlace, float moveTime) {
			GetTile(pos).MoveToAndDestroy(deathPlace, moveTime);
			ClearTile(pos);
		}

		public bool IsCellInsideField(in Vector2Int pos) => pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
		private bool IsCellFree(in Vector2Int pos) => GetTile(pos) == null;
		public bool CanCellBeMoved(in Vector2Int pos, in Vector2Int dir) => !IsCellFree(pos) && IsCellFree(pos + dir);
		public bool CanCellsBeCombined(in Vector2Int cell1, in Vector2Int cell2) {
			return !IsCellFree(cell1) &&
				GetTileIndex(cell1) == GetTileIndex(cell2) &&
				!IsTileNew(cell1) && !IsTileNew(cell2);
		}

		private BaseTile GetTile(in Vector2Int pos) => gameField[pos.x, pos.y];
		private void SetTile(in Vector2Int pos, in BaseTile tile) => gameField[pos.x, pos.y] = tile;
		private void ClearTile(in Vector2Int pos) => SetTile(pos, null);
		private bool IsTileNew(in Vector2Int pos) => GetTile(pos)?.IsNew == true;
		public int GetTileIndex(in Vector2Int pos) => GetTile(pos)?.TileIndex ?? -1;

	}
}