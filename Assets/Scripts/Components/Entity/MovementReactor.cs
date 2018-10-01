using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity), typeof(FaceDirectionComponent))]
	public class MovementReactor : MonoBehaviour
	{
		private Entity entity;
		private FaceDirectionComponent faceDirection;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			faceDirection = GetComponent<FaceDirectionComponent>();

			entity.Events.AddListener(OnEntityStartsMoving, EntityStartMovementEvent.EVENT_TYPE);
		}

		private void OnEntityStartsMoving(IEventData eventData)
		{
			EntityStartMovementEvent entityEvent = eventData as EntityStartMovementEvent;
			faceDirection.Direction = GetDirection(entityEvent.CurrentPosition, entityEvent.NextPosition);
		}

		private Direction GetDirection(int2 currentPosition, int2 nextPosition)
		{
			Direction result = Direction.North;
			int2 movement = nextPosition - currentPosition;

			if (movement == int2.up)
			{
				result = Direction.North;
			}
			else if (movement == int2.right)
			{
				result = Direction.East;
			}
			else if (movement == int2.down)
			{
				result = Direction.South;
			}
			else
			{
				result = Direction.West;
			}

			return result;
		}
	}
}
