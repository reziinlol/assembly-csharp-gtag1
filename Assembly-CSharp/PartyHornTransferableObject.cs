using System;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000207 RID: 519
public class PartyHornTransferableObject : TransferrableObject
{
	// Token: 0x06000DBB RID: 3515 RVA: 0x0004B3EE File Offset: 0x000495EE
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x0004B3FC File Offset: 0x000495FC
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x0004B404 File Offset: 0x00049604
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x0004B414 File Offset: 0x00049614
	protected Vector3 CalcMouthPiecePos()
	{
		if (!this.mouthPiece)
		{
			return base.transform.position + this.mouthPieceZOffset * base.transform.forward;
		}
		return this.mouthPiece.position;
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x0004B460 File Offset: 0x00049660
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState != TransferrableObject.ItemStates.State0)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 b = this.CalcMouthPiecePos();
		float num = this.mouthPieceRadius * this.mouthPieceRadius * GTPlayer.Instance.scale * GTPlayer.Instance.scale;
		bool flag = (GorillaTagger.Instance.offlineVRRig.GetMouthPosition() - b).sqrMagnitude < num;
		if (this.soundActivated && PhotonNetwork.InRoom)
		{
			bool flag2;
			if (flag)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance == null)
				{
					flag2 = false;
				}
				else
				{
					Recorder myRecorder = instance.myRecorder;
					bool? flag3 = (myRecorder != null) ? new bool?(myRecorder.IsCurrentlyTransmitting) : null;
					bool flag4 = true;
					flag2 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
				}
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
		{
			VRRig rig = VRRigCache.ActiveRigContainers[i].Rig;
			if (flag)
			{
				break;
			}
			flag = ((rig.GetMouthPosition() - b).sqrMagnitude < num);
			if (this.soundActivated)
			{
				bool flag5;
				if (flag)
				{
					RigContainer rigContainer = rig.rigContainer;
					if (rigContainer == null)
					{
						flag5 = false;
					}
					else
					{
						PhotonVoiceView voice = rigContainer.Voice;
						bool? flag3 = (voice != null) ? new bool?(voice.IsSpeaking) : null;
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
					}
				}
				else
				{
					flag5 = false;
				}
				flag = flag5;
			}
		}
		this.itemState = (flag ? TransferrableObject.ItemStates.State1 : this.itemState);
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x0004B5F4 File Offset: 0x000497F4
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (TransferrableObject.ItemStates.State1 != this.itemState)
		{
			return;
		}
		if (!this.localWasActivated)
		{
			if (this.effectsGameObject)
			{
				this.effectsGameObject.SetActive(true);
			}
			this.cooldownRemaining = this.cooldown;
			this.localWasActivated = true;
			UnityEvent onCooldownStart = this.OnCooldownStart;
			if (onCooldownStart != null)
			{
				onCooldownStart.Invoke();
			}
		}
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.InitToDefault();
		}
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x0004B67C File Offset: 0x0004987C
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.effectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.cooldownRemaining = this.cooldown;
		this.localWasActivated = false;
		UnityEvent onCooldownReset = this.OnCooldownReset;
		if (onCooldownReset == null)
		{
			return;
		}
		onCooldownReset.Invoke();
	}

	// Token: 0x04001059 RID: 4185
	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	// Token: 0x0400105A RID: 4186
	public float cooldown = 2f;

	// Token: 0x0400105B RID: 4187
	public float mouthPieceZOffset = -0.18f;

	// Token: 0x0400105C RID: 4188
	public float mouthPieceRadius = 0.05f;

	// Token: 0x0400105D RID: 4189
	public Transform mouthPiece;

	// Token: 0x0400105E RID: 4190
	public bool soundActivated;

	// Token: 0x0400105F RID: 4191
	public UnityEvent OnCooldownStart;

	// Token: 0x04001060 RID: 4192
	public UnityEvent OnCooldownReset;

	// Token: 0x04001061 RID: 4193
	private float cooldownRemaining;

	// Token: 0x04001062 RID: 4194
	private PartyHornTransferableObject.PartyHornState partyHornStateLastFrame;

	// Token: 0x04001063 RID: 4195
	private bool localWasActivated;

	// Token: 0x02000208 RID: 520
	private enum PartyHornState
	{
		// Token: 0x04001065 RID: 4197
		None = 1,
		// Token: 0x04001066 RID: 4198
		CoolingDown
	}
}
