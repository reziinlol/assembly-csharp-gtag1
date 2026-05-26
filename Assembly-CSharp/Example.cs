using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class Example : MonoBehaviour
{
	// Token: 0x060003D9 RID: 985 RVA: 0x00016FBC File Offset: 0x000151BC
	private void OnDrawGizmos()
	{
		if (this.debugPoint)
		{
			DebugExtension.DrawPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale);
		}
		if (this.debugBounds)
		{
			DebugExtension.DrawBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color);
		}
		if (this.debugCircle)
		{
			DebugExtension.DrawCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius);
		}
		if (this.debugWireSphere)
		{
			Gizmos.color = this.debugWireSphere_Color;
			Gizmos.DrawWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Radius);
		}
		if (this.debugCylinder)
		{
			DebugExtension.DrawCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius);
		}
		if (this.debugCone)
		{
			DebugExtension.DrawCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle);
		}
		if (this.debugArrow)
		{
			DebugExtension.DrawArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color);
		}
		if (this.debugCapsule)
		{
			DebugExtension.DrawCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius);
		}
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00017148 File Offset: 0x00015348
	private void Update()
	{
		DebugExtension.DebugPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale, 0f, true);
		DebugExtension.DebugBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color, 0f, true);
		DebugExtension.DebugCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius, 0f, true);
		DebugExtension.DebugWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Color, this.debugWireSphere_Radius, 0f, true);
		DebugExtension.DebugCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius, 0f, true);
		DebugExtension.DebugCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle, 0f, true);
		DebugExtension.DebugArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color, 0f, true);
		DebugExtension.DebugCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius, 0f, true);
	}

	// Token: 0x0400042C RID: 1068
	public bool debugPoint;

	// Token: 0x0400042D RID: 1069
	public Vector3 debugPoint_Position;

	// Token: 0x0400042E RID: 1070
	public float debugPoint_Scale;

	// Token: 0x0400042F RID: 1071
	public Color debugPoint_Color;

	// Token: 0x04000430 RID: 1072
	public bool debugBounds;

	// Token: 0x04000431 RID: 1073
	public Vector3 debugBounds_Position;

	// Token: 0x04000432 RID: 1074
	public Vector3 debugBounds_Size;

	// Token: 0x04000433 RID: 1075
	public Color debugBounds_Color;

	// Token: 0x04000434 RID: 1076
	public bool debugCircle;

	// Token: 0x04000435 RID: 1077
	public Vector3 debugCircle_Up;

	// Token: 0x04000436 RID: 1078
	public float debugCircle_Radius;

	// Token: 0x04000437 RID: 1079
	public Color debugCircle_Color;

	// Token: 0x04000438 RID: 1080
	public bool debugWireSphere;

	// Token: 0x04000439 RID: 1081
	public float debugWireSphere_Radius;

	// Token: 0x0400043A RID: 1082
	public Color debugWireSphere_Color;

	// Token: 0x0400043B RID: 1083
	public bool debugCylinder;

	// Token: 0x0400043C RID: 1084
	public Vector3 debugCylinder_End;

	// Token: 0x0400043D RID: 1085
	public float debugCylinder_Radius;

	// Token: 0x0400043E RID: 1086
	public Color debugCylinder_Color;

	// Token: 0x0400043F RID: 1087
	public bool debugCone;

	// Token: 0x04000440 RID: 1088
	public Vector3 debugCone_Direction;

	// Token: 0x04000441 RID: 1089
	public float debugCone_Angle;

	// Token: 0x04000442 RID: 1090
	public Color debugCone_Color;

	// Token: 0x04000443 RID: 1091
	public bool debugArrow;

	// Token: 0x04000444 RID: 1092
	public Vector3 debugArrow_Direction;

	// Token: 0x04000445 RID: 1093
	public Color debugArrow_Color;

	// Token: 0x04000446 RID: 1094
	public bool debugCapsule;

	// Token: 0x04000447 RID: 1095
	public Vector3 debugCapsule_End;

	// Token: 0x04000448 RID: 1096
	public float debugCapsule_Radius;

	// Token: 0x04000449 RID: 1097
	public Color debugCapsule_Color;
}
