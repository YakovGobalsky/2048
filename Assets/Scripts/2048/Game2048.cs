using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public class Game2048: MonoBehaviour {
		[SerializeField] private float stepDelay = 0.1f;
		[SerializeField] private GameBoard board;

		public event System.Action<DirectionsSet> onAllowedDirectionsChanged;
		public event System.Action onWinGame;
		public event System.Action<int> onScoresChanged;

		private bool isShiftAnimationWorking = false;
		private readonly DirectionsSet allowedDirections = new DirectionsSet();

		private int _scores;
		private int Scores {
			get => _scores;
			set {
				_scores = value;
				onScoresChanged?.Invoke(_scores);

				if (GameSettings.highScore < _scores) {
					GameSettings.highScore.Value = _scores;
				}
			}
		}

		private void Start() {
			board.SetAnimationScale(1f / stepDelay);

			RestartGame();
		}

		public void RestartGame() {
			StopAllCoroutines();
			isShiftAnimationWorking = false;

			Scores = 0;

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
			int maxIndex = 0;

			bool wasMovement;
			do {
				wasMovement = false;
				foreach (var traveller in board.EachCellInDirection(direction)) {
					Vector2Int targetCell = traveller - shiftVector;
					if (board.IsCellInsideField(targetCell)) {
						if (board.CanCellBeMoved(targetCell, shiftVector)) {
							board.MoveTile(targetCell, shiftVector, stepDelay);
							wasMovement = true;
						} else if (board.CanCellsBeCombined(traveller, targetCell)) {
							int index = board.GetTileIndex(traveller);

							board.MoveAndDestroyTile(targetCell, traveller, stepDelay);
							board.DestroyTile(traveller);

							var newTile = board.SpawnGameTile(index + 1, traveller);

							Scores += newTile.Scores;
							maxIndex = Mathf.Max(index, newTile.TileIndex);
							wasMovement = true;
						}
					}
				}

				if (wasMovement) {
					yield return new WaitForSecondsRealtime(stepDelay);
				}
			} while (wasMovement);

			board.TrySpawnRandomTile();

			board.UnmarkNewTiles();

			CalculateAllowedDirections();

			if (maxIndex == board.MaxIndex) {
				onWinGame?.Invoke();
			} else {
				isShiftAnimationWorking = false;
			}
		}

		public void QuitApplication() {
			Application.Quit();
		}
	}
}