using System;
using System.Runtime.CompilerServices;
using PlayFab.Internal;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

// Token: 0x020001EE RID: 494
public static class VoxelExtensions
{
	// Token: 0x06000CEE RID: 3310 RVA: 0x00046BBA File Offset: 0x00044DBA
	public static void Mine(this VoxelWorld world, Collision collision, VoxelAction action)
	{
		world.Mine(collision.ToRaycastHit(), action);
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00046BCC File Offset: 0x00044DCC
	public static void Mine(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		MeshGenerationMode meshGenerationMode = world.MeshGenerationMode;
		if (meshGenerationMode == MeshGenerationMode.MarchingCubes)
		{
			world.Mine_MarchingCubes(hit, action);
			return;
		}
		if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.Mine_SurfaceNets(hit, action);
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00046C00 File Offset: 0x00044E00
	private static void Mine_MarchingCubes(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 vector = hit.point;
		VoxelExtensions._lastHitPoint = vector;
		int triangleIndex = hit.triangleIndex;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.red, 20f);
		}
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider != null)
		{
			Mesh sharedMesh = meshCollider.sharedMesh;
			if (sharedMesh == null || triangleIndex < 0 || triangleIndex >= sharedMesh.triangles.Length / 3)
			{
				Debug.LogWarning(string.Format("Invalid triangle index {0} for mesh {1}", triangleIndex, (sharedMesh != null) ? sharedMesh.name : null));
				return;
			}
			Vector3 a = meshCollider.transform.InverseTransformPoint(vector);
			int[] triangles = sharedMesh.triangles;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3 vector2 = vertices[triangles[triangleIndex * 3]];
			Vector3 vector3 = vertices[triangles[triangleIndex * 3 + 1]];
			Vector3 vector4 = vertices[triangles[triangleIndex * 3 + 2]];
			Vector3 vector5 = ((a - vector2).sqrMagnitude < (a - vector3).sqrMagnitude) ? (((a - vector2).sqrMagnitude < (a - vector4).sqrMagnitude) ? vector2 : vector4) : (((a - vector3).sqrMagnitude < (a - vector4).sqrMagnitude) ? vector3 : vector4);
			vector5 = (VoxelExtensions._lastVertex = meshCollider.transform.TransformPoint(vector5));
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Closest vertex to {0}: {1}", vector, vector5));
			}
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(vector, vector5, Color.blue, 20f);
			}
			vector5 = world.GetLocalPosition(vector5);
			vector = vector5.SnapToInt();
			if (VoxelExtensions._cascade && !world.GetDensityAt(vector).IsSolid())
			{
				if (VoxelExtensions._showDebug)
				{
					Debug.Log(string.Format("Hit air at {0}, moving to next voxel", vector));
				}
				if (VoxelExtensions._showDebug)
				{
					Debug.DrawLine(vector, vector + (vector5 - vector).normalized, Color.cyan, 20f);
				}
				vector += (vector5 - vector).normalized;
			}
		}
		VoxelExtensions._lastGridPoint = vector;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.white, 20f);
		}
		int3 v = vector.ToInt3();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		VoxelManager.Mine(world, VoxelExtensions._lastBounds, hit.point, hit.normal, vector, action);
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00046F10 File Offset: 0x00045110
	private static void Mine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 vector = hit.point;
		VoxelExtensions._lastHitPoint = vector;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(VoxelExtensions.GetTriangleCenter(hit));
		vector = localPosition.SnapToInt();
		if (VoxelExtensions._cascade && world.GetDensityAt(vector) == 0)
		{
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Hit air at {0}, moving to next voxel", vector));
			}
			int3 closestCardinalNeighbour = vector.ToInt3().GetClosestCardinalNeighbour(localPosition - hit.normal * 0.5f);
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(vector, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			vector = closestCardinalNeighbour.ToFloat3();
		}
		VoxelExtensions._lastGridPoint = vector;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.white, 20f);
		}
		int3 v = vector.ToInt3();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		VoxelManager.Mine(world, VoxelExtensions._lastBounds, hit.point, hit.normal, vector, action);
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x000470A8 File Offset: 0x000452A8
	[return: TupleElementNames(new string[]
	{
		"dirt",
		"stone"
	})]
	public static ValueTuple<int, int> PerformLocalMiningOperation(this VoxelWorld world, UnityEngine.BoundsInt bounds, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action)
	{
		VoxelExtensions._lastBounds = bounds;
		VoxelExtensions._opAction = action;
		VoxelExtensions._opOrigin = origin;
		VoxelExtensions._opTotalMined = 0;
		VoxelExtensions._opDirtMined = 0;
		VoxelExtensions._opStoneMined = 0;
		OperationType operation = VoxelExtensions._opAction.operation;
		if (operation != OperationType.Subtract)
		{
			if (operation != OperationType.Add)
			{
				throw new ArgumentOutOfRangeException();
			}
			world.SetVoxelDataCustom(bounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.UnMineAt), true);
		}
		else
		{
			world.SetVoxelDataCustom(bounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.MineAt), true);
			if (VoxelExtensions._opTotalMined > 0)
			{
				SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hitPoint, hitNormal, VoxelExtensions._opDirtMined, VoxelExtensions._opStoneMined);
			}
		}
		return new ValueTuple<int, int>(VoxelExtensions._opDirtMined, VoxelExtensions._opStoneMined);
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00047150 File Offset: 0x00045350
	public static void PerformLocalOperation(this VoxelWorld world, Vector3 localPosition, VoxelAction action)
	{
		UnityEngine.BoundsInt bounds = world.GetBounds(localPosition, action.radius);
		VoxelExtensions._opAction = action;
		VoxelExtensions._opOrigin = localPosition;
		OperationType operation = VoxelExtensions._opAction.operation;
		if (operation == OperationType.Subtract)
		{
			world.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(VoxelExtensions.SubtractAt), true);
			return;
		}
		if (operation != OperationType.Add)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(VoxelExtensions.AddAt), true);
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x000471C0 File Offset: 0x000453C0
	[return: TupleElementNames(new string[]
	{
		"density",
		"material"
	})]
	private static ValueTuple<byte, byte> MineAt(int3 point, [TupleElementNames(new string[]
	{
		"density",
		"material"
	})] ValueTuple<byte, byte> data)
	{
		byte item = data.Item1;
		byte item2 = data.Item2;
		float num = (item2 == 0) ? VoxelExtensions._opAction.strength : (VoxelExtensions._opAction.strength * 0.2f);
		float num2 = math.distance(VoxelExtensions._opOrigin, point);
		byte b = (num2 > VoxelExtensions._opAction.radius) ? item : ((byte)math.clamp((float)item - num * math.lerp(255f, 0f, num2 / VoxelExtensions._opAction.radius), 0f, 255f));
		if (VoxelExtensions._showDebug && item != b)
		{
			Debug.Log(string.Format("Hit at {0}->{1}=d{2:F2} with density {3}[{4}] -> {5}[{6}]", new object[]
			{
				VoxelExtensions._opOrigin,
				point,
				num2,
				item,
				item.ToFloat(),
				b,
				b.ToFloat()
			}));
		}
		if (item.IsSolid())
		{
			int num3 = (int)((float)(item - b) / 10f);
			VoxelExtensions._opTotalMined += num3;
			if (item2 == 0)
			{
				VoxelExtensions._opDirtMined += num3;
			}
			else if (item2 == 1)
			{
				VoxelExtensions._opStoneMined += num3;
			}
		}
		return new ValueTuple<byte, byte>(b, item2);
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00047314 File Offset: 0x00045514
	[return: TupleElementNames(new string[]
	{
		"density",
		"material"
	})]
	private static ValueTuple<byte, byte> UnMineAt(int3 point, [TupleElementNames(new string[]
	{
		"density",
		"material"
	})] ValueTuple<byte, byte> data)
	{
		byte item = data.Item1;
		byte b = data.Item2;
		float num = math.distance(VoxelExtensions._opOrigin, point);
		byte b2 = (num > VoxelExtensions._opAction.radius) ? item : ((byte)math.clamp((float)item + VoxelExtensions._opAction.strength * math.lerp(255f, 0f, num / VoxelExtensions._opAction.radius), 0f, 255f));
		if (item != b2)
		{
			b = VoxelExtensions._opAction.material;
		}
		if (VoxelExtensions._showDebug && item != b2)
		{
			Debug.Log(string.Format("Unmined at {0}->{1}=d{2:F2} with density {3}[{4}] -> {5}[{6}]", new object[]
			{
				VoxelExtensions._opOrigin,
				point,
				num,
				item,
				item.ToFloat(),
				b2,
				b2.ToFloat()
			}));
		}
		if (!item.IsSolid() && b2.IsSolid())
		{
			int num2 = (int)((float)(b2 - item) / 10f);
			VoxelExtensions._opTotalMined += num2;
			if (b == 0)
			{
				VoxelExtensions._opDirtMined += num2;
			}
			else if (b == 1)
			{
				VoxelExtensions._opStoneMined += num2;
			}
		}
		return new ValueTuple<byte, byte>(b2, b);
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00047460 File Offset: 0x00045660
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte SubtractAt(int3 point, byte density)
	{
		float num = math.distance(VoxelExtensions._opOrigin, point);
		if (num <= VoxelExtensions._opAction.radius)
		{
			return (byte)math.clamp((float)density - VoxelExtensions._opAction.strength * math.lerp(255f, 0f, num / VoxelExtensions._opAction.radius), 0f, 255f);
		}
		return density;
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x000474CC File Offset: 0x000456CC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte AddAt(int3 point, byte density)
	{
		float num = math.distance(VoxelExtensions._opOrigin, point);
		if (num <= VoxelExtensions._opAction.radius)
		{
			return (byte)math.clamp((float)density + VoxelExtensions._opAction.strength * math.lerp(255f, 0f, num / VoxelExtensions._opAction.radius), 0f, 255f);
		}
		return density;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00047537 File Offset: 0x00045737
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: TupleElementNames(new string[]
	{
		"density",
		"materialID"
	})]
	private static ValueTuple<byte, byte> SetVoxelAt(int3 point, [TupleElementNames(new string[]
	{
		"density",
		"materialId"
	})] ValueTuple<byte, byte> data)
	{
		return new ValueTuple<byte, byte>(VoxelExtensions._opDensity, VoxelExtensions._opMaterialId);
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x00047548 File Offset: 0x00045748
	public static void PerformAction(this VoxelWorld world, Vector3 position, VoxelAction action)
	{
		VoxelManager.PerformOperation(world, position, action);
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x00047552 File Offset: 0x00045752
	public static void Dig(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelManager.PerformOperation(world, position, new VoxelAction(OperationType.Subtract, radius, strength, 0));
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00047564 File Offset: 0x00045764
	public static void Add(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelManager.PerformOperation(world, position, new VoxelAction(OperationType.Add, radius, strength, 0));
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00047578 File Offset: 0x00045778
	public static void SetVoxel(this VoxelWorld world, int x, int y, int z, byte density, byte materialId)
	{
		VoxelExtensions.<>c__DisplayClass28_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass28_0();
		CS$<>8__locals1.density = density;
		CS$<>8__locals1.materialId = materialId;
		Vector3Int position = new Vector3Int(x, y, z);
		UnityEngine.BoundsInt worldBounds = new UnityEngine.BoundsInt(position, Vector3Int.zero);
		world.SetVoxelDataCustom(worldBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<SetVoxel>g__SetVoxelAt|0), true);
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x000475C6 File Offset: 0x000457C6
	public static void SetVoxels(this VoxelWorld world, UnityEngine.BoundsInt worldBounds, byte density, byte materialId, bool immediate = true)
	{
		VoxelExtensions._opDensity = density;
		VoxelExtensions._opMaterialId = materialId;
		world.SetVoxelDataCustom(worldBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.SetVoxelAt), immediate);
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x000475E9 File Offset: 0x000457E9
	public static void SetVoxels(this VoxelWorld world, int3[] voxels, byte density, byte materialId, bool immediate = true)
	{
		VoxelExtensions._opDensity = density;
		VoxelExtensions._opMaterialId = materialId;
		world.SetVoxelDataCustom(voxels, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.SetVoxelAt), immediate);
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0004760C File Offset: 0x0004580C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetVoxelCount(this UnityEngine.BoundsInt bounds)
	{
		return (bounds.max.x - bounds.min.x + 1) * (bounds.max.y - bounds.min.y + 1) * (bounds.max.z - bounds.min.z + 1);
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x0004767E File Offset: 0x0004587E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains(this UnityEngine.BoundsInt a, UnityEngine.BoundsInt b)
	{
		return a.Contains(b.min) && a.Contains(b.max);
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x000476A0 File Offset: 0x000458A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UnityEngine.BoundsInt Union(this UnityEngine.BoundsInt a, UnityEngine.BoundsInt b)
	{
		return new UnityEngine.BoundsInt(VectorUtilities.Min(a.min, b.min), VectorUtilities.Max(a.max, b.max));
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x000476D0 File Offset: 0x000458D0
	public static UnityEngine.BoundsInt GetBounds(this VoxelWorld world, float3 point, float radius)
	{
		int3 voxelForLocalPosition = world.GetVoxelForLocalPosition(point);
		int num = Mathf.CeilToInt(radius);
		return new UnityEngine.BoundsInt((voxelForLocalPosition - num).ToVectorInt(), new int3(num * 2).ToVectorInt());
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00047710 File Offset: 0x00045910
	private static Vector3 GetTriangleCenter(RaycastHit hit)
	{
		ValueTuple<Vector3, Vector3, Vector3> worldTriangle = VoxelExtensions.GetWorldTriangle(hit);
		Vector3 item = worldTriangle.Item1;
		Vector3 item2 = worldTriangle.Item2;
		Vector3 item3 = worldTriangle.Item3;
		return (item + item2 + item3) / 3f;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00047750 File Offset: 0x00045950
	[return: TupleElementNames(new string[]
	{
		"v1",
		"v2",
		"v3"
	})]
	private static ValueTuple<Vector3, Vector3, Vector3> GetWorldTriangle(RaycastHit hit)
	{
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null)
		{
			return new ValueTuple<Vector3, Vector3, Vector3>(Vector3.zero, Vector3.zero, Vector3.zero);
		}
		Mesh sharedMesh = meshCollider.sharedMesh;
		if (sharedMesh == null || hit.triangleIndex < 0 || hit.triangleIndex >= sharedMesh.triangles.Length / 3)
		{
			Debug.LogWarning(string.Format("Invalid triangle index {0} for mesh {1}", hit.triangleIndex, (sharedMesh != null) ? sharedMesh.name : null));
			return new ValueTuple<Vector3, Vector3, Vector3>(Vector3.zero, Vector3.zero, Vector3.zero);
		}
		int[] triangles = sharedMesh.triangles;
		Vector3[] vertices = sharedMesh.vertices;
		Transform transform = meshCollider.transform;
		Vector3 item = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
		Vector3 item2 = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
		Vector3 item3 = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
		return new ValueTuple<Vector3, Vector3, Vector3>(item, item2, item3);
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0004785B File Offset: 0x00045A5B
	public static string GetFullPath(this Component component)
	{
		if (!component)
		{
			return "";
		}
		return component.gameObject.GetFullPath() + "/" + component.GetType().Name;
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x0004788C File Offset: 0x00045A8C
	public static string GetFullPath(this GameObject go)
	{
		if (!go)
		{
			return "";
		}
		string str = go.name;
		Transform parent = go.transform.parent;
		while (parent)
		{
			str = parent.name + "/" + str;
			parent = parent.parent;
		}
		return go.scene.name + "/" + str;
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x000478F6 File Offset: 0x00045AF6
	public static int GenerateHashcodeFromPath(this Component component)
	{
		return component.GetFullPath().GetHashCode();
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00047903 File Offset: 0x00045B03
	public static int GenerateHashcodeFromPath(this GameObject go)
	{
		return go.GetFullPath().GetHashCode();
	}

	// Token: 0x04000F93 RID: 3987
	private static UnityEngine.BoundsInt _lastBounds;

	// Token: 0x04000F94 RID: 3988
	private static Vector3 _lastHitPoint;

	// Token: 0x04000F95 RID: 3989
	private static Vector3 _lastGridPoint;

	// Token: 0x04000F96 RID: 3990
	private static Vector3 _lastVertex;

	// Token: 0x04000F97 RID: 3991
	private static bool _showDebug;

	// Token: 0x04000F98 RID: 3992
	private static bool _centerOnly;

	// Token: 0x04000F99 RID: 3993
	private static bool _cascade = true;

	// Token: 0x04000F9A RID: 3994
	private static VoxelAction _opAction;

	// Token: 0x04000F9B RID: 3995
	private static Vector3 _opOrigin;

	// Token: 0x04000F9C RID: 3996
	private static int _opTotalMined;

	// Token: 0x04000F9D RID: 3997
	private static int _opDirtMined;

	// Token: 0x04000F9E RID: 3998
	private static int _opStoneMined;

	// Token: 0x04000F9F RID: 3999
	private static byte _opDensity;

	// Token: 0x04000FA0 RID: 4000
	private static byte _opMaterialId;
}
