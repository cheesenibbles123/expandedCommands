using BWModLoader;

namespace expandedCommands
{
	internal static class Log
	{
		public static readonly ModLogger logger = new ModLogger("[expandedCommands]", ModLoader.LogPath + "\\expandedCommands.txt");
		/// <summary>
		/// Log given message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void log(string message)
		{
			logger.Log(message);
		}
		/// <summary>
		/// Log given text using the formatted "Command" Log output
		/// </summary>
		/// <param name="command">Command ran by user</param>
		/// <param name="user">SteamID of the user that ran the command</param>
		public static void logCommand(string command, ulong user)
		{
			logger.Log($"[COMMAND:{command}:{user}");
		}
	}
}
