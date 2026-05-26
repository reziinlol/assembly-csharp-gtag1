using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaTag;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;

// Token: 0x02000A9D RID: 2717
public abstract class CustomMapsScreenTouchPoint : MonoBehaviour, IClickable
{
	// Token: 0x06004563 RID: 17763 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void Awake()
	{
	}

	// Token: 0x06004564 RID: 17764 RVA: 0x00177514 File Offset: 0x00175714
	protected virtual void OnDisable()
	{
		if (this.colorUpdateCoroutine != null)
		{
			base.StopCoroutine(this.colorUpdateCoroutine);
		}
		if (this.buttonColorSettings != null)
		{
			this.touchPointRenderer.color = this.buttonColorSettings.UnpressedColor;
		}
	}

	// Token: 0x06004565 RID: 17765 RVA: 0x00177550 File Offset: 0x00175750
	private void OnTriggerEnter(Collider collider)
	{
		GTDev.Log<string>(string.Format("trigger {0} pressTime={1} time={2}", base.gameObject.name, CustomMapsScreenTouchPoint.pressTime, Time.time), null);
		if (Time.time < CustomMapsScreenTouchPoint.pressTime + CustomMapsScreenTouchPoint.pressedTime)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			Vector3 rhs = this.GetForwardDirection();
			if (Vector3.Dot((collider.transform.position - base.transform.position).normalized, rhs) < 0f)
			{
				return;
			}
			GTDev.Log<string>(string.Format("trigger {0} collider {1} postion {2}", base.gameObject.name, collider.gameObject.name, collider.transform.position), null);
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			CustomMapsScreenTouchPoint.pressTime = Time.time;
			this.OnButtonPressedEvent();
			this.PressButtonColourUpdate();
			if (this.screen != null)
			{
				this.screen.PressButton(this.keyBinding);
			}
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06004566 RID: 17766 RVA: 0x0017768D File Offset: 0x0017588D
	public virtual void PressButtonColourUpdate()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.touchPointRenderer.color = this.buttonColorSettings.PressedColor;
		this.colorUpdateCoroutine = base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|12_0());
	}

	// Token: 0x06004567 RID: 17767 RVA: 0x001776C8 File Offset: 0x001758C8
	private Vector3 GetForwardDirection()
	{
		switch (this.forwardDirection)
		{
		case CustomMapsScreenTouchPoint.TouchPointDirections.Forward:
			return base.transform.forward;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Backward:
			return -base.transform.forward;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Left:
			return -base.transform.right;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Right:
			return base.transform.right;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Up:
			return base.transform.up;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Down:
			return -base.transform.up;
		default:
			return base.transform.forward;
		}
	}

	// Token: 0x06004568 RID: 17768
	protected abstract void OnButtonPressedEvent();

	// Token: 0x06004569 RID: 17769 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Click(bool leftHand = false)
	{
	}

	// Token: 0x0600456C RID: 17772 RVA: 0x0017776A File Offset: 0x0017596A
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|12_0()
	{
		yield return new WaitForSeconds(CustomMapsScreenTouchPoint.pressedTime);
		if (CustomMapsScreenTouchPoint.pressTime != 0f && Time.time > CustomMapsScreenTouchPoint.pressedTime + CustomMapsScreenTouchPoint.pressTime)
		{
			this.touchPointRenderer.color = this.buttonColorSettings.UnpressedColor;
			CustomMapsScreenTouchPoint.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x040057D0 RID: 22480
	[SerializeField]
	private CustomMapsTerminalScreen screen;

	// Token: 0x040057D1 RID: 22481
	[SerializeField]
	private CustomMapKeyboardBinding keyBinding;

	// Token: 0x040057D2 RID: 22482
	[SerializeField]
	private CustomMapsScreenTouchPoint.TouchPointDirections forwardDirection;

	// Token: 0x040057D3 RID: 22483
	[SerializeField]
	protected SpriteRenderer touchPointRenderer;

	// Token: 0x040057D4 RID: 22484
	[SerializeField]
	protected ButtonColorSettings buttonColorSettings;

	// Token: 0x040057D5 RID: 22485
	private static float pressedTime = 0.25f;

	// Token: 0x040057D6 RID: 22486
	protected static float pressTime;

	// Token: 0x040057D7 RID: 22487
	private Coroutine colorUpdateCoroutine;

	// Token: 0x02000A9E RID: 2718
	public enum TouchPointDirections
	{
		// Token: 0x040057D9 RID: 22489
		Forward,
		// Token: 0x040057DA RID: 22490
		Backward,
		// Token: 0x040057DB RID: 22491
		Left,
		// Token: 0x040057DC RID: 22492
		Right,
		// Token: 0x040057DD RID: 22493
		Up,
		// Token: 0x040057DE RID: 22494
		Down
	}
}
