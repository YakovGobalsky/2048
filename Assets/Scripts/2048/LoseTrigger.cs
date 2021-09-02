using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace G2048 {
	public class LoseTrigger: MonoBehaviour {
		[SerializeField] private GameObject loseOverlay;

		private void OnEnable() {
			GameBoard.onAllowedDirectionsChanged += OnAllowedDirectionsChanged;
		}

		private void OnDisable() {
			GameBoard.onAllowedDirectionsChanged -= OnAllowedDirectionsChanged;
		}

		private void OnAllowedDirectionsChanged(DirectionsSet directions) {
			loseOverlay.SetActive(directions.IsEmpty());
		}
	}
}