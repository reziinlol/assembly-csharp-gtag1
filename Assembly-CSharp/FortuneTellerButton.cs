using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000698 RID: 1688
public class FortuneTellerButton : GorillaPressableButton
{
	// Token: 0x06002A0B RID: 10763 RVA: 0x000E2D61 File Offset: 0x000E0F61
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x000E2D74 File Offset: 0x000E0F74
	public override void ButtonActivation()
	{
		this.PressButtonUpdate();
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x000E2D7C File Offset: 0x000E0F7C
	public void PressButtonUpdate()
	{
		if (this.pressTime != 0f)
		{
			return;
		}
		base.transform.localPosition = this.startingPos + this.pressedOffset;
		this.buttonRenderer.material = this.pressedMaterial;
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonUpdate>g__ButtonColorUpdate_Local|6_0());
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x000E2E09 File Offset: 0x000E1009
	[CompilerGenerated]
	private IEnumerator <PressButtonUpdate>g__ButtonColorUpdate_Local|6_0()
	{
		yield return new WaitForSeconds(this.durationPressed);
		if (this.pressTime != 0f && Time.time > this.durationPressed + this.pressTime)
		{
			base.transform.localPosition = this.startingPos;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x040036CA RID: 14026
	[SerializeField]
	private float durationPressed = 0.25f;

	// Token: 0x040036CB RID: 14027
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x040036CC RID: 14028
	private float pressTime;

	// Token: 0x040036CD RID: 14029
	private Vector3 startingPos;
}
