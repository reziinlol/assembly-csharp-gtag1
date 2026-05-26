using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class SIAutoPressButtonOnAwake : MonoBehaviour
{
	// Token: 0x060007C0 RID: 1984 RVA: 0x0002A692 File Offset: 0x00028892
	private void Awake()
	{
		this.button = base.GetComponent<SITouchscreenButton>();
		this.terminalParent = this.button.GetComponentInParent<SICombinedTerminal>();
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0002A6B1 File Offset: 0x000288B1
	private void OnEnable()
	{
		if (this.button == null)
		{
			return;
		}
		this.awakeTime = Time.time;
		this.buttonPressed = false;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0002A6D4 File Offset: 0x000288D4
	private void Update()
	{
		if (this.buttonPressed || Time.time < this.awakeTime + this.delay)
		{
			return;
		}
		if (this.terminalParent.activePlayer.ActorNr == SIPlayer.LocalPlayer.ActorNr)
		{
			this.button.PressButton();
		}
		this.buttonPressed = true;
	}

	// Token: 0x040009CE RID: 2510
	private SICombinedTerminal terminalParent;

	// Token: 0x040009CF RID: 2511
	private SITouchscreenButton button;

	// Token: 0x040009D0 RID: 2512
	private float awakeTime;

	// Token: 0x040009D1 RID: 2513
	private bool buttonPressed;

	// Token: 0x040009D2 RID: 2514
	public float delay = 2f;
}
