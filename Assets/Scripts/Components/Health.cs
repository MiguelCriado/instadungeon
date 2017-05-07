using InstaDungeon.Models;
using UnityEngine;

namespace InstaDungeon.Components
{
	public class Health : MonoBehaviour
	{
		public int MaxHealth { get { return maxHealth; } }
		public int CurrentHealth { get { return currentHealth; } }

		[SerializeField] private int maxHealth;
		[SerializeField] private int currentHealth;

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
			currentHealth = maxHealth;
		}

		public int Attack(Weapon weapon)
		{
			int weaponDamage = weapon.GetRandomDamage();
			int totalDamage = weaponDamage - CalculateDefense();
			int damageDealt = Mathf.Max(0, totalDamage);

			currentHealth = Mathf.Max(0, currentHealth - damageDealt);

			if (currentHealth <= 0)
			{
				// TODO: notify entity death
			}

			return totalDamage;
		}

		public int Heal(int amount)
		{
			int effectiveHeal = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
			currentHealth += effectiveHeal;

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
