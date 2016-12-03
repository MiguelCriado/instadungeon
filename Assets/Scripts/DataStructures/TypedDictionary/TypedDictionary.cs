using System.Collections.Generic;

public abstract class TypedDictionary
{

}

public class TypedDictionary<T> : TypedDictionary
{
	private Dictionary<string, T> values;

	public TypedDictionary()
	{
		values = new Dictionary<string, T>();
	}

	public void Set(string key, T value)
	{
		values[key] = value;
	}

	public bool TryGet(string key, out T value)
	{
		return values.TryGetValue(key, out value);
	}
}
