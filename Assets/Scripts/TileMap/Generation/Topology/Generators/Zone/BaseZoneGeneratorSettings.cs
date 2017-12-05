using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public abstract class BaseZoneGeneratorSettings<T> : IZoneGeneratorSettings<T> where T : IZoneLevelSettings
	{
		[SerializeField] protected T fallbackSettings;
		[SerializeField] protected List<T> settings;

		private bool sorted;

		public BaseZoneGeneratorSettings(T fallbackSettings, List<T> settings)
		{
			this.fallbackSettings = fallbackSettings;
			this.settings = settings;
			sorted = false;
			SortSettingsIfNeeded();
		}

		public T GetSettings(int level)
		{
			T result = fallbackSettings;
			int i = 0;
			bool found = false;

			SortSettingsIfNeeded();

			while (found == false && i < settings.Count)
			{
				int nextMinLevel = int.MaxValue;

				if (settings.Count - 1 > i)
				{
					nextMinLevel = settings[i + 1].MinLevel;
				}

				if (level >= settings[i].MinLevel && level < nextMinLevel)
				{
					result = settings[i];
					found = true;
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
