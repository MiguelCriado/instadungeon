using System;
using System.Text;

namespace InstaDungeon.Services
{
	public enum LogLevel
	{
		Debug,
		Info,
		Notice,
		Warning,
		Error
	}

	public class ConsoleLogger : Logger
	{
		private LogLevel minLogLevel = LogLevel.Debug;
		private StringBuilder stringBuilder;

		public ConsoleLogger(LogLevel minLogLevel)
		{
			this.minLogLevel = minLogLevel;
			stringBuilder = new StringBuilder();
		}

		public override void Debug(string message, params object[] args)
		{
			if (minLogLevel <= LogLevel.Debug)
			{
				UnityEngine.Debug.Log(string.Format(GetCurrentTime() + " " + message, args));
			}
		}

		public override void Info(string message, params object[] args)
		{
			if (minLogLevel <= LogLevel.Info)
			{
				UnityEngine.Debug.Log(string.Format(GetCurrentTime() + " " + message, args));
			}
		}

		public override void Notice(string message, params object[] args)
		{
			if (minLogLevel <= LogLevel.Notice)
			{
				UnityEngine.Debug.Log(string.Format(GetCurrentTime() + " " + message, args));
			}
		}

		public override void Warning(string message, params object[] args)
		{
			if (minLogLevel <= LogLevel.Warning)
			{
				UnityEngine.Debug.LogWarning(string.Format(GetCurrentTime() + " " + message, args));
			}
		}

		public override void Error(string message, params object[] args)
		{
			if (minLogLevel <= LogLevel.Error)
			{
				UnityEngine.Debug.LogError(string.Format(GetCurrentTime() + " " + message, args));
			}
		}

		protected string GetCurrentTime()
		{
			var now = DateTime.Now;
			stringBuilder.Length = 0;

			stringBuilder.Append(now.Hour.ToString().PadLeft(2, '0'));
			stringBuilder.Append(":");
			stringBuilder.Append(now.Minute.ToString().PadLeft(2, '0'));
			stringBuilder.Append(":");
			stringBuilder.Append(now.Second.ToString().PadLeft(2, '0'));

			return stringBuilder.ToString();
		}
	}
}
