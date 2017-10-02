using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Configuration;

namespace InstaDungeon.Actions
{
	public class OpenTrapDoorAction : BaseAction<OpenTrapDoorCommand>
	{
		public OpenTrapDoorAction(Entity actor, Entity trapDoor, KeyInfo requiredKey)
		{
			command = new OpenTrapDoorCommand(actor, trapDoor, requiredKey);
		}

		public override void Act()
		{
			base.Act();

			ItemInteractor itemInteractor = command.TrapDoor.GetComponent<ItemInteractor>();

			if (itemInteractor != null)
			{
				itemInteractor.AddItem(command.RequiredKey)
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
