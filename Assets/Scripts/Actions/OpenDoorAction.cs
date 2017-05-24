﻿using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Configuration;

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

			ItemInteractor itemInteraction = command.Door.GetComponent<ItemInteractor>();

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
