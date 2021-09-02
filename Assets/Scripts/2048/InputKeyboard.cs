using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public class InputKeyboard: MonoBehaviour {
		[SerializeField] private GameBoard gameBoard; //or global events?

		private void Update() {
			if (Input.GetKeyDown(KeyCode.DownArrow)) {
				gameBoard.ShiftBoard(GameBoard.Direction.DIR_DOWN);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				gameBoard.ShiftBoard(GameBoard.Direction.DIR_UP);
			}
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				gameBoard.ShiftBoard(GameBoard.Direction.DIR_LEFT);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				gameBoard.ShiftBoard(GameBoard.Direction.DIR_RIGHT);
			}
		}
	}
}