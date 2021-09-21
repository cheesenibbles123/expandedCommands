using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;
using BWModLoader;
using System.IO;

namespace expandedCommands
{
	[Mod]
	public class expandedCommands : MonoBehaviour
	{
		public static expandedCommands Instance;
		/// <summary>
		/// WakeNetObject (Identical to the chatbox wno)
		/// </summary>
		WakeNetObject wno;
		/// <summary>
		/// Config file location
		/// </summary>
		static readonly string config = "/config/expandedCommands.cfg";
		/// <summary>
		/// Command prefix
		/// </summary>
		static string prefix = "!";

		void Awake()
        {
			if (!Instance)
            {
				Instance = this;
            }
            else
            {
				DestroyImmediate(this);
            }
        }

		void Start()
        {
			try
			{
				HarmonyInstance harmonyInstance = HarmonyInstance.Create("com.github.archie");
				harmonyInstance.PatchAll();
			}
			catch (Exception e)
			{
				Log.log("##### Error setting up harmony #####");
				Log.log(e.Message);
				Log.log("##### Stack Trace #####");
				Log.log(e.StackTrace);
				Log.log("##### Source #####");
				Log.log(e.Source);
			}

			wno = UI.Instance.chatbox.GetComponent<WakeNetObject>();
			loadSettings();
		}

		/// <summary>
		/// Generate settings file based on current loaded setup
		/// </summary>
		void generateSettings()
		{
			StreamWriter streamWriter = new StreamWriter(config);
			streamWriter.WriteLine("prefix=" + prefix);
			streamWriter.Close();
		}

