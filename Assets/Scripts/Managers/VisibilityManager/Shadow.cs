namespace InstaDungeon
{
	public class Shadow
	{
		public float Start { get; set; }
		public float End { get; set; }

		public Shadow(float start, float end)
		{
			Start = start;
			End = end;
		}

		public bool Contains(Shadow other)
		{
			return Start <= other.Start && End >= other.End;
		}
	}
}
