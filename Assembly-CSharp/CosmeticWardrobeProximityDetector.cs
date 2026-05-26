using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000573 RID: 1395
[RequireComponent(typeof(SphereCollider))]
public class CosmeticWardrobeProximityDetector : MonoBehaviour
{
	// Token: 0x06002372 RID: 9074 RVA: 0x000BF146 File Offset: 0x000BD346
	private void OnEnable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Add(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000BF166 File Offset: 0x000BD366
	private void OnDisable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Remove(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x000BF188 File Offset: 0x000BD388
	public static bool IsUserNearWardrobe(int actorNr)
	{
		LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		});
		LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		});
		VRRigCache.Instance.GetActiveRigs(CosmeticWardrobeProximityDetector.rigs);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetPlayer.Get(actorNr).GetPlayerRef(), out rigContainer))
		{
			return false;
		}
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			if ((rigContainer.HeadCollider.transform.position - sphereCollider.transform.position).magnitude <= sphereCollider.radius)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002E97 RID: 11927
	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	// Token: 0x04002E98 RID: 11928
	private static List<VRRig> rigs = new List<VRRig>();

	// Token: 0x04002E99 RID: 11929
	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	// Token: 0x04002E9A RID: 11930
	private static readonly Collider[] overlapColliders = new Collider[20];
}
