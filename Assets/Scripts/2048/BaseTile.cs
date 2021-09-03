using System.Collections;
using UnityEngine;

namespace G2048 {
	[RequireComponent(typeof(RectTransform))]
	public abstract class BaseTile: MonoBehaviour { //ITile
		public int TileIndex { get; private set; }
		public bool IsNew { get; private set; } = true;

		public abstract int Scores {get; }

		private Vector2 scaleMultiplier;

		protected RectTransform rectTransform;

		protected virtual void Awake() {
			rectTransform = transform as RectTransform;
		}

		public virtual void InitTile(int index, Vector2Int pos, Vector2 cellScale, float animationScale) {
			TileIndex = index;
			scaleMultiplier = cellScale;

			rectTransform.anchorMin = Vector2.Scale(pos, scaleMultiplier);
			rectTransform.anchorMax = Vector2.Scale(pos + Vector2Int.one, scaleMultiplier);
		}

		public virtual void UnmarkNewFlag() => IsNew = false;

		public virtual void MoveTo(Vector2Int targetPos, float moveTime) {
			var targetAnchorMin = Vector2.Scale(targetPos, scaleMultiplier);
			var targetAnchorMax = Vector2.Scale(targetPos + Vector2Int.one, scaleMultiplier);
			StopAllCoroutines();
			StartCoroutine(MoveAnchorsRoutine(targetAnchorMin, targetAnchorMax, moveTime));
		}

		public virtual void MoveToAndDestroy(Vector2Int targetPos, float moveTime) {
			var targetAnchorMin = Vector2.Scale(targetPos, scaleMultiplier);
			var targetAnchorMax = Vector2.Scale(targetPos + Vector2Int.one, scaleMultiplier);
			StopAllCoroutines();
			StartCoroutine(MoveToAndDestroyRoutine(targetAnchorMin, targetAnchorMax, moveTime));
		}

		private IEnumerator MoveAnchorsRoutine(Vector2 targetAnchorMin, Vector2 targetAnchorMax, float moveTime) {
			var sourceAnchorMin = rectTransform.anchorMin;
			var sourceAnchorMax = rectTransform.anchorMax;
			float t = 0f;
			do {
				t += Time.deltaTime / moveTime;
				rectTransform.anchorMin = Vector2.Lerp(sourceAnchorMin, targetAnchorMin, t);
				rectTransform.anchorMax = Vector2.Lerp(sourceAnchorMax, targetAnchorMax, t);
				yield return null;
			} while (t <= 1f);
		}

		private IEnumerator MoveToAndDestroyRoutine(Vector2 targetAnchorMin, Vector2 targetAnchorMax, float moveTime) {
			yield return StartCoroutine(MoveAnchorsRoutine(targetAnchorMin, targetAnchorMax, moveTime));
			Destroy(gameObject);
		}

	}
}