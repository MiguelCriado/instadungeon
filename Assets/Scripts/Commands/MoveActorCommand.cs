public class MoveActorCommand : Command
{
	private CellTransform actorTransform;
	private int2 lastPosition;
	private int2 position;

	public MoveActorCommand(CellTransform actorTransform, int2 position)
	{
		this.actorTransform = actorTransform;
		this.position = position;
	}

	public override void Execute()
	{
		base.Execute();

		lastPosition = actorTransform.Position;
		actorTransform.MoveTo(position);
	}

	public override void Undo()
	{
		base.Undo();

		actorTransform.MoveTo(lastPosition);
	}
}
