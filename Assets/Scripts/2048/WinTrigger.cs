using UnityEngine;

namespace G2048 {
	public class WinTrigger: MonoBehaviour {
		[SerializeField] private GameObject winOverlay;

		private void OnEnable() {
			Game2048.onWinGame += OnWinGame;
		}

		private void OnDisable() {
			Game2048.onWinGame -= OnWinGame;
		}

		private void OnWinGame() {
			winOverlay.SetActive(true);
		}
	}
}