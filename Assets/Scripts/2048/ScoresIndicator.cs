using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	[RequireComponent(typeof(Text))]
	public class ScoresIndicator: MonoBehaviour {
		private Text text;

		private void Awake() {
			text = GetComponent<Text>();
		}

		private void OnEnable() {
			Game2048.onScoresChanged += OnScoresChanged;
		}

		private void OnDisable() {
			Game2048.onScoresChanged -= OnScoresChanged;
		}

		private void OnScoresChanged(int scores) {
			text.text = scores.ToString();
		}
	}
}