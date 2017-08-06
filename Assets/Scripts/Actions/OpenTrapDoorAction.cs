using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Configuration;

namespace InstaDungeon.Actions
{
	public class OpenTrapDoorAction : BaseAction<OpenTrapDoorCommand>
	{
		public OpenTrapDoorAction(Entity actor, Entity trapDoor, ItemInfo requiredItem)
		{
			command = new OpenTrapDoorCommand(actor, trapDoor, requiredItem);
		}

		public override void Act()
		{
			base.Act();

			ItemInteractor itemInteractor = command.TrapDoor.GetComponent<ItemInteractor>();

			if (itemInteractor != null)
			{
				itemInteractor.AddItem(command.RequiredItem)
				.Done(() =>
				{
					command.Execute();
					ActionDone();
				});
			}
			else
			{
				command.Execute();
				ActionDone();
			}

			// TODO: trigger events
		}
	}
}
