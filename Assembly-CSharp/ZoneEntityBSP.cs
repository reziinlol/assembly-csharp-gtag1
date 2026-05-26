using System;
using UnityEngine;

// Token: 0x02000E20 RID: 3616
public class ZoneEntityBSP : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x140000A0 RID: 160
	// (add) Token: 0x0600580F RID: 22543 RVA: 0x001C9E0C File Offset: 0x001C800C
	// (remove) Token: 0x06005810 RID: 22544 RVA: 0x001C9E40 File Offset: 0x001C8040
	public static event ZoneEntityBSP.PlayerZoneChange onPlayerZoneChange;

	// Token: 0x1700084F RID: 2127
	// (get) Token: 0x06005811 RID: 22545 RVA: 0x001C9E73 File Offset: 0x001C8073
	public VRRig entityRig
	{
		get
		{
			return this._entityRig;
		}
	}

	// Token: 0x17000850 RID: 2128
	// (get) Token: 0x06005812 RID: 22546 RVA: 0x001C9E7B File Offset: 0x001C807B
	public GTZone currentZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return GTZone.none;
			}
			return zoneDef.zoneId;
		}
	}

	// Token: 0x17000851 RID: 2129
	// (get) Token: 0x06005813 RID: 22547 RVA: 0x001C9E8F File Offset: 0x001C808F
	public GTSubZone currentSubZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return GTSubZone.none;
			}
			return zoneDef.subZoneId;
		}
	}

	// Token: 0x17000852 RID: 2130
	// (get) Token: 0x06005814 RID: 22548 RVA: 0x001C9EA4 File Offset: 0x001C80A4
	public GroupJoinZoneAB GroupZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return default(GroupJoinZoneAB);
			}
			return zoneDef.groupZoneAB;
		}
	}

	// Token: 0x06005815 RID: 22549 RVA: 0x001C9ECA File Offset: 0x001C80CA
	private void Start()
	{
		if (!this._entityRig.isOfflineVRRig)
		{
			this._emitTelemetry = false;
		}
		this.SliceUpdate();
	}

	// Token: 0x06005816 RID: 22550 RVA: 0x0008264A File Offset: 0x0008084A
	public virtual void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x06005817 RID: 22551 RVA: 0x00082653 File Offset: 0x00080853
	public virtual void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x06005818 RID: 22552 RVA: 0x001C9EE8 File Offset: 0x001C80E8
	public void SliceUpdate()
	{
		if (this.isUpdateDisabled)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraphBSP.Instance.FindZoneAtPoint(base.transform.position);
		if (!zoneDef.IsSameZone(this.currentNode))
		{
			this.lastExitedNode = this.currentNode;
			this.currentNode = zoneDef;
			this.lastEnteredNode = zoneDef;
			if (this._entityRig != null)
			{
				bool isOfflineVRRig = this._entityRig.isOfflineVRRig;
			}
			GTZone gtzone = this.lastExitedNode ? this.lastExitedNode.zoneId : GTZone.none;
			GTZone gtzone2 = zoneDef ? zoneDef.zoneId : GTZone.none;
			if (gtzone != gtzone2)
			{
				ZoneEntityBSP.PlayerZoneChange playerZoneChange = ZoneEntityBSP.onPlayerZoneChange;
				if (playerZoneChange != null)
				{
					playerZoneChange(this._entityRig, gtzone, gtzone2);
				}
			}
			if (this._emitTelemetry)
			{
				ZoneDef zoneDef2 = this.lastEnteredNode;
				if (zoneDef2 != null && zoneDef2.trackEnter)
				{
					GorillaTelemetry.EnqueueZoneEvent(this.lastEnteredNode, GTZoneEventType.zone_enter);
				}
				ZoneDef zoneDef3 = this.lastExitedNode;
				if (zoneDef3 != null && zoneDef3.trackExit)
				{
					GorillaTelemetry.EnqueueZoneEvent(this.lastExitedNode, GTZoneEventType.zone_exit);
					return;
				}
			}
		}
		else if (this._emitTelemetry)
		{
			ZoneDef zoneDef4 = this.currentNode;
			if (zoneDef4 != null && zoneDef4.trackStay)
			{
				GorillaTelemetry.EnqueueZoneEvent(this.currentNode, GTZoneEventType.zone_stay);
			}
		}
	}

	// Token: 0x06005819 RID: 22553 RVA: 0x001CA016 File Offset: 0x001C8216
	public void EnableZoneChanges()
	{
		this.isUpdateDisabled = false;
	}

	// Token: 0x0600581A RID: 22554 RVA: 0x001CA01F File Offset: 0x001C821F
	public void DisableZoneChanges()
	{
		this.isUpdateDisabled = true;
	}

	// Token: 0x040068BE RID: 26814
	[Space]
	[SerializeField]
	private bool _emitTelemetry = true;

	// Token: 0x040068BF RID: 26815
	[Space]
	[SerializeField]
	private VRRig _entityRig;

	// Token: 0x040068C0 RID: 26816
	[Space]
	[NonSerialized]
	public ZoneDef currentNode;

	// Token: 0x040068C1 RID: 26817
	[NonSerialized]
	public ZoneDef lastEnteredNode;

	// Token: 0x040068C2 RID: 26818
	[NonSerialized]
	public ZoneDef lastExitedNode;

	// Token: 0x040068C3 RID: 26819
	private bool isUpdateDisabled;

	// Token: 0x02000E21 RID: 3617
	// (Invoke) Token: 0x0600581D RID: 22557
	public delegate void PlayerZoneChange(VRRig rig, GTZone fromZone, GTZone toZone);
}
