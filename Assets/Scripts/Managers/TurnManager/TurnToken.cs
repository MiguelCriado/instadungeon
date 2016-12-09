using InstaDungeon.Components;

namespace InstaDungeon
{
	public class TurnToken
	{
		public int Round { get; set; }
		public int Turn { get; set; }
		public TurnComponent Target { get; set; }

		public TurnToken()
		{
			Round = 0;
			Turn = 0;
		}
	}
}
