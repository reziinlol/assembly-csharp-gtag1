using System;
using GorillaTagScripts.Builder;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class ArtilleryCannon : MonoBehaviour
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000A8D RID: 2701 RVA: 0x00038BCD File Offset: 0x00036DCD
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00038BE2 File Offset: 0x00036DE2
	private void Awake()
	{
		if (this.projectilePrefab != null)
		{
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00038C04 File Offset: 0x00036E04
	private void OnEnable()
	{
		if (this.fireHitNotifier != null)
		{
			this.fireHitNotifier.OnProjectileHit += this.OnFireProjectileHit;
		}
		ArtilleryCannonState newState;
		if (this.stateRef.TryResolve<ArtilleryCannonState>(out newState))
		{
			this.Bind(newState);
			return;
		}
		this.stateRef.AddCallbackOnLoad(new Action(this.OnStateSceneLoaded));
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00038C64 File Offset: 0x00036E64
	private void OnDisable()
	{
		if (this.fireHitNotifier != null)
		{
			this.fireHitNotifier.OnProjectileHit -= this.OnFireProjectileHit;
		}
		this.stateRef.RemoveCallbackOnLoad(new Action(this.OnStateSceneLoaded));
		this.Unbind();
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x00038CB4 File Offset: 0x00036EB4
	private void OnStateSceneLoaded()
	{
		ArtilleryCannonState newState;
		if (this.stateRef.TryResolve<ArtilleryCannonState>(out newState))
		{
			this.Bind(newState);
		}
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00038CD8 File Offset: 0x00036ED8
	private void Bind(ArtilleryCannonState newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.Unbind();
		this.state = newState;
		if (this.state == null)
		{
			return;
		}
		this.state.onRotationChanged += this.OnRotationChanged;
		this.state.onFired += this.OnFiredRemote;
		this.ApplyRotation();
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00038D44 File Offset: 0x00036F44
	private void Unbind()
	{
		if (this.state == null)
		{
			return;
		}
		this.state.onRotationChanged -= this.OnRotationChanged;
		this.state.onFired -= this.OnFiredRemote;
		this.state = null;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00038D98 File Offset: 0x00036F98
	private void LateUpdate()
	{
		if (this.state == null)
		{
			return;
		}
		int localActorNr = this.LocalActorNr;
		if (this.pitchCrank != null && this.state.pitchCrankSync.holderActorNr == localActorNr)
		{
			this.state.UpdateLocalCrankState(0, this.pitchCrank.IsHeldLeftHand, this.pitchCrank.CurrentAngle);
		}
		if (this.yawCrank != null && this.state.yawCrankSync.holderActorNr == localActorNr)
		{
			this.state.UpdateLocalCrankState(1, this.yawCrank.IsHeldLeftHand, this.yawCrank.CurrentAngle);
		}
		this.UpdateRemoteCrankVisual(this.pitchCrank, this.state.pitchCrankSync, localActorNr);
		this.UpdateRemoteCrankVisual(this.yawCrank, this.state.yawCrankSync, localActorNr);
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x00038E74 File Offset: 0x00037074
	private void UpdateRemoteCrankVisual(ArtilleryCrank crank, ArtilleryCannonState.CrankSyncState syncState, int localActor)
	{
		if (crank == null || syncState.holderActorNr == localActor)
		{
			return;
		}
		if (syncState.holderActorNr != -1)
		{
			VRRig vrrig = ArtilleryCannonState.FindRigForActor(syncState.holderActorNr);
			if (vrrig != null)
			{
				crank.UpdateFromRemoteHand(vrrig, syncState.isLeftHand);
				return;
			}
		}
		crank.SetVisualAngle(syncState.angle);
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00038ECC File Offset: 0x000370CC
	internal bool IsCrankHeldLocally(int crankIndex)
	{
		return (ref (crankIndex == 0) ? ref this.state.pitchCrankSync : ref this.state.yawCrankSync).holderActorNr == this.LocalActorNr;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00038EF6 File Offset: 0x000370F6
	internal bool OnCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		return this.state.NotifyCrankGrabbed(crankIndex, isLeftHand);
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00038F05 File Offset: 0x00037105
	internal void OnCrankReleased(int crankIndex, float finalAngle)
	{
		this.state.NotifyCrankReleased(crankIndex, finalAngle);
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00038F14 File Offset: 0x00037114
	internal void OnCrankInput(int crankIndex, float degrees)
	{
		this.state.NotifyCrankInput(crankIndex, degrees);
		this.ApplyRotation();
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x00038F29 File Offset: 0x00037129
	private void OnRotationChanged()
	{
		this.ApplyRotation();
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00038F34 File Offset: 0x00037134
	private void ApplyRotation()
	{
		if (this.state == null)
		{
			return;
		}
		if (this.yawTransform != null)
		{
			this.yawTransform.localRotation = Quaternion.Euler(0f, this.state.CurrentYaw, 0f);
		}
		if (this.pitchTransform != null)
		{
			this.pitchTransform.localRotation = Quaternion.Euler(-this.state.CurrentPitch, 0f, 0f);
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00038FB7 File Offset: 0x000371B7
	public void Fire()
	{
		if (this.state == null)
		{
			return;
		}
		if (this.state.TryFire())
		{
			this.FireLocal();
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00038FDB File Offset: 0x000371DB
	private void OnFireProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.Fire();
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x00038FE3 File Offset: 0x000371E3
	private void OnFiredRemote()
	{
		this.FireLocal();
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00038FEC File Offset: 0x000371EC
	private void FireLocal()
	{
		if (this.projectilePrefab == null || this.muzzle == null)
		{
			return;
		}
		Vector3 position = this.muzzle.position;
		Vector3 forward = this.muzzle.forward;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.projectileHash, true);
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.LookRotation(forward);
		BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
		if (component != null)
		{
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(this.knockbackConfig);
		}
		Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.linearVelocity = forward * this.launchSpeed;
		}
		if (this.fireSound != null)
		{
			this.fireSound.GTPlay();
		}
	}

	// Token: 0x04000CD0 RID: 3280
	[Header("Network State")]
	[SerializeField]
	private XSceneRef stateRef;

	// Token: 0x04000CD1 RID: 3281
	[Header("Cranks")]
	[SerializeField]
	private ArtilleryCrank pitchCrank;

	// Token: 0x04000CD2 RID: 3282
	[SerializeField]
	private ArtilleryCrank yawCrank;

	// Token: 0x04000CD3 RID: 3283
	[Header("Rotation")]
	[SerializeField]
	private Transform yawTransform;

	// Token: 0x04000CD4 RID: 3284
	[SerializeField]
	private Transform pitchTransform;

	// Token: 0x04000CD5 RID: 3285
	[Header("Firing")]
	[SerializeField]
	private Transform muzzle;

	// Token: 0x04000CD6 RID: 3286
	[SerializeField]
	private GameObject projectilePrefab;

	// Token: 0x04000CD7 RID: 3287
	[SerializeField]
	private float launchSpeed = 30f;

	// Token: 0x04000CD8 RID: 3288
	[SerializeField]
	private AudioSource fireSound;

	// Token: 0x04000CD9 RID: 3289
	[SerializeField]
	private SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

	// Token: 0x04000CDA RID: 3290
	[Header("Fire Trigger")]
	[Tooltip("When a projectile hits this notifier, the cannon fires.")]
	[SerializeField]
	private SlingshotProjectileHitNotifier fireHitNotifier;

	// Token: 0x04000CDB RID: 3291
	private ArtilleryCannonState state;

	// Token: 0x04000CDC RID: 3292
	private int projectileHash;
}
