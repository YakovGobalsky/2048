using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	[RequireComponent(typeof(Graphic))]
	public class TristateGraphicElement: MonoBehaviour {
		[SerializeField] private Graphic graphic;

		[SerializeField] private Color colourDisabled = Color.gray;
		[SerializeField] private Color colourNormal = Color.white;
		[SerializeField] private Color colourHighlighted = Color.green;

		private bool isDisabled = false;

		private void Awake() {
			if (!graphic) {
				graphic = GetComponent<Graphic>();
			}
		}

		public bool StateDisabled {
			set {
				isDisabled = value;
				graphic.color = isDisabled ? colourDisabled : colourNormal;
			}
		}

		public bool StateHighlighted {
			set {
				if (!isDisabled) {
					graphic.color = value ? colourHighlighted : colourNormal;
				}
			}
		}
	}
}