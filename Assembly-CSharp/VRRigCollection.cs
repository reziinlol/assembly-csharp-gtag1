using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000DA3 RID: 3491
[RequireComponent(typeof(CompositeTriggerEvents))]
public class VRRigCollection : MonoBehaviour
{
	// Token: 0x17000801 RID: 2049
	// (get) Token: 0x060055A6 RID: 21926 RVA: 0x001BE61A File Offset: 0x001BC81A
	public List<RigContainer> Rigs
	{
		get
		{
			return this.containedRigs;
		}
	}

	// Token: 0x060055A7 RID: 21927 RVA: 0x001BE622 File Offset: 0x001BC822
	private void OnEnable()
	{
		this.collisionTriggerEvents.CompositeTriggerEnter += this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit += this.OnRigTriggerExit;
	}

	// Token: 0x060055A8 RID: 21928 RVA: 0x001BE654 File Offset: 0x001BC854
	private void OnDisable()
	{
		for (int i = this.containedRigs.Count - 1; i >= 0; i--)
		{
			this.RigDisabled(this.containedRigs[i]);
		}
		this.collisionTriggerEvents.CompositeTriggerEnter -= this.OnRigTriggerEnter;
		this.collisionTriggerEvents.CompositeTriggerExit -= this.OnRigTriggerExit;
	}

	// Token: 0x060055A9 RID: 21929 RVA: 0x001BE6BC File Offset: 0x001BC8BC
	private void OnRigTriggerEnter(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		rigContainer.RigEvents.disableEvent += this.RigDisabled;
		this.containedRigs.Add(rigContainer);
		Action<RigContainer> action = this.playerEnteredCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x060055AA RID: 21930 RVA: 0x001BE740 File Offset: 0x001BC940
	private void OnRigTriggerExit(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		RigContainer rigContainer;
		if (attachedRigidbody == null || !attachedRigidbody.TryGetComponent<RigContainer>(out rigContainer) || other != rigContainer.HeadCollider || !this.containedRigs.Contains(rigContainer))
		{
			return;
		}
		rigContainer.RigEvents.disableEvent -= this.RigDisabled;
		this.containedRigs.Remove(rigContainer);
		Action<RigContainer> action = this.playerLeftCollection;
		if (action == null)
		{
			return;
		}
		action(rigContainer);
	}

	// Token: 0x060055AB RID: 21931 RVA: 0x001BE7C4 File Offset: 0x001BC9C4
	private void RigDisabled(RigContainer rig)
	{
		this.collisionTriggerEvents.ResetColliderMask(rig.HeadCollider);
		this.collisionTriggerEvents.ResetColliderMask(rig.BodyCollider);
	}

	// Token: 0x060055AC RID: 21932 RVA: 0x001BE7E8 File Offset: 0x001BC9E8
	private bool HasRig(VRRig rig)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060055AD RID: 21933 RVA: 0x001BE828 File Offset: 0x001BCA28
	private bool HasRig(NetPlayer player)
	{
		for (int i = 0; i < this.containedRigs.Count; i++)
		{
			if (this.containedRigs[i].Creator == player)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040065BF RID: 26047
	public readonly List<RigContainer> containedRigs = new List<RigContainer>(20);

	// Token: 0x040065C0 RID: 26048
	[SerializeField]
	private CompositeTriggerEvents collisionTriggerEvents;

	// Token: 0x040065C1 RID: 26049
	public Action<RigContainer> playerEnteredCollection;

	// Token: 0x040065C2 RID: 26050
	public Action<RigContainer> playerLeftCollection;
}
