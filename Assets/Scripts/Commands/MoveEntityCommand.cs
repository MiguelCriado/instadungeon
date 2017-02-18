using InstaDungeon.Components;

namespace InstaDungeon.Commands
{
	public class MoveEntityCommand : Command
	{
		public Entity Entity { get; private set; }
		public int2 LastPosition { get; private set; }
		public int2 Position { get; private set; }

		public MoveEntityCommand(Entity entity, int2 position)
		{
			Entity = entity;
			Position = position;
		}

		public override void Execute()
		{
			base.Execute();

			LastPosition = Entity.CellTransform.Position;
			Entity.CellTransform.MoveTo(Position);
		}

		public override void Undo()
		{
			base.Undo();

			Entity.CellTransform.MoveTo(LastPosition);
		}
	}
}
