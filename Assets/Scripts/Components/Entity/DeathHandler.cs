using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity), typeof(Actor), typeof(Health))]
	public class DeathHandler : MonoBehaviour
	{
		private Entity entity;

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		private void Start()
		{
			entity.Events.AddListener(OnEntityDies, EntityDieEvent.EVENT_TYPE);
		}

		private void OnEntityDies(IEventData eventData)
		{
			Locator.Get<ParticleSystemManager>().Spawn("Puff FX", transform.position + Vector3.up * 0.25f);
			Locator.Get<MapManager>().RemoveActor(entity, entity.CellTransform.Position);
			Locator.Get<EntityManager>().Recycle(entity.Guid);
		}
	}
}
