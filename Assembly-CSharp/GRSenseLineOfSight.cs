using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007D8 RID: 2008
[Serializable]
public class GRSenseLineOfSight
{
	// Token: 0x06003349 RID: 13129 RVA: 0x0011A432 File Offset: 0x00118632
	public bool HasLineOfSight(Vector3 headPos, Vector3 targetPos)
	{
		return GRSenseLineOfSight.HasLineOfSight(headPos, targetPos, this.sightDist, this.visibilityMask.value, this.rayCastMode);
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x0011A454 File Offset: 0x00118654
	public static bool HasLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask, GRSenseLineOfSight.RaycastMode rayCastMode = GRSenseLineOfSight.RaycastMode.Geometry)
	{
		switch (rayCastMode)
		{
		case GRSenseLineOfSight.RaycastMode.Geometry:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		case GRSenseLineOfSight.RaycastMode.Navmesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryAndNavMesh:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask) && GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryOrNavMesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist) || GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		default:
			return false;
		}
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x0011A4BC File Offset: 0x001186BC
	public static bool HasGeoLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask)
	{
		float num = Vector3.Distance(targetPos, headPos);
		return num <= sightDist && Physics.RaycastNonAlloc(new Ray(headPos, targetPos - headPos), GRSenseLineOfSight.visibilityHits, Mathf.Min(num, sightDist), layerMask, QueryTriggerInteraction.Ignore) < 1;
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x0011A4FC File Offset: 0x001186FC
	public static bool HasNavmeshLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist)
	{
		NavMeshHit navMeshHit;
		NavMeshHit navMeshHit2;
		return (targetPos - headPos).sqrMagnitude <= sightDist * sightDist && NavMesh.SamplePosition(headPos, out navMeshHit, 1f, -1) && !NavMesh.Raycast(navMeshHit.position, targetPos, out navMeshHit2, -1);
	}

	// Token: 0x040042E1 RID: 17121
	public float sightDist;

	// Token: 0x040042E2 RID: 17122
	public LayerMask visibilityMask;

	// Token: 0x040042E3 RID: 17123
	public GRSenseLineOfSight.RaycastMode rayCastMode;

	// Token: 0x040042E4 RID: 17124
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x020007D9 RID: 2009
	public enum RaycastMode
	{
		// Token: 0x040042E6 RID: 17126
		Geometry,
		// Token: 0x040042E7 RID: 17127
		Navmesh,
		// Token: 0x040042E8 RID: 17128
		GeometryAndNavMesh,
		// Token: 0x040042E9 RID: 17129
		GeometryOrNavMesh
	}
}
