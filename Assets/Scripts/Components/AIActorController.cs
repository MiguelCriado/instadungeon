using AI.BehaviorTrees;
using InstaDungeon.AI;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity), typeof(TurnComponent), typeof(Actor))]
	public class AIActorController : MonoBehaviour
	{
		[SerializeField] private AIBrain brain;

		private Entity entity;
		private TurnComponent turn;
		private Actor actor;
		private Blackboard blackboard;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			turn = GetComponent<TurnComponent>();
			actor = GetComponent<Actor>();
			blackboard = new Blackboard();
		}

		private void Update()
		{
			if (turn.EntityCanAct)
			{
				brain.Think(entity, actor, blackboard);
			}
		}
	}
}
