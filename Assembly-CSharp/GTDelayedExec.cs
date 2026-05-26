using System;
using UnityEngine;

// Token: 0x02000CDE RID: 3294
public class GTDelayedExec : ITickSystemTick
{
	// Token: 0x1700079F RID: 1951
	// (get) Token: 0x0600519B RID: 20891 RVA: 0x001ADDD5 File Offset: 0x001ABFD5
	// (set) Token: 0x0600519C RID: 20892 RVA: 0x001ADDDC File Offset: 0x001ABFDC
	public static GTDelayedExec instance { get; private set; }

	// Token: 0x170007A0 RID: 1952
	// (get) Token: 0x0600519D RID: 20893 RVA: 0x001ADDE4 File Offset: 0x001ABFE4
	// (set) Token: 0x0600519E RID: 20894 RVA: 0x001ADDEB File Offset: 0x001ABFEB
	public static int listenerCount { get; private set; }

	// Token: 0x0600519F RID: 20895 RVA: 0x001ADDF3 File Offset: 0x001ABFF3
	[OnEnterPlay_Run]
	private static void EdReInit()
	{
		GTDelayedExec._listenerDelays = new float[1024];
		GTDelayedExec._listeners = new GTDelayedExec.Listener[1024];
	}

	// Token: 0x060051A0 RID: 20896 RVA: 0x001ADE13 File Offset: 0x001AC013
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void InitializeAfterAssemblies()
	{
		GTDelayedExec.listenerCount = 0;
		GTDelayedExec.instance = new GTDelayedExec();
		TickSystem<object>.AddTickCallback(GTDelayedExec.instance);
	}

	// Token: 0x060051A1 RID: 20897 RVA: 0x001ADE30 File Offset: 0x001AC030
	internal static void Add(IDelayedExecListener listener, float delay, int contextId)
	{
		if (GTDelayedExec.listenerCount >= GTDelayedExec.maxListenersCount)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ERROR!!!  GTDelayedExec: Recovering from default maximum number of delayed listeners ",
				1024.ToString(),
				" reached. Please set the k_defaultMaxListenersCount value to ",
				(GTDelayedExec.maxListenersCount * 2).ToString(),
				"."
			}));
			GTDelayedExec.maxListenersCount *= 2;
			Array.Resize<float>(ref GTDelayedExec._listenerDelays, GTDelayedExec.maxListenersCount);
			Array.Resize<GTDelayedExec.Listener>(ref GTDelayedExec._listeners, GTDelayedExec.maxListenersCount);
		}
		GTDelayedExec._listenerDelays[GTDelayedExec.listenerCount] = Time.unscaledTime + delay;
		GTDelayedExec._listeners[GTDelayedExec.listenerCount] = new GTDelayedExec.Listener(listener, contextId);
		GTDelayedExec.listenerCount++;
	}

	// Token: 0x170007A1 RID: 1953
	// (get) Token: 0x060051A2 RID: 20898 RVA: 0x001ADEF1 File Offset: 0x001AC0F1
	// (set) Token: 0x060051A3 RID: 20899 RVA: 0x001ADEF9 File Offset: 0x001AC0F9
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060051A4 RID: 20900 RVA: 0x001ADF04 File Offset: 0x001AC104
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < GTDelayedExec.listenerCount; i++)
		{
			if (Time.unscaledTime >= GTDelayedExec._listenerDelays[i])
			{
				try
				{
					GTDelayedExec._listeners[i].listener.OnDelayedAction(GTDelayedExec._listeners[i].contextId);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				GTDelayedExec.listenerCount--;
				GTDelayedExec._listenerDelays[i] = GTDelayedExec._listenerDelays[GTDelayedExec.listenerCount];
				GTDelayedExec._listeners[i] = GTDelayedExec._listeners[GTDelayedExec.listenerCount];
				i--;
			}
		}
	}

	// Token: 0x040062EF RID: 25327
	public const int k_defaultMaxListenersCount = 1024;

	// Token: 0x040062F0 RID: 25328
	public static int maxListenersCount = 1024;

	// Token: 0x040062F2 RID: 25330
	private static float[] _listenerDelays = new float[1024];

	// Token: 0x040062F3 RID: 25331
	private static GTDelayedExec.Listener[] _listeners = new GTDelayedExec.Listener[1024];

	// Token: 0x02000CDF RID: 3295
	private struct Listener
	{
		// Token: 0x060051A7 RID: 20903 RVA: 0x001ADFDA File Offset: 0x001AC1DA
		public Listener(IDelayedExecListener listener, int contextId)
		{
			this.listener = listener;
			this.contextId = contextId;
		}

		// Token: 0x040062F5 RID: 25333
		public readonly IDelayedExecListener listener;

		// Token: 0x040062F6 RID: 25334
		public readonly int contextId;
	}
}
