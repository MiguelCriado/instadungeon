using System;
using Identifier = System.String;

namespace InstaDungeon.Services
{
	public static class ContainerExtensions
	{
		private static readonly Identifier DEFAULT_ID = "base";

		public static T Get<T>(this Container container)
		{
			return container.Get<T>(DEFAULT_ID);
		}

		public static void Provide<T>(this Container container, Func<T> classConstructor)
		{
			container.Provide<T>(DEFAULT_ID, classConstructor);
		}

		public static void ProvideFactory<T>(this Container container, Func<T> classConstructor)
		{
			container.ProvideFactory<T>(DEFAULT_ID, classConstructor);
		}

		public static bool Contains<T>(this Container container)
		{
			return container.Contains<T>(DEFAULT_ID);
		}
	}
}
