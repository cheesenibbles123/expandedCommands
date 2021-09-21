using BWModLoader;

namespace expandedCommands
{
	internal static class Log
	{
		public static readonly ModLogger logger = new ModLogger("[expandedCommands]", ModLoader.LogPath + "\\expandedCommands.txt");
		public static void log(string message)
		{
			logger.Log(message);
		}
		public static void logCommand(string command, ulong user)
		{
			logger.Log($"[COMMAND:{command}:{user}");
		}
	}
}
