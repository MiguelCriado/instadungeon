using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public abstract class BaseLayoutGeneratorSettings<T> : ILayoutGeneratorSettings<T> where T : ILayoutLevelSettings
	{
		[SerializeField] protected T fallbackSettings;
		[SerializeField] protected List<T> settings;

		private bool sorted;

		public BaseLayoutGeneratorSettings(T fallbackSettings, List<T> settings)
		{
			this.fallbackSettings = fallbackSettings;
			this.settings = settings;
			sorted = false;
			SortSettingsIfNeeded();
		}

		public T GetSettings(int level)
		{
			T result = fallbackSettings;
			bool found = false;
			int i = 0;

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
