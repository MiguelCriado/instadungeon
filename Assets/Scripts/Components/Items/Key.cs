﻿using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	public class Key : Item
	{
		public override ItemInfo ItemInfo { get { return keyInfo; } }
		public KeyInfo KeyInfo { get { return keyInfo; } }

		[SerializeField] private KeyInfo keyInfo;

		public void Initialize(string itemName, KeyInfo keyInfo)
		{
			this.itemName = itemName;
			this.keyInfo = keyInfo;
		}
	}
}
