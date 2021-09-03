using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace G2048 {
	public class LoseTrigger: MonoBehaviour {
		[SerializeField] private GameObject loseOverlay;

		private void OnEnable() {
			Game2048.onAllowedDirectionsChanged += OnAllowedDirectionsChanged;
		}

		private void OnDisable() {
			Game2048.onAllowedDirectionsChanged -= OnAllowedDirectionsChanged;
		}

		private void OnAllowedDirectionsChanged(DirectionsSet directions) {
			loseOverlay.SetActive(directions.IsEmpty());
		}
	}
}