using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	[RequireComponent(typeof(Text))]
	public class ScoresIndicator: MonoBehaviour {
		[SerializeField] private Game2048 game;
		private Text text;

		private void Awake() {
			text = GetComponent<Text>();
		}

		private void OnEnable() {
			game.onScoresChanged += OnScoresChanged;
		}

		private void OnDisable() {
			game.onScoresChanged -= OnScoresChanged;
		}

		private void OnScoresChanged(int scores) {
			text.text = scores.ToString();
		}
	}
}