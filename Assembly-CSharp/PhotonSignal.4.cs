using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000CB9 RID: 3257
[Serializable]
public class PhotonSignal<T1, T2, T3> : PhotonSignal
{
	// Token: 0x17000789 RID: 1929
	// (get) Token: 0x060050DF RID: 20703 RVA: 0x00135CD9 File Offset: 0x00133ED9
	public override int argCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x1400008F RID: 143
	// (add) Token: 0x060050E0 RID: 20704 RVA: 0x001AB3CB File Offset: 0x001A95CB
	// (remove) Token: 0x060050E1 RID: 20705 RVA: 0x001AB3FF File Offset: 0x001A95FF
	public new event OnSignalReceived<T1, T2, T3> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060050E2 RID: 20706 RVA: 0x001AB10F File Offset: 0x001A930F
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x060050E3 RID: 20707 RVA: 0x001AB118 File Offset: 0x001A9318
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x060050E4 RID: 20708 RVA: 0x001AB41C File Offset: 0x001A961C
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060050E5 RID: 20709 RVA: 0x001AB42B File Offset: 0x001A962B
	public void Raise(T1 arg1, T2 arg2, T3 arg3)
	{
		this.Raise(this._receivers, arg1, arg2, arg3);
	}

	// Token: 0x060050E6 RID: 20710 RVA: 0x001AB43C File Offset: 0x001A963C
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3)
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
		array[3] = arg2;
		array[4] = arg3;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060050E7 RID: 20711 RVA: 0x001AB4EC File Offset: 0x001A96EC
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3>(this._callbacks, arg, arg2, arg3, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3>(this._callbacks, arg, arg2, arg3, info);
	}

	// Token: 0x060050E8 RID: 20712 RVA: 0x001AB530 File Offset: 0x001A9730
	public new static implicit operator PhotonSignal<T1, T2, T3>(string s)
	{
		return new PhotonSignal<T1, T2, T3>(s);
	}

	// Token: 0x060050E9 RID: 20713 RVA: 0x001AB538 File Offset: 0x001A9738
	public new static explicit operator PhotonSignal<T1, T2, T3>(int i)
	{
		return new PhotonSignal<T1, T2, T3>(i);
	}

	// Token: 0x04006275 RID: 25205
	private OnSignalReceived<T1, T2, T3> _callbacks;

	// Token: 0x04006276 RID: 25206
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3>).FullName.GetStaticHash();
}
