using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000AF7 RID: 2807
[ExecuteAlways]
public class Xform : MonoBehaviour
{
	// Token: 0x170006AF RID: 1711
	// (get) Token: 0x060047E2 RID: 18402 RVA: 0x00181A53 File Offset: 0x0017FC53
	public float3 localExtents
	{
		get
		{
			return this.localScale * 0.5f;
		}
	}

	// Token: 0x060047E3 RID: 18403 RVA: 0x00181A65 File Offset: 0x0017FC65
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, this.localRotation, this.localScale);
	}

	// Token: 0x060047E4 RID: 18404 RVA: 0x00181A88 File Offset: 0x0017FC88
	public Matrix4x4 TRS()
	{
		if (this.parent.AsNull<Transform>() == null)
		{
			return this.LocalTRS();
		}
		return this.parent.localToWorldMatrix * this.LocalTRS();
	}

	// Token: 0x060047E5 RID: 18405 RVA: 0x00181ABC File Offset: 0x0017FCBC
	private unsafe void Update()
	{
		Matrix4x4 matrix = this.TRS();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(matrix))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.PlaneWithNormal(Xform.AXIS_XR_RT * 0.5f, Xform.AXIS_XR_RT, Xform.F2_ONE, Xform.CR);
				commandBuilder.PlaneWithNormal(Xform.AXIS_YG_UP * 0.5f, Xform.AXIS_YG_UP, Xform.F2_ONE, Xform.CG);
				commandBuilder.PlaneWithNormal(Xform.AXIS_ZB_FW * 0.5f, Xform.AXIS_ZB_FW, Xform.F2_ONE, Xform.CB);
				commandBuilder.WireBox(float3.zero, quaternion.identity, 1f, this.displayColor);
			}
		}
	}

	// Token: 0x040059FC RID: 23036
	public Transform parent;

	// Token: 0x040059FD RID: 23037
	[Space]
	public Color displayColor = SRand.New().NextColor();

	// Token: 0x040059FE RID: 23038
	[Space]
	public float3 localPosition = float3.zero;

	// Token: 0x040059FF RID: 23039
	public float3 localScale = Vector3.one;

	// Token: 0x04005A00 RID: 23040
	public Quaternion localRotation = quaternion.identity;

	// Token: 0x04005A01 RID: 23041
	private static readonly float3 F3_ONE = 1f;

	// Token: 0x04005A02 RID: 23042
	private static readonly float2 F2_ONE = 1f;

	// Token: 0x04005A03 RID: 23043
	private static readonly float3 AXIS_ZB_FW = new float3(0f, 0f, 1f);

	// Token: 0x04005A04 RID: 23044
	private static readonly float3 AXIS_YG_UP = new float3(0f, 1f, 0f);

	// Token: 0x04005A05 RID: 23045
	private static readonly float3 AXIS_XR_RT = new float3(1f, 0f, 0f);

	// Token: 0x04005A06 RID: 23046
	private static readonly Color CR = new Color(1f, 0f, 0f, 0.24f);

	// Token: 0x04005A07 RID: 23047
	private static readonly Color CG = new Color(0f, 1f, 0f, 0.24f);

	// Token: 0x04005A08 RID: 23048
	private static readonly Color CB = new Color(0f, 0f, 1f, 0.24f);
}
