using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	public class HighscoresIndicator: MonoBehaviour {
		private Text text;

		private void Awake() {
			text = GetComponent<Text>();
		}

		private void OnEnable() {
			GameSettings.highScore.onChanged += OnHighscoresChanged;
			text.text = GameSettings.highScore.Value.ToString();
		}

		private void OnDisable() {
			GameSettings.highScore.onChanged -= OnHighscoresChanged;
		}

		private void OnHighscoresChanged(int scores) {
			text.text = scores.ToString();
		}
	}
}