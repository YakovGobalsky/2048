using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace G2048 {
	public class InputTouch: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
		[SerializeField] private Game2048 game;

		[Header("Joystick")]
		[SerializeField] private float maxDist;
		[SerializeField] private RectTransform screenJoystick;
		[SerializeField] private RectTransform knob;

		[Header("Arrows")]
		[SerializeField] private TristateGraphicElement arrowLeft;
		[SerializeField] private TristateGraphicElement arrowRight;
		[SerializeField] private TristateGraphicElement arrowUp;
		[SerializeField] private TristateGraphicElement arrowDown;

		private const float effectiveNormalizedDist = 0.5f;

		private Direction? inputDirection = null;

		private void OnEnable() {
			Game2048.onAllowedDirectionsChanged += OnAllowedDirectionsChanged;
		}

		private void OnDisable() {
			Game2048.onAllowedDirectionsChanged -= OnAllowedDirectionsChanged;
		}

		private void OnAllowedDirectionsChanged(DirectionsSet directions) {
			arrowLeft.StateDisabled = !directions.Contains(Direction.DIR_LEFT);
			arrowRight.StateDisabled = !directions.Contains(Direction.DIR_RIGHT);
			arrowDown.StateDisabled = !directions.Contains(Direction.DIR_DOWN);
			arrowUp.StateDisabled = !directions.Contains(Direction.DIR_UP);
		}

		public virtual void OnDrag(PointerEventData eventData) {
			UpdateKnobPos(eventData.position);
		}

		public virtual void OnPointerDown(PointerEventData eventData) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(screenJoystick.parent as RectTransform, eventData.position, null, out Vector2 localPos);
			screenJoystick.transform.localPosition = localPos;
			screenJoystick.gameObject.SetActive(true);

			inputDirection = null;

			UpdateKnobPos(eventData.position);
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			if (inputDirection != null) {
				game.ShiftBoard(inputDirection.Value);
			}
			screenJoystick.gameObject.SetActive(false);
		}

		protected virtual void UpdateKnobPos(Vector3 pos) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(screenJoystick, pos, null, out Vector2 dir);
			float dirLength = dir.magnitude;
			float dist = Mathf.Min(dirLength, maxDist);
			knob.transform.localPosition = dir.normalized * dist;

			inputDirection = GetDirection(dir, dist / maxDist >= effectiveNormalizedDist);
			arrowLeft.StateHighlighted	= inputDirection == Direction.DIR_LEFT;
			arrowRight.StateHighlighted	= inputDirection == Direction.DIR_RIGHT;
			arrowDown.StateHighlighted	= inputDirection == Direction.DIR_DOWN;
			arrowUp.StateHighlighted		= inputDirection == Direction.DIR_UP;
		}

		private Direction? GetDirection(Vector2 dir, bool isEffectiveDist) {
			if (!isEffectiveDist) {
				return null;
			} else if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
				return dir.x < 0 ? Direction.DIR_LEFT : Direction.DIR_RIGHT;
			} else {
				return dir.y < 0 ? Direction.DIR_DOWN : Direction.DIR_UP;
			}
		}

	}
}