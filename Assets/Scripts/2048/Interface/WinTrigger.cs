using UnityEngine;
using UnityEngine.Events;

namespace G2048 {
	public class WinTrigger: MonoBehaviour {
		[SerializeField] private Game2048 game;
		[SerializeField] private UnityEvent onWinGame;

		private void OnEnable() {
			game.onWinGame += OnWinGame;
		}

		private void OnDisable() {
			game.onWinGame -= OnWinGame;
		}

		private void OnWinGame() {
			onWinGame?.Invoke();
		}
	}
}