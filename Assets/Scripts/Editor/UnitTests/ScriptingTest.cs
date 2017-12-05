using InstaDungeon;
using MoonSharp.Interpreter;
using NUnit.Framework;

public class ScriptingTest
{
	private static readonly string PassFunction = 
		@"
		function pass (value)
			return value
		end";

	public ScriptingTest()
	{
		Locator.Provide(() =>
		{
			return new ScriptingManager();
		});
	}

	[Test]
	public void PassInt2()
	{
		int2 originalPoint = new int2();
		int2 passedPoint = PassValueToScript(originalPoint);
		Assert.IsTrue(originalPoint == passedPoint);
	}

	[Test]
	public void PassValuedInt2()
	{
		int2 originalPoint = new int2(1, 2);
		int2 passedPoint = PassValueToScript(originalPoint);
		Assert.IsTrue(originalPoint == passedPoint);
	}

	[Test]
	public void PassZone()
	{
		Zone originalZone = new Zone();
		Zone passedZone = PassValueToScript(originalZone);
		Assert.IsNotNull(passedZone);
	}

	[Test]
	public void PassIdedZone()
	{
		Zone originalZone = new Zone(14);
		Zone passedZone = PassValueToScript(originalZone);
		Assert.IsTrue(originalZone.id == passedZone.id);
	}

	[Test]
	public void PassBoundedZone()
	{
		Zone originalZone = new Zone();
		originalZone.bounds = new RectangleInt(5, 10, 20, 30);
		Zone passedZone = PassValueToScript(originalZone);
		Assert.IsTrue(originalZone.bounds == passedZone.bounds);
	}

	[Test]
	public void PassTiledZone()
	{
		Zone originalZone = new Zone();
		originalZone.bounds = new RectangleInt(0, 0, 10, 10);
		originalZone.tiles.Add(new int2(0, 0));
		originalZone.tiles.Add(new int2(5, 7));
		originalZone.tiles.Add(new int2(3, 9));
		originalZone.tiles.Add(new int2(6, 2));
		Zone passedZone = PassValueToScript(originalZone);
		Assert.IsTrue(originalZone.tiles.SetEquals(passedZone.tiles));
	}

	[Test]
	public void PassConnectedZone1()
	{
		Zone originalZone = new Zone();
		Zone connectedZone = new Zone();
		originalZone.AddConnectionPoint(new int2(0, 0), connectedZone);
		Zone passedZone = PassValueToScript(originalZone);
		Assert.IsTrue(passedZone.connections[new int2(0, 0)] != null && passedZone.connections[new int2(0, 0)].id == connectedZone.id);
	}

	[Test]
	public void PassConnectedZone2()
	{
		Zone originalZone = new Zone();
		Zone connectedZone = new Zone();
		originalZone.AddConnectionPoint(new int2(0, 0), connectedZone);
		originalZone.AddConnectionPoint(new int2(7, 13), connectedZone);
		Zone passedZone = PassValueToScript(originalZone);
		Assert.IsTrue(passedZone.connections[new int2(0, 0)] != null && passedZone.connections[new int2(0, 0)].id == connectedZone.id);
		Assert.IsTrue(passedZone.connections[new int2(7, 13)] != null && passedZone.connections[new int2(7, 13)].id == connectedZone.id);
	}

	[Test]
	public void PassLayout()
	{
		Layout originalLayout = new Layout();
		Layout passedLayout = PassValueToScript(originalLayout);
		Assert.IsNotNull(passedLayout);
	}

	[Test]
	public void PassLayoutWithTwoZones()
	{
		Layout originalLayout = new Layout();
		Zone zoneA = new Zone();
		Zone zoneB = new Zone();
		originalLayout.Add(zoneA);
		originalLayout.Add(zoneB);
		Layout passedLayout = PassValueToScript(originalLayout);
		Assert.IsTrue(passedLayout.Zones.Find(x => x.id == zoneA.id) != null);
		Assert.IsTrue(passedLayout.Zones.Find(x => x.id == zoneB.id) != null);
	}

	[Test]
	public void PassLayoutWithEndZones()
	{
		Layout originalLayout = new Layout();
		Zone zoneA = new Zone();
		Zone zoneB = new Zone();
		originalLayout.Add(zoneA);
		originalLayout.Add(zoneB);
		originalLayout.InitialZone = zoneA;
		originalLayout.FinalZone = zoneB;
		Layout passedLayout = PassValueToScript(originalLayout);
		Assert.IsTrue(passedLayout.InitialZone.id == zoneA.id);
		Assert.IsTrue(passedLayout.FinalZone.id == zoneB.id);
	}

	[Test]
	public void PassTileMap()
	{
		TileMap<TileType> originalMap = new TileMap<TileType>();
		TileMap<TileType> passedMap = PassValueToScript(originalMap);
		Assert.IsNotNull(passedMap);
	}

	[Test]
	public void PassTileMapWithLayout()
	{
		TileMap<TileType> originalMap = new TileMap<TileType>();
		originalMap.Layout = new Layout();
		TileMap<TileType> passedMap = PassValueToScript(originalMap);
		Assert.IsNotNull(passedMap.Layout);
	}

	[Test]
	public void PassTileMapWithEndPoints()
	{
		TileMap<TileType> originalMap = new TileMap<TileType>();
		originalMap.SpawnPoint = new int2(8, 5);
		originalMap.ExitPoint = new int2(18, 23);
		TileMap<TileType> passedMap = PassValueToScript(originalMap);
		Assert.IsTrue(passedMap.SpawnPoint == new int2(8, 5));
		Assert.IsTrue(passedMap.ExitPoint == new int2(18, 23));
	}

	[Test]
	public void PassTileMapWithTiles()
	{
		TileMap<TileType> originalMap = new TileMap<TileType>();
		Zone zoneA = new Zone();
		zoneA.tiles.Add(new int2(0, 0));
		zoneA.tiles.Add(new int2(9, 7));
		zoneA.tiles.Add(new int2(34, 58));
		originalMap.Layout.Add(zoneA);
		originalMap.Add(new int2(0, 0), TileType.Floor);
		originalMap.Add(new int2(9, 7), TileType.Wall);
		originalMap.Add(new int2(34, 58), TileType.Floor);
		TileMap<TileType> passedMap = PassValueToScript(originalMap);
		Assert.IsTrue(passedMap[0, 0] == TileType.Floor);
		Assert.IsTrue(passedMap[9, 7] == TileType.Wall);
		Assert.IsTrue(passedMap[34, 58] == TileType.Floor);
	}

	#region [Helpers]

	private T PassValueToScript<T>(T value)
	{
		Locator.Get<ScriptingManager>();
		Script script = new Script();
		script.DoString(PassFunction);
		DynValue result = script.Call(script.Globals["pass"], value);
		return result.ToObject<T>();
	}

	#endregion
}