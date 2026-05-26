using System;
using UnityEngine;

// Token: 0x0200093C RID: 2364
public class OwnerRig : MonoBehaviour, IVariable<VRRig>, IVariable, IRigAware
{
	// Token: 0x06003DFC RID: 15868 RVA: 0x0014F053 File Offset: 0x0014D253
	public void TryFindRig()
	{
		this._rig = base.GetComponentInParent<VRRig>();
		if (this._rig != null)
		{
			return;
		}
		this._rig = base.GetComponentInChildren<VRRig>();
	}

	// Token: 0x06003DFD RID: 15869 RVA: 0x0014F07C File Offset: 0x0014D27C
	public VRRig Get()
	{
		return this._rig;
	}

	// Token: 0x06003DFE RID: 15870 RVA: 0x0014F084 File Offset: 0x0014D284
	public void Set(VRRig value)
	{
		this._rig = value;
	}

	// Token: 0x06003DFF RID: 15871 RVA: 0x0014F08D File Offset: 0x0014D28D
	public void Set(GameObject obj)
	{
		this._rig = ((obj != null) ? obj.GetComponentInParent<VRRig>() : null);
	}

	// Token: 0x06003E00 RID: 15872 RVA: 0x0014F084 File Offset: 0x0014D284
	void IRigAware.SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06003E01 RID: 15873 RVA: 0x0014F0A7 File Offset: 0x0014D2A7
	public static implicit operator bool(OwnerRig or)
	{
		return or != null && !(or == null) && or._rig != null && !(or._rig == null);
	}

	// Token: 0x06003E02 RID: 15874 RVA: 0x0014F0D4 File Offset: 0x0014D2D4
	public static implicit operator VRRig(OwnerRig or)
	{
		if (!or)
		{
			return null;
		}
		return or._rig;
	}

	// Token: 0x04004E3A RID: 20026
	[SerializeField]
	private VRRig _rig;
}
