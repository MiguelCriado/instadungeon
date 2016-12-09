using InstaDungeon.Components;

namespace InstaDungeon.Commands
{
	public class MoveActorCommand : Command
	{
		public CellTransform ActorTransform { get; private set; }
		public int2 LastPosition { get; private set; }
		public int2 Position { get; private set; }

		public MoveActorCommand(CellTransform actorTransform, int2 position)
		{
			ActorTransform = actorTransform;
			Position = position;
		}

		public override void Execute()
		{
			base.Execute();

			LastPosition = ActorTransform.Position;
			ActorTransform.MoveTo(Position);
		}

		public override void Undo()
		{
			base.Undo();

			ActorTransform.MoveTo(LastPosition);
		}
	}
}
