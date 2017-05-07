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
		private Blackboard blackboard;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			turn = GetComponent<TurnComponent>();
			blackboard = new Blackboard();
		}

		private void Start()
		{
			brain.CreateTree();
		}

		private void OnEnable()
		{
			TurnManager turnManager = Locator.Get<TurnManager>();
			turnManager.AddActor(turn);
		}

		private void OnDisable()
		{
			TurnManager turnManager = Locator.Get<TurnManager>();
			turnManager.RemoveActor(turn);
		}

		private void Update()
		{
			if (turn.EntityCanAct)
			{
				brain.Think(entity, blackboard);
			}
		}
	}
}
