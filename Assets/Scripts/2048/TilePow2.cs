using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	public class TilePow2: BaseTile {
		[SerializeField] private Text text;

		private int num = 0;
		private int _index;

		public override int Index => _index;

		public override void InitTile(int index) {
			_index = index;

			num = 2 << index;
			text.text = num.ToString();
		}
	}

}