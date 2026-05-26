using System;
using UnityEngine;

// Token: 0x02000425 RID: 1061
[Serializable]
public class NativeSizeChangerSettings
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x0600193C RID: 6460 RVA: 0x0008DCDA File Offset: 0x0008BEDA
	// (set) Token: 0x0600193D RID: 6461 RVA: 0x0008DCE2 File Offset: 0x0008BEE2
	public Vector3 WorldPosition
	{
		get
		{
			return this.worldPosition;
		}
		set
		{
			this.worldPosition = value;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x0600193E RID: 6462 RVA: 0x0008DCEB File Offset: 0x0008BEEB
	// (set) Token: 0x0600193F RID: 6463 RVA: 0x0008DCF3 File Offset: 0x0008BEF3
	public float ActivationTime
	{
		get
		{
			return this.activationTime;
		}
		set
		{
			this.activationTime = value;
		}
	}

	// Token: 0x04002436 RID: 9270
	public const float MinAllowedSize = 0.1f;

	// Token: 0x04002437 RID: 9271
	public const float MaxAllowedSize = 10f;

	// Token: 0x04002438 RID: 9272
	private Vector3 worldPosition;

	// Token: 0x04002439 RID: 9273
	private float activationTime;

	// Token: 0x0400243A RID: 9274
	[Range(0.1f, 10f)]
	public float playerSizeScale = 1f;

	// Token: 0x0400243B RID: 9275
	public bool ExpireOnRoomJoin = true;

	// Token: 0x0400243C RID: 9276
	public bool ExpireInWater = true;

	// Token: 0x0400243D RID: 9277
	public float ExpireAfterSeconds;

	// Token: 0x0400243E RID: 9278
	public float ExpireOnDistance;
}
