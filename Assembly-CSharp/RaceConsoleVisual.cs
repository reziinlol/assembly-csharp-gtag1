using System;
using UnityEngine;

// Token: 0x02000366 RID: 870
public class RaceConsoleVisual : MonoBehaviour
{
	// Token: 0x0600153E RID: 5438 RVA: 0x00070980 File Offset: 0x0006EB80
	public void ShowRaceInProgress(int laps)
	{
		this.button1.sharedMaterial = this.inactiveButton;
		this.button3.sharedMaterial = this.inactiveButton;
		this.button5.sharedMaterial = this.inactiveButton;
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		switch (laps)
		{
		default:
			this.button1.sharedMaterial = this.selectedButton;
			this.button1.transform.localPosition = this.buttonPressedOffset;
			return;
		case 3:
			this.button3.sharedMaterial = this.selectedButton;
			this.button3.transform.localPosition = this.buttonPressedOffset;
			return;
		case 5:
			this.button5.sharedMaterial = this.selectedButton;
			this.button5.transform.localPosition = this.buttonPressedOffset;
			return;
		}
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x00070A94 File Offset: 0x0006EC94
	public void ShowCanStartRace()
	{
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		this.button1.sharedMaterial = this.pressableButton;
		this.button3.sharedMaterial = this.pressableButton;
		this.button5.sharedMaterial = this.pressableButton;
	}

	// Token: 0x04001A10 RID: 6672
	[SerializeField]
	private MeshRenderer button1;

	// Token: 0x04001A11 RID: 6673
	[SerializeField]
	private MeshRenderer button3;

	// Token: 0x04001A12 RID: 6674
	[SerializeField]
	private MeshRenderer button5;

	// Token: 0x04001A13 RID: 6675
	[SerializeField]
	private Vector3 buttonPressedOffset;

	// Token: 0x04001A14 RID: 6676
	[SerializeField]
	private Material pressableButton;

	// Token: 0x04001A15 RID: 6677
	[SerializeField]
	private Material selectedButton;

	// Token: 0x04001A16 RID: 6678
	[SerializeField]
	private Material inactiveButton;
}
