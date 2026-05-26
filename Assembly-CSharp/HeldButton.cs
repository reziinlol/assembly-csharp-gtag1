using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020008E4 RID: 2276
public class HeldButton : MonoBehaviour
{
	// Token: 0x06003B88 RID: 15240 RVA: 0x001461A4 File Offset: 0x001443A4
	private void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		if ((componentInParent.isLeftHand && !this.leftHandPressable) || (!componentInParent.isLeftHand && !this.rightHandPressable))
		{
			return;
		}
		if (!this.pendingPress || other != this.pendingPressCollider)
		{
			UnityEvent unityEvent = this.onStartPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.touchTime = Time.time;
			this.pendingPressCollider = other;
			this.pressingHand = componentInParent;
			this.pendingPress = true;
			this.SetOn(true);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06003B89 RID: 15241 RVA: 0x00146264 File Offset: 0x00144464
	private void LateUpdate()
	{
		if (!this.pendingPress)
		{
			return;
		}
		if (this.touchTime < this.releaseTime && this.releaseTime + this.debounceTime < Time.time)
		{
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.SetOn(false);
			return;
		}
		if (this.touchTime + this.pressDuration < Time.time)
		{
			this.onPressButton.Invoke();
			if (this.pressingHand != null)
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, this.pressingHand.isLeftHand, 0.1f);
				GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			UnityEvent unityEvent2 = this.onStopPressingButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.releaseTime = Time.time;
			this.SetOn(false);
			return;
		}
		if (this.touchTime > this.releaseTime && this.pressingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, Time.fixedDeltaTime);
		}
	}

	// Token: 0x06003B8A RID: 15242 RVA: 0x001463C3 File Offset: 0x001445C3
	private void OnTriggerExit(Collider other)
	{
		if (this.pendingPress && this.pendingPressCollider == other)
		{
			this.releaseTime = Time.time;
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06003B8B RID: 15243 RVA: 0x001463F8 File Offset: 0x001445F8
	public void SetOn(bool inOn)
	{
		if (inOn == this.isOn)
		{
			return;
		}
		this.isOn = inOn;
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x04004C16 RID: 19478
	public Material pressedMaterial;

	// Token: 0x04004C17 RID: 19479
	public Material unpressedMaterial;

	// Token: 0x04004C18 RID: 19480
	public MeshRenderer buttonRenderer;

	// Token: 0x04004C19 RID: 19481
	private bool isOn;

	// Token: 0x04004C1A RID: 19482
	public float debounceTime = 0.25f;

	// Token: 0x04004C1B RID: 19483
	public bool leftHandPressable;

	// Token: 0x04004C1C RID: 19484
	public bool rightHandPressable = true;

	// Token: 0x04004C1D RID: 19485
	public float pressDuration = 0.5f;

	// Token: 0x04004C1E RID: 19486
	public UnityEvent onStartPressingButton;

	// Token: 0x04004C1F RID: 19487
	public UnityEvent onStopPressingButton;

	// Token: 0x04004C20 RID: 19488
	public UnityEvent onPressButton;

	// Token: 0x04004C21 RID: 19489
	[TextArea]
	public string offText;

	// Token: 0x04004C22 RID: 19490
	[TextArea]
	public string onText;

	// Token: 0x04004C23 RID: 19491
	public Text myText;

	// Token: 0x04004C24 RID: 19492
	private float touchTime;

	// Token: 0x04004C25 RID: 19493
	private float releaseTime;

	// Token: 0x04004C26 RID: 19494
	private bool pendingPress;

	// Token: 0x04004C27 RID: 19495
	private Collider pendingPressCollider;

	// Token: 0x04004C28 RID: 19496
	private GorillaTriggerColliderHandIndicator pressingHand;
}
