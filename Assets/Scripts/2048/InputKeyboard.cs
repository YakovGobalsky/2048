using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public class InputKeyboard: MonoBehaviour {
		[SerializeField] private Game2048 game; //or global events?

		private void Update() {
			if (Input.GetKeyDown(KeyCode.DownArrow)) {
				game.ShiftBoard(Direction.DIR_DOWN);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				game.ShiftBoard(Direction.DIR_UP);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				game.ShiftBoard(Direction.DIR_LEFT);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				game.ShiftBoard(Direction.DIR_RIGHT);
			}
		}
	}
}