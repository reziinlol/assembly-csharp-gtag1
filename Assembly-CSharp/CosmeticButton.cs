using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200056C RID: 1388
public class CosmeticButton : GorillaPressableButton
{
	// Token: 0x170003BB RID: 955
	// (get) Token: 0x06002350 RID: 9040 RVA: 0x000BE05B File Offset: 0x000BC25B
	// (set) Token: 0x06002351 RID: 9041 RVA: 0x000BE063 File Offset: 0x000BC263
	public bool Initialized { get; private set; }

	// Token: 0x06002352 RID: 9042 RVA: 0x000BE06C File Offset: 0x000BC26C
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
		this.Initialized = true;
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000BE088 File Offset: 0x000BC288
	public override void UpdateColor()
	{
		if (!base.enabled)
		{
			this.buttonRenderer.material = this.disabledMaterial;
			this.SetOffText(this.myText != null, false, false);
		}
		else if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			this.SetOnText(this.myText.IsNotNull(), false, false);
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.SetOffText(this.myText != null, false, false);
		}
		this.UpdatePosition();
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000BE120 File Offset: 0x000BC320
	public virtual void UpdatePosition()
	{
		Vector3 vector = this.startingPos;
		if (!base.enabled)
		{
			vector += this.disabledOffset;
		}
		else if (this.isOn)
		{
			vector += this.pressedOffset;
		}
		this.posOffset = base.transform.position;
		base.transform.localPosition = vector;
		this.posOffset = base.transform.position - this.posOffset;
		if (this.myText != null)
		{
			this.myText.transform.position += this.posOffset;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.transform.position += this.posOffset;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.transform.position += this.posOffset;
		}
	}

	// Token: 0x04002E66 RID: 11878
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04002E67 RID: 11879
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x04002E68 RID: 11880
	[SerializeField]
	private Vector3 disabledOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04002E69 RID: 11881
	private Vector3 startingPos;

	// Token: 0x04002E6A RID: 11882
	protected Vector3 posOffset;
}
