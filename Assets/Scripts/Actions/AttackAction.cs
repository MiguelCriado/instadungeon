using DG.Tweening;
using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace InstaDungeon.Actions
{
	public class AttackAction : BaseAction<AttackCommand>
	{
		public AttackAction(Entity attacker, Entity defender)
		{
			int damage = SimulateAttack(attacker, defender);
			Assert.IsTrue(damage != int.MinValue);

			command = new AttackCommand(attacker, defender, damage);
			DOTween.Init();
		}

		public static bool IsValidInteraction(Entity attacker, Entity defender)
		{
			return SimulateAttack(attacker, defender) != int.MinValue;
		}

		public override void Act()
		{
			base.Act();

			Entity attacker = command.Attacker;
			Entity defender = command.Defender;

			Vector3 attackerOriginalPosition = attacker.transform.position;
			Vector3 hitPosition = Vector3.Lerp(attacker.transform.position, defender.transform.position, 0.6f);

			Vector3 defenderDisplacement = (defender.transform.position - attacker.transform.position) * 0.4f;
			Vector3 defenderOriginalPosition = defender.transform.position;

			Sequence attackSequence = DOTween.Sequence();
			attackSequence
			.Append
			(
				attacker.transform.DOMove
				(
					hitPosition,
					0.2f
				)
				.SetEase(Ease.InOutSine)
				.OnComplete
				(
					() =>
					{
						Locator.Get<ParticleSystemManager>().Spawn("Hit FX", defender.transform.position + Vector3.up * 1/3f);

						command.Execute();
					}
				)
			)
			.AppendCallback
			(
				() => 
				{
					if (defender != null)
					{
						defender.transform.DOMove
						(
							defender.transform.position + defenderDisplacement,
							0.2f
						)
						.SetEase(Ease.OutExpo);
					}
				}
			)
			.Join
			(
				attacker.transform.DOMove
				(
					attackerOriginalPosition,
					0.3f
				)
				.SetEase(Ease.OutExpo)
			)
			.InsertCallback
			(
				0.4f,
				() => 
				{
					if (defender != null)
					{
						defender.transform.DOMove
						(
							defenderOriginalPosition,
							0.3f
						)
						.SetEase(Ease.InOutSine);
					}
				}
			)
			.AppendInterval
			(
				0.1f
			)
			.OnComplete
			(
				() => 
				{
					ActionDone();
				}
			);
		}

		private static int SimulateAttack(Entity attacker, Entity defender)
		{
			int result = int.MinValue;

			Inventory attackerInventory = attacker.GetComponent<Inventory>();
			Health defenderHealth = defender.GetComponent<Health>();

			if (attackerInventory != null && defenderHealth != null)
			{
				Item mainHandItem = attackerInventory.GetEquippedItem(InventorySlotType.MainHand);

				if (mainHandItem != null && mainHandItem.GetType() == typeof(Weapon))
				{
					Weapon weapon = mainHandItem as Weapon;

					result = defenderHealth.SimulateAttack(weapon);
				}
			}

			return result;
		}
	}
}
