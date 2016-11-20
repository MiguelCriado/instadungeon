using System.Collections.Generic;

namespace AI.BehaviorTrees
{
	public class Blackboard
	{
		public static readonly string NODE_MEMORY = "nodeMemory";
		public static readonly string OPEN_NODES = "openNodes";

		private Dictionary<string, object> baseMemory;
		private Dictionary<string, object> treeMemory;

		public Blackboard()
		{
			baseMemory = new Dictionary<string, object>();
			treeMemory = new Dictionary<string, object>();
		}

		public void Set(string key, object value, string treeScope, string nodeScope)
		{
			Dictionary<string, object> memory = GetMemory(treeScope, nodeScope);

			memory[key] = value;
		}

		public void Set(string key, object value)
		{
			Set(key, value, null, null);
		}

		public void Set(string key, object value, string treeScope)
		{
			Set(key, value, treeScope, null);
		}

		public bool TryGet<T>(string key, string treeScope, string nodeScope, out T value)
		{
			bool result = false;

			Dictionary<string, object> memory = GetMemory(treeScope, nodeScope);

			object returnValue;

			if (memory.TryGetValue(key, out returnValue))
			{
				value = (T)returnValue;
				result = true;
			}
			else
			{
				value = default(T);
			}

			return result;
		}

		public bool TryGet<T>(string key, out T value)
		{
			return TryGet(key, null, null, out value);
		}

		public bool TryGet<T>(string key, string treeScope, out T value)
		{
			return TryGet(key, treeScope, null, out value);
		}

		private Dictionary<string, object> GetTreeMemory(string treeScope)
		{
			if (!treeMemory.ContainsKey(treeScope))
			{
				var value = new Dictionary<string, object>();
				value.Add(NODE_MEMORY, new Dictionary<string, object>());
				value.Add(OPEN_NODES, new List<BaseNode>());

				treeMemory.Add(treeScope, value);
			}

			return treeMemory[treeScope] as Dictionary<string, object>;
		}

		private Dictionary<string, object> GetNodeMemory(Dictionary<string, object> treeMemory, string nodeScope)
		{
			var nodeMemory = treeMemory[NODE_MEMORY] as Dictionary<string, object>;

			if (!nodeMemory.ContainsKey(nodeScope))
			{
				nodeMemory.Add(nodeScope, new Dictionary<string, object>());
			}

			return nodeMemory[nodeScope] as Dictionary<string, object>;
		}

		private Dictionary<string, object> GetMemory(string treeScope, string nodeScope)
		{
			var result = baseMemory;

			if (!string.IsNullOrEmpty(treeScope))
			{
				result = GetTreeMemory(treeScope);

				if (!string.IsNullOrEmpty(nodeScope))
				{
					result = GetNodeMemory(result, nodeScope);
				}
			}

			return result;
		}
	}
}
