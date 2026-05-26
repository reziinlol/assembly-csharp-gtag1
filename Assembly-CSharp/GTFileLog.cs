using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// Token: 0x02000326 RID: 806
public static class GTFileLog
{
	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060013F0 RID: 5104 RVA: 0x0006BAE8 File Offset: 0x00069CE8
	private static GTFileLog.FLogInstance Default
	{
		get
		{
			if (GTFileLog._default != null)
			{
				return GTFileLog._default;
			}
			object registryLock = GTFileLog._registryLock;
			GTFileLog.FLogInstance @default;
			lock (registryLock)
			{
				if (GTFileLog._default == null)
				{
					GTFileLog._default = new GTFileLog.FLogInstance("main");
				}
				@default = GTFileLog._default;
			}
			return @default;
		}
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0006BB4C File Offset: 0x00069D4C
	public static GTFileLog.FLogInstance GetLog(string name)
	{
		object registryLock = GTFileLog._registryLock;
		GTFileLog.FLogInstance result;
		lock (registryLock)
		{
			GTFileLog.FLogInstance flogInstance;
			if (GTFileLog._instances.TryGetValue(name, out flogInstance))
			{
				result = flogInstance;
			}
			else
			{
				GTFileLog.FLogInstance flogInstance2 = new GTFileLog.FLogInstance(name);
				GTFileLog._instances[name] = flogInstance2;
				result = flogInstance2;
			}
		}
		return result;
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0006BBB4 File Offset: 0x00069DB4
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(string msg)
	{
		GTFileLog.Default.WriteEntry("LOG", msg, StackTraceUtility.ExtractStackTrace());
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0006BBCB File Offset: 0x00069DCB
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(string msg)
	{
		GTFileLog.Default.WriteEntry("WARN", msg, StackTraceUtility.ExtractStackTrace());
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x0006BBE2 File Offset: 0x00069DE2
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogError(string msg)
	{
		GTFileLog.Default.WriteEntry("ERR", msg, StackTraceUtility.ExtractStackTrace());
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x0006BBF9 File Offset: 0x00069DF9
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("LOG", msg);
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0006BC0B File Offset: 0x00069E0B
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("WARN", msg);
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0006BC1D File Offset: 0x00069E1D
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("ERR", msg);
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x0006BC30 File Offset: 0x00069E30
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLog(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("LOG", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("LOG", msg);
				}
			}
		}
		Debug.Log("[GT/FLog] " + msg);
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x0006BCF0 File Offset: 0x00069EF0
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLogWarning(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("WARN", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("WARN", msg);
				}
			}
		}
		Debug.LogWarning("[GT/FLog] " + msg);
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x0006BDB0 File Offset: 0x00069FB0
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLogError(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("ERR", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("ERR", msg);
				}
			}
		}
		Debug.LogError("[GT/FLog] " + msg);
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x0006BE70 File Offset: 0x0006A070
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void Reset()
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null)
			{
				GTFileLog._default.Close();
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				flogInstance.Close();
			}
		}
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0006BEFC File Offset: 0x0006A0FC
	private static void OnUnityLogMessage(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (GTFileLog._inCallback)
		{
			return;
		}
		GTFileLog._inCallback = true;
		try
		{
			string level = (type == LogType.Exception) ? "EXCEPTION" : ((type == LogType.Assert) ? "ASSERT" : "UNITY_ERR");
			GTFileLog.Default.WriteEntry(level, condition, stackTrace);
		}
		finally
		{
			GTFileLog._inCallback = false;
		}
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x0006BF68 File Offset: 0x0006A168
	internal static string GetTimestamp()
	{
		if (!(NetworkSystem.Instance != null))
		{
			return Mathf.FloorToInt(Time.realtimeSinceStartup * 1000f).ToString() + "u";
		}
		return NetworkSystem.Instance.ServerTimestamp.ToString();
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0006BFB8 File Offset: 0x0006A1B8
	internal static string ExtractFirstExternalCaller(string stackTrace)
	{
		if (string.IsNullOrEmpty(stackTrace))
		{
			return "(unknown)";
		}
		int num;
		for (int i = 0; i < stackTrace.Length; i = num + 1)
		{
			num = stackTrace.IndexOf('\n', i);
			if (num < 0)
			{
				num = stackTrace.Length;
			}
			int num2 = num - i;
			if (num2 > 0 && stackTrace.IndexOf("GTFileLog", i, Math.Min(num2, 60), StringComparison.Ordinal) < 0)
			{
				return stackTrace.Substring(i, num2).Trim();
			}
		}
		return "(unknown)";
	}

	// Token: 0x040018D4 RID: 6356
	private static readonly object _registryLock = new object();

	// Token: 0x040018D5 RID: 6357
	private static Dictionary<string, GTFileLog.FLogInstance> _instances = new Dictionary<string, GTFileLog.FLogInstance>();

	// Token: 0x040018D6 RID: 6358
	private static GTFileLog.FLogInstance _default;

	// Token: 0x040018D7 RID: 6359
	[ThreadStatic]
	private static bool _inCallback;

	// Token: 0x02000327 RID: 807
	public sealed class FLogInstance
	{
		// Token: 0x06001400 RID: 5120 RVA: 0x0006C047 File Offset: 0x0006A247
		internal FLogInstance(string prefix)
		{
			this._prefix = prefix;
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001401 RID: 5121 RVA: 0x0006C064 File Offset: 0x0006A264
		internal bool IsActive
		{
			get
			{
				object @lock = this._lock;
				bool result;
				lock (@lock)
				{
					result = (this._writer != null);
				}
				return result;
			}
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x0006C0AC File Offset: 0x0006A2AC
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void Log(string msg)
		{
			this.WriteEntry("LOG", msg, StackTraceUtility.ExtractStackTrace());
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x0006C0BF File Offset: 0x0006A2BF
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogWarning(string msg)
		{
			this.WriteEntry("WARN", msg, StackTraceUtility.ExtractStackTrace());
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0006C0D2 File Offset: 0x0006A2D2
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogError(string msg)
		{
			this.WriteEntry("ERR", msg, StackTraceUtility.ExtractStackTrace());
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0006C0E5 File Offset: 0x0006A2E5
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogNoTrace(string msg)
		{
			this.WriteEntryNoTrace("LOG", msg);
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0006C0F3 File Offset: 0x0006A2F3
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogWarningNoTrace(string msg)
		{
			this.WriteEntryNoTrace("WARN", msg);
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0006C101 File Offset: 0x0006A301
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogErrorNoTrace(string msg)
		{
			this.WriteEntryNoTrace("ERR", msg);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0006C10F File Offset: 0x0006A30F
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLog(string msg)
		{
			this.WriteEntryNoTrace("LOG", msg);
			Debug.Log("[GT/FLog:" + this._prefix + "] " + msg);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0006C138 File Offset: 0x0006A338
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLogWarning(string msg)
		{
			this.WriteEntryNoTrace("WARN", msg);
			Debug.LogWarning("[GT/FLog:" + this._prefix + "] " + msg);
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0006C161 File Offset: 0x0006A361
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLogError(string msg)
		{
			this.WriteEntryNoTrace("ERR", msg);
			Debug.LogError("[GT/FLog:" + this._prefix + "] " + msg);
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0006C18C File Offset: 0x0006A38C
		internal void WriteEntryNoTrace(string level, string msg)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			object @lock = this._lock;
			lock (@lock)
			{
				this.EnsureWriter(null);
				if (this._writer != null)
				{
					try
					{
						string timestamp = GTFileLog.GetTimestamp();
						this._writer.WriteLine(string.Concat(new string[]
						{
							"[",
							timestamp,
							"] [",
							level,
							"] ",
							msg
						}));
					}
					catch (Exception ex)
					{
						Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Write failed: " + ex.Message);
						this.CloseWriter();
					}
				}
			}
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0006C258 File Offset: 0x0006A458
		internal void WriteEntry(string level, string msg, string trace)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			object @lock = this._lock;
			lock (@lock)
			{
				this.EnsureWriter(trace);
				if (this._writer != null)
				{
					try
					{
						string timestamp = GTFileLog.GetTimestamp();
						this._writer.WriteLine(string.Concat(new string[]
						{
							"[",
							timestamp,
							"] [",
							level,
							"] ",
							msg,
							"\n- - - -"
						}));
						this._writer.WriteLine(trace);
						this._writer.WriteLine("");
					}
					catch (Exception ex)
					{
						Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Write failed: " + ex.Message);
						this.CloseWriter();
					}
				}
			}
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0006C348 File Offset: 0x0006A548
		private void EnsureWriter(string callerTrace)
		{
			if (this._writer != null || this._failed)
			{
				return;
			}
			if (ApplicationQuittingState.IsQuitting)
			{
				this._failed = true;
				return;
			}
			try
			{
				string persistentDataPath = Application.persistentDataPath;
				Directory.CreateDirectory(persistentDataPath);
				string str = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
				string str2 = "flog_" + this._prefix + "_" + str;
				string text = Path.Combine(persistentDataPath, str2 + ".log");
				for (int i = 1; i <= 10; i++)
				{
					try
					{
						this._writer = new StreamWriter(text, true)
						{
							AutoFlush = true
						};
						break;
					}
					catch (IOException obj) when (i < 10)
					{
						text = Path.Combine(persistentDataPath, str2 + "_" + (i + 1).ToString() + ".log");
					}
				}
				if (this._writer == null)
				{
					throw new IOException("All 10 log file attempts failed due to sharing violations.");
				}
				this._writer.WriteLine(string.Format("--- {0} log started {1:u} ---", this._prefix, DateTime.UtcNow));
				this._writer.WriteLine("--- playerName: " + PlayerPrefs.GetString("playerName", "(unset)") + " ---");
				string text2 = (callerTrace != null) ? GTFileLog.ExtractFirstExternalCaller(callerTrace) : "(no-trace)";
				Debug.Log(string.Concat(new string[]
				{
					"<color=orange><b>[GT/GTFileLog:",
					this._prefix,
					"]</b> Writing to \"",
					text,
					"\". First caller: ",
					text2,
					"</color>"
				}));
				GTFileLog.FLogInstance.PruneOldFlogFiles(persistentDataPath);
			}
			catch (Exception ex)
			{
				this._failed = true;
				Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Failed to create log file: " + ex.Message);
			}
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0006C544 File Offset: 0x0006A744
		private static void PruneOldFlogFiles(string dir)
		{
			try
			{
				string[] files = Directory.GetFiles(dir, "flog_*.log");
				if (files.Length > 10)
				{
					Array.Sort<string>(files, (string a, string b) => File.GetLastWriteTimeUtc(a).CompareTo(File.GetLastWriteTimeUtc(b)));
					int num = files.Length - 10;
					for (int i = 0; i < num; i++)
					{
						try
						{
							File.Delete(files[i]);
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0006C5D0 File Offset: 0x0006A7D0
		private void CloseWriter()
		{
			try
			{
				StreamWriter writer = this._writer;
				if (writer != null)
				{
					writer.Flush();
				}
				StreamWriter writer2 = this._writer;
				if (writer2 != null)
				{
					writer2.Dispose();
				}
			}
			catch
			{
			}
			this._writer = null;
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0006C61C File Offset: 0x0006A81C
		internal void Close()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.CloseWriter();
				this._failed = false;
			}
		}

		// Token: 0x040018D8 RID: 6360
		private StreamWriter _writer;

		// Token: 0x040018D9 RID: 6361
		private bool _failed;

		// Token: 0x040018DA RID: 6362
		private readonly object _lock = new object();

		// Token: 0x040018DB RID: 6363
		private readonly string _prefix;

		// Token: 0x040018DC RID: 6364
		private const string FilePrefix = "flog_";

		// Token: 0x040018DD RID: 6365
		private const int MaxFlogFiles = 10;
	}
}
