using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace G2048 {
	public class LoseTrigger: MonoBehaviour {
		[SerializeField] private Game2048 game;
		[SerializeField] private UnityEvent onLoseGame;

		private void OnEnable() {
			game.onAllowedDirectionsChanged += OnAllowedDirectionsChanged;
		}

		private void OnDisable() {
			game.onAllowedDirectionsChanged -= OnAllowedDirectionsChanged;
		}

		private void OnAllowedDirectionsChanged(DirectionsSet directions) {
			if (directions.IsEmpty()) {
				onLoseGame?.Invoke();
			}
		}
	}
}