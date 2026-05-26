using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000CBA RID: 3258
[Serializable]
public class PhotonSignal<T1, T2, T3, T4> : PhotonSignal
{
	// Token: 0x1700078A RID: 1930
	// (get) Token: 0x060050EB RID: 20715 RVA: 0x001AB55B File Offset: 0x001A975B
	public override int argCount
	{
		get
		{
			return 4;
		}
	}

	// Token: 0x14000090 RID: 144
	// (add) Token: 0x060050EC RID: 20716 RVA: 0x001AB55E File Offset: 0x001A975E
	// (remove) Token: 0x060050ED RID: 20717 RVA: 0x001AB592 File Offset: 0x001A9792
	public new event OnSignalReceived<T1, T2, T3, T4> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060050EE RID: 20718 RVA: 0x001AB10F File Offset: 0x001A930F
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x060050EF RID: 20719 RVA: 0x001AB118 File Offset: 0x001A9318
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x060050F0 RID: 20720 RVA: 0x001AB5AF File Offset: 0x001A97AF
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060050F1 RID: 20721 RVA: 0x001AB5BE File Offset: 0x001A97BE
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060050F2 RID: 20722 RVA: 0x001AB5D4 File Offset: 0x001A97D4
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
		array[5] = arg4;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060050F3 RID: 20723 RVA: 0x001AB690 File Offset: 0x001A9890
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		T4 arg4;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3, out arg4))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4>(this._callbacks, arg, arg2, arg3, arg4, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4>(this._callbacks, arg, arg2, arg3, arg4, info);
	}

	// Token: 0x060050F4 RID: 20724 RVA: 0x001AB6D8 File Offset: 0x001A98D8
	public new static implicit operator PhotonSignal<T1, T2, T3, T4>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4>(s);
	}

	// Token: 0x060050F5 RID: 20725 RVA: 0x001AB6E0 File Offset: 0x001A98E0
	public new static explicit operator PhotonSignal<T1, T2, T3, T4>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4>(i);
	}

	// Token: 0x04006277 RID: 25207
	private OnSignalReceived<T1, T2, T3, T4> _callbacks;

	// Token: 0x04006278 RID: 25208
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4>).FullName.GetStaticHash();
}
