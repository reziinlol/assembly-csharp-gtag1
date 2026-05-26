using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000311 RID: 785
public class DevConsole : MonoBehaviour, IDebugObject
{
	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x060013C8 RID: 5064 RVA: 0x0006B4CA File Offset: 0x000696CA
	public static DevConsole instance
	{
		get
		{
			if (DevConsole._instance == null)
			{
				DevConsole._instance = Object.FindAnyObjectByType<DevConsole>();
			}
			return DevConsole._instance;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060013C9 RID: 5065 RVA: 0x0006B4E8 File Offset: 0x000696E8
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0006B4F4 File Offset: 0x000696F4
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x000440BC File Offset: 0x000422BC
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001860 RID: 6240
	private static DevConsole _instance;

	// Token: 0x04001861 RID: 6241
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04001862 RID: 6242
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001863 RID: 6243
	[SerializeField]
	private float maxHeight;

	// Token: 0x04001864 RID: 6244
	public static readonly string[] tracebackScrubbing = new string[]
	{
		"ExitGames.Client.Photon",
		"Photon.Realtime.LoadBalancingClient",
		"Photon.Pun.PhotonHandler"
	};

	// Token: 0x04001865 RID: 6245
	private const int kLogEntriesCapacityIncrementAmount = 1024;

	// Token: 0x04001866 RID: 6246
	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	// Token: 0x04001867 RID: 6247
	public int targetLogIndex = -1;

	// Token: 0x04001868 RID: 6248
	public int currentLogIndex;

	// Token: 0x04001869 RID: 6249
	public bool isMuted;

	// Token: 0x0400186A RID: 6250
	public float currentZoomLevel = 1f;

	// Token: 0x0400186B RID: 6251
	public List<GameObject> disableWhileActive;

	// Token: 0x0400186C RID: 6252
	public List<GameObject> enableWhileActive;

	// Token: 0x0400186D RID: 6253
	public int expandAmount = 20;

	// Token: 0x0400186E RID: 6254
	public int expandedMessageIndex = -1;

	// Token: 0x0400186F RID: 6255
	public bool canExpand = true;

	// Token: 0x04001870 RID: 6256
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04001871 RID: 6257
	public float lineStartHeight;

	// Token: 0x04001872 RID: 6258
	public float textStartHeight;

	// Token: 0x04001873 RID: 6259
	public float lineStartTextWidth;

	// Token: 0x04001874 RID: 6260
	public double textScale = 0.5;

	// Token: 0x04001875 RID: 6261
	public List<DevConsoleInstance> instances;

	// Token: 0x02000312 RID: 786
	[Serializable]
	public class LogEntry
	{
		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060013CE RID: 5070 RVA: 0x0006B5DE File Offset: 0x000697DE
		public string Message
		{
			get
			{
				if (this.repeatCount > 1)
				{
					return string.Format("({0}) {1}", this.repeatCount, this._Message);
				}
				return this._Message;
			}
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0006B60C File Offset: 0x0006980C
		public LogEntry(string message, LogType type, string trace)
		{
			this._Message = message;
			this.Type = type;
			this.Trace = trace;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = trace.Split("\n".ToCharArray(), StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string line = array[i];
				if (!DevConsole.tracebackScrubbing.Any((string scrubString) => line.Contains(scrubString)))
				{
					stringBuilder.AppendLine(line);
				}
			}
			this.Trace = stringBuilder.ToString();
			DevConsole.LogEntry.TotalIndex++;
			this.index = DevConsole.LogEntry.TotalIndex;
		}

		// Token: 0x04001876 RID: 6262
		private static int TotalIndex;

		// Token: 0x04001877 RID: 6263
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x04001878 RID: 6264
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x04001879 RID: 6265
		public readonly string Trace;

		// Token: 0x0400187A RID: 6266
		public bool forwarded;

		// Token: 0x0400187B RID: 6267
		public int repeatCount = 1;

		// Token: 0x0400187C RID: 6268
		public bool filtered;

		// Token: 0x0400187D RID: 6269
		public int index;
	}

	// Token: 0x02000314 RID: 788
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060013D2 RID: 5074 RVA: 0x0006B6C6 File Offset: 0x000698C6
		// (set) Token: 0x060013D3 RID: 5075 RVA: 0x0006B6CE File Offset: 0x000698CE
		public Type data { get; set; }

		// Token: 0x060013D4 RID: 5076 RVA: 0x0006B6D8 File Offset: 0x000698D8
		public DisplayedLogLine(GameObject obj)
		{
			this.lineText = obj.GetComponentInChildren<Text>();
			this.buttons = obj.GetComponentsInChildren<GorillaDevButton>();
			this.transform = obj.GetComponent<RectTransform>();
			this.backdrop = obj.GetComponentInChildren<SpriteRenderer>();
			foreach (GorillaDevButton gorillaDevButton in this.buttons)
			{
				if (gorillaDevButton.Type == DevButtonType.LineExpand)
				{
					this.maximizeButton = gorillaDevButton;
				}
				if (gorillaDevButton.Type == DevButtonType.LineForward)
				{
					this.forwardButton = gorillaDevButton;
				}
			}
		}

		// Token: 0x0400187F RID: 6271
		public GorillaDevButton[] buttons;

		// Token: 0x04001880 RID: 6272
		public Text lineText;

		// Token: 0x04001881 RID: 6273
		public RectTransform transform;

		// Token: 0x04001882 RID: 6274
		public int targetMessage;

		// Token: 0x04001883 RID: 6275
		public GorillaDevButton maximizeButton;

		// Token: 0x04001884 RID: 6276
		public GorillaDevButton forwardButton;

		// Token: 0x04001885 RID: 6277
		public SpriteRenderer backdrop;

		// Token: 0x04001886 RID: 6278
		private bool expanded;

		// Token: 0x04001887 RID: 6279
		public DevInspector inspector;
	}

	// Token: 0x02000315 RID: 789
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x0006B754 File Offset: 0x00069954
		public static List<DevConsole.MessagePayload> GeneratePayloads(string username, List<DevConsole.LogEntry> entries)
		{
			List<DevConsole.MessagePayload> list = new List<DevConsole.MessagePayload>();
			List<DevConsole.MessagePayload.Block> list2 = new List<DevConsole.MessagePayload.Block>();
			entries.Sort((DevConsole.LogEntry e1, DevConsole.LogEntry e2) => e1.index.CompareTo(e2.index));
			string text = "";
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block("User `" + username + "` Forwarded some errors"));
			foreach (DevConsole.LogEntry logEntry in entries)
			{
				string[] array = logEntry.Trace.Split("\n".ToCharArray());
				string text2 = "";
				foreach (string str in array)
				{
					text2 = text2 + "    " + str + "\n";
				}
				string text3 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text3.Length > 3000)
				{
					text += "```";
					list2.Add(new DevConsole.MessagePayload.Block(text));
					list.Add(new DevConsole.MessagePayload
					{
						blocks = list2.ToArray()
					});
					list2 = new List<DevConsole.MessagePayload.Block>();
					text = "```";
				}
				text += string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
			}
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block(text));
			list.Add(new DevConsole.MessagePayload
			{
				blocks = list2.ToArray()
			});
			return list;
		}

		// Token: 0x04001889 RID: 6281
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x02000316 RID: 790
		[Serializable]
		public class Block
		{
			// Token: 0x060013D7 RID: 5079 RVA: 0x0006B924 File Offset: 0x00069B24
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x0400188A RID: 6282
			public string type;

			// Token: 0x0400188B RID: 6283
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x02000317 RID: 791
		[Serializable]
		public class TextBlock
		{
			// Token: 0x0400188C RID: 6284
			public string type;

			// Token: 0x0400188D RID: 6285
			public string text;
		}
	}
}
