using InstaDungeon.Configuration;
using System;
using UnityEngine;

namespace InstaDungeon.Models
{
	public abstract class Item : MonoBehaviour
	{
		public string ItemName { get { return itemName; } }
		public abstract ItemInfo ItemInfo { get; }
		public int Amount
		{
			get { return amount; }

			set
			{
				if (ItemInfo.Stackable == false)
				{
					throw new Exception("Item not stackable");
				}
				else if (value < 1)
				{
					throw new ArgumentException("Amount must be > 0");
				}
				else
				{
					amount = value;
				}
			}
		}

		[SerializeField] protected string itemName;
		[SerializeField] protected int amount;

		private void OnValidate()
		{
			if (amount < 1)
			{
				amount = 1;
			}
		}
	}
}
