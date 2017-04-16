using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Configuration;
using InstaDungeon.Events;

namespace InstaDungeon.Actions
{
	public class OpenDoorAction : BaseAction<OpenDoorCommand>
	{
		public OpenDoorAction(Entity actor, Entity door, ItemInfo requiredKey)
		{
			command = new OpenDoorCommand(actor, door, requiredKey);
		}

		public override void Act()
		{
			base.Act();

			ItemInteraction itemInteraction = command.Door.GetComponent<ItemInteraction>();

			if (itemInteraction != null)
			{
				itemInteraction.AddItem(command.RequiredKey)
					.Done
					(
						() => { ActionDone(); }
					);
			}
			else
			{
				ActionDone();
			}

			// TODO: trigger events
		}
	}
}
