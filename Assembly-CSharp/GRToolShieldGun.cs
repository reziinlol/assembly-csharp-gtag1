using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200080B RID: 2059
public class GRToolShieldGun : MonoBehaviour
{
	// Token: 0x060034CB RID: 13515 RVA: 0x00122C21 File Offset: 0x00120E21
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x060034CC RID: 13516 RVA: 0x00122C54 File Offset: 0x00120E54
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength1))
		{
			this.firingSound = this.upgrade1FiringSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength2))
		{
			this.firingSound = this.upgrade2FiringSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength3))
		{
			this.firingSound = this.upgrade3FiringSound;
		}
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x00122CA5 File Offset: 0x00120EA5
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x00122CC0 File Offset: 0x00120EC0
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x00122CF4 File Offset: 0x00120EF4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolShieldGun.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Firing);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolShieldGun.State.Firing:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Cooldown);
				return;
			}
			break;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x00122DDC File Offset: 0x00120FDC
	private void OnUpdateRemote(float dt)
	{
		GRToolShieldGun.State state = (GRToolShieldGun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetStateAuthority(state);
		}
	}

	// Token: 0x060034D1 RID: 13521 RVA: 0x00122E06 File Offset: 0x00121006
	private void SetStateAuthority(GRToolShieldGun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x00122E28 File Offset: 0x00121028
	private void SetState(GRToolShieldGun.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolShieldGun.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolShieldGun.State.Firing:
			this.StartFiring();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x00122EAC File Offset: 0x001210AC
	private void StartCharge()
	{
		if (this.chargeSound != null)
		{
			this.audioSource.PlayOneShot(this.chargeSound, this.chargeSoundVolume);
		}
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x00122EFC File Offset: 0x001210FC
	private void StartFiring()
	{
		if (this.firingSound != null)
		{
			this.audioSource.PlayOneShot(this.firingSound, this.firingSoundVolume);
		}
		this.timeLastFired = Time.time;
		this.tool.UseEnergy();
		Vector3 position = this.firingTransform.position;
		Vector3 velocity = this.firingTransform.forward * this.projectileSpeed;
		float scale = GTPlayer.Instance.scale;
		int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		this.firedProjectile = ObjectPools.instance.Instantiate(hash, true).GetComponent<SlingshotProjectile>();
		this.firedProjectile.transform.localScale = Vector3.one * scale;
		if (this.projectileTrailPrefab != null)
		{
			int trailHash = PoolUtils.GameObjHashCode(this.projectileTrailPrefab);
			this.AttachTrail(trailHash, this.firedProjectile.gameObject, position, false, false);
		}
		Collider component = this.firedProjectile.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			for (int i = 0; i < this.colliders.Count; i++)
			{
				Physics.IgnoreCollision(this.colliders[i], component);
			}
		}
		if (this.IsHeldLocal())
		{
			this.firedProjectile.OnImpact += this.OnProjectileImpact;
		}
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		this.firedProjectile.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, 1, scale, true, this.projectileColor);
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x00123080 File Offset: 0x00121280
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam, false, default(Color));
	}

	// Token: 0x060034D6 RID: 13526 RVA: 0x001230D4 File Offset: 0x001212D4
	private void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		projectile.OnImpact -= this.OnProjectileImpact;
		GRPlayer grplayer = null;
		RigContainer rigContainer;
		if (hitPlayer != null && VRRigCache.Instance.TryGetVrrig(hitPlayer, out rigContainer) && rigContainer.Rig != null)
		{
			grplayer = rigContainer.Rig.GetComponent<GRPlayer>();
		}
		else if (this.allowAoeHits)
		{
			GRToolShieldGun.vrRigs.Clear();
			GRToolShieldGun.vrRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GRToolShieldGun.vrRigs);
			VRRig vrrig = null;
			float num = float.MaxValue;
			for (int i = 0; i < GRToolShieldGun.vrRigs.Count; i++)
			{
				float sqrMagnitude = (GRToolShieldGun.vrRigs[i].bodyTransform.position - impactPos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vrrig = GRToolShieldGun.vrRigs[i];
				}
			}
			if (vrrig != null)
			{
				grplayer = vrrig.GetComponent<GRPlayer>();
			}
		}
		if (grplayer != null)
		{
			int num2 = 0;
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength1))
			{
				num2 |= 1;
			}
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength2))
			{
				num2 |= 2;
			}
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength3))
			{
				num2 |= 4;
			}
			this.gameEntity.manager.ghostReactorManager.RequestGrantPlayerShield(grplayer, this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ShieldSize), num2);
		}
	}

	// Token: 0x060034D7 RID: 13527 RVA: 0x00123238 File Offset: 0x00121438
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x060034D8 RID: 13528 RVA: 0x00123298 File Offset: 0x00121498
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x001232EC File Offset: 0x001214EC
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFired + this.cooldownMinimum);
	}

	// Token: 0x040044F5 RID: 17653
	public GameEntity gameEntity;

	// Token: 0x040044F6 RID: 17654
	public GRTool tool;

	// Token: 0x040044F7 RID: 17655
	public GRAttributes attributes;

	// Token: 0x040044F8 RID: 17656
	public GameObject projectilePrefab;

	// Token: 0x040044F9 RID: 17657
	public GameObject projectileTrailPrefab;

	// Token: 0x040044FA RID: 17658
	public Transform firingTransform;

	// Token: 0x040044FB RID: 17659
	public List<Collider> colliders;

	// Token: 0x040044FC RID: 17660
	public float projectileSpeed = 25f;

	// Token: 0x040044FD RID: 17661
	public Color projectileColor = new Color(0.25f, 0.25f, 1f);

	// Token: 0x040044FE RID: 17662
	public bool allowAoeHits;

	// Token: 0x040044FF RID: 17663
	public float aeoHitRadius = 0.5f;

	// Token: 0x04004500 RID: 17664
	public float chargeDuration = 0.75f;

	// Token: 0x04004501 RID: 17665
	public float flashDuration = 0.1f;

	// Token: 0x04004502 RID: 17666
	public float cooldownDuration;

	// Token: 0x04004503 RID: 17667
	public AudioSource audioSource;

	// Token: 0x04004504 RID: 17668
	public AudioClip chargeSound;

	// Token: 0x04004505 RID: 17669
	public float chargeSoundVolume = 0.5f;

	// Token: 0x04004506 RID: 17670
	public AudioClip firingSound;

	// Token: 0x04004507 RID: 17671
	public float firingSoundVolume = 0.5f;

	// Token: 0x04004508 RID: 17672
	public AudioClip upgrade1FiringSound;

	// Token: 0x04004509 RID: 17673
	public AudioClip upgrade2FiringSound;

	// Token: 0x0400450A RID: 17674
	public AudioClip upgrade3FiringSound;

	// Token: 0x0400450B RID: 17675
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x0400450C RID: 17676
	private GRToolShieldGun.State state;

	// Token: 0x0400450D RID: 17677
	private float stateTimeRemaining;

	// Token: 0x0400450E RID: 17678
	private bool activatedLocally;

	// Token: 0x0400450F RID: 17679
	private bool waitingForButtonRelease;

	// Token: 0x04004510 RID: 17680
	private float timeLastFired;

	// Token: 0x04004511 RID: 17681
	private float cooldownMinimum = 0.35f;

	// Token: 0x04004512 RID: 17682
	private SlingshotProjectile firedProjectile;

	// Token: 0x04004513 RID: 17683
	private static List<VRRig> vrRigs = new List<VRRig>(10);

	// Token: 0x0200080C RID: 2060
	private enum State
	{
		// Token: 0x04004515 RID: 17685
		Idle,
		// Token: 0x04004516 RID: 17686
		Charging,
		// Token: 0x04004517 RID: 17687
		Firing,
		// Token: 0x04004518 RID: 17688
		Cooldown,
		// Token: 0x04004519 RID: 17689
		Count
	}
}
