using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008D6 RID: 2262
public class GTSignalRelay : MonoBehaviourStatic<GTSignalRelay>, IOnEventCallback
{
	// Token: 0x17000545 RID: 1349
	// (get) Token: 0x06003B27 RID: 15143 RVA: 0x00144076 File Offset: 0x00142276
	public static IReadOnlyList<GTSignalListener> ActiveListeners
	{
		get
		{
			return GTSignalRelay.gActiveListeners;
		}
	}

	// Token: 0x06003B28 RID: 15144 RVA: 0x0014407D File Offset: 0x0014227D
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06003B29 RID: 15145 RVA: 0x0014408C File Offset: 0x0014228C
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06003B2A RID: 15146 RVA: 0x0014409C File Offset: 0x0014229C
	public static void Register(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		int num = listener.signal;
		if (num == 0)
		{
			return;
		}
		if (!GTSignalRelay.gListenerSet.Add(listener))
		{
			return;
		}
		GTSignalRelay.gActiveListeners.Add(listener);
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, out list))
		{
			list = new List<GTSignalListener>(64);
			GTSignalRelay.gSignalIdToListeners.Add(num, list);
		}
		list.Add(listener);
	}

	// Token: 0x06003B2B RID: 15147 RVA: 0x00144108 File Offset: 0x00142308
	public static void Unregister(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		GTSignalRelay.gListenerSet.Remove(listener);
		GTSignalRelay.gActiveListeners.Remove(listener);
		List<GTSignalListener> list;
		if (GTSignalRelay.gSignalIdToListeners.TryGetValue(listener.signal, out list))
		{
			list.Remove(listener);
		}
	}

	// Token: 0x06003B2C RID: 15148 RVA: 0x00144158 File Offset: 0x00142358
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		Object.DontDestroyOnLoad(new GameObject("GTSignalRelay").AddComponent<GTSignalRelay>());
	}

	// Token: 0x06003B2D RID: 15149 RVA: 0x00144170 File Offset: 0x00142370
	void IOnEventCallback.OnEvent(EventData eventData)
	{
		if (eventData.Code == 186)
		{
			object[] array = eventData.CustomData as object[];
			if (array != null)
			{
				int key = (int)array[0];
				List<GTSignalListener> list;
				if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(key, out list))
				{
					return;
				}
				int sender = eventData.Sender;
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						GTSignalListener gtsignalListener = list[i];
						if (!gtsignalListener.deafen)
						{
							if (gtsignalListener.IsReady())
							{
								if (!gtsignalListener.ignoreSelf || sender != gtsignalListener.rigActorID)
								{
									if (!gtsignalListener.listenToSelfOnly || sender == gtsignalListener.rigActorID)
									{
										gtsignalListener.HandleSignalReceived(sender, array);
										if (gtsignalListener.callUnityEvent)
										{
											UnityEvent onSignalReceived = gtsignalListener.onSignalReceived;
											if (onSignalReceived != null)
											{
												onSignalReceived.Invoke();
											}
										}
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
				return;
			}
		}
	}

	// Token: 0x04004B7E RID: 19326
	private static List<GTSignalListener> gActiveListeners = new List<GTSignalListener>(128);

	// Token: 0x04004B7F RID: 19327
	private static HashSet<GTSignalListener> gListenerSet = new HashSet<GTSignalListener>(128);

	// Token: 0x04004B80 RID: 19328
	private static Dictionary<int, List<GTSignalListener>> gSignalIdToListeners = new Dictionary<int, List<GTSignalListener>>(128);
}
