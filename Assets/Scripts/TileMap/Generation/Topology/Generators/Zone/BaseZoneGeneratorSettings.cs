﻿using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public abstract class BaseZoneGeneratorSettings<T> : IZoneGeneratorSettings<T> where T : IZoneLevelSettings
	{
		[SerializeField] protected List<T> settings;

		private bool sorted;

		public BaseZoneGeneratorSettings(List<T> settings)
		{
			this.settings = settings;
			sorted = false;
			SortSettingsIfNeeded();
		}

		public T GetSettings(int level)
		{
			T result = default(T);
			int i = 0;

			SortSettingsIfNeeded();

			while (result == null && i < settings.Count)
			{
				if (settings[i].MinLevel >= level)
				{
					result = settings[i];
				}

				i++;
			}

			return result;
		}

		private void SortSettingsIfNeeded()
		{
			if (sorted == false)
			{
				settings.Sort((T a, T b) =>
				{
					return a.MinLevel.CompareTo(b.MinLevel);
				});

				sorted = true;
			}
		}
	}
}
