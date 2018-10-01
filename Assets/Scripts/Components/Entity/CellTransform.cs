using UnityEngine;

namespace InstaDungeon.Components
{
	public class CellTransform : MonoBehaviour
	{
		public int2 Position { get { return position; } }

		[SerializeField] private int2 position;

		private GameManager gameManager;

		private void Awake()
		{
			gameManager = Locator.Get<GameManager>();
		}

		public void MoveTo(int2 position)
		{
			this.position = position;
			transform.position = gameManager.Renderer.SnappedTileMapToWorldPosition(position);
		}
	}
}
