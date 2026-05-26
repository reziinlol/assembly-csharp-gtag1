using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000890 RID: 2192
public class GorillaTagCompetitiveForcedLeaveRoomVolume : MonoBehaviour
{
	// Token: 0x06003943 RID: 14659 RVA: 0x00138EAC File Offset: 0x001370AC
	private void Start()
	{
		this.VolumeCollider = base.GetComponent<Collider>();
		this.CompetitiveManager = (GameMode.GetGameModeInstance(GameModeType.InfectionCompetitive) as GorillaTagCompetitiveManager);
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.RegisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x06003944 RID: 14660 RVA: 0x00138EE6 File Offset: 0x001370E6
	private void OnDestroy()
	{
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.UnregisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x06003945 RID: 14661 RVA: 0x00138F04 File Offset: 0x00137104
	public bool ContainsPoint(Vector3 position)
	{
		SphereCollider sphereCollider = this.VolumeCollider as SphereCollider;
		if (sphereCollider != null)
		{
			return Vector3.SqrMagnitude(position - (sphereCollider.transform.position + sphereCollider.center)) <= sphereCollider.radius * sphereCollider.radius;
		}
		BoxCollider boxCollider = this.VolumeCollider as BoxCollider;
		return boxCollider != null && boxCollider.bounds.Contains(position);
	}

	// Token: 0x04004952 RID: 18770
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04004953 RID: 18771
	private Collider VolumeCollider;
}
