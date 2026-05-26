using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000CA4 RID: 3236
[Serializable]
public class PhotonEvent : IEquatable<PhotonEvent>
{
	// Token: 0x17000781 RID: 1921
	// (get) Token: 0x06005038 RID: 20536 RVA: 0x001A9FAD File Offset: 0x001A81AD
	// (set) Token: 0x06005039 RID: 20537 RVA: 0x001A9FB5 File Offset: 0x001A81B5
	public bool reliable
	{
		get
		{
			return this._reliable;
		}
		set
		{
			this._reliable = value;
		}
	}

	// Token: 0x17000782 RID: 1922
	// (get) Token: 0x0600503A RID: 20538 RVA: 0x001A9FBE File Offset: 0x001A81BE
	// (set) Token: 0x0600503B RID: 20539 RVA: 0x001A9FC6 File Offset: 0x001A81C6
	public bool failSilent
	{
		get
		{
			return this._failSilent;
		}
		set
		{
			this._failSilent = value;
		}
	}

	// Token: 0x0600503C RID: 20540 RVA: 0x001A9FCF File Offset: 0x001A81CF
	private PhotonEvent()
	{
	}

	// Token: 0x0600503D RID: 20541 RVA: 0x001A9FDE File Offset: 0x001A81DE
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x0600503E RID: 20542 RVA: 0x001AA019 File Offset: 0x001A8219
	public PhotonEvent(string eventId) : this(StaticHash.Compute(eventId))
	{
	}

	// Token: 0x0600503F RID: 20543 RVA: 0x001AA027 File Offset: 0x001A8227
	public PhotonEvent(int eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06005040 RID: 20544 RVA: 0x001AA037 File Offset: 0x001A8237
	public PhotonEvent(string eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback) : this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x06005041 RID: 20545 RVA: 0x001AA048 File Offset: 0x001A8248
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x06005042 RID: 20546 RVA: 0x001AA074 File Offset: 0x001A8274
	public void AddCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (this._delegate != null)
		{
			foreach (Delegate @delegate in this._delegate.GetInvocationList())
			{
				if (@delegate != null && @delegate.Equals(callback))
				{
					return;
				}
			}
		}
		this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Combine(this._delegate, callback);
	}

