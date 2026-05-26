using System;
using UnityEngine;

// Token: 0x0200041F RID: 1055
public class ArcadeMachineButton : GorillaPressableButton
{
	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06001921 RID: 6433 RVA: 0x0008DA50 File Offset: 0x0008BC50
	// (remove) Token: 0x06001922 RID: 6434 RVA: 0x0008DA88 File Offset: 0x0008BC88
	public event ArcadeMachineButton.ArcadeMachineButtonEvent OnStateChange;

	// Token: 0x06001923 RID: 6435 RVA: 0x0008DABD File Offset: 0x0008BCBD
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.state)
		{
			this.state = true;
			if (this.OnStateChange != null)
			{
				this.OnStateChange(this.ButtonID, this.state);
			}
		}
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x0008DAF4 File Offset: 0x0008BCF4
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled || !this.state)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.state = false;
		if (this.OnStateChange != null)
		{
			this.OnStateChange(this.ButtonID, this.state);
		}
	}

	// Token: 0x0400242F RID: 9263
	private bool state;

	// Token: 0x04002430 RID: 9264
	[SerializeField]
	private int ButtonID;

	// Token: 0x02000420 RID: 1056
	// (Invoke) Token: 0x06001927 RID: 6439
	public delegate void ArcadeMachineButtonEvent(int id, bool state);
}
