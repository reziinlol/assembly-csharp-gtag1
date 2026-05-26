using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000718 RID: 1816
public static class AbilityHelperFunctions
{
	// Token: 0x06002E1A RID: 11802 RVA: 0x000FCAE0 File Offset: 0x000FACE0
	public static float EaseOutPower(float t, float power)
	{
		return 1f - Mathf.Pow(1f - t, power);
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x000FCAF8 File Offset: 0x000FACF8
	public static int RandomRangeUnique(int minInclusive, int maxExclusive, int lastValue)
	{
		int num = maxExclusive - minInclusive;
		if (num <= 1)
		{
			return minInclusive;
		}
		int num2 = Random.Range(minInclusive, maxExclusive);
		if (num2 != lastValue)
		{
			return num2;
		}
		return (num2 + 1) % num;
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x000FCB22 File Offset: 0x000FAD22
	public static int GetNavMeshWalkableArea()
	{
		if (AbilityHelperFunctions.navMeshWalkableArea == -1)
		{
			AbilityHelperFunctions.navMeshWalkableArea = NavMesh.GetAreaFromName("walkable");
		}
		return AbilityHelperFunctions.navMeshWalkableArea;
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000FCB40 File Offset: 0x000FAD40
	public static Vector3? GetLocationToInvestigate(Vector3 listenerLocation, float hearingRadius, Vector3? currentInvestigationLocation)
	{
		GameNoiseEvent gameNoiseEvent;
		NavMeshHit navMeshHit;
		if (GRNoiseEventManager.instance.GetMostRecentNoiseEventInRadius(listenerLocation, hearingRadius, out gameNoiseEvent) && NavMesh.SamplePosition(gameNoiseEvent.position, out navMeshHit, 1f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
		{
			return new Vector3?(navMeshHit.position);
		}
		if (currentInvestigationLocation != null)
		{
			return currentInvestigationLocation;
		}
		return null;
	}

	// Token: 0x04003B32 RID: 15154
	private static int navMeshWalkableArea = -1;
}
