using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Actions
{
	public class MoveEntityAction : BaseAction<MoveEntityCommand>
	{
		protected readonly static float MovementSpeed = 10f;

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
			return mapManager.CanCellBeOccupiedByActor(destiny);
		}

		public override void Act()
		{
			base.Act();

			if (IsValid(Command.Entity, Command.Position))
			{
				int2 origin = Command.Entity.CellTransform.Position;
				int2 destiny = Command.Position;
				float distance = int2.EuclideanDistance(origin, destiny);
				targetMovementTime = distance / MovementSpeed;
				elapsedMovementTime = 0;
				MapManager mapManager = Locator.Get<MapManager>();

				if (mapManager.Map[origin.x, origin.y].Visibility == VisibilityType.Obscured
					&& mapManager.Map[destiny.x, destiny.y].Visibility == VisibilityType.Obscured)
				{
					targetMovementTime = 0;
				}

				GameManager gameManager = Locator.Get<GameManager>();
				cachedOrigin = gameManager.Renderer.TileMapToWorldPosition(origin);
				cachedDestiny = gameManager.Renderer.TileMapToWorldPosition(destiny);

				Entity entity = command.Entity;
				originalPosition = entity.CellTransform.Position;
				entity.Events.TriggerEvent(new EntityStartMovementEvent(entity.Guid, origin, destiny));
			}
		}

		public override void Update(float deltaTime)
		{
			if (IsActing)
			{
				float clampedStepMovementTime = Mathf.Min(targetMovementTime - elapsedMovementTime, deltaTime);
				elapsedMovementTime += clampedStepMovementTime;
				float displacementFraction = targetMovementTime == 0 ? 1 : elapsedMovementTime / targetMovementTime;
				Vector3 targetPosition = Vector3.Lerp(cachedOrigin, cachedDestiny, displacementFraction);
				Command.Entity.transform.position = targetPosition;

				if (elapsedMovementTime >= targetMovementTime)
				{
					Locator.Get<MapManager>().MoveActorTo(Command.Entity, Command.Position);

					Entity entity = command.Entity;
					command.Execute();
					entity.Events.TriggerEvent(new EntityFinishMovementEvent(entity.Guid, originalPosition, command.Position));

					ActionDone();
				}
			}
		}
	}
}
