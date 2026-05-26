using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public static class MathHelper
{
	// Token: 0x06000CB4 RID: 3252 RVA: 0x00045F11 File Offset: 0x00044111
	public static float RoundTo(this float value, float increment)
	{
		return Mathf.Floor(value / increment + 0.5f) * increment;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x00045F24 File Offset: 0x00044124
	public static Vector3 RoundTo(this Vector3 value, float increment)
	{
		value.x = Mathf.Floor(value.x / increment + 0.5f) * increment;
		value.y = Mathf.Floor(value.y / increment + 0.5f) * increment;
		value.z = Mathf.Floor(value.z / increment + 0.5f) * increment;
		return value;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00045F88 File Offset: 0x00044188
	public static Vector3 SnapToInt(this Vector3 value)
	{
		value.x = Mathf.Floor(value.x + 0.5f);
		value.y = Mathf.Floor(value.y + 0.5f);
		value.z = Mathf.Floor(value.z + 0.5f);
		return value;
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x00045FE0 File Offset: 0x000441E0
	public static Quaternion RoundTo(this Quaternion value, float increment)
	{
		Vector3 eulerAngles = value.eulerAngles;
		eulerAngles.x = Mathf.Floor(eulerAngles.x / increment + 0.5f) * increment;
		eulerAngles.y = Mathf.Floor(eulerAngles.y / increment + 0.5f) * increment;
		eulerAngles.z = Mathf.Floor(eulerAngles.z / increment + 0.5f) * increment;
		value.eulerAngles = eulerAngles;
		return value;
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00046054 File Offset: 0x00044254
	public static Quaternion SnapToCardinal(this Quaternion value)
	{
		Vector3 eulerAngles = value.eulerAngles;
		eulerAngles.x = (eulerAngles.z = 0f);
		eulerAngles.y = Mathf.Floor(eulerAngles.y / 90f + 0.5f) * 90f;
		return Quaternion.Euler(eulerAngles);
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x000460AC File Offset: 0x000442AC
	public static bool IsInBounds(this int3 a, int3 min, int3 max)
	{
		return min.x <= a.x && max.x >= a.x && min.y <= a.y && max.y >= a.y && min.z <= a.z && max.z >= a.z;
	}
}
