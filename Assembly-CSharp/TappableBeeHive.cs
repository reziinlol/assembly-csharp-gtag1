using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009E2 RID: 2530
public class TappableBeeHive : Tappable
{
	// Token: 0x060040D1 RID: 16593 RVA: 0x0015ABA0 File Offset: 0x00158DA0
	private void Awake()
	{
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			Debug.LogError("TappableBeeHive: Disabling because swarmEmergePoint is null at: " + base.transform.GetPath(), this);
			base.enabled = false;
			return;
		}
		base.GetComponent<SlingshotProjectileHitNotifier>().OnProjectileHit += this.OnSlingshotHit;
	}

	// Token: 0x060040D2 RID: 16594 RVA: 0x0015AC04 File Offset: 0x00158E04
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x060040D3 RID: 16595 RVA: 0x0015AC78 File Offset: 0x00158E78
	public void OnSlingshotHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x04005161 RID: 20833
	[SerializeField]
	private GameObject swarmEmergeFromPoint;

	// Token: 0x04005162 RID: 20834
	[SerializeField]
	private GameObject swarmEmergeToPoint;

	// Token: 0x04005163 RID: 20835
	[SerializeField]
	private GameObject honeycombSurface;

	// Token: 0x04005164 RID: 20836
	[SerializeField]
	private float honeycombDisableDuration;

	// Token: 0x04005165 RID: 20837
	[NonSerialized]
	private TimeSince _timeSinceLastTap;

	// Token: 0x04005166 RID: 20838
	private float reenableHoneycombAtTimestamp;

	// Token: 0x04005167 RID: 20839
	private Coroutine reenableHoneycombCoroutine;
}
