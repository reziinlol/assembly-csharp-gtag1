using System;
using UnityEngine.Events;

// Token: 0x02000A1D RID: 2589
public class GorillaToggleActionButton : GorillaPressableButton
{
	// Token: 0x0600423E RID: 16958 RVA: 0x00162087 File Offset: 0x00160287
	public override void Start()
	{
		this.BindToggleAction();
	}

	// Token: 0x0600423F RID: 16959 RVA: 0x00162090 File Offset: 0x00160290
	private void BindToggleAction()
	{
		if (this.ToggleAction == null || !this.ToggleAction.IsValid)
		{
			return;
		}
		this.ToggleAction.Cache();
		this.onPressButton = new UnityEvent();
		this.onPressButton.AddListener(new UnityAction(this.ExecuteToggleAction));
	}

	// Token: 0x06004240 RID: 16960 RVA: 0x001620E0 File Offset: 0x001602E0
	private void ExecuteToggleAction()
	{
		ComponentFunctionReference<bool> toggleAction = this.ToggleAction;
		this.isOn = (toggleAction != null && toggleAction.Invoke());
		this.UpdateColor();
	}

	// Token: 0x0400541B RID: 21531
	public ComponentFunctionReference<bool> ToggleAction;

	// Token: 0x0400541C RID: 21532
	private Func<bool> toggleFunc;
}
