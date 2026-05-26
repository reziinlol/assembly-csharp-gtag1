using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200065E RID: 1630
public class ConditionalTrigger : MonoBehaviour, IRigAware
{
	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x06002895 RID: 10389 RVA: 0x000DCA79 File Offset: 0x000DAC79
	private int intValue
	{
		get
		{
			return (int)this._tracking;
		}
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000DCA81 File Offset: 0x000DAC81
	public void SetProximityFromRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._from = this._rig.transform;
		}
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x000DCABF File Offset: 0x000DACBF
	public void SetProximityToRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._to = this._rig.transform;
		}
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x000DCAFD File Offset: 0x000DACFD
	public void SetProximityFrom(Transform from)
	{
		this._from = from;
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000DCB06 File Offset: 0x000DAD06
	public void SetProxmityTo(Transform to)
	{
		this._to = to;
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000DCB0F File Offset: 0x000DAD0F
	public void TrackedSet(TriggerCondition conditions)
	{
		this._tracking = conditions;
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000DCB18 File Offset: 0x000DAD18
	public void TrackedAdd(TriggerCondition conditions)
	{
		this._tracking |= conditions;
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x000DCB28 File Offset: 0x000DAD28
	public void TrackedRemove(TriggerCondition conditions)
	{
		this._tracking &= ~conditions;
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x000DCB0F File Offset: 0x000DAD0F
	public void TrackedSet(int conditions)
	{
		this._tracking = (TriggerCondition)conditions;
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x000DCB18 File Offset: 0x000DAD18
	public void TrackedAdd(int conditions)
	{
		this._tracking |= (TriggerCondition)conditions;
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000DCB28 File Offset: 0x000DAD28
	public void TrackedRemove(int conditions)
	{
		this._tracking &= (TriggerCondition)(~(TriggerCondition)conditions);
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000DCB39 File Offset: 0x000DAD39
	public void TrackedClear()
	{
		this._tracking = TriggerCondition.None;
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000DCB42 File Offset: 0x000DAD42
	private void OnEnable()
	{
		this._timeSince = 0f;
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000DCB54 File Offset: 0x000DAD54
	private void Update()
	{
		if (this.IsTracking(TriggerCondition.TimeElapsed))
		{
			this.TrackTimeElapsed();
		}
		if (this.IsTracking(TriggerCondition.Proximity))
		{
			this.TrackProximity();
			return;
		}
		this._distance = 0f;
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000DCB80 File Offset: 0x000DAD80
	private void TrackTimeElapsed()
	{
		if (this._timeSince.HasElapsed(this._interval, true))
		{
			UnityEvent unityEvent = this.onTimeElapsed;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000DCBA8 File Offset: 0x000DADA8
	private void TrackProximity()
	{
		if (!this._from || !this._to)
		{
			this._distance = 0f;
			return;
		}
		this._distance = Vector3.Distance(this._to.position, this._from.position);
		if (this._distance >= this._maxDistance)
		{
			UnityEvent unityEvent = this.onMaxDistance;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000DCC1A File Offset: 0x000DAE1A
	private bool IsTracking(TriggerCondition condition)
	{
		return (this._tracking & condition) == condition;
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000DCC27 File Offset: 0x000DAE27
	private static void FindRig(out VRRig rig)
	{
		if (PhotonNetwork.InRoom)
		{
			rig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.Get(PhotonNetwork.LocalPlayer));
			return;
		}
		rig = VRRig.LocalRig;
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000DCC49 File Offset: 0x000DAE49
	public void SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x04003506 RID: 13574
	[Space]
	[SerializeField]
	private TriggerCondition _tracking;

	// Token: 0x04003507 RID: 13575
	[Space]
	[SerializeField]
	private Transform _from;

	// Token: 0x04003508 RID: 13576
	[SerializeField]
	private Transform _to;

	// Token: 0x04003509 RID: 13577
	[SerializeField]
	private float _maxDistance;

	// Token: 0x0400350A RID: 13578
	[NonSerialized]
	private float _distance;

	// Token: 0x0400350B RID: 13579
	[Space]
	public UnityEvent onMaxDistance;

	// Token: 0x0400350C RID: 13580
	[SerializeField]
	private float _interval = 1f;

	// Token: 0x0400350D RID: 13581
	[NonSerialized]
	private TimeSince _timeSince;

	// Token: 0x0400350E RID: 13582
	[Space]
	public UnityEvent onTimeElapsed;

	// Token: 0x0400350F RID: 13583
	[Space]
	private VRRig _rig;
}
