using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000216 RID: 534
public class FingerTorch : MonoBehaviour, ISpawnable
{
	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x0004C663 File Offset: 0x0004A863
	// (set) Token: 0x06000DF7 RID: 3575 RVA: 0x0004C66B File Offset: 0x0004A86B
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x0004C674 File Offset: 0x0004A874
	// (set) Token: 0x06000DF9 RID: 3577 RVA: 0x0004C67C File Offset: 0x0004A87C
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000DFA RID: 3578 RVA: 0x0004C685 File Offset: 0x0004A885
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000DFC RID: 3580 RVA: 0x0004C6A8 File Offset: 0x0004A8A8
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected void OnDisable()
	{
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x0004C6E0 File Offset: 0x0004A8E0
	private void UpdateLocal()
	{
		int node = this.attachedToLeftHand ? 4 : 5;
		bool flag = ControllerInputPoller.GripFloat((XRNode)node) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress((XRNode)node);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress((XRNode)node);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x0004C760 File Offset: 0x0004A960
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
			this.particleFX.SetActive(this.extended);
		}
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x0004C794 File Offset: 0x0004A994
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x0004C7CD File Offset: 0x0004A9CD
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x0004C7EA File Offset: 0x0004A9EA
	protected void LateUpdate()
	{
		if (this.IsMyItem())
		{
			this.UpdateLocal();
		}
		else
		{
			this.UpdateReplicated();
		}
		this.UpdateShared();
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0004C808 File Offset: 0x0004AA08
	private void OnExtendStateChanged(bool playAudio)
	{
		this.audioSource.clip = (this.extended ? this.extendAudioClip : this.retractAudioClip);
		if (playAudio)
		{
			this.audioSource.GTPlay();
		}
		if (this.IsMyItem() && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(this.attachedToLeftHand, this.extended ? this.extendVibrationDuration : this.retractVibrationDuration, this.extended ? this.extendVibrationStrength : this.retractVibrationStrength);
		}
	}

	// Token: 0x040010C1 RID: 4289
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x040010C2 RID: 4290
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x040010C3 RID: 4291
	public Transform thumbRingBone;

	// Token: 0x040010C4 RID: 4292
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x040010C5 RID: 4293
	public AudioClip extendAudioClip;

	// Token: 0x040010C6 RID: 4294
	public AudioClip retractAudioClip;

	// Token: 0x040010C7 RID: 4295
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x040010C8 RID: 4296
	public float extendVibrationStrength = 0.2f;

	// Token: 0x040010C9 RID: 4297
	public float retractVibrationDuration = 0.05f;

	// Token: 0x040010CA RID: 4298
	public float retractVibrationStrength = 0.2f;

	// Token: 0x040010CB RID: 4299
	[Header("Particle FX")]
	public GameObject particleFX;

	// Token: 0x040010CC RID: 4300
	private bool networkedExtended;

	// Token: 0x040010CD RID: 4301
	private bool extended;

	// Token: 0x040010CE RID: 4302
	private InputDevice inputDevice;

	// Token: 0x040010CF RID: 4303
	private VRRig myRig;

	// Token: 0x040010D0 RID: 4304
	private int stateBitIndex;
}
