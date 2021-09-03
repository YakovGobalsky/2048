using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public static class GameSettings {
		public static readonly PlayerPrefsValueInt highScore = new PlayerPrefsValueInt("highscore", 0);
	}
}