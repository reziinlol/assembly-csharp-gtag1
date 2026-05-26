using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002F8 RID: 760
public class StickyHand : MonoBehaviour, ISpawnable
{
	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001359 RID: 4953 RVA: 0x000663A3 File Offset: 0x000645A3
	// (set) Token: 0x0600135A RID: 4954 RVA: 0x000663AB File Offset: 0x000645AB
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x0600135B RID: 4955 RVA: 0x000663B4 File Offset: 0x000645B4
	// (set) Token: 0x0600135C RID: 4956 RVA: 0x000663BC File Offset: 0x000645BC
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600135D RID: 4957 RVA: 0x000663C8 File Offset: 0x000645C8
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.isLocal = rig.isLocal;
		this.flatHand.enabled = false;
		this.defaultLocalPosition = this.stringParent.transform.InverseTransformPoint(this.rb.transform.position);
		int num = (this.CosmeticSelectedSide == ECosmeticSelectSide.Left) ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x00066440 File Offset: 0x00064640
	private void Update()
	{
		if (this.isLocal)
		{
			if (this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringDetachLength))
			{
				this.Unstick();
			}
			else if (!this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringTeleportLength))
			{
				this.rb.transform.position = this.stringParent.transform.TransformPoint(this.defaultLocalPosition);
			}
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.rb.isKinematic);
			return;
		}
		if (GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex) != this.rb.isKinematic)
		{
			if (this.rb.isKinematic)
			{
				this.Unstick();
				return;
			}
			this.Stick();
		}
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x0006656E File Offset: 0x0006476E
	private void Stick()
	{
		this.thwackSound.Play();
		this.flatHand.enabled = true;
		this.regularHand.enabled = false;
		this.rb.isKinematic = true;
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0006659F File Offset: 0x0006479F
	private void Unstick()
	{
		this.schlupSound.Play();
		this.rb.isKinematic = false;
		this.flatHand.enabled = false;
		this.regularHand.enabled = true;
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x000665D0 File Offset: 0x000647D0
	private void OnCollisionStay(Collision collision)
	{
		if (!this.isLocal || this.rb.isKinematic)
		{
			return;
		}
		if ((this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringMaxAttachLength))
		{
			return;
		}
		this.Stick();
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		this.rb.transform.rotation = Quaternion.LookRotation(normal, this.rb.transform.up);
		Vector3 vector = this.rb.transform.position - point;
		vector -= Vector3.Dot(vector, normal) * normal;
		this.rb.transform.position = point + vector + this.surfaceOffsetDistance * normal;
	}

	// Token: 0x040017BC RID: 6076
	[SerializeField]
	private MeshRenderer flatHand;

	// Token: 0x040017BD RID: 6077
	[SerializeField]
	private MeshRenderer regularHand;

	// Token: 0x040017BE RID: 6078
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x040017BF RID: 6079
	[SerializeField]
	private GameObject stringParent;

	// Token: 0x040017C0 RID: 6080
	[SerializeField]
	private float surfaceOffsetDistance;

	// Token: 0x040017C1 RID: 6081
	[SerializeField]
	private float stringMaxAttachLength;

	// Token: 0x040017C2 RID: 6082
	[SerializeField]
	private float stringDetachLength;

	// Token: 0x040017C3 RID: 6083
	[SerializeField]
	private float stringTeleportLength;

	// Token: 0x040017C4 RID: 6084
	[SerializeField]
	private SoundBankPlayer thwackSound;

	// Token: 0x040017C5 RID: 6085
	[SerializeField]
	private SoundBankPlayer schlupSound;

	// Token: 0x040017C6 RID: 6086
	private VRRig myRig;

	// Token: 0x040017C7 RID: 6087
	private bool isLocal;

	// Token: 0x040017C8 RID: 6088
	private int stateBitIndex;

	// Token: 0x040017C9 RID: 6089
	private Vector3 defaultLocalPosition;
}
