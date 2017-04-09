using InstaDungeon;
using InstaDungeon.Components;
using System.Collections.Generic;

public class Cell
{
	public TileInfo TileInfo { get; set; }
	public Entity Actor { get { return actor; } set { actor = value; } }
	public Entity Prop { get { return prop; } set { prop = value; } }
	public List<Entity> Items { get { return items; } }
	public VisibilityType Visibility { get { return visibility; } }

	private Entity actor;
	private Entity prop;
	private List<Entity> items;
	private VisibilityType visibility;

	public Cell(TileInfo tileInfo)
	{
		TileInfo = tileInfo;
		items = new List<Entity>();
		visibility = VisibilityType.Obscured;
	}

	public void AddItem(Entity item)
	{
		items.Add(item);
	}

	public bool RemoveItem(Entity item)
	{
		return items.Remove(item);
	}

	public void RefreshVisibility(bool visible)
	{
		if (visible == true)
		{
			visibility = VisibilityType.Visible;
		}
		else
		{
			if (visibility == VisibilityType.Visible)
			{
				visibility = VisibilityType.PreviouslySeen;
			}
		}
	}

	public bool BreaksLineOfSight()
	{
		bool result = false;

		result |= 
			(
				TileInfo.BreaksLineOfSight
				|| (actor != null && actor.BlocksLineOfSight)
				|| (prop != null && prop.BlocksLineOfSight)
			);

		if (result == false)
		{
			int i = 0; 

			while (result == false && i < items.Count)
			{
				result |= items[i].BlocksLineOfSight;
				i++;
			}
		}

		return result;
	}

	public bool IsWalkable()
	{
		bool blocked = false;

		blocked |=
			(
				!TileInfo.Walkable
				|| (actor != null && actor.BlocksMovement)
				|| (prop != null && prop.BlocksMovement)
			);

		if (blocked == false)
		{
			int i = 0;

			while (blocked == false && i < items.Count)
			{
				blocked |= items[i].BlocksMovement;
				i++;
			}
		}

		return !blocked;
	}
}
