using System;
using System.Collections.Generic;

namespace InstaDungeon.DataStructures
{
	public class Blackboard
	{
		private Dictionary<Type, TypedDictionary> memory;
		private Type type;

		public Blackboard()
		{
			memory = new Dictionary<Type, TypedDictionary>();
		}

		public bool TryGet<T>(string key, out T value)
		{
			type = typeof(T);

			TypedDictionary dictionary;

			if (memory.TryGetValue(type, out dictionary))
			{
				return ((TypedDictionary<T>)dictionary).TryGet(key, out value);
			}
			else
			{
				value = default(T);
				return false;
			}
		}

		public void Set<T>(string key, T value)
		{
			type = typeof(T);

			if (!memory.ContainsKey(type))
			{
				memory.Add(type, new TypedDictionary<T>());
			}

			((TypedDictionary<T>)memory[type]).Set(key, value);
		}
	}
}
