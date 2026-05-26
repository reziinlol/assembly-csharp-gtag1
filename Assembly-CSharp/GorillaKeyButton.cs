using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A08 RID: 2568
public abstract class GorillaKeyButton<TBinding> : MonoBehaviour where TBinding : Enum
{
	// Token: 0x06004192 RID: 16786 RVA: 0x0015EC1E File Offset: 0x0015CE1E
	private void Awake()
	{
		if (this.ButtonRenderer == null)
		{
			this.ButtonRenderer = base.GetComponent<Renderer>();
		}
		this.propBlock = new MaterialPropertyBlock();
		this.pressTime = 0f;
	}

	// Token: 0x06004193 RID: 16787 RVA: 0x0015EC50 File Offset: 0x0015CE50
	private void OnEnable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(true);
			}
		}
		this.OnEnableEvents();
	}

	// Token: 0x06004194 RID: 16788 RVA: 0x0015EC94 File Offset: 0x0015CE94
	private void OnDisable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(false);
			}
		}
		this.OnDisableEvents();
	}

	// Token: 0x06004195 RID: 16789 RVA: 0x0015ECD8 File Offset: 0x0015CED8
	private void OnTriggerEnter(Collider collider)
	{
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton(componentInParent.isLeftHand);
		}
	}

	// Token: 0x06004196 RID: 16790 RVA: 0x0015ED00 File Offset: 0x0015CF00
	private void PressButton(bool isLeftHand)
	{
		this.OnButtonPressedEvent();
		UnityEvent<TBinding> onKeyButtonPressed = this.OnKeyButtonPressed;
		if (onKeyButtonPressed != null)
		{
			onKeyButtonPressed.Invoke(this.Binding);
		}
		this.PressButtonColourUpdate();
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, isLeftHand, 0.1f);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				66,
				isLeftHand,
				0.1f
			});
		}
	}

	// Token: 0x06004197 RID: 16791 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnEnableEvents()
	{
	}

	// Token: 0x06004198 RID: 16792 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnDisableEvents()
	{
	}

	// Token: 0x06004199 RID: 16793 RVA: 0x0015EDC5 File Offset: 0x0015CFC5
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x0600419A RID: 16794 RVA: 0x0015EDD0 File Offset: 0x0015CFD0
	public virtual void PressButtonColourUpdate()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.PressedColor);
		this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.PressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|21_0());
	}

	// Token: 0x0600419B RID: 16795
	protected abstract void OnButtonPressedEvent();

	// Token: 0x0600419D RID: 16797 RVA: 0x0015EE68 File Offset: 0x0015D068
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|21_0()
	{
		yield return new WaitForSeconds(this.ButtonColorSettings.PressedTime);
		if (this.pressTime != 0f && Time.time > this.ButtonColorSettings.PressedTime + this.pressTime)
		{
			this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.UnpressedColor);
			this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.UnpressedColor);
			this.ButtonRenderer.SetPropertyBlock(this.propBlock);
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x0400534B RID: 21323
	public string characterString;

	// Token: 0x0400534C RID: 21324
	public TBinding Binding;

	// Token: 0x0400534D RID: 21325
	public bool functionKey;

	// Token: 0x0400534E RID: 21326
	public Renderer ButtonRenderer;

	// Token: 0x0400534F RID: 21327
	public ButtonColorSettings ButtonColorSettings;

	// Token: 0x04005350 RID: 21328
	[Tooltip("These GameObjects will be Activated/Deactivated when this button is Activated/Deactivated")]
	public GameObject[] linkedObjects;

	// Token: 0x04005351 RID: 21329
	[Tooltip("Intended for use with GorillaKeyWrapper")]
	public UnityEvent<TBinding> OnKeyButtonPressed = new UnityEvent<TBinding>();

	// Token: 0x04005352 RID: 21330
	public bool testClick;

	// Token: 0x04005353 RID: 21331
	public bool repeatTestClick;

	// Token: 0x04005354 RID: 21332
	public float repeatCooldown = 2f;

	// Token: 0x04005355 RID: 21333
	private float pressTime;

	// Token: 0x04005356 RID: 21334
	private float lastTestClick;

	// Token: 0x04005357 RID: 21335
	protected MaterialPropertyBlock propBlock;
}
