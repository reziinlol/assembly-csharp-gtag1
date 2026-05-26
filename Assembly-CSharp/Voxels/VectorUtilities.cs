using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012ED RID: 4845
	public static class VectorUtilities
	{
		// Token: 0x060078B6 RID: 30902 RVA: 0x0027B6DA File Offset: 0x002798DA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int ToVectorInt(this int3 v)
		{
			return new Vector3Int(v.x, v.y, v.z);
		}

		// Token: 0x060078B7 RID: 30903 RVA: 0x0027B6F3 File Offset: 0x002798F3
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ToInt3(this Vector3Int v)
		{
			return new int3(v.x, v.y, v.z);
		}

		// Token: 0x060078B8 RID: 30904 RVA: 0x0027B70F File Offset: 0x0027990F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ToInt3(this Vector3 v)
		{
			return new int3((int)v.x, (int)v.y, (int)v.z);
		}

		// Token: 0x060078B9 RID: 30905 RVA: 0x0027B72B File Offset: 0x0027992B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ToInt3(this float3 v)
		{
			return new int3((int)v.x, (int)v.y, (int)v.z);
		}

		// Token: 0x060078BA RID: 30906 RVA: 0x0027B747 File Offset: 0x00279947
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 RoundToInt(this Vector3 v)
		{
			return (int3)math.round(v);
		}

		// Token: 0x060078BB RID: 30907 RVA: 0x0027B759 File Offset: 0x00279959
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 RoundToInt(this float3 v)
		{
			return (int3)math.round(v);
		}

		// Token: 0x060078BC RID: 30908 RVA: 0x0027B766 File Offset: 0x00279966
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 CeilToInt(this float3 v)
		{
			return new int3(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
		}

		// Token: 0x060078BD RID: 30909 RVA: 0x0027B78E File Offset: 0x0027998E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Floor(this Vector3 v)
		{
			return new Vector3(math.floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		}

		// Token: 0x060078BE RID: 30910 RVA: 0x0027B7B6 File Offset: 0x002799B6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 Floor(this float3 v)
		{
			return new float3(math.floor(v.x), math.floor(v.y), math.floor(v.z));
		}

		// Token: 0x060078BF RID: 30911 RVA: 0x0027B7DE File Offset: 0x002799DE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ToVector3(this int3 v)
		{
			return new Vector3((float)v.x, (float)v.y, (float)v.z);
		}

		// Token: 0x060078C0 RID: 30912 RVA: 0x0027B7FA File Offset: 0x002799FA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 ToFloat3(this int3 v)
		{
			return new float3((float)v.x, (float)v.y, (float)v.z);
		}

		// Token: 0x060078C1 RID: 30913 RVA: 0x0027B818 File Offset: 0x00279A18
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 FloorToMultipleOfX(this Vector3 v, int3 x)
		{
			return (int3)(math.floor(new float3(v.x / (float)x.x, v.y / (float)x.y, v.z / (float)x.z)) * x);
		}

		// Token: 0x060078C2 RID: 30914 RVA: 0x0027B86C File Offset: 0x00279A6C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 FloorToMultipleOfX(this Vector3Int v, int3 x)
		{
			return (int3)(math.floor(new float3((float)v.x / (float)x.x, (float)v.y / (float)x.y, (float)v.z / (float)x.z)) * x);
		}

		// Token: 0x060078C3 RID: 30915 RVA: 0x0027B8C4 File Offset: 0x00279AC4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 FloorToMultipleOfX(this int3 v, int3 x)
		{
			return (int3)(math.floor(new float3((float)v.x / (float)x.x, (float)v.y / (float)x.y, (float)v.z / (float)x.z)) * x);
		}

		// Token: 0x060078C4 RID: 30916 RVA: 0x0027B918 File Offset: 0x00279B18
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 LocalPositionToChunkId(this Vector3 localWorldPosition, int3 chunkSize)
		{
			return localWorldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
		}

		// Token: 0x060078C5 RID: 30917 RVA: 0x0027B927 File Offset: 0x00279B27
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 LocalPositionToChunkId(this Vector3Int localWorldPosition, int3 chunkSize)
		{
			return localWorldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
		}

		// Token: 0x060078C6 RID: 30918 RVA: 0x0027B936 File Offset: 0x00279B36
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 LocalPositionToChunkId(this int3 localWorldPosition, int3 chunkSize)
		{
			return localWorldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
		}

		// Token: 0x060078C7 RID: 30919 RVA: 0x0027B945 File Offset: 0x00279B45
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ToByte(this float value)
		{
			return (byte)math.clamp(value * 127f + 128f, 0f, 255f);
		}

		// Token: 0x060078C8 RID: 30920 RVA: 0x0027B964 File Offset: 0x00279B64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToFloat(this byte value)
		{
			return (float)(value - 128) / 127f;
		}

		// Token: 0x060078C9 RID: 30921 RVA: 0x0027B974 File Offset: 0x00279B74
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSolid(this byte density)
		{
			return density > 127;
		}

		// Token: 0x060078CA RID: 30922 RVA: 0x0027B97C File Offset: 0x00279B7C
		public static int3[] GetCardinalNeighbours(this int3 center)
		{
			return new int3[]
			{
				center + new int3(1, 0, 0),
				center + new int3(-1, 0, 0),
				center + new int3(0, 1, 0),
				center + new int3(0, -1, 0),
				center + new int3(0, 0, 1),
				center + new int3(0, 0, -1)
			};
		}

		// Token: 0x060078CB RID: 30923 RVA: 0x0027BA10 File Offset: 0x00279C10
		public static int3 GetClosestCardinalNeighbour(this int3 center, Vector3 target)
		{
			int3[] cardinalNeighbours = center.GetCardinalNeighbours();
			int num = 0;
			float num2 = math.distance(cardinalNeighbours[0], target);
			for (int i = 1; i < cardinalNeighbours.Length; i++)
			{
				float num3 = math.distance(cardinalNeighbours[i], target);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			return cardinalNeighbours[num];
		}

		// Token: 0x060078CC RID: 30924 RVA: 0x0027BA76 File Offset: 0x00279C76
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Min(Vector3Int v1, Vector3Int v2)
		{
			return new Vector3Int(math.min(v1.x, v2.x), math.min(v1.y, v2.y), math.min(v1.z, v2.z));
		}

		// Token: 0x060078CD RID: 30925 RVA: 0x0027BAB6 File Offset: 0x00279CB6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Max(Vector3Int v1, Vector3Int v2)
		{
			return new Vector3Int(math.max(v1.x, v2.x), math.max(v1.y, v2.y), math.max(v1.z, v2.z));
		}
	}
}
