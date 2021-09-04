using UnityEngine;

namespace G2048 {
	[RequireComponent(typeof(RectTransform))]
	public class SafeArea: MonoBehaviour {
		private Rect curSafeArea = new Rect(0, 0, 0, 0);
		private RectTransform rectTransform;

		private void Awake() {
			rectTransform = transform as RectTransform;
			Rect safeArea = Screen.safeArea;

			if (safeArea != curSafeArea) {
				UpdateTransform(safeArea);
			}
			curSafeArea = new Rect(0, 0, 0, 0);
		}

		private void Update() {
			Rect safeArea = Screen.safeArea;

			if (safeArea != curSafeArea) {
				UpdateTransform(safeArea);
			}
		}

		private void UpdateTransform(Rect safeArea) {
			var anchorMin = safeArea.position;
			var anchorMax = safeArea.position + safeArea.size;
			anchorMin.x /= Screen.width;
			anchorMin.y /= Screen.height;
			anchorMax.x /= Screen.width;
			anchorMax.y /= Screen.height;
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;

			curSafeArea = safeArea;
		}
	}
}