using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.UI
{
	public class HUDInitializer : MonoBehaviour
	{
		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			Entity player = Locator.Get<GameManager>().Player;

			if (player != null)
			{
				InitializeHealthControllers(player);
				InitializeSlotControllers(player);
			}
		}

		private void InitializeHealthControllers(Entity player)
		{
			HealthControllerHUD[] healthControllers = GetComponentsInChildren<HealthControllerHUD>();

			for (int i = 0; i < healthControllers.Length; i++)
			{
				healthControllers[i].Initialize(player);
			}
		}

		private void InitializeSlotControllers(Entity player)
		{
			InventorySlotController[] slotControllers = GetComponentsInChildren<InventorySlotController>();

			for (int i = 0; i < slotControllers.Length; i++)
			{
				slotControllers[i].Initialize(player);
			}
		}
	}
}
