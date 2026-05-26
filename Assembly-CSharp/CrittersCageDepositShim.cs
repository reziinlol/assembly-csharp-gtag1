using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class CrittersCageDepositShim : MonoBehaviour
{
	// Token: 0x060001CE RID: 462 RVA: 0x0000AD74 File Offset: 0x00008F74
	[ContextMenu("Copy Deposit Data To Shim")]
	private CrittersCageDeposit CopySpawnerDataInPrefab()
	{
		CrittersCageDeposit component = base.gameObject.GetComponent<CrittersCageDeposit>();
		this.cageBoxCollider = (BoxCollider)component.gameObject.GetComponent<Collider>();
		this.type = component.actorType;
		this.disableGrabOnAttach = component.disableGrabOnAttach;
		this.allowMultiAttach = component.allowMultiAttach;
		this.snapOnAttach = component.snapOnAttach;
		this.startLocation = component.depositStartLocation;
		this.endLocation = component.depositEndLocation;
		this.submitDuration = component.submitDuration;
		this.returnDuration = component.returnDuration;
		this.depositAudio = component.depositAudio;
		this.depositStartSound = component.depositStartSound;
		this.depositEmptySound = component.depositEmptySound;
		this.depositCritterSound = component.depositCritterSound;
		this.attachPointTransform = component.GetComponentInChildren<CrittersActor>().transform;
		this.visiblePlatformTransform = this.attachPointTransform.transform.GetChild(0).transform;
		return component;
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000AE64 File Offset: 0x00009064
	[ContextMenu("Replace Deposit With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersCageDeposit crittersCageDeposit = this.CopySpawnerDataInPrefab();
		if (crittersCageDeposit.attachPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersCageDeposit.attachPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersCageDeposit.attachPoint);
		Object.DestroyImmediate(crittersCageDeposit);
	}

	// Token: 0x04000204 RID: 516
	public BoxCollider cageBoxCollider;

	// Token: 0x04000205 RID: 517
	public CrittersActor.CrittersActorType type;

	// Token: 0x04000206 RID: 518
	public bool disableGrabOnAttach;

	// Token: 0x04000207 RID: 519
	public bool allowMultiAttach;

	// Token: 0x04000208 RID: 520
	public bool snapOnAttach;

	// Token: 0x04000209 RID: 521
	public Vector3 startLocation;

	// Token: 0x0400020A RID: 522
	public Vector3 endLocation;

	// Token: 0x0400020B RID: 523
	public float submitDuration;

	// Token: 0x0400020C RID: 524
	public float returnDuration;

	// Token: 0x0400020D RID: 525
	public AudioSource depositAudio;

	// Token: 0x0400020E RID: 526
	public AudioClip depositStartSound;

	// Token: 0x0400020F RID: 527
	public AudioClip depositEmptySound;

	// Token: 0x04000210 RID: 528
	public AudioClip depositCritterSound;

	// Token: 0x04000211 RID: 529
	public Transform attachPointTransform;

	// Token: 0x04000212 RID: 530
	public Transform visiblePlatformTransform;
}
