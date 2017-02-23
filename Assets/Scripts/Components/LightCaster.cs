using UnityEngine;

namespace InstaDungeon.Components
{
	public class LightCaster : MonoBehaviour
	{
		public int LightRadius { get { return lightRadius; } set { lightRadius = value; } }

		[SerializeField] protected int lightRadius;
	}
}
