using InstaDungeon.Components;

namespace InstaDungeon.Commands
{
	public class AttackCommand : Command
	{
		public Entity Attacker { get; private set; }
		public Entity Defender { get; private set; }
		public int DamageDealt { get; private set; }

		public AttackCommand(Entity attacker, Entity defender, int damageDealt)
		{
			Attacker = attacker;
			Defender = defender;
			DamageDealt = damageDealt;
		}

		public override void Execute()
		{
			base.Execute();

			Health defenderHealth = Defender.GetComponent<Health>();
			defenderHealth.Hurt(DamageDealt);
			
			// TODO trigger Hurt event
		}

		public override void Undo()
		{
			base.Undo();

			Health defenderHealth = Defender.GetComponent<Health>();

			if (defenderHealth != null)
			{
				defenderHealth.Heal(DamageDealt);

				// TODO trigger Heal event
			}
		}
	}
}
