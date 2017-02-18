using System.Collections.Generic;

namespace InstaDungeon
{
	public class CommandManager : Manager
	{
		protected List<Command> commandSequence;

		public CommandManager() : base()
		{
			commandSequence = new List<Command>();
		}

		public void RegisterCommand(Command command)
		{
			commandSequence.Add(command);
			command.Execute();
		}

		public Command Undo()
		{
			Command result = null;

			if (commandSequence.Count > 0)
			{
				result = commandSequence[commandSequence.Count - 1];
				result.Undo();

				commandSequence.RemoveAt(commandSequence.Count - 1);
			}

			return result;
		}
	}
}
