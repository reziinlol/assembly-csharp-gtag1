using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000810 RID: 2064
public class GRToolUpgradePiece : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060034DE RID: 13534 RVA: 0x001233A0 File Offset: 0x001215A0
	private void Start()
	{
		MeshFilter componentInChildren = base.GetComponentInChildren<MeshFilter>();
		if (componentInChildren != null)
		{
			this.meshCollider.sharedMesh = componentInChildren.sharedMesh;
		}
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x001233D0 File Offset: 0x001215D0
	private void EnableProcAnimLoop()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.Tick));
		if (!this.humAudioSource.isPlaying)
		{
			this.humAudioSource.volume = 0f;
			this.humAudioSource.GTPlay();
		}
	}

	// Token: 0x060034E0 RID: 13536 RVA: 0x0012342C File Offset: 0x0012162C
	private void DisableProcAnimLoop()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.Tick));
		this.SwitchMagnetizedTarget(null);
		this.childVisualTransform.localPosition = Vector3.zero;
		this.childVisualTransform.localRotation = Quaternion.identity;
		this.childVisualTransform.localScale = Vector3.one;
		this.humAudioSource.Stop();
		if (this.attractParticleSystem != null)
		{
			this.attractParticleSystem.Stop();
		}
	}

	// Token: 0x060034E1 RID: 13537 RVA: 0x001234BB File Offset: 0x001216BB
	private void SwitchMagnetizedTarget(GameEntity entity)
	{
		this.currentMagnetizingTool = entity;
	}

	// Token: 0x060034E2 RID: 13538 RVA: 0x001234C4 File Offset: 0x001216C4
	private void Tick()
	{
		Vector3 position = base.transform.position;
		List<GameEntity> gameEntities = this.gameEntity.manager.GetGameEntities();
		int num = this.gameEntityListCheckIndex;
		int num2 = (this.toolSearchesPerFrame < gameEntities.Count) ? this.toolSearchesPerFrame : gameEntities.Count;
		GRTool grtool = (this.currentMagnetizingTool != null) ? this.currentMagnetizingTool.GetComponent<GRTool>() : null;
		GRTool.Upgrade upgrade = (grtool != null) ? grtool.FindMatchingUpgrade(this.matchingUpgrade) : null;
		float num3 = (grtool != null) ? grtool.GetPointDistanceToUpgrade(position, upgrade) : 1E+10f;
		if (num3 > this.minDistToStartMagnetize)
		{
			this.SwitchMagnetizedTarget(null);
			grtool = null;
			upgrade = null;
			num3 = 1E+10f;
		}
		for (int i = 0; i < num2; i++)
		{
			num = (num + 1) % gameEntities.Count;
			GameEntity gameEntity = gameEntities[num];
			if (!(gameEntity == null))
			{
				GRTool component = gameEntity.GetComponent<GRTool>();
				if (component != null && gameEntity.heldByActorNumber != -1)
				{
					GRTool.Upgrade upgrade2 = component.FindMatchingUpgrade(this.matchingUpgrade);
					if (upgrade2 != null)
					{
						float pointDistanceToUpgrade = component.GetPointDistanceToUpgrade(position, upgrade2);
						if (pointDistanceToUpgrade > 0f && pointDistanceToUpgrade < num3 && pointDistanceToUpgrade < this.minDistToStartMagnetize)
						{
							this.SwitchMagnetizedTarget(gameEntity);
							grtool = component;
							upgrade = upgrade2;
							num3 = pointDistanceToUpgrade;
						}
					}
				}
			}
		}
		this.gameEntityListCheckIndex = num;
		if (grtool != null)
		{
			Transform upgradeAttachTransform = grtool.GetUpgradeAttachTransform(upgrade);
			if (num3 >= this.minDistToSnap)
			{
				float num4 = Mathf.Clamp01(num3 / this.minDistToStartMagnetize);
				this.humAudioSource.volume = Mathf.Lerp(this.magnetizingLoopMaxVolume, this.magnetizingLoopMinVolume, num4);
				float num5 = this.shakeMaxAmount * (1f - num4);
				float t = Mathf.Clamp01((this.visualDistanceCurve != null) ? this.visualDistanceCurve.Evaluate(num4) : num4);
				this.shakePhase += Time.deltaTime * this.shakeFrequency;
				if (this.shakePhase > 6.2831855f)
				{
					this.shakePhase -= 6.2831855f;
				}
				Transform transform = base.transform;
				if (this.childVisualTransform != null)
				{
					Vector3 position2 = Vector3.Lerp(upgradeAttachTransform.position, transform.position, t);
					Quaternion quaternion = Quaternion.Slerp(upgradeAttachTransform.rotation, transform.rotation, t);
					Vector3 localScale = Vector3.Lerp(upgradeAttachTransform.localScale, transform.localScale, t);
					localScale.x /= transform.localScale.x;
					localScale.y /= transform.localScale.y;
					localScale.z /= transform.localScale.y;
					quaternion *= Quaternion.Euler(new Vector3(num5 * Mathf.Sin(this.shakePhase), num5 * Mathf.Cos(this.shakePhase), 0f));
					this.childVisualTransform.position = position2;
					this.childVisualTransform.rotation = quaternion;
					this.childVisualTransform.localScale = localScale;
				}
				if (this.attractParticleSystem != null)
				{
					if (!this.attractParticleSystem.isPlaying)
					{
						this.attractParticleSystem.Play();
					}
					this.attractParticleSystem.emission.enabled = true;
				}
				this.forceField.transform.position = upgradeAttachTransform.position;
				return;
			}
			this.humAudioSource.volume = 0f;
			if (this.attractParticleSystem != null)
			{
				this.attractParticleSystem.Stop();
			}
			this.childVisualTransform.position = upgradeAttachTransform.position;
			this.childVisualTransform.rotation = upgradeAttachTransform.rotation;
			this.childVisualTransform.localScale = new Vector3(upgradeAttachTransform.localScale.x / base.transform.localScale.x, upgradeAttachTransform.localScale.y / base.transform.localScale.y, upgradeAttachTransform.localScale.z / base.transform.localScale.z);
			if (this.currentMagnetizingTool != null)
			{
				GhostReactor instance = GhostReactor.instance;
				if (instance != null)
				{
					instance.grManager.ToolSnapRequestUpgrade(this.gameEntity.GetNetId(), this.matchingUpgrade, this.currentMagnetizingTool.GetComponent<GameEntity>().GetNetId());
					return;
				}
			}
		}
		else
		{
			if (this.attractParticleSystem != null)
			{
				this.attractParticleSystem.emission.enabled = false;
			}
			this.humAudioSource.volume = 0f;
		}
	}

	// Token: 0x060034E3 RID: 13539 RVA: 0x00123974 File Offset: 0x00121B74
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.ReleasedByPlayer));
	}

	// Token: 0x060034E4 RID: 13540 RVA: 0x001239D0 File Offset: 0x00121BD0
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.ReleasedByPlayer));
	}

	// Token: 0x060034E5 RID: 13541 RVA: 0x00123A2C File Offset: 0x00121C2C
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
		this.EnableProcAnimLoop();
	}

	// Token: 0x060034E6 RID: 13542 RVA: 0x00123A8D File Offset: 0x00121C8D
	public void ReleasedByPlayer()
	{
		this.DisableProcAnimLoop();
	}

	// Token: 0x060034E7 RID: 13543 RVA: 0x00123A98 File Offset: 0x00121C98
	public void OnEntityInit()
	{
		GhostReactor.ToolEntityCreateData toolEntityCreateData = GhostReactor.ToolEntityCreateData.Unpack(this.gameEntity.createData);
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null)
		{
			GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = ghostReactorManager.GetToolUpgradeStationFullForIndex(toolEntityCreateData.stationIndex);
			if (toolUpgradeStationFullForIndex != null)
			{
				toolUpgradeStationFullForIndex.InitLinkedEntity(this.gameEntity);
			}
		}
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x04004525 RID: 17701
	public GameEntity gameEntity;

	// Token: 0x04004526 RID: 17702
	public GRToolProgressionManager.ToolParts matchingUpgrade;

	// Token: 0x04004527 RID: 17703
	private int gameEntityListCheckIndex;

	// Token: 0x04004528 RID: 17704
	private GameEntity currentMagnetizingTool;

	// Token: 0x04004529 RID: 17705
	public AnimationCurve visualDistanceCurve;

	// Token: 0x0400452A RID: 17706
	public float shakeMaxAmount = 10f;

	// Token: 0x0400452B RID: 17707
	public float shakeFrequency = 100f;

	// Token: 0x0400452C RID: 17708
	public Transform childVisualTransform;

	// Token: 0x0400452D RID: 17709
	public AudioSource humAudioSource;

	// Token: 0x0400452E RID: 17710
	public AudioSource audioSource;

	// Token: 0x0400452F RID: 17711
	public AudioClip snapAudioClip;

	// Token: 0x04004530 RID: 17712
	public MeshCollider meshCollider;

	// Token: 0x04004531 RID: 17713
	public ParticleSystem attractParticleSystem;

	// Token: 0x04004532 RID: 17714
	public ParticleSystemForceField forceField;

	// Token: 0x04004533 RID: 17715
	public float minDistToStartMagnetize = 0.5f;

	// Token: 0x04004534 RID: 17716
	public float minDistToSnap;

	// Token: 0x04004535 RID: 17717
	public float magnetizingLoopMinVolume = 0.2f;

	// Token: 0x04004536 RID: 17718
	public float magnetizingLoopMaxVolume = 1f;

	// Token: 0x04004537 RID: 17719
	public float snapAudioVolume = 1f;

	// Token: 0x04004538 RID: 17720
	private int toolSearchesPerFrame = 5;

	// Token: 0x04004539 RID: 17721
	private float shakePhase;
}
