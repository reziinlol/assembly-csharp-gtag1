using System;
using UnityEngine;

// Token: 0x020005E1 RID: 1505
public class AnimatorReset : MonoBehaviour
{
	// Token: 0x06002570 RID: 9584 RVA: 0x000C6767 File Offset: 0x000C4967
	public void Reset()
	{
		if (!this.target)
		{
			return;
		}
		this.target.Rebind();
		this.target.Update(0f);
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000C6792 File Offset: 0x000C4992
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Reset();
		}
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000C67A2 File Offset: 0x000C49A2
	private void OnDisable()
	{
		if (this.onDisable)
		{
			this.Reset();
		}
	}

	// Token: 0x040030EC RID: 12524
	public Animator target;

	// Token: 0x040030ED RID: 12525
	public bool onEnable;

	// Token: 0x040030EE RID: 12526
	public bool onDisable = true;
}
