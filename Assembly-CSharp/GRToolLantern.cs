using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007FC RID: 2044
[RequireComponent(typeof(GameEntity))]
public class GRToolLantern : MonoBehaviour, IGRSummoningEntity
{
	// Token: 0x0600343F RID: 13375 RVA: 0x0011F848 File Offset: 0x0011DA48
	private void Awake()
	{
		this.trackedEntities = new List<int>();
		this.state = GRToolLantern.State.Off;
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06003440 RID: 13376 RVA: 0x0011F8FD File Offset: 0x0011DAFD
	private void OnEnable()
	{
		this.TurnOff();
		this.state = GRToolLantern.State.Off;
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x0011F90C File Offset: 0x0011DB0C
	private void OnDestroy()
	{
		if (this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.DisableXRay();
		}
	}

	// Token: 0x06003442 RID: 13378 RVA: 0x0011F92C File Offset: 0x0011DB2C
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity1))
		{
			this.turnOnSound = this.upgrade1TurnOnSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity2))
		{
			this.turnOnSound = this.upgrade2TurnOnSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.turnOnSound = this.upgrade3TurnOnSound;
		}
	}

	// Token: 0x06003443 RID: 13379 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnGrabbed()
	{
	}

	// Token: 0x06003444 RID: 13380 RVA: 0x0011F97D File Offset: 0x0011DB7D
	public void OnReleased()
	{
		if (this.WasLastHeldLocal())
		{
			this.DisableXRay();
		}
	}

	// Token: 0x06003445 RID: 13381 RVA: 0x0011F98D File Offset: 0x0011DB8D
	private void EnableXRay()
	{
		if (!this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			GRPlayer.GetLocal().xRayVisionRefCount++;
			this.providingXRay = true;
		}
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x0011F9BF File Offset: 0x0011DBBF
	private void DisableXRay()
	{
		if (this.providingXRay && this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			GRPlayer.GetLocal().xRayVisionRefCount--;
			this.providingXRay = false;
		}
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x0011F9F4 File Offset: 0x0011DBF4
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.tool.energy > 0)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x0011FA2C File Offset: 0x0011DC2C
	private void OnUpdateAuthority(float dt)
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			bool isOn = this.IsHeld();
			this.EnableLights(isOn);
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity2))
		{
			this.SetState(GRToolLantern.State.On);
			if (Time.timeAsDouble > this.lastFlareDropTime + this.minFlareDropInterval && this.IsButtonHeld() && this.tool.HasEnoughEnergy() && this.trackedEntities.Count < this.maxSpawnedFlares && this.lanternFlarePrefab != null)
			{
				if (this.gameEntity.IsAuthority())
				{
					Vector3 b = base.transform.rotation * this.flareSpawnoffset;
					this.gameEntity.manager.RequestCreateItem(this.lanternFlarePrefab.name.GetStaticHash(), base.transform.position + b, base.transform.rotation * Quaternion.Euler(10f, 0f, 10f), (long)this.gameEntity.GetNetId());
				}
				this.lastFlareDropTime = Time.timeAsDouble;
				this.tool.UseEnergy();
				this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
				return;
			}
		}
		else
		{
			GRToolLantern.State state = this.state;
			if (state != GRToolLantern.State.Off)
			{
				if (state != GRToolLantern.State.On)
				{
					return;
				}
				this.timeOnSpentEnergy -= dt;
				if ((!this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f) || this.tool.energy <= 0)
				{
					this.SetState(GRToolLantern.State.Off);
					this.gameEntity.RequestState(this.gameEntity.id, 0L);
					return;
				}
				if (this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f)
				{
					this.TryConsumeEnergy();
				}
			}
			else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolLantern.State.On);
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
				return;
			}
		}
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x0011FC34 File Offset: 0x0011DE34
	private void TryConsumeEnergy()
	{
		if (this.tool.HasEnoughEnergy())
		{
			this.tool.UseEnergy();
			this.timeOnSpentEnergy = this.timeOnPerEnergyUseDurationSeconds * 10f * (float)this.tool.GetEnergyUseCost() / (float)this.tool.GetEnergyMax();
		}
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x0011FC88 File Offset: 0x0011DE88
	private void OnUpdateRemote(float dt)
	{
		GRToolLantern.State state = (GRToolLantern.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x0011FCB4 File Offset: 0x0011DEB4
	private void SetState(GRToolLantern.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state == GRToolLantern.State.On)
			{
				this.TurnOn();
				return;
			}
		}
		else
		{
			this.TurnOff();
		}
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x0011FCF8 File Offset: 0x0011DEF8
	private void TurnOn()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.EnableXRay();
		}
		else
		{
			this.EnableLights(true);
		}
		this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		this.timeLastTurnedOn = Time.time;
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x0011FD58 File Offset: 0x0011DF58
	private void EnableLights(bool isOn)
	{
		if (this.gameLight.gameObject.activeSelf == isOn)
		{
			return;
		}
		if (this.attributes.HasBeenInitialized())
		{
			this.gameLight.light.intensity = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.LightIntensity);
		}
		this.gameLight.gameObject.SetActive(isOn);
		for (int i = 0; i < this.meshAndMaterials.Count; i++)
		{
			MaterialUtils.SwapMaterial(this.meshAndMaterials[i], !isOn);
		}
	}

	// Token: 0x0600344E RID: 13390 RVA: 0x0011FDDF File Offset: 0x0011DFDF
	private void TurnOff()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.LanternIntensity3))
		{
			this.DisableXRay();
			return;
		}
		this.EnableLights(false);
	}

	// Token: 0x0600344F RID: 13391 RVA: 0x0011FDFE File Offset: 0x0011DFFE
	private bool IsHeld()
	{
		return this.gameEntity.IsHeld();
	}

	// Token: 0x06003450 RID: 13392 RVA: 0x0011FE0B File Offset: 0x0011E00B
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x0011FE24 File Offset: 0x0011E024
	private bool WasLastHeldLocal()
	{
		return this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x0011FE40 File Offset: 0x0011E040
	private bool IsButtonHeld()
	{
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return false;
		}
		if (!GamePlayer.IsLeftHand(num))
		{
			return gamePlayer.rig.rightIndex.calcT > 0.25f;
		}
		return gamePlayer.rig.leftIndex.calcT > 0.25f;
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnStateChanged(long prevState, long nextState)
	{
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x0011FEB4 File Offset: 0x0011E0B4
	public bool CanChangeState(long newStateIndex)
	{
		if (newStateIndex < 0L || newStateIndex >= 2L)
		{
			return false;
		}
		GRToolLantern.State state = (GRToolLantern.State)newStateIndex;
		if (state != GRToolLantern.State.Off)
		{
			return state == GRToolLantern.State.On && this.tool.energy > 0;
		}
		return Time.time > this.timeLastTurnedOn + this.minOnDuration || this.tool.energy <= 0;
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x0011FF10 File Offset: 0x0011E110
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x0011FF30 File Offset: 0x0011E130
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x0011FF5F File Offset: 0x0011E15F
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x0011FF68 File Offset: 0x0011E168
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x04004435 RID: 17461
	public GameEntity gameEntity;

	// Token: 0x04004436 RID: 17462
	public GRTool tool;

	// Token: 0x04004437 RID: 17463
	public GameLight gameLight;

	// Token: 0x04004438 RID: 17464
	public GRAttributes attributes;

	// Token: 0x04004439 RID: 17465
	[SerializeField]
	private float timeOnPerEnergyUseDurationSeconds = 2f;

	// Token: 0x0400443A RID: 17466
	[SerializeField]
	private int minEnergyPerUse = 1;

	// Token: 0x0400443B RID: 17467
	[SerializeField]
	private float turnOnSoundVolume;

	// Token: 0x0400443C RID: 17468
	[SerializeField]
	private AudioClip turnOnSound;

	// Token: 0x0400443D RID: 17469
	[SerializeField]
	private AudioClip upgrade1TurnOnSound;

	// Token: 0x0400443E RID: 17470
	[SerializeField]
	private AudioClip upgrade2TurnOnSound;

	// Token: 0x0400443F RID: 17471
	[SerializeField]
	private AudioClip upgrade3TurnOnSound;

	// Token: 0x04004440 RID: 17472
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004441 RID: 17473
	public List<MeshAndMaterials> meshAndMaterials;

	// Token: 0x04004442 RID: 17474
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x04004443 RID: 17475
	private float timeOnSpentEnergy;

	// Token: 0x04004444 RID: 17476
	private float timeLastTurnedOn;

	// Token: 0x04004445 RID: 17477
	private float minOnDuration = 0.5f;

	// Token: 0x04004446 RID: 17478
	private GRToolLantern.State state;

	// Token: 0x04004447 RID: 17479
	private List<int> trackedEntities;

	// Token: 0x04004448 RID: 17480
	private double lastFlareDropTime;

	// Token: 0x04004449 RID: 17481
	public double minFlareDropInterval = 1.0;

	// Token: 0x0400444A RID: 17482
	public GameEntity lanternFlarePrefab;

	// Token: 0x0400444B RID: 17483
	public int maxSpawnedFlares = 10;

	// Token: 0x0400444C RID: 17484
	private bool providingXRay;

	// Token: 0x0400444D RID: 17485
	public Vector3 flareSpawnoffset = Vector3.zero;

	// Token: 0x020007FD RID: 2045
	private enum State
	{
		// Token: 0x0400444F RID: 17487
		Off,
		// Token: 0x04004450 RID: 17488
		On,
		// Token: 0x04004451 RID: 17489
		Count
	}
}
