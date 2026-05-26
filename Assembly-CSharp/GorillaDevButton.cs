using System;
using UnityEngine;

// Token: 0x02000A04 RID: 2564
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x06004183 RID: 16771 RVA: 0x0015E7A5 File Offset: 0x0015C9A5
	// (set) Token: 0x06004184 RID: 16772 RVA: 0x0015E7AD File Offset: 0x0015C9AD
	public bool on
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				this.isOn = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x06004185 RID: 16773 RVA: 0x0015E7C5 File Offset: 0x0015C9C5
	public new void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x0400532F RID: 21295
	public DevButtonType Type;

	// Token: 0x04005330 RID: 21296
	public LogType levelType;

	// Token: 0x04005331 RID: 21297
	public DevConsoleInstance targetConsole;

	// Token: 0x04005332 RID: 21298
	public int lineNumber;

	// Token: 0x04005333 RID: 21299
	public bool repeatIfHeld;

	// Token: 0x04005334 RID: 21300
	public float holdForSeconds;

	// Token: 0x04005335 RID: 21301
	private Coroutine pressCoroutine;
}
