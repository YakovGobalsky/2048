using UnityEngine;
using UnityEngine.UI;

// dummy code for androids button "back"/Esc. Invokes button onClick event
namespace G2048 {
	[RequireComponent(typeof(Button))]
	public class BackButtonProcessor: MonoBehaviour {
		private Button button;

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (TryGetComponent<Button>(out var button)) {
					button.onClick?.Invoke();
				}
			}
		}

	}
}