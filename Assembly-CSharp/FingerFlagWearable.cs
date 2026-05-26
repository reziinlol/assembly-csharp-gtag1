using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020001F8 RID: 504
public class FingerFlagWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000D39 RID: 3385 RVA: 0x00048571 File Offset: 0x00046771
	// (set) Token: 0x06000D3A RID: 3386 RVA: 0x00048579 File Offset: 0x00046779
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000D3B RID: 3387 RVA: 0x00048582 File Offset: 0x00046782
	// (set) Token: 0x06000D3C RID: 3388 RVA: 0x0004858A File Offset: 0x0004678A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000D3D RID: 3389 RVA: 0x00048593 File Offset: 0x00046793
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = base.GetComponentInParent<VRRig>(true);
		if (!this.myRig)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x000485BC File Offset: 0x000467BC
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x000485F4 File Offset: 0x000467F4
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

	// Token: 0x06000D41 RID: 3393 RVA: 0x00048674 File Offset: 0x00046874
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = (this.extended && this.retractExtendTime <= 0f);
		if (flag != this.fullyRetracted)
		{
			Transform[] array = this.clothRigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(!this.fullyRetracted);
			}
		}
		this.UpdateAnimation();
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00048702 File Offset: 0x00046902
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0004873B File Offset: 0x0004693B
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00048758 File Offset: 0x00046958
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

	// Token: 0x06000D45 RID: 3397 RVA: 0x00048778 File Offset: 0x00046978
	private void UpdateAnimation()
	{
		float num = this.extended ? this.extendSpeed : (-this.retractSpeed);
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x000487D0 File Offset: 0x000469D0
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

	// Token: 0x04000FD0 RID: 4048
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x04000FD1 RID: 4049
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x04000FD2 RID: 4050
	public Transform thumbRingBone;

	// Token: 0x04000FD3 RID: 4051
	public Transform[] clothBones;

	// Token: 0x04000FD4 RID: 4052
	public Transform[] clothRigidbodies;

	// Token: 0x04000FD5 RID: 4053
	[Header("Animation")]
	public Animator animator;

	// Token: 0x04000FD6 RID: 4054
	public float extendSpeed = 1.5f;

	// Token: 0x04000FD7 RID: 4055
	public float retractSpeed = 2.25f;

	// Token: 0x04000FD8 RID: 4056
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000FD9 RID: 4057
	public AudioClip extendAudioClip;

	// Token: 0x04000FDA RID: 4058
	public AudioClip retractAudioClip;

	// Token: 0x04000FDB RID: 4059
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x04000FDC RID: 4060
	public float extendVibrationStrength = 0.2f;

	// Token: 0x04000FDD RID: 4061
	public float retractVibrationDuration = 0.05f;

	// Token: 0x04000FDE RID: 4062
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04000FDF RID: 4063
	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	// Token: 0x04000FE0 RID: 4064
	private bool networkedExtended;

	// Token: 0x04000FE1 RID: 4065
	private bool extended;

	// Token: 0x04000FE2 RID: 4066
	private bool fullyRetracted;

	// Token: 0x04000FE3 RID: 4067
	private float retractExtendTime;

	// Token: 0x04000FE4 RID: 4068
	private InputDevice inputDevice;

	// Token: 0x04000FE5 RID: 4069
	private VRRig myRig;

	// Token: 0x04000FE6 RID: 4070
	private int stateBitIndex;
}
