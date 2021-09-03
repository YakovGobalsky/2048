using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	[RequireComponent(typeof(GameBoard))]
	public class Game2048: MonoBehaviour {
		[SerializeField] private float stepDelay = 0.1f;

		private GameBoard board;
		private bool isShiftAnimationWorking = false;
		private readonly DirectionsSet allowedDirections = new DirectionsSet();

		public static event System.Action<DirectionsSet> onAllowedDirectionsChanged;

		private void Awake() {
			board = GetComponent<GameBoard>();
		}

		private void Start() {
			RestartGame();
		}

		public void RestartGame() {
			StopAllCoroutines();
			isShiftAnimationWorking = false;

			//? cast "new game" event?
			board.ClearField();
			board.TrySpawnRandomTile();
			board.TrySpawnRandomTile();
			board.UnmarkNewTiles();

			CalculateAllowedDirections();
		}

		private bool IsMoveAllowed(in Vector2Int cell, in Vector2Int dir) {
			Vector2Int targetCell = cell + dir;
			if (board.IsCellInsideField(targetCell)) {
				if (board.CanCellBeMoved(cell, dir)) {
					return true;
				};
				return board.CanCellsBeCombined(cell, targetCell);
			}

			return false;
		}

		private void CalculateAllowedDirections() {
			allowedDirections.Clear();
			foreach (var cell in board.EachCell()) {
				foreach (var dir in DirectionsSet.GetAllDirections()) {
					if (IsMoveAllowed(cell, dir.ToVec2())) {
						allowedDirections.Add(dir);
					}
				}
			}

			onAllowedDirectionsChanged?.Invoke(allowedDirections);
		}


		public void ShiftBoard(Direction direction) {
			if (!isShiftAnimationWorking) {
				if (allowedDirections.Contains(direction)) {
					StartCoroutine(ShiftBoardRoutine(direction));
				}
			}
		}

		private IEnumerator ShiftBoardRoutine(Direction direction) {
			isShiftAnimationWorking = true;

			var shiftVector = direction.ToVec2();

			bool wasMovement;
			do {
				wasMovement = false;
				bool needDelay = false;
				foreach (var traveller in board.EachCell(direction)) {
					Vector2Int targetCell = traveller - shiftVector;
					if (board.CanCellBeMoved(targetCell, shiftVector)) {
						board.MoveTile(targetCell, shiftVector);
						wasMovement = true;
						needDelay = true;
					} else if (board.CanCellsBeCombined(traveller, targetCell)) {
						int index = board.GetTileIndex(traveller);
						board.DestroyTile(traveller);
						board.DestroyTile(targetCell); //2do: оставить движение для анимации?
						board.SpawnGameTile(index + 1, traveller);
						wasMovement = true;
						//needDelay = true;
					}
				}

				if (needDelay) {
					yield return new WaitForSecondsRealtime(stepDelay);
				}
			} while (wasMovement);

			board.TrySpawnRandomTile();

			board.UnmarkNewTiles();

			CalculateAllowedDirections();

			isShiftAnimationWorking = false;
		}

	}
}