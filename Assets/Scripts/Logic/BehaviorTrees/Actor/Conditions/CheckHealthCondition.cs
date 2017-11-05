using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public enum ValueType
	{
		Absolute,
		Percent
	}

	public enum Operation
	{
		LessThan,
		LessThanOrEqualTo,
		EqualTo,
		GreaterThan,
		GreaterThanOrEqualTo
	}

	public class CheckHealthCondition : ConditionNode
	{
		private Operation operation;
		private float value;
		private ValueType valueType;

		public CheckHealthCondition(Operation operation, float value, ValueType valueType)
		{
			this.operation = operation;
			this.value = value;
			this.valueType = valueType;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Entity target = tick.Target as Entity;
			Health health = target.GetComponent<Health>();

			if (health != null)
			{
				float healthValue = GetHealthValue(health);

				if (CheckHealthValue(healthValue))
				{
					result = NodeStates.Success;
				}
			}
			else
			{
				result = NodeStates.Error;
			}

			return result;
		}

		private float GetHealthValue(Health health)
		{
			float result = health.CurrentHealth;

			if (valueType == ValueType.Percent)
			{
				result = (health.CurrentHealth / (float)health.MaxHealth) * 100;
			}

			return result;
		}

		private bool CheckHealthValue(float healthValue)
		{
			bool result = false;

			switch (operation)
			{
				case Operation.LessThan:				result = healthValue < value; break;
				case Operation.LessThanOrEqualTo:		result = healthValue <= value; break;
				case Operation.EqualTo:					result = healthValue == value; break;
				case Operation.GreaterThan:				result = healthValue > value; break;
				case Operation.GreaterThanOrEqualTo:	result = healthValue >= value; break;
			}

			return result;
		}
	}
}
