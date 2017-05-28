using InstaDungeon.Commands;

namespace InstaDungeon.Actions
{
	public class PassTurnAction : BaseAction<PassTurnCommand>
	{
		public PassTurnAction()
		{
			command = new PassTurnCommand();
		}

		public override void Act()
		{
			base.Act();
			command.Execute();
			ActionDone();

			// TODO: trigger events
		}
	}
}
