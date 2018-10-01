using InstaDungeon.Services;
using System;
using Identifier = System.String;

namespace InstaDungeon
{
	public static class Locator
	{
		public static Container Container { get { return container; } }
		private static Container container = new Container();

		public static T Get<T>()
		{
			return container.Get<T>();
		}

		public static T Get<T>(Identifier id)
		{
			return container.Get<T>(id);
		}

		public static void Provide<T>(Func<T> classConstructor)
		{
			container.Provide<T>(classConstructor);
		}

		public static void Provide<T>(Identifier id, Func<T> classConstructor)
		{
			container.Provide<T>(id, classConstructor);
		}

		public static void ProvideFactory<T>(Func<T> classConstructor)
		{
			container.ProvideFactory<T>(classConstructor);
		}

		public static void ProvideFactory<T>(Identifier id, Func<T> classConstructor)
		{
			container.ProvideFactory<T>(id, classConstructor);
		}

		public static bool Contains<T>()
		{
			return container.Contains<T>();
		}

		public static bool Contains<T>(Identifier id)
		{
			return container.Contains<T>(id);
		}

		public static Logger Log
		{
			get { return container.Get<Logger>(); }
		}

		public static AssetBundleService AssetBundle
		{
			get { return container.Get<AssetBundleService>(); }
		}
	}
}
