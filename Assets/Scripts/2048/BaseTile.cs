using UnityEngine;

namespace G2048 {
	[RequireComponent(typeof(RectTransform))]
	public abstract class BaseTile: MonoBehaviour { //ITile
		public abstract int Index { get; }

		public abstract void InitTile(int index); //2do: spawn direction

		public bool IsNew { get; private set; } = true;
		public void UnmarkNewFlag() => IsNew = false;

	}
}