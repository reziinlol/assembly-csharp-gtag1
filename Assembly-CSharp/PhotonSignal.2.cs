using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000CB7 RID: 3255
[Serializable]
public class PhotonSignal<T1> : PhotonSignal
{
	// Token: 0x17000787 RID: 1927
	// (get) Token: 0x060050C7 RID: 20679 RVA: 0x00023994 File Offset: 0x00021B94
	public override int argCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x1400008D RID: 141
	// (add) Token: 0x060050C8 RID: 20680 RVA: 0x001AB0BE File Offset: 0x001A92BE
	// (remove) Token: 0x060050C9 RID: 20681 RVA: 0x001AB0F2 File Offset: 0x001A92F2
	public new event OnSignalReceived<T1> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060050CA RID: 20682 RVA: 0x001AB10F File Offset: 0x001A930F
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x060050CB RID: 20683 RVA: 0x001AB118 File Offset: 0x001A9318
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x060050CC RID: 20684 RVA: 0x001AB121 File Offset: 0x001A9321
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060050CD RID: 20685 RVA: 0x001AB130 File Offset: 0x001A9330
	public void Raise(T1 arg1)
	{
		this.Raise(this._receivers, arg1);
	}

	// Token: 0x060050CE RID: 20686 RVA: 0x001AB140 File Offset: 0x001A9340
	public void Raise(ReceiverGroup receivers, T1 arg1)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		RaiseEventOptions raiseEventOptions = PhotonSignal.gGroupToOptions[receivers];
		object[] array = PhotonUtils.FetchScratchArray(2 + this.argCount);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		array[2] = arg1;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060050CF RID: 20687 RVA: 0x001AB1E0 File Offset: 0x001A93E0
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		if (!args.TryParseArgs(2, out arg))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1>(this._callbacks, arg, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1>(this._callbacks, arg, info);
	}

	// Token: 0x060050D0 RID: 20688 RVA: 0x001AB21C File Offset: 0x001A941C
	public new static implicit operator PhotonSignal<T1>(string s)
	{
		return new PhotonSignal<T1>(s);
	}

	// Token: 0x060050D1 RID: 20689 RVA: 0x001AB224 File Offset: 0x001A9424
	public new static explicit operator PhotonSignal<T1>(int i)
	{
		return new PhotonSignal<T1>(i);
	}

	// Token: 0x04006271 RID: 25201
	private OnSignalReceived<T1> _callbacks;

	// Token: 0x04006272 RID: 25202
	private static readonly int kSignature = typeof(PhotonSignal<T1>).FullName.GetStaticHash();
}
