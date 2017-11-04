using InstaDungeon.Components;
using RSG;

namespace InstaDungeon
{
	public interface IConsumable
	{
		IPromise Consume(Entity entity);
	}
}
