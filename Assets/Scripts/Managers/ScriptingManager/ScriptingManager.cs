using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InstaDungeon
{
	public class ScriptingManager : Manager
	{
		private static readonly string ScriptsPath = Path.Combine(Application.streamingAssetsPath, "Generators");
		private static readonly string LayoutGeneratorsPath = Path.Combine(ScriptsPath, "Layout");
		private static readonly string ZoneGeneratorsPath = Path.Combine(ScriptsPath, "Zone");

		private Dictionary<string, Script> layoutGenerators;
		private Dictionary<string, Script> zoneGenerators;

		public ScriptingManager() : base(true, false)
		{
			layoutGenerators = new Dictionary<string, Script>();
			zoneGenerators = new Dictionary<string, Script>();

			Script.DefaultOptions.ScriptLoader = new ReplInterpreterScriptLoader();
			LoadLayoutGenerationScripts();
			LoadZoneGenerationScripts();
		}

		public Script GetLayoutGenerator(string name)
		{
			Script result = null;
			layoutGenerators.TryGetValue(name, out result);
			return result;
		}

		public Script GetZoneGenerator(string name)
		{
			Script result = null;
			zoneGenerators.TryGetValue(name, out result);
			return result;
		}

		private void LoadLayoutGenerationScripts()
		{
			LoadScriptsIntoDictionary(LayoutGeneratorsPath, layoutGenerators);
		}

		private void LoadZoneGenerationScripts()
		{
			LoadScriptsIntoDictionary(ZoneGeneratorsPath, zoneGenerators);
		}

		private void LoadScriptsIntoDictionary(string path, Dictionary<string, Script> dictionary)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			FileInfo[] files = dirInfo.GetFiles("*.lua");

			for (int i = 0; i < files.Length; i++)
			{
				Script script = new Script();
				script.DoFile(files[i].FullName);
				dictionary.Add(files[i].Name, script);
			}
		}
	}
}