		/// <summary>
		/// Load settings from the config file
		/// </summary>
		void loadSettings()
		{
			if (!File.Exists(config))
			{
				generateSettings();
			}
			string[] allLines = File.ReadAllLines(config);
			char splitCharacter = '=';
			for (int i = 0; i < allLines.Length; i++)
			{
				if (allLines[i].Contains("="))
				{
					string[] line = allLines[i].Split(splitCharacter);
					if (line.Length == 2)
					{
						switch (line[0])
						{
							case "prefix":
								prefix = line[1];
								break;
							default:
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Send a chat message
		/// </summary>
		/// <param name="msg">Message contents</param>
		/// <param name="target">Message target</param>
		/// <param name="logText">Whether or not to log the passed text</param>
		static void sendChatMessage(string msg, int target, bool logText)
        {
			Instance.wno.òäóæåòîððòä("broadcastChat", target, new object[]
			{
				1,
				1,
				"game",
				msg
			});
			if (logText)
            {
				Log.log(msg);
            }
		}

		/// <summary>
		/// Get a ship by team ID
		/// </summary>
		/// <param name="ID">Team ID</param>
		/// <param name="ship">Given teams shipHealth component</param>
		/// <returns>Boolean ship exists</returns>
		public static bool getShipHealthByID(int ID, out ShipHealth ship)
		{
			if (GameMode.Instance.teamParents[ID])
			{
				ship = GameMode.Instance.teamParents[ID].GetComponent<ShipHealth>();
				return true;
			}
			else
			{
				ship = null;
				return false;
			}
		}

        [HarmonyPatch(typeof(Chat), "sendChat")]
		class ChatPatch
		{
			private bool Prefix(int chatType, int senderTeam, string sender, string text, ïçîìäîóäìïæ.åéðñðçîîïêç info)
			{
				ulong steamID = GameMode.getPlayerBySocket(info.éäñåíéíìééä).steamPlayer.ðñéèéåóëìêé.m_SteamID;
				bool isAdmin = éæñêääóîîèò.ðïîñðçòäêëæ(steamID);

				if (!text.StartsWith(prefix)) return true; // Ignore if no prefix

				text = text.Substring(1); // Remove prefix from text
				string[] args = text.Split(' '); // Split text
				string cmd = args[0]; // Isolate command
				args = args.Skip(1).ToArray(); // Remove command from args

				if (isAdmin)
				{
					if (cmd == "xyz") // Get players X,Y,Z and rotation
					{
						Log.logCommand("xyz", steamID);
						PlayerInfo playerBySocket = GameMode.getPlayerBySocket(info.éäñåíéíìééä);
						sendChatMessage($"X: {playerBySocket.transform.position.x} Y: {playerBySocket.transform.position.y} Z: {playerBySocket.transform.position.z}", info.éäñåíéíìééä, false);
						sendChatMessage($"rX: {playerBySocket.transform.rotation.eulerAngles.x} rY: {playerBySocket.transform.rotation.eulerAngles.y} rZ: {playerBySocket.transform.rotation.eulerAngles.z}", info.éäñåíéíìééä, false);
						return false;
					}
					else if (cmd == "location") // Gets location and rotation of given ship
					{
						Log.logCommand("location", steamID);
						Vector3 pos;
						Vector3 rot;
						if (int.TryParse(args[1], out int num))
						{
							if (getShipHealthByID(num, out ShipHealth ship))
							{
								pos = ship.ññçäîèäíñðó.transform.position;// Distance from each other, height, distance from center
								rot = ship.ññçäîèäíñðó.transform.eulerAngles;
								sendChatMessage($"X: {pos.x} Y: {pos.y} Z: {pos.z}", info.éäñåíéíìééä, false);
								sendChatMessage($"rX: {rot.x} rY: {rot.y} rZ: {rot.z}", info.éäñåíéíìééä, false);
							}
							else
							{
								sendChatMessage($"No ship found with ID: {args[1]}", info.éäñåíéíìééä, false);
							}
						}
						else
						{
							sendChatMessage("Please enter a valid number", info.éäñåíéíìééä, false);
						}
						return false;
					}
					else if (cmd == "tp")
					{
						Log.logCommand("tp", steamID);
						Vector3 newPos = new Vector3(float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
						if (int.TryParse(args[1], out int num))
						{
							if (getShipHealthByID(num, out ShipHealth ship))
							{
								ship.ññçäîèäíñðó.transform.position = newPos;
								sendChatMessage($"Sank ship for team {num}", info.éäñåíéíìééä, true);
							}
							else
							{
								sendChatMessage("Please enter a valid team number", info.éäñåíéíìééä, false);
							}
						}
						else
						{
							sendChatMessage("Please enter a valid number", info.éäñåíéíìééä, false);
						}
						return false;
					}
					else if (cmd == "rot")
					{
						Log.logCommand("rot", steamID);
						string[] txt = text.Split(' ');
						Vector3 newRot = new Vector3(float.Parse(txt[2]), float.Parse(txt[3]), float.Parse(txt[4]));
						if (int.TryParse(args[1], out int num))
						{
							if (getShipHealthByID(num, out ShipHealth ship))
							{
								ship.ññçäîèäíñðó.transform.eulerAngles = newRot;
								sendChatMessage($"Set team {txt[1]} to rot {txt[2]}:{txt[3]}:{txt[4]}", info.éäñåíéíìééä, true);
							}
							else
							{
								sendChatMessage($"Team not found with ID: {txt[1]}", info.éäñåíéíìééä, false);
							}
						}
						else
						{
							sendChatMessage($"Please enter a valid number", info.éäñåíéíìééä, false);
						}
						return false;
					}
					else if (cmd == "sink")
					{
						Log.logCommand("sink", steamID);
						if (int.TryParse(args[1], out int num))
						{
							if (getShipHealthByID(num, out ShipHealth ship))
							{
								ship.ääåìäðêòòåä();
								sendChatMessage($"Sank ship for team {num}", info.éäñåíéíìééä, true);
							}
							else
							{
								sendChatMessage("Please enter a valid team number", info.éäñåíéíìééä, false);
							}
						}
						else
						{
							sendChatMessage("Please enter a valid number", info.éäñåíéíìééä, false);
						}
					}
				}
				return true; // If not found just consider as normal text
			}
		}
	}
}
