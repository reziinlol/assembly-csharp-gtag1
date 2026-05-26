using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class PersistLog : MonoBehaviour
{
	// Token: 0x06001C4B RID: 7243 RVA: 0x00099494 File Offset: 0x00097694
	private void OnEnable()
	{
		PersistLog.<OnEnable>d__4 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<PersistLog.<OnEnable>d__4>(ref <OnEnable>d__);
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x000994CB File Offset: 0x000976CB
	private void OnDisable()
	{
		this.OnDestroy();
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x000994D3 File Offset: 0x000976D3
	private void OnDestroy()
	{
		Application.logMessageReceived -= this.LogMessageEnqueue;
		Application.logMessageReceived -= this.LogMessageReceived;
		if (PersistLog.sr == null)
		{
			return;
		}
		PersistLog.sr.Close();
		PersistLog.sr = null;
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x0009950F File Offset: 0x0009770F
	private void LogMessageEnqueue(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.earlyQ.Add(Tuple.Create<string, string>(msg, strace));
		}
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x00099530 File Offset: 0x00097730
	private void LogMessageReceived(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			if (this.plog == msg + strace)
			{
				if (!this.dup)
				{
					PersistLog.sr.Write(string.Format("T+{0} >> Duplicate log entry... Supressing further\n\n", Time.time));
					this.dup = true;
				}
			}
			else
			{
				PersistLog.sr.Write(string.Format("T+{0} >> {1}\n==========================\n{2}\n\n", Time.time, msg, strace));
				this.dup = false;
			}
			PersistLog.sr.Flush();
		}
		this.plog = msg + strace;
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x000995C9 File Offset: 0x000977C9
	public static void Log(string msg)
	{
		PersistLog.Log(LogType.Log, msg);
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x000995D4 File Offset: 0x000977D4
	public static void Log(LogType type, string msg)
	{
		msg = string.Format("T+{0} >[DEV MSG]> {1}\n\n", Time.time, msg);
		Debug.unityLogger.Log(type, msg);
		if (PersistLog.sr == null)
		{
			return;
		}
		PersistLog.sr.Write(msg);
		PersistLog.sr.Flush();
	}

	// Token: 0x04002661 RID: 9825
	private static StreamWriter sr;

	// Token: 0x04002662 RID: 9826
	private string plog;

	// Token: 0x04002663 RID: 9827
	private bool dup;

	// Token: 0x04002664 RID: 9828
	private List<Tuple<string, string>> earlyQ;
}
