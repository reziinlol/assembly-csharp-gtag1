using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A0B RID: 2571
public class GorillaPhysicalButton : MonoBehaviour
{
	// Token: 0x14000082 RID: 130
	// (add) Token: 0x060041A5 RID: 16805 RVA: 0x0015EF5C File Offset: 0x0015D15C
	// (remove) Token: 0x060041A6 RID: 16806 RVA: 0x0015EF94 File Offset: 0x0015D194
	public event Action<GorillaPhysicalButton, bool> onPressedOn;

	// Token: 0x14000083 RID: 131
	// (add) Token: 0x060041A7 RID: 16807 RVA: 0x0015EFCC File Offset: 0x0015D1CC
	// (remove) Token: 0x060041A8 RID: 16808 RVA: 0x0015F004 File Offset: 0x0015D204
	public event Action<GorillaPhysicalButton, bool> onToggledOff;

	// Token: 0x060041A9 RID: 16809 RVA: 0x0015F03C File Offset: 0x0015D23C
	public virtual void Start()
	{
		if (this.moveableChildren != null)
		{
			this.moveableChildrenStartPositions = new List<Vector3>(this.moveableChildren.Count);
			for (int i = 0; i < this.moveableChildren.Count; i++)
			{
				this.moveableChildrenStartPositions.Add(this.moveableChildren[i].position);
			}
		}
		this.startButtonPosition = base.transform.position;
		base.enabled = true;
	}

	// Token: 0x060041AA RID: 16810 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x060041AB RID: 16811 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnDisable()
	{
	}

	// Token: 0x060041AC RID: 16812 RVA: 0x0015F0B4 File Offset: 0x0015D2B4
	private float GetSurfaceDistanceFromKeyToCollider(Collider collider)
	{
		if (collider == null)
		{
			return 1f;
		}
		SphereCollider sphereCollider = collider as SphereCollider;
		float num = sphereCollider ? sphereCollider.radius : 0f;
		float num2 = base.transform.localScale.z * 0.5f;
		if (Vector3.Distance(collider.transform.position, base.transform.position) > (base.transform.localScale.magnitude * 0.5f + num) * 1.5f)
		{
			return 1f;
		}
		return Vector3.Dot(base.transform.position - collider.transform.position, -base.transform.forward) - num - num2;
	}

