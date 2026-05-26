using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000CB8 RID: 3256
[Serializable]
public class PhotonSignal<T1, T2> : PhotonSignal
{
	// Token: 0x17000788 RID: 1928
	// (get) Token: 0x060050D3 RID: 20691 RVA: 0x0001309F File Offset: 0x0001129F
	public override int argCount
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x1400008E RID: 142
	// (add) Token: 0x060050D4 RID: 20692 RVA: 0x001AB247 File Offset: 0x001A9447
	// (remove) Token: 0x060050D5 RID: 20693 RVA: 0x001AB27B File Offset: 0x001A947B
	public new event OnSignalReceived<T1, T2> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060050D6 RID: 20694 RVA: 0x001AB10F File Offset: 0x001A930F
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x060050D7 RID: 20695 RVA: 0x001AB118 File Offset: 0x001A9318
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x060050D8 RID: 20696 RVA: 0x001AB298 File Offset: 0x001A9498
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060050D9 RID: 20697 RVA: 0x001AB2A7 File Offset: 0x001A94A7
	public void Raise(T1 arg1, T2 arg2)
	{
		this.Raise(this._receivers, arg1, arg2);
	}

	// Token: 0x060050DA RID: 20698 RVA: 0x001AB2B8 File Offset: 0x001A94B8
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060050DB RID: 20699 RVA: 0x001AB360 File Offset: 0x001A9560
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		if (!args.TryParseArgs(2, out arg, out arg2))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2>(this._callbacks, arg, arg2, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2>(this._callbacks, arg, arg2, info);
	}

	// Token: 0x060050DC RID: 20700 RVA: 0x001AB3A0 File Offset: 0x001A95A0
	public new static implicit operator PhotonSignal<T1, T2>(string s)
	{
		return new PhotonSignal<T1, T2>(s);
	}

	// Token: 0x060050DD RID: 20701 RVA: 0x001AB3A8 File Offset: 0x001A95A8
	public new static explicit operator PhotonSignal<T1, T2>(int i)
	{
		return new PhotonSignal<T1, T2>(i);
	}

	// Token: 0x04006273 RID: 25203
	private OnSignalReceived<T1, T2> _callbacks;

	// Token: 0x04006274 RID: 25204
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2>).FullName.GetStaticHash();
}
