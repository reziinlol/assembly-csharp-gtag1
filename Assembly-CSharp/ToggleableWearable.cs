using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020003A4 RID: 932
public class ToggleableWearable : MonoBehaviour
{
	// Token: 0x06001690 RID: 5776 RVA: 0x00082A10 File Offset: 0x00080C10
	protected void Awake()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
		if (this.ownerRig == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.ownerRig = componentInParent.offlineVRRig;
				this.ownerIsLocal = (this.ownerRig != null);
			}
		}
		if (this.ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = this.startOn;
		}
		this.hasAudioSource = (this.audioSource != null);
		this.assignedSlotBitIndex = (int)this.assignedSlot;
		if (this.oneShot)
		{
			this.toggleCooldownRange.x = this.toggleCooldownRange.x + this.animationTransitionDuration;
			this.toggleCooldownRange.y = this.toggleCooldownRange.y + this.animationTransitionDuration;
		}
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x00082B18 File Offset: 0x00080D18
	protected void LateUpdate()
	{
		if (this.ownerIsLocal)
		{
			this.toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(this.triggerOffset), this.triggerRadius * transform.lossyScale.x, this.colliders, this.layerMask) > 0 && this.toggleCooldownTimer < 0f)
			{
				XRController componentInParent = this.colliders[0].GetComponentInParent<XRController>();
				if (componentInParent != null)
				{
					this.LocalToggle(componentInParent.controllerNode == XRNode.LeftHand, true, true);
				}
				this.toggleCooldownTimer = Random.Range(this.toggleCooldownRange.x, this.toggleCooldownRange.y);
				this.toggleTimer = 0f;
			}
			if (this.resetTimer > 0f)
			{
				this.toggleTimer += Time.deltaTime;
				if (this.toggleTimer > this.resetTimer && this.startOn != this.isOn)
				{
					this.LocalToggle(false, true, false);
					this.toggleTimer = 0f;
				}
			}
		}
		else
		{
			bool flag = (this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0;
			if (this.isOn != flag)
			{
				this.SharedSetState(flag, true);
			}
		}
		if (this.oneShot)
		{
			if (this.isOn)
			{
				this.progress = Mathf.MoveTowards(this.progress, 1f, Time.deltaTime / this.animationTransitionDuration);
				if (this.progress == 1f)
				{
					if (this.ownerIsLocal)
					{
						this.LocalToggle(false, false, false);
					}
					else
					{
						this.SharedSetState(false, false);
					}
					this.progress = 0f;
				}
			}
		}
		else
		{
			this.progress = Mathf.MoveTowards(this.progress, this.isOn ? 1f : 0f, Time.deltaTime / this.animationTransitionDuration);
		}
		Animator[] array = this.animators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(ToggleableWearable.animParam_Progress, this.progress);
		}
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x00082D28 File Offset: 0x00080F28
	private void LocalToggle(bool isLeftHand, bool playAudio, bool playHaptics)
	{
		this.ownerRig.WearablePackedStates ^= 1 << this.assignedSlotBitIndex;
		this.SharedSetState((this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0, playAudio);
		if (playHaptics && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.isOn ? this.turnOnVibrationDuration : this.turnOffVibrationDuration, this.isOn ? this.turnOnVibrationStrength : this.turnOffVibrationStrength);
		}
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x00082DBC File Offset: 0x00080FBC
	private void SharedSetState(bool state, bool playAudio)
	{
		this.isOn = state;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isOn;
		}
		if (!playAudio || !this.hasAudioSource)
		{
			return;
		}
		AudioClip audioClip = this.isOn ? this.toggleOnSound : this.toggleOffSound;
		if (audioClip == null)
		{
			return;
		}
		if (this.oneShot)
		{
			this.audioSource.clip = audioClip;
			this.audioSource.GTPlay();
			return;
		}
		this.audioSource.GTPlayOneShot(audioClip, 1f);
	}

	// Token: 0x040020AB RID: 8363
	public Renderer[] renderers;

	// Token: 0x040020AC RID: 8364
	public Animator[] animators;

	// Token: 0x040020AD RID: 8365
	public float animationTransitionDuration = 1f;

	// Token: 0x040020AE RID: 8366
	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	// Token: 0x040020AF RID: 8367
	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	// Token: 0x040020B0 RID: 8368
	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	// Token: 0x040020B1 RID: 8369
	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	// Token: 0x040020B2 RID: 8370
	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	// Token: 0x040020B3 RID: 8371
	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	// Token: 0x040020B4 RID: 8372
	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	// Token: 0x040020B5 RID: 8373
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	// Token: 0x040020B6 RID: 8374
	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	// Token: 0x040020B7 RID: 8375
	public float turnOnVibrationStrength = 0.2f;

	// Token: 0x040020B8 RID: 8376
	public float turnOffVibrationDuration = 0.05f;

	// Token: 0x040020B9 RID: 8377
	public float turnOffVibrationStrength = 0.2f;

	// Token: 0x040020BA RID: 8378
	private VRRig ownerRig;

	// Token: 0x040020BB RID: 8379
	private bool ownerIsLocal;

	// Token: 0x040020BC RID: 8380
	private bool isOn;

	// Token: 0x040020BD RID: 8381
	[SerializeField]
	private Vector2 toggleCooldownRange = new Vector2(0.2f, 0.2f);

	// Token: 0x040020BE RID: 8382
	private bool hasAudioSource;

	// Token: 0x040020BF RID: 8383
	private readonly Collider[] colliders = new Collider[1];

	// Token: 0x040020C0 RID: 8384
	private int framesSinceCooldownAndExitingVolume;

	// Token: 0x040020C1 RID: 8385
	private float toggleCooldownTimer;

	// Token: 0x040020C2 RID: 8386
	private int assignedSlotBitIndex;

	// Token: 0x040020C3 RID: 8387
	private static readonly int animParam_Progress = Animator.StringToHash("Progress");

	// Token: 0x040020C4 RID: 8388
	private float progress;

	// Token: 0x040020C5 RID: 8389
	[SerializeField]
	private bool oneShot;

	// Token: 0x040020C6 RID: 8390
	[SerializeField]
	[Tooltip("Seconds before reverting to its default state, as defined by 'Start On.' A value of 0 or less means never.")]
	private float resetTimer;

	// Token: 0x040020C7 RID: 8391
	private float toggleTimer;
}
