using UnityEngine;
using UnityEngine.UI;

namespace G2048 {
	[RequireComponent(typeof(Animator))]
	public class TilePow2: BaseTile {
		[SerializeField] protected Text text;

		private int num = 0;
		private Animator animator;

		protected override void Awake() {
			base.Awake();
			animator = GetComponent<Animator>();
		}

		public override void InitTile(int index, Vector2Int pos, Vector2 scale, float animationScale) {
			base.InitTile(index, pos, scale, animationScale);

			animator.SetFloat("timeScale", animationScale);
			num = 2 << index;
			text.text = num.ToString();
		}

		public override void MoveToAndDestroy(Vector2Int targetPos, float moveTime) {
			animator.SetTrigger("death");
			base.MoveToAndDestroy(targetPos, moveTime);
		}
	}

}