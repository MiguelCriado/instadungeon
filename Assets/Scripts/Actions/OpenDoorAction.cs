using InstaDungeon.Commands;
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

			// TODO: add animations
			// TODO: trigger events

			ActionDone();
		}
	}
}
