using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000CBB RID: 3259
[Serializable]
public class PhotonSignal<T1, T2, T3, T4, T5> : PhotonSignal
{
	// Token: 0x1700078B RID: 1931
	// (get) Token: 0x060050F7 RID: 20727 RVA: 0x001AB703 File Offset: 0x001A9903
	public override int argCount
	{
		get
		{
			return 5;
		}
	}

	// Token: 0x14000091 RID: 145
	// (add) Token: 0x060050F8 RID: 20728 RVA: 0x001AB706 File Offset: 0x001A9906
	// (remove) Token: 0x060050F9 RID: 20729 RVA: 0x001AB73A File Offset: 0x001A993A
	public new event OnSignalReceived<T1, T2, T3, T4, T5> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060050FA RID: 20730 RVA: 0x001AB10F File Offset: 0x001A930F
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x060050FB RID: 20731 RVA: 0x001AB118 File Offset: 0x001A9318
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x060050FC RID: 20732 RVA: 0x001AB757 File Offset: 0x001A9957
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060050FD RID: 20733 RVA: 0x001AB766 File Offset: 0x001A9966
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060050FE RID: 20734 RVA: 0x001AB77C File Offset: 0x001A997C
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
		array[6] = arg5;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060050FF RID: 20735 RVA: 0x001AB840 File Offset: 0x001A9A40
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		T4 arg4;
		T5 arg5;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3, out arg4, out arg5))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4, T5>(this._callbacks, arg, arg2, arg3, arg4, arg5, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4, T5>(this._callbacks, arg, arg2, arg3, arg4, arg5, info);
	}

	// Token: 0x06005100 RID: 20736 RVA: 0x001AB88E File Offset: 0x001A9A8E
	public new static implicit operator PhotonSignal<T1, T2, T3, T4, T5>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(s);
	}

	// Token: 0x06005101 RID: 20737 RVA: 0x001AB896 File Offset: 0x001A9A96
	public new static explicit operator PhotonSignal<T1, T2, T3, T4, T5>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(i);
	}

	// Token: 0x04006279 RID: 25209
	private OnSignalReceived<T1, T2, T3, T4, T5> _callbacks;

	// Token: 0x0400627A RID: 25210
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4, T5>).FullName.GetStaticHash();
}
