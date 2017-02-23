using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Actions
{
	public class MoveEntityAction : BaseAction<MoveEntityCommand>
	{
		protected readonly static float MovementSpeed = 7f;

		protected float targetMovementTime;
		protected float elapsedMovementTime;
		protected Vector3 cachedOrigin;
		protected Vector3 cachedDestiny;
		protected int2 originalPosition;

		public MoveEntityAction(Entity entity, int2 position)
		{
			command = new MoveEntityCommand(entity, position);
		}

		public static bool IsValid(Entity entity, int2 destiny)
		{
			MapManager mapManager = Locator.Get<MapManager>();
			return mapManager.CanCellBeOccupied(destiny);
		}

		public override void Act()
		{
			base.Act();

			if (IsValid(Command.Entity, Command.Position))
			{
				float distance = int2.EuclideanDistance(Command.Entity.CellTransform.Position, Command.Position);
				targetMovementTime = distance / MovementSpeed;
				elapsedMovementTime = 0;

				cachedOrigin = GameManager.Renderer.TileMapToWorldPosition(Command.Entity.CellTransform.Position);
				cachedDestiny = GameManager.Renderer.TileMapToWorldPosition(Command.Position);

				Entity entity = command.Entity;
				originalPosition = entity.CellTransform.Position;
				entity.Events.TriggerEvent(new EntityStartMovementEvent(entity.Guid, originalPosition, command.Position));
			}
		}

		public override void Update(float deltaTime)
		{
			if (IsActing)
			{
				float clampedStepMovementTime = Mathf.Min(targetMovementTime - elapsedMovementTime, deltaTime);
				elapsedMovementTime += clampedStepMovementTime;

				Vector3 targetPosition = Vector3.Lerp(cachedOrigin, cachedDestiny, elapsedMovementTime / targetMovementTime);

				Command.Entity.transform.position = targetPosition;

				if (elapsedMovementTime >= targetMovementTime)
				{
					Locator.Get<MapManager>().MoveTo(Command.Entity, Command.Position);

					Entity entity = command.Entity;
					entity.Events.TriggerEvent(new EntityFinishMovementEvent(entity.Guid, originalPosition, command.Position));

					ActionDone();
				}
			}
		}
	}
}
