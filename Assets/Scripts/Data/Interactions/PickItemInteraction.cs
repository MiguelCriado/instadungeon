using InstaDungeon.Actions;
using InstaDungeon.Components;
using InstaDungeon.Models;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	[CreateAssetMenu(menuName = "InstaDungeon/Interactions/PickItemInteraction", fileName = "new PickItemInteraction", order = 1000)]
	public class PickItemInteraction : Interaction
	{
		private static List<ItemInteractionController> visualizerList;

		[SerializeField] private ItemInteractionController visualizerPrefab;

		public override void EntityEntersRange(Entity activeActor, Entity pasiveActor)
		{
			Inventory inventory = activeActor.GetComponent<Inventory>();
			Item item = pasiveActor.GetComponent<Item>();

			if (inventory != null && item != null)
			{
				Transform worldTransform = GameObject.FindGameObjectWithTag("World").transform;
				Vector3 spawnPosition = Locator.Get<GameManager>().Renderer.SnappedTileMapToWorldPosition(pasiveActor.CellTransform.Position);
				ItemInteractionController visualizer = visualizerPrefab.Spawn(worldTransform, spawnPosition);
				visualizer.Initialize(activeActor, item);
				GetVisualizersList().Add(visualizer);
			}
		}

		public override void EntityExitsRange(Entity activeActor, Entity pasiveActor)
		{
			ItemInteractionController visualizer = GetVisualizersList().Find(x => x.ReplaceItem.GetComponent<Entity>() == pasiveActor);

			if (visualizer != null)
			{
				GetVisualizersList().Remove(visualizer);
				visualizer.Dispose();
			}
		}

		public override IAction Interact(Entity activeActor, Entity pasiveActor)
		{
			PickItemAction result = new PickItemAction(activeActor, pasiveActor);
			return result;
		}

		protected override bool IsValidInteractionInternal(Entity activeActor, Entity pasiveActor)
		{
			return PickItemAction.IsValidInteraction(activeActor, pasiveActor);
		}

		private static List<ItemInteractionController> GetVisualizersList()
		{
			if (visualizerList == null)
			{
				visualizerList = new List<ItemInteractionController>();
			}

			return visualizerList;
		}
	}
}
