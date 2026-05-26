using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000913 RID: 2323
public class Tappable : MonoBehaviour
{
	// Token: 0x06003CB5 RID: 15541 RVA: 0x0014A796 File Offset: 0x00148996
	public void Validate()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003CB6 RID: 15542 RVA: 0x0014A79F File Offset: 0x0014899F
	protected virtual void OnEnable()
	{
		if (!this.useStaticId)
		{
			this.CalculateId(false);
		}
		TappableManager.Register(this);
	}

	// Token: 0x06003CB7 RID: 15543 RVA: 0x0014A7B6 File Offset: 0x001489B6
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x06003CB8 RID: 15544 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool CanTap(bool isLeftHand)
	{
		return true;
	}

	// Token: 0x06003CB9 RID: 15545 RVA: 0x0014A7BE File Offset: 0x001489BE
	public void OnTap()
	{
		this.OnTap(1f);
	}

	// Token: 0x06003CBA RID: 15546 RVA: 0x0014A7CC File Offset: 0x001489CC
	public void OnTap(float tapStrength)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OnTapLocal(tapStrength, Time.time, default(PhotonMessageInfoWrapped));
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, new object[]
		{
			this.tappableId,
			tapStrength
		});
	}

	// Token: 0x06003CBB RID: 15547 RVA: 0x0014A83C File Offset: 0x00148A3C
	public void OnGrab()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OnGrabLocal(Time.time, default(PhotonMessageInfoWrapped));
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnGrabRPC", RpcTarget.All, new object[]
		{
			this.tappableId
		});
	}

	// Token: 0x06003CBC RID: 15548 RVA: 0x0014A8A4 File Offset: 0x00148AA4
	public void OnRelease()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OnReleaseLocal(Time.time, default(PhotonMessageInfoWrapped));
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnReleaseRPC", RpcTarget.All, new object[]
		{
			this.tappableId
		});
	}

	// Token: 0x06003CBD RID: 15549 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003CBE RID: 15550 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003CBF RID: 15551 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003CC0 RID: 15552 RVA: 0x0014A796 File Offset: 0x00148996
	private void EdRecalculateId()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003CC1 RID: 15553 RVA: 0x0014A90C File Offset: 0x00148B0C
	private void CalculateId(bool force = false)
	{
		Transform transform = base.transform;
		int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
		int staticHash = base.GetType().Name.GetStaticHash();
		int hashCode2 = transform.position.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
		if (this.useStaticId)
		{
			if (string.IsNullOrEmpty(this.staticId) || force)
			{
				int instanceID = transform.GetInstanceID();
				int num2 = StaticHash.Compute(num, instanceID);
				this.staticId = string.Format("#ID_{0:X8}", num2);
			}
			this.tappableId = this.staticId.GetStaticHash();
			return;
		}
		this.tappableId = (Application.isPlaying ? num : 0);
	}

	// Token: 0x06003CC2 RID: 15554 RVA: 0x0014A9D6 File Offset: 0x00148BD6
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		this.CalculateId(false);
	}

	// Token: 0x04004D5F RID: 19807
	public int tappableId;

	// Token: 0x04004D60 RID: 19808
	public string staticId;

	// Token: 0x04004D61 RID: 19809
	public bool useStaticId;

	// Token: 0x04004D62 RID: 19810
	[Tooltip("If true, tap cooldown will be ignored.  Tapping will be allowed/disallowed based on result of CanTap()")]
	public bool overrideTapCooldown;

	// Token: 0x04004D63 RID: 19811
	[Space]
	public TappableManager manager;

	// Token: 0x04004D64 RID: 19812
	public RpcTarget rpcTarget;
}
