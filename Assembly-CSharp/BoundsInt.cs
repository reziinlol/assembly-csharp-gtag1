using System;
using UnityEngine;

// Token: 0x02000E14 RID: 3604
[Serializable]
public struct BoundsInt
{
	// Token: 0x060057C0 RID: 22464 RVA: 0x001C6D2D File Offset: 0x001C4F2D
	public BoundsInt(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x060057C1 RID: 22465 RVA: 0x001C6D40 File Offset: 0x001C4F40
	public BoundsInt(Vector3 center, Vector3 size)
	{
		Vector3 b = size * 0.5f;
		this.min = global::BoundsInt.FloatToInt(center - b);
		this.max = global::BoundsInt.FloatToInt(center + b);
	}

	// Token: 0x17000846 RID: 2118
	// (get) Token: 0x060057C2 RID: 22466 RVA: 0x001C6D7D File Offset: 0x001C4F7D
	public Vector3Int center
	{
		get
		{
			return (this.min + this.max) / 2;
		}
	}

	// Token: 0x17000847 RID: 2119
	// (get) Token: 0x060057C3 RID: 22467 RVA: 0x001C6D96 File Offset: 0x001C4F96
	public Vector3Int size
	{
		get
		{
			return this.max - this.min;
		}
	}

	// Token: 0x17000848 RID: 2120
	// (get) Token: 0x060057C4 RID: 22468 RVA: 0x001C6DA9 File Offset: 0x001C4FA9
	public Vector3 centerFloat
	{
		get
		{
			return global::BoundsInt.IntToFloat(this.center);
		}
	}

	// Token: 0x17000849 RID: 2121
	// (get) Token: 0x060057C5 RID: 22469 RVA: 0x001C6DB6 File Offset: 0x001C4FB6
	public Vector3 sizeFloat
	{
		get
		{
			return global::BoundsInt.IntToFloat(this.size);
		}
	}

	// Token: 0x060057C6 RID: 22470 RVA: 0x001C6DC3 File Offset: 0x001C4FC3
	public static Vector3Int FloatToInt(Vector3 v)
	{
		return new Vector3Int(Mathf.RoundToInt(v.x * 1000f), Mathf.RoundToInt(v.y * 1000f), Mathf.RoundToInt(v.z * 1000f));
	}

	// Token: 0x060057C7 RID: 22471 RVA: 0x001C6DFD File Offset: 0x001C4FFD
	public static Vector3 IntToFloat(Vector3Int v)
	{
		return new Vector3((float)v.x / 1000f, (float)v.y / 1000f, (float)v.z / 1000f);
	}

	// Token: 0x060057C8 RID: 22472 RVA: 0x001C6E2E File Offset: 0x001C502E
	public static global::BoundsInt FromBounds(Bounds bounds)
	{
		return new global::BoundsInt(bounds.center, bounds.size);
	}

	// Token: 0x060057C9 RID: 22473 RVA: 0x001C6E43 File Offset: 0x001C5043
	public Bounds ToBounds()
	{
		return new Bounds(this.centerFloat, this.sizeFloat);
	}

	// Token: 0x060057CA RID: 22474 RVA: 0x001C6D2D File Offset: 0x001C4F2D
	public void SetMinMax(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x060057CB RID: 22475 RVA: 0x001C6E56 File Offset: 0x001C5056
	public void SetMinMax(Vector3 min, Vector3 max)
	{
		this.min = global::BoundsInt.FloatToInt(min);
		this.max = global::BoundsInt.FloatToInt(max);
	}

	// Token: 0x060057CC RID: 22476 RVA: 0x001C6E70 File Offset: 0x001C5070
	public void Encapsulate(global::BoundsInt other)
	{
		this.min = new Vector3Int(Mathf.Min(this.min.x, other.min.x), Mathf.Min(this.min.y, other.min.y), Mathf.Min(this.min.z, other.min.z));
		this.max = new Vector3Int(Mathf.Max(this.max.x, other.max.x), Mathf.Max(this.max.y, other.max.y), Mathf.Max(this.max.z, other.max.z));
	}

	// Token: 0x060057CD RID: 22477 RVA: 0x001C6F3C File Offset: 0x001C513C
	public void Expand(float amount)
	{
		int num = Mathf.RoundToInt(amount * 1000f);
		Vector3Int b = new Vector3Int(num, num, num);
		this.min -= b;
		this.max += b;
	}

	// Token: 0x060057CE RID: 22478 RVA: 0x001C6F84 File Offset: 0x001C5184
	public bool Intersects(global::BoundsInt other)
	{
		return this.min.x < other.max.x && this.max.x > other.min.x && this.min.y < other.max.y && this.max.y > other.min.y && this.min.z < other.max.z && this.max.z > other.min.z;
	}

	// Token: 0x060057CF RID: 22479 RVA: 0x001C7030 File Offset: 0x001C5230
	public bool Contains(global::BoundsInt other)
	{
		return this.min.x <= other.min.x && this.min.y <= other.min.y && this.min.z <= other.min.z && this.max.x >= other.max.x && this.max.y >= other.max.y && this.max.z >= other.max.z;
	}

	// Token: 0x060057D0 RID: 22480 RVA: 0x001C70DC File Offset: 0x001C52DC
	public bool Contains(Vector3 point)
	{
		Vector3Int vector3Int = global::BoundsInt.FloatToInt(point);
		return vector3Int.x >= this.min.x && vector3Int.x <= this.max.x && vector3Int.y >= this.min.y && vector3Int.y <= this.max.y && vector3Int.z >= this.min.z && vector3Int.z <= this.max.z;
	}

	// Token: 0x060057D1 RID: 22481 RVA: 0x001C7170 File Offset: 0x001C5370
	public global::BoundsInt GetIntersection(global::BoundsInt other)
	{
		Vector3Int vector3Int = new Vector3Int(Mathf.Max(this.min.x, other.min.x), Mathf.Max(this.min.y, other.min.y), Mathf.Max(this.min.z, other.min.z));
		Vector3Int vector3Int2 = new Vector3Int(Mathf.Min(this.max.x, other.max.x), Mathf.Min(this.max.y, other.max.y), Mathf.Min(this.max.z, other.max.z));
		if (vector3Int.x > vector3Int2.x || vector3Int.y > vector3Int2.y || vector3Int.z > vector3Int2.z)
		{
			return new global::BoundsInt(Vector3Int.zero, Vector3Int.zero);
		}
		return new global::BoundsInt(vector3Int, vector3Int2);
	}

	// Token: 0x060057D2 RID: 22482 RVA: 0x001C727C File Offset: 0x001C547C
	public long Volume()
	{
		Vector3Int size = this.size;
		return (long)size.x * (long)size.y * (long)size.z;
	}

	// Token: 0x060057D3 RID: 22483 RVA: 0x001C72AA File Offset: 0x001C54AA
	public float VolumeFloat()
	{
		return (float)this.Volume() / 1E+09f;
	}

	// Token: 0x060057D4 RID: 22484 RVA: 0x001C72B9 File Offset: 0x001C54B9
	public static bool operator ==(global::BoundsInt a, global::BoundsInt b)
	{
		return a.min == b.min && a.max == b.max;
	}

	// Token: 0x060057D5 RID: 22485 RVA: 0x001C72E1 File Offset: 0x001C54E1
	public static bool operator !=(global::BoundsInt a, global::BoundsInt b)
	{
		return !(a == b);
	}

	// Token: 0x060057D6 RID: 22486 RVA: 0x001C72F0 File Offset: 0x001C54F0
	public override bool Equals(object obj)
	{
		if (obj is global::BoundsInt)
		{
			global::BoundsInt b = (global::BoundsInt)obj;
			return this == b;
		}
		return false;
	}

	// Token: 0x060057D7 RID: 22487 RVA: 0x001C731A File Offset: 0x001C551A
	public override int GetHashCode()
	{
		return this.min.GetHashCode() ^ this.max.GetHashCode() << 2;
	}

	// Token: 0x060057D8 RID: 22488 RVA: 0x001C7341 File Offset: 0x001C5541
	public override string ToString()
	{
		return string.Format("BoundsInt(min: {0}, max: {1})", this.min, this.max);
	}

	// Token: 0x04006892 RID: 26770
	private const int SCALE_FACTOR = 1000;

	// Token: 0x04006893 RID: 26771
	public Vector3Int min;

	// Token: 0x04006894 RID: 26772
	public Vector3Int max;
}
