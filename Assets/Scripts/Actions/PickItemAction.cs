using DG.Tweening;
using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.Actions
{
	public class PickItemAction : BaseAction<PickItemCommand>
	{
		private ItemInteractionController interactionVisualizer;

		public PickItemAction(Entity actor, Entity item, ItemInteractionController interactionVisualizer)
		{
			command = new PickItemCommand(actor, item);
			this.interactionVisualizer = interactionVisualizer;
			DOTween.Init();
		}

		public static bool IsValidInteraction(Entity actor, Entity item)
		{
			return actor.GetComponent<Inventory>() != null && item.GetComponent<Item>() != null;
		}

		public override void Act()
		{
			base.Act();

			interactionVisualizer.AnimateReplaceItem()
			.Done(() => 
			{
				DOTween.Sequence()
				.AppendInterval(0.5f)
				.AppendCallback(() => 
				{
					interactionVisualizer.Dispose();
					command.Execute();
					ActionDone();
				});
			});
		}
	}
}
