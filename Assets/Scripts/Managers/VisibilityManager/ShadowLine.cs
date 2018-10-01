using System.Collections.Generic;

namespace InstaDungeon
{
	public class ShadowLine
	{
		public bool IsFullShadow { get { return shadows.Count == 1 && shadows[0].Start == 0 && shadows[0].End == 1; } }

		protected List<Shadow> shadows;

		public ShadowLine()
		{
			shadows = new List<Shadow>();
		}

		public bool IsInShadow(Shadow projection)
		{
			for (int i = 0; i < shadows.Count; i++)
			{
				if (shadows[i].Contains(projection))
				{
					return true;
				}
			}

			return false;
		}

		public void Add(Shadow shadow)
		{
			int index = 0;

			for (; index < shadows.Count; index++)
			{
				if (shadows[index].Start >= shadow.Start)
				{
					break;
				}
			}

			Shadow overlapingPrevious = null;

			if (index > 0 && shadows[index - 1].End > shadow.Start)
			{
				overlapingPrevious = shadows[index - 1];
			}

			Shadow overlapingNext = null;

			if (index < shadows.Count && shadows[index].Start < shadow.End)
			{
				overlapingNext = shadows[index];
			}

			if (overlapingNext != null)
			{
				if (overlapingPrevious != null)
				{
					overlapingPrevious.End = overlapingNext.End;
					shadows.RemoveAt(index);
				}
				else
				{
					overlapingNext.Start = shadow.Start;
				}
			}
			else
			{
				if (overlapingPrevious != null)
				{
					overlapingPrevious.End = shadow.End;
				}
				else
				{
					shadows.Insert(index, shadow);
				}
			}
		}
	}
}