	// Token: 0x06005043 RID: 20547 RVA: 0x001AA0E2 File Offset: 0x001A82E2
	public void RemoveCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Remove(this._delegate, callback);
		}
	}

	// Token: 0x06005044 RID: 20548 RVA: 0x001AA107 File Offset: 0x001A8307
	public void Enable()
	{
		if (this._disposed)
		{
			return;
		}
		if (this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonEvent.AddPhotonEvent(this);
		}
		this._enabled = true;
	}

	// Token: 0x06005045 RID: 20549 RVA: 0x001AA12F File Offset: 0x001A832F
	public void Disable()
	{
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonEvent.RemovePhotonEvent(this);
		}
		this._enabled = false;
	}

	// Token: 0x06005046 RID: 20550 RVA: 0x001AA157 File Offset: 0x001A8357
	public void Dispose()
	{
		this._delegate = null;
		if (this._enabled)
		{
			this._enabled = false;
			if (Application.isPlaying)
			{
				PhotonEvent.RemovePhotonEvent(this);
			}
		}
		this._eventId = -1;
		this._disposed = true;
	}

	// Token: 0x1400008B RID: 139
	// (add) Token: 0x06005047 RID: 20551 RVA: 0x001AA18C File Offset: 0x001A838C
	// (remove) Token: 0x06005048 RID: 20552 RVA: 0x001AA1C0 File Offset: 0x001A83C0
	public static event Action<EventData, Exception> OnError;

	// Token: 0x06005049 RID: 20553 RVA: 0x001AA1F3 File Offset: 0x001A83F3
	private void InvokeDelegate(int sender, object[] args, PhotonMessageInfoWrapped info)
	{
		Action<int, int, object[], PhotonMessageInfoWrapped> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, this._eventId, args, info);
	}

	// Token: 0x0600504A RID: 20554 RVA: 0x001AA20E File Offset: 0x001A840E
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x0600504B RID: 20555 RVA: 0x001AA218 File Offset: 0x001A8418
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x0600504C RID: 20556 RVA: 0x001AA222 File Offset: 0x001A8422
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x0600504D RID: 20557 RVA: 0x001AA22C File Offset: 0x001A842C
	private void Raise(PhotonEvent.RaiseMode mode, params object[] args)
	{
		if (this._disposed)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (args != null && args.Length > 20)
		{
			Debug.LogError(string.Format("{0}: too many event args, max is {1}, trying to send {2}. Stopping!", "PhotonEvent", 20, args.Length));
			return;
		}
		SendOptions sendOptions = this._reliable ? PhotonEvent.gSendReliable : PhotonEvent.gSendUnreliable;
		switch (mode)
		{
		case PhotonEvent.RaiseMode.Local:
			this.InvokeDelegate(this._eventId, args, new PhotonMessageInfoWrapped(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.ServerTimestamp));
			return;
		case PhotonEvent.RaiseMode.RemoteOthers:
		{
			object[] eventContent = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, eventContent, PhotonEvent.gReceiversOthers, sendOptions);
			return;
		}
		case PhotonEvent.RaiseMode.RemoteAll:
		{
			object[] eventContent2 = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, eventContent2, PhotonEvent.gReceiversAll, sendOptions);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x0600504E RID: 20558 RVA: 0x001AA320 File Offset: 0x001A8520
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x0600504F RID: 20559 RVA: 0x001AA380 File Offset: 0x001A8580
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x06005050 RID: 20560 RVA: 0x001AA3A0 File Offset: 0x001A85A0
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int i = StaticHash.Compute(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Compute(staticHash, i);
	}

	// Token: 0x06005051 RID: 20561 RVA: 0x001AA3DC File Offset: 0x001A85DC
	static PhotonEvent()
	{
		PhotonEvent.gReceiversAll = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		};
		PhotonEvent.gReceiversOthers = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others
		};
		PhotonEvent.gSendUnreliable = SendOptions.SendUnreliable;
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x06005052 RID: 20562 RVA: 0x001AA441 File Offset: 0x001A8641
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void StaticLoadAfterPhotonNetwork()
	{
		PhotonNetwork.NetworkingClient.EventReceived += PhotonEvent.StaticOnEvent;
	}

	// Token: 0x06005053 RID: 20563 RVA: 0x001AA459 File Offset: 0x001A8659
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06005054 RID: 20564 RVA: 0x001AA467 File Offset: 0x001A8667
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06005055 RID: 20565 RVA: 0x001AA478 File Offset: 0x001A8678
	private static void StaticOnEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		try
		{
			object[] array = evData.CustomData as object[];
			if (array != null && array.Length != 0 && array.Length <= 21)
			{
				object obj = array[0];
				if (obj is int)
				{
					int sender = (int)obj;
					if (sender != -1)
					{
						ListProcessor<PhotonEvent> listProcessor;
						if (PhotonEvent._photonEvents.TryGetValue(sender, out listProcessor))
						{
							object[] args;
							if (array.Length > 1)
							{
								args = new object[array.Length - 1];
								Array.Copy(array, 1, args, 0, args.Length);
							}
							else
							{
								args = Array.Empty<object>();
							}
							PhotonMessageInfoWrapped info = new PhotonMessageInfoWrapped(evData.Sender, PhotonNetwork.ServerTimestamp);
							listProcessor.ItemProcessor = delegate(in PhotonEvent pEv)
							{
								if (pEv._eventId == -1 || pEv._disposed || !pEv._enabled)
								{
									return;
								}
								pEv.InvokeDelegate(sender, args, info);
							};
							listProcessor.ProcessList();
						}
					}
				}
			}
		}
		catch (Exception arg)
		{
			Action<EventData, Exception> onError = PhotonEvent.OnError;
			if (onError != null)
			{
				onError(evData, arg);
			}
		}
	}

	// Token: 0x06005056 RID: 20566 RVA: 0x001AA590 File Offset: 0x001A8790
	private static void AddPhotonEvent(PhotonEvent photonEvent)
	{
		int eventId = photonEvent._eventId;
		if (eventId == -1)
		{
			return;
		}
		ListProcessor<PhotonEvent> listProcessor;
		if (!PhotonEvent._photonEvents.TryGetValue(eventId, out listProcessor))
		{
			listProcessor = new ListProcessor<PhotonEvent>(10, null);
			PhotonEvent._photonEvents.Add(eventId, listProcessor);
		}
		if (listProcessor.Contains(photonEvent))
		{
			return;
		}
		listProcessor.Add(photonEvent);
	}

	// Token: 0x06005057 RID: 20567 RVA: 0x001AA5E0 File Offset: 0x001A87E0
	private static void RemovePhotonEvent(PhotonEvent photonEvent)
	{
		ListProcessor<PhotonEvent> listProcessor;
		if (!PhotonEvent._photonEvents.TryGetValue(photonEvent._eventId, out listProcessor))
		{
			return;
		}
		listProcessor.Remove(photonEvent);
		if (listProcessor.Count == 0)
		{
			PhotonEvent._photonEvents.Remove(photonEvent._eventId);
		}
	}

	// Token: 0x06005058 RID: 20568 RVA: 0x001AA624 File Offset: 0x001A8824
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x06005059 RID: 20569 RVA: 0x001AA642 File Offset: 0x001A8842
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x04006247 RID: 25159
	private const int MAX_EVENT_ARGS = 20;

	// Token: 0x04006248 RID: 25160
	private const int INVALID_ID = -1;

	// Token: 0x04006249 RID: 25161
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x0400624A RID: 25162
	[SerializeField]
	private bool _enabled;

	// Token: 0x0400624B RID: 25163
	[SerializeField]
	private bool _reliable;

	// Token: 0x0400624C RID: 25164
	[SerializeField]
	private bool _failSilent;

	// Token: 0x0400624D RID: 25165
	[NonSerialized]
	private bool _disposed;

	// Token: 0x0400624E RID: 25166
	private Action<int, int, object[], PhotonMessageInfoWrapped> _delegate;

	// Token: 0x04006250 RID: 25168
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x04006251 RID: 25169
	private static readonly RaiseEventOptions gReceiversAll;

	// Token: 0x04006252 RID: 25170
	private static readonly RaiseEventOptions gReceiversOthers;

	// Token: 0x04006253 RID: 25171
	private static readonly SendOptions gSendReliable;

	// Token: 0x04006254 RID: 25172
	private static readonly SendOptions gSendUnreliable;

	// Token: 0x04006255 RID: 25173
	private static readonly Dictionary<int, ListProcessor<PhotonEvent>> _photonEvents = new Dictionary<int, ListProcessor<PhotonEvent>>(20);

	// Token: 0x02000CA5 RID: 3237
	public enum RaiseMode
	{
		// Token: 0x04006257 RID: 25175
		Local,
		// Token: 0x04006258 RID: 25176
		RemoteOthers,
		// Token: 0x04006259 RID: 25177
		RemoteAll
	}
}
