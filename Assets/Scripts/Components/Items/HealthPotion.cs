using System;
using InstaDungeon.Components;
using InstaDungeon.Configuration;
using RSG;
using UnityEngine;

namespace InstaDungeon.Models
{
	public class HealthPotion : Item, IConsumable
	{
		public override ItemInfo ItemInfo { get { return healthPotionInfo; } }
		public HealthPotionInfo HealthPotionInfo { get { return healthPotionInfo; } }

		[SerializeField] private HealthPotionInfo healthPotionInfo;

		public IPromise Consume(Entity entity)
		{
			Promise result = new Promise();
			Health health = entity.GetComponent<Health>();

			if (health != null)
			{
				health.Heal(healthPotionInfo.HealthRestored);
				Locator.Get<ParticleSystemManager>().Spawn("Heal FX", entity.transform.position + Vector3.up * 1 / 3f);
				result.Resolve();
			}
			else
			{
				result.Reject(new Exception(string.Format("Health component not found in entity '{0}'", entity.name)));
			}

			return result;
		}
	}
}
