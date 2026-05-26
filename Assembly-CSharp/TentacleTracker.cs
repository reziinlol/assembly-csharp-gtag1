using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class TentacleTracker : MonoBehaviour
{
	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x0600116C RID: 4460 RVA: 0x0005D927 File Offset: 0x0005BB27
	// (set) Token: 0x0600116D RID: 4461 RVA: 0x0005D92F File Offset: 0x0005BB2F
	public VRRig currentTargetRig { get; private set; }

	// Token: 0x0600116E RID: 4462 RVA: 0x0005D938 File Offset: 0x0005BB38
	private void OnEnable()
	{
		this.tracking = true;
		this.testTriggersRemaining.Clear();
		this.testTriggersRemaining.AddRange(this.testTriggers);
		this.currentTargetRig = null;
		this.currentTargetIsLocal = false;
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0005D96C File Offset: 0x0005BB6C
	public void BeginGrab(VRRig targetRig, bool isLocalPlayer)
	{
		if (targetRig == null)
		{
			return;
		}
		base.gameObject.SetActive(true);
		this.tracking = true;
		this.currentTargetRig = targetRig;
		this.currentTargetIsLocal = isLocalPlayer;
		this.testTriggersRemaining.Clear();
		this.testTriggersRemaining.AddRange(this.testTriggers);
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x0005D9C0 File Offset: 0x0005BBC0
	public void Anim_OnReachEnded()
	{
		this.animator.SetTrigger(this.testTriggersRemaining[0]);
		this.testTriggersRemaining.RemoveAt(0);
		if (this.currentTargetIsLocal)
		{
			GTPlayer.Instance.BeginClimbing(this.climbable, EquipmentInteractor.instance.BodyClimber, null);
			EquipmentInteractor.instance.BodyClimber.SetCanRelease(false);
			this.tracking = false;
		}
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0005DA30 File Offset: 0x0005BC30
	private void Update()
	{
		if (!this.tracking)
		{
			return;
		}
		Vector3 a = (this.currentTargetRig != null && !this.currentTargetIsLocal) ? this.currentTargetRig.head.rigTarget.position : GTPlayer.Instance.mainCamera.transform.position;
		Vector3 a2 = a - this.anchorPoint.position;
		float magnitude = a2.magnitude;
		if (magnitude < 0.001f)
		{
			return;
		}
		Vector3 forward = a2 / magnitude;
		Vector3 normalized = (this.playerRefPoint.localPosition - this.anchorRefPoint.localPosition).normalized;
		base.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Inverse(Quaternion.LookRotation(normalized));
		base.transform.position += a - this.playerRefPoint.position;
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0005DB24 File Offset: 0x0005BD24
	public void TestDrop()
	{
		if (this.currentTargetRig == null || this.currentTargetIsLocal)
		{
			EquipmentInteractor.instance.BodyClimber.SetCanRelease(true);
			GTPlayer.Instance.EndClimbing(EquipmentInteractor.instance.BodyClimber, false, false);
		}
		this.currentTargetRig = null;
		this.currentTargetIsLocal = false;
		this.tracking = true;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x000440BC File Offset: 0x000422BC
	public void TestDisappear()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040014C6 RID: 5318
	[SerializeField]
	private Transform anchorPoint;

	// Token: 0x040014C7 RID: 5319
	[SerializeField]
	private Transform anchorRefPoint;

	// Token: 0x040014C8 RID: 5320
	[SerializeField]
	private Transform playerRefPoint;

	// Token: 0x040014C9 RID: 5321
	[SerializeField]
	private Animator animator;

	// Token: 0x040014CA RID: 5322
	[SerializeField]
	private GorillaClimbable climbable;

	// Token: 0x040014CB RID: 5323
	[SerializeField]
	private string[] testTriggers;

	// Token: 0x040014CC RID: 5324
	private List<string> testTriggersRemaining = new List<string>();

	// Token: 0x040014CD RID: 5325
	private bool tracking = true;

	// Token: 0x040014CF RID: 5327
	private bool currentTargetIsLocal;
}
