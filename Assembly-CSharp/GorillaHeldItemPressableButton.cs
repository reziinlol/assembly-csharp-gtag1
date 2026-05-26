using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A07 RID: 2567
public class GorillaHeldItemPressableButton : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x14000080 RID: 128
	// (add) Token: 0x06004187 RID: 16775 RVA: 0x0015E7D0 File Offset: 0x0015C9D0
	// (remove) Token: 0x06004188 RID: 16776 RVA: 0x0015E808 File Offset: 0x0015CA08
	public event Action<GorillaHeldItemPressableButton, TransferrableObject, bool> onPressed;

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x06004189 RID: 16777 RVA: 0x0015E840 File Offset: 0x0015CA40
	// (remove) Token: 0x0600418A RID: 16778 RVA: 0x0015E878 File Offset: 0x0015CA78
	public event Action<GorillaHeldItemPressableButton, TransferrableObject, bool> onReleased;

	// Token: 0x0600418B RID: 16779 RVA: 0x0015E8B0 File Offset: 0x0015CAB0
	private void Start()
	{
		if (this.acceptAnyHoldableThatMatchesType)
		{
			this.acceptedTypes = new List<Type>();
			foreach (TransferrableObject transferrableObject in this.acceptedHoldables)
			{
				this.acceptedTypes.Add(transferrableObject.GetType());
			}
		}
	}

	// Token: 0x0600418C RID: 16780 RVA: 0x0015E920 File Offset: 0x0015CB20
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.delayBetweenSuccessfulPresses >= Time.time)
		{
			return;
		}
		TransferrableObject componentInParent = collider.GetComponentInParent<TransferrableObject>();
		if (componentInParent == null)
		{
			componentInParent = collider.transform.parent.GetComponentInParent<TransferrableObject>();
		}
		if (componentInParent == null || !componentInParent.InHand())
		{
			return;
		}
		if (this.acceptAnyHoldableThatMatchesType)
		{
			if (!this.acceptedTypes.Contains(componentInParent.GetType()))
			{
				return;
			}
		}
		else if (!this.acceptedHoldables.Contains(componentInParent))
		{
			return;
		}
		this.touchTime = Time.time;
		switch (this.mode)
		{
		case HeldItemButtonMode.OneShot:
		{
			UnityEvent<TransferrableObject> unityEvent = this.onPressButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke(componentInParent);
			}
			Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action = this.onPressed;
			if (action != null)
			{
				action(this, componentInParent, componentInParent.InLeftHand());
			}
			this.ButtonActivation(componentInParent);
			this.ButtonActivationWithHand(componentInParent, componentInParent.InLeftHand());
			break;
		}
		case HeldItemButtonMode.ResetAfterDelay:
		{
			this.isOn = true;
			UnityEvent<TransferrableObject> unityEvent2 = this.onPressButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(componentInParent);
			}
			Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action2 = this.onPressed;
			if (action2 != null)
			{
				action2(this, componentInParent, componentInParent.InLeftHand());
			}
			this.ButtonActivation(componentInParent);
			this.ButtonActivationWithHand(componentInParent, componentInParent.InLeftHand());
			GTDelayedExec.Add(this, this.delayBetweenSuccessfulPresses, 0);
			break;
		}
		case HeldItemButtonMode.Toggle:
			this.isOn = !this.isOn;
			if (this.isOn)
			{
				UnityEvent<TransferrableObject> unityEvent3 = this.onPressButton;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(componentInParent);
				}
				Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action3 = this.onPressed;
				if (action3 != null)
				{
					action3(this, componentInParent, componentInParent.InLeftHand());
				}
				this.ButtonActivation(componentInParent);
				this.ButtonActivationWithHand(componentInParent, componentInParent.InLeftHand());
			}
			else
			{
				UnityEvent<TransferrableObject> unityEvent4 = this.onReleaseButton;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke(componentInParent);
				}
				Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action4 = this.onReleased;
				if (action4 != null)
				{
					action4(this, componentInParent, componentInParent.InLeftHand());
				}
			}
			break;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, componentInParent.InLeftHand(), 0.05f);
		GorillaTagger.Instance.StartVibration(componentInParent.InLeftHand(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				67,
				componentInParent.InLeftHand(),
				0.05f
			});
		}
		switch (this.consumeItem)
		{
		case HeldItemButtonConsumeMode.None:
			break;
		case HeldItemButtonConsumeMode.Destroy:
			componentInParent.OnMyCreatorLeft();
			return;
		case HeldItemButtonConsumeMode.Disable:
			componentInParent.gameObject.SetActive(false);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600418D RID: 16781 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonActivation(TransferrableObject holdable)
	{
	}

	// Token: 0x0600418E RID: 16782 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonActivationWithHand(TransferrableObject holdable, bool isLeftHand)
	{
	}

	// Token: 0x0600418F RID: 16783 RVA: 0x0015EBC6 File Offset: 0x0015CDC6
	public virtual void ResetState()
	{
		this.isOn = false;
		UnityEvent<TransferrableObject> unityEvent = this.onReleaseButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke(null);
		}
		Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action = this.onReleased;
		if (action == null)
		{
			return;
		}
		action(this, null, false);
	}

	// Token: 0x06004190 RID: 16784 RVA: 0x0015EBF4 File Offset: 0x0015CDF4
	public void OnDelayedAction(int contextIndex)
	{
		this.ResetState();
	}

	// Token: 0x0400533E RID: 21310
	public int pressButtonSoundIndex = 67;

	// Token: 0x0400533F RID: 21311
	public bool isOn;

	// Token: 0x04005340 RID: 21312
	public float delayBetweenSuccessfulPresses = 0.25f;

	// Token: 0x04005341 RID: 21313
	private float touchTime;

	// Token: 0x04005342 RID: 21314
	public HeldItemButtonMode mode;

	// Token: 0x04005343 RID: 21315
	public List<TransferrableObject> acceptedHoldables;

	// Token: 0x04005344 RID: 21316
	private List<Type> acceptedTypes;

	// Token: 0x04005345 RID: 21317
	public bool acceptAnyHoldableThatMatchesType = true;

	// Token: 0x04005346 RID: 21318
	public HeldItemButtonConsumeMode consumeItem;

	// Token: 0x04005347 RID: 21319
	[Space]
	public UnityEvent<TransferrableObject> onPressButton;

	// Token: 0x04005349 RID: 21321
	[Space]
	public UnityEvent<TransferrableObject> onReleaseButton;
}
