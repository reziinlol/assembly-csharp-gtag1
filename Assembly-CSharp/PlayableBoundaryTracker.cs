using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class PlayableBoundaryTracker : MonoBehaviour
{
	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060010DF RID: 4319 RVA: 0x0005A813 File Offset: 0x00058A13
	// (set) Token: 0x060010E0 RID: 4320 RVA: 0x0005A81B File Offset: 0x00058A1B
	public float signedDistanceToBoundary { get; private set; }

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060010E1 RID: 4321 RVA: 0x0005A824 File Offset: 0x00058A24
	// (set) Token: 0x060010E2 RID: 4322 RVA: 0x0005A82C File Offset: 0x00058A2C
	public float prevSignedDistanceToBoundary { get; private set; }

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x060010E3 RID: 4323 RVA: 0x0005A835 File Offset: 0x00058A35
	// (set) Token: 0x060010E4 RID: 4324 RVA: 0x0005A83D File Offset: 0x00058A3D
	public float timeSinceCrossingBorder { get; private set; }

	// Token: 0x060010E5 RID: 4325 RVA: 0x0005A846 File Offset: 0x00058A46
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInsideZone()
	{
		return Mathf.Sign(this.signedDistanceToBoundary) < 0f;
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x0005A85C File Offset: 0x00058A5C
	public void UpdateSignedDistanceToBoundary(float newDistance, float elapsed)
	{
		this.prevSignedDistanceToBoundary = this.signedDistanceToBoundary;
		this.signedDistanceToBoundary = newDistance;
		if ((int)Mathf.Sign(this.prevSignedDistanceToBoundary) != (int)Mathf.Sign(this.signedDistanceToBoundary))
		{
			this.timeSinceCrossingBorder = 0f;
			return;
		}
		this.timeSinceCrossingBorder += elapsed;
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x0005A8B0 File Offset: 0x00058AB0
	internal void ResetValues()
	{
		this.timeSinceCrossingBorder = 0f;
	}

	// Token: 0x0400141F RID: 5151
	public float radius = 1f;
}
