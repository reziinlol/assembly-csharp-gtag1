using System;
using UnityEngine;

// Token: 0x02000DDA RID: 3546
public class ThrowableBugBeacon : MonoBehaviour
{
	// Token: 0x1400009B RID: 155
	// (add) Token: 0x060056CB RID: 22219 RVA: 0x001C27E0 File Offset: 0x001C09E0
	// (remove) Token: 0x060056CC RID: 22220 RVA: 0x001C2814 File Offset: 0x001C0A14
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	// Token: 0x1400009C RID: 156
	// (add) Token: 0x060056CD RID: 22221 RVA: 0x001C2848 File Offset: 0x001C0A48
	// (remove) Token: 0x060056CE RID: 22222 RVA: 0x001C287C File Offset: 0x001C0A7C
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	// Token: 0x1400009D RID: 157
	// (add) Token: 0x060056CF RID: 22223 RVA: 0x001C28B0 File Offset: 0x001C0AB0
	// (remove) Token: 0x060056D0 RID: 22224 RVA: 0x001C28E4 File Offset: 0x001C0AE4
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	// Token: 0x1400009E RID: 158
	// (add) Token: 0x060056D1 RID: 22225 RVA: 0x001C2918 File Offset: 0x001C0B18
	// (remove) Token: 0x060056D2 RID: 22226 RVA: 0x001C294C File Offset: 0x001C0B4C
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	// Token: 0x1400009F RID: 159
	// (add) Token: 0x060056D3 RID: 22227 RVA: 0x001C2980 File Offset: 0x001C0B80
	// (remove) Token: 0x060056D4 RID: 22228 RVA: 0x001C29B4 File Offset: 0x001C0BB4
	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	// Token: 0x17000828 RID: 2088
	// (get) Token: 0x060056D5 RID: 22229 RVA: 0x001C29E7 File Offset: 0x001C0BE7
	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	// Token: 0x17000829 RID: 2089
	// (get) Token: 0x060056D6 RID: 22230 RVA: 0x001C29EF File Offset: 0x001C0BEF
	public float Range
	{
		get
		{
			return this.range;
		}
	}

	// Token: 0x060056D7 RID: 22231 RVA: 0x001C29F7 File Offset: 0x001C0BF7
	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	// Token: 0x060056D8 RID: 22232 RVA: 0x001C2A0B File Offset: 0x001C0C0B
	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	// Token: 0x060056D9 RID: 22233 RVA: 0x001C2A1F File Offset: 0x001C0C1F
	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	// Token: 0x060056DA RID: 22234 RVA: 0x001C2A33 File Offset: 0x001C0C33
	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x060056DB RID: 22235 RVA: 0x001C2A47 File Offset: 0x001C0C47
	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	// Token: 0x060056DC RID: 22236 RVA: 0x001C2A33 File Offset: 0x001C0C33
	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x040066E8 RID: 26344
	[SerializeField]
	private float range;

	// Token: 0x040066E9 RID: 26345
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x02000DDB RID: 3547
	// (Invoke) Token: 0x060056DF RID: 22239
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	// Token: 0x02000DDC RID: 3548
	// (Invoke) Token: 0x060056E3 RID: 22243
	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}
