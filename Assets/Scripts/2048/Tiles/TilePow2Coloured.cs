using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	public class TilePow2Coloured: TilePow2 {
		[SerializeField] private Graphic bgObject;

		[SerializeField] private Color[] bgColours;
		[SerializeField] private Color[] fontColours;

		public override void InitTile(int index, Vector2Int pos, Vector2 scale, float animationScale) {
			base.InitTile(index, pos, scale, animationScale);

			bgObject.color = bgColours[index % bgColours.Length];
			text.color = fontColours[index % fontColours.Length];
		}
	}
}