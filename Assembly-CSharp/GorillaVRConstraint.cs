using System;
using UnityEngine;

// Token: 0x020008CC RID: 2252
public class GorillaVRConstraint : MonoBehaviourTick
{
	// Token: 0x06003ADE RID: 15070 RVA: 0x001433FC File Offset: 0x001415FC
	public override void Tick()
	{
		if (NetworkSystem.Instance.WrongVersion)
		{
			this.isConstrained = true;
		}
		if (this.isConstrained && Time.realtimeSinceStartup > this.angle)
		{
			GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
		}
	}

	// Token: 0x04004B4F RID: 19279
	public bool isConstrained;

	// Token: 0x04004B50 RID: 19280
	public float angle = 3600f;
}
