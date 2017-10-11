using InstaDungeon.Events;
using InstaDungeon.Models;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class Health : MonoBehaviour
	{
		public Entity Entity { get { return entity; } }
		public int MaxHealth { get { return maxHealth; } }
		public int CurrentHealth { get { return currentHealth; } }

		[SerializeField] private int maxHealth;
		[SerializeField] private int currentHealth;

		private Entity entity;
		private Inventory inventory;

		private void Reset()
		{
			maxHealth = 10;
			currentHealth = maxHealth;
		}

		private void OnValidate()
		{
			currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
		}

		private void Awake()
		{
			Initialize();
		}

		public void Initialize()
		{
			entity = GetComponent<Entity>();
			ResetComponent();
		}

		public void ResetComponent()
		{
			int previousHealth = currentHealth;
			currentHealth = maxHealth;
			entity.Events.TriggerEvent(new EntityHealthChangeEvent(entity, this, previousHealth, currentHealth));
		}

		public int SimulateAttack(Weapon weapon)
		{
			int weaponDamage = weapon.GetRandomDamage();
			int totalDamage = weaponDamage - CalculateDefense();
			
			return totalDamage;
		}

		public int Attack(Weapon weapon)
		{
			int totalDamage = SimulateAttack(weapon);
			Hurt(totalDamage);

			return totalDamage;
		}

		public int Hurt(int amount)
		{
			int previousHealth = currentHealth;
			int effectiveDamage = Mathf.Max(0, amount);
			currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
			entity.Events.TriggerEvent(new EntityHealthChangeEvent(entity, this, previousHealth, currentHealth));

			if (currentHealth <= 0)
			{
				entity.Events.TriggerEvent(new EntityDieEvent(entity, this));
			}

			return effectiveDamage;
		}

		public int Heal(int amount)
		{
			int previousHealth = currentHealth;
			int effectiveHeal = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
			currentHealth += effectiveHeal;
			entity.Events.TriggerEvent(new EntityHealthChangeEvent(entity, this, previousHealth, currentHealth));

			return effectiveHeal;
		}

		private int CalculateDefense()
		{
			int result = 0;

			// TODO fetch the inventory for defense equipment

			return result;
		}
	}
}