	// Token: 0x060041AD RID: 16813 RVA: 0x0015F17C File Offset: 0x0015D37C
	protected void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.recentFingerCollider = other;
		if (this.buttonTestCoroutine == null)
		{
			this.buttonTestCoroutine = base.StartCoroutine(this.ButtonUpdate());
		}
	}

	// Token: 0x060041AE RID: 16814 RVA: 0x0015F1B7 File Offset: 0x0015D3B7
	protected IEnumerator ButtonUpdate()
	{
		for (;;)
		{
			this.UpdateButtonFromCollider();
			if (!base.enabled || this.recentFingerCollider == null)
			{
				break;
			}
			yield return null;
		}
		this.buttonTestCoroutine = null;
		yield break;
	}

	// Token: 0x060041AF RID: 16815 RVA: 0x0015F1C8 File Offset: 0x0015D3C8
	protected void UpdateButtonFromCollider()
	{
		if (this.recentFingerCollider != null)
		{
			float surfaceDistanceFromKeyToCollider = this.GetSurfaceDistanceFromKeyToCollider(this.recentFingerCollider);
			this.currentButtonDepthFromPressing -= surfaceDistanceFromKeyToCollider;
			this.currentButtonDepthFromPressing = Mathf.Clamp(this.currentButtonDepthFromPressing, 0f, this.buttonPushDepth);
		}
		else
		{
			this.currentButtonDepthFromPressing = 0f;
		}
		if (this.currentButtonDepthFromPressing == 0f)
		{
			if (!this.canToggleOn && !this.canToggleOff)
			{
				this.isOn = false;
			}
			this.recentFingerCollider = null;
			this.waitingForReleaseAfterStateChange = false;
		}
		this.TestForButtonStateChange();
		this.UpdateButtonVisuals();
	}

	// Token: 0x060041B0 RID: 16816 RVA: 0x0015F268 File Offset: 0x0015D468
	protected void TestForButtonStateChange()
	{
		if (this.waitingForReleaseAfterStateChange)
		{
			return;
		}
		if (this.currentButtonDepthFromPressing > this.buttonDepthForTrigger && !this.isOn && this.recentFingerCollider != null)
		{
			this.isOn = true;
			this.waitingForReleaseAfterStateChange = true;
			GorillaTriggerColliderHandIndicator component = this.recentFingerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component == null)
			{
				return;
			}
			UnityEvent unityEvent = this.onPressButtonOn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			Action<GorillaPhysicalButton, bool> action = this.onPressedOn;
			if (action != null)
			{
				action(this, component.isLeftHand);
			}
			this.ButtonPressedOn();
			this.ButtonPressedOnWithHand(component.isLeftHand);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
				{
					67,
					component.isLeftHand,
					0.05f
				});
				return;
			}
		}
		else if (this.currentButtonDepthFromPressing > this.buttonDepthForTrigger && this.canToggleOff && this.isOn && this.recentFingerCollider != null)
		{
			this.isOn = false;
			this.waitingForReleaseAfterStateChange = true;
			GorillaTriggerColliderHandIndicator component2 = this.recentFingerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component2 == null)
			{
				return;
			}
			UnityEvent unityEvent2 = this.onPressButtonToggleOff;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			Action<GorillaPhysicalButton, bool> action2 = this.onToggledOff;
			if (action2 != null)
			{
				action2(this, component2.isLeftHand);
			}
			this.ButtonToggledOff();
			this.ButtonToggledOffWithHand(component2.isLeftHand);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component2.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
				{
					67,
					component2.isLeftHand,
					0.05f
				});
			}
		}
	}

	// Token: 0x060041B1 RID: 16817 RVA: 0x0015F4F8 File Offset: 0x0015D6F8
	protected void UpdateButtonVisuals()
	{
		float num = this.currentButtonDepthFromPressing;
		if ((this.canToggleOff || this.canToggleOn) && this.isOn)
		{
			num = Mathf.Max(this.buttonDepthForTrigger, num);
		}
		base.transform.position = this.startButtonPosition - base.transform.forward * num;
		if (this.moveableChildren != null)
		{
			for (int i = 0; i < this.moveableChildren.Count; i++)
			{
				this.moveableChildren[i].position = this.moveableChildrenStartPositions[i] - base.transform.forward * num;
			}
		}
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x060041B2 RID: 16818 RVA: 0x0015F5B8 File Offset: 0x0015D7B8
	protected void UpdateColorWithState(bool state)
	{
		if (state)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if ((!string.IsNullOrEmpty(this.onText) || !string.IsNullOrEmpty(this.offText)) && this.textField != null)
			{
				this.textField.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if ((!string.IsNullOrEmpty(this.offText) || !string.IsNullOrEmpty(this.onText)) && this.textField != null)
			{
				this.textField.text = this.offText;
			}
		}
	}

	// Token: 0x060041B3 RID: 16819 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonPressedOn()
	{
	}

	// Token: 0x060041B4 RID: 16820 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonPressedOnWithHand(bool isLeftHand)
	{
	}

	// Token: 0x060041B5 RID: 16821 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonToggledOff()
	{
	}

	// Token: 0x060041B6 RID: 16822 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonToggledOffWithHand(bool isLeftHand)
	{
	}

	// Token: 0x060041B7 RID: 16823 RVA: 0x0015F65D File Offset: 0x0015D85D
	public virtual void ResetState()
	{
		this.isOn = false;
		this.currentButtonDepthFromPressing = 0f;
		this.waitingForReleaseAfterStateChange = false;
		this.UpdateButtonVisuals();
	}

	// Token: 0x060041B8 RID: 16824 RVA: 0x0015F67E File Offset: 0x0015D87E
	public void SetText(string newText)
	{
		if (this.textField != null)
		{
			this.textField.text = this.offText;
		}
	}

	// Token: 0x060041B9 RID: 16825 RVA: 0x0015F6A0 File Offset: 0x0015D8A0
	public virtual void SetButtonState(bool setToOn)
	{
		if (this.canToggleOn || this.canToggleOff)
		{
			if (this.isOn != setToOn)
			{
				this.isOn = setToOn;
				if (this.isOn)
				{
					UnityEvent unityEvent = this.onPressButtonOn;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.ButtonPressedOn();
				}
				else
				{
					UnityEvent unityEvent2 = this.onPressButtonToggleOff;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.ButtonToggledOff();
				}
			}
			this.UpdateButtonVisuals();
		}
	}

	// Token: 0x0400535B RID: 21339
	public Material pressedMaterial;

	// Token: 0x0400535C RID: 21340
	public Material unpressedMaterial;

	// Token: 0x0400535D RID: 21341
	public MeshRenderer buttonRenderer;

	// Token: 0x0400535E RID: 21342
	public int pressButtonSoundIndex = 67;

	// Token: 0x0400535F RID: 21343
	[SerializeField]
	public bool canToggleOn;

	// Token: 0x04005360 RID: 21344
	public bool canToggleOff;

	// Token: 0x04005361 RID: 21345
	private bool waitingForReleaseAfterStateChange;

	// Token: 0x04005362 RID: 21346
	public bool isOn;

	// Token: 0x04005363 RID: 21347
	public bool testPress;

	// Token: 0x04005364 RID: 21348
	public bool testHandLeft;

	// Token: 0x04005365 RID: 21349
	[SerializeField]
	protected float buttonPushDepth = 0.0125f;

	// Token: 0x04005366 RID: 21350
	[SerializeField]
	protected float buttonDepthForTrigger = 0.01f;

	// Token: 0x04005367 RID: 21351
	[SerializeField]
	public List<Transform> moveableChildren;

	// Token: 0x04005368 RID: 21352
	[NonSerialized]
	public List<Vector3> moveableChildrenStartPositions;

	// Token: 0x04005369 RID: 21353
	private Vector3 startButtonPosition;

	// Token: 0x0400536A RID: 21354
	[TextArea]
	public string offText = "OFF";

	// Token: 0x0400536B RID: 21355
	[TextArea]
	public string onText = "ON";

	// Token: 0x0400536C RID: 21356
	[SerializeField]
	public TMP_Text textField;

	// Token: 0x0400536D RID: 21357
	[Space]
	public UnityEvent onPressButtonOn;

	// Token: 0x0400536E RID: 21358
	public UnityEvent onPressButtonToggleOff;

	// Token: 0x04005371 RID: 21361
	private Collider recentFingerCollider;

	// Token: 0x04005372 RID: 21362
	protected float currentButtonDepthFromPressing;

	// Token: 0x04005373 RID: 21363
	private Coroutine buttonTestCoroutine;
}
