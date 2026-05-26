using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020009C0 RID: 2496
public class InfectionLavaController : MonoBehaviour, ITickSystemPost
{
	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x06003FDD RID: 16349 RVA: 0x0015540D File Offset: 0x0015360D
	public static IReadOnlyList<InfectionLavaController> ActiveControllers
	{
		get
		{
			return InfectionLavaController.activeControllers;
		}
	}

	// Token: 0x06003FDE RID: 16350 RVA: 0x00155414 File Offset: 0x00153614
	public static InfectionLavaController GetControllerForZone(GTZone zone)
	{
		for (int i = 0; i < InfectionLavaController.activeControllers.Count; i++)
		{
			if (InfectionLavaController.activeControllers[i].zone == zone)
			{
				return InfectionLavaController.activeControllers[i];
			}
		}
		return null;
	}

	// Token: 0x170005E4 RID: 1508
	// (get) Token: 0x06003FDF RID: 16351 RVA: 0x00155456 File Offset: 0x00153656
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x06003FE0 RID: 16352 RVA: 0x00155460 File Offset: 0x00153660
	private bool IsAuthority
	{
		get
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return true;
			}
			int zoneAuthorityActorNumber = this.GetZoneAuthorityActorNumber();
			if (zoneAuthorityActorNumber == 2147483647)
			{
				return RoomSystem.AmITheHost;
			}
			return zoneAuthorityActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x06003FE1 RID: 16353 RVA: 0x001554A2 File Offset: 0x001536A2
	public bool LavaCurrentlyActivated
	{
		get
		{
			return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
		}
	}

	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x06003FE2 RID: 16354 RVA: 0x001554B2 File Offset: 0x001536B2
	public Plane LavaPlane
	{
		get
		{
			return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x06003FE3 RID: 16355 RVA: 0x001554CF File Offset: 0x001536CF
	public Vector3 SurfaceCenter
	{
		get
		{
			return this.lavaSurfacePlaneTransform.position;
		}
	}

	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003FE4 RID: 16356 RVA: 0x001554DC File Offset: 0x001536DC
	private int PlayerCount
	{
		get
		{
			int result = 1;
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null && instance.currentNetPlayerArray != null)
			{
				result = instance.currentNetPlayerArray.Length;
			}
			return result;
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003FE5 RID: 16357 RVA: 0x0015550C File Offset: 0x0015370C
	private bool InCompetitiveQueue
	{
		get
		{
			return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
		}
	}

	// Token: 0x06003FE6 RID: 16358 RVA: 0x00155530 File Offset: 0x00153730
	private void Awake()
	{
		this.lavaActivationMPB = new MaterialPropertyBlock();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		RoomSystem.OnLavaSyncReceived = (Action<RoomSystem.LavaSyncEventData>)Delegate.Combine(RoomSystem.OnLavaSyncReceived, new Action<RoomSystem.LavaSyncEventData>(this.OnLavaSyncReceived));
	}

	// Token: 0x06003FE7 RID: 16359 RVA: 0x001555BC File Offset: 0x001537BC
	protected void OnEnable()
	{
		InfectionLavaController.activeControllers.Add(this);
		this.VerifyReferences();
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			if (this.volcanoEffects[i] != null)
			{
				this.volcanoEffects[i].PreloadAssets();
			}
		}
		if (this.lavaVolume != null)
		{
			this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
		}
		if (this.lavaActivationProjectileHitNotifier != null)
		{
			this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
		}
		if (this.localPlayerInZone && this.lavaZoneShaderSettings != null && this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			this.lavaZoneShaderSettings.BecomeActiveInstance(false);
		}
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06003FE8 RID: 16360 RVA: 0x0015568C File Offset: 0x0015388C
	protected void OnDisable()
	{
		InfectionLavaController.activeControllers.Remove(this);
		TickSystem<object>.RemovePostTickCallback(this);
		if (this.lavaVolume != null)
		{
			this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
		}
		if (this.lavaActivationProjectileHitNotifier != null)
		{
			this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
		}
		this.ResetLavaState();
	}

	// Token: 0x06003FE9 RID: 16361 RVA: 0x001556FC File Offset: 0x001538FC
	private void VerifyReferences()
	{
		this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
		}
	}

	// Token: 0x06003FEA RID: 16362 RVA: 0x001557B0 File Offset: 0x001539B0
	private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
	{
		if (obj != null)
		{
			return;
		}
		fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
		Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
		base.enabled = false;
	}

	// Token: 0x06003FEB RID: 16363 RVA: 0x00155800 File Offset: 0x00153A00
	private void OnDestroy()
	{
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		RoomSystem.OnLavaSyncReceived = (Action<RoomSystem.LavaSyncEventData>)Delegate.Remove(RoomSystem.OnLavaSyncReceived, new Action<RoomSystem.LavaSyncEventData>(this.OnLavaSyncReceived));
	}

	// Token: 0x06003FEC RID: 16364 RVA: 0x00155880 File Offset: 0x00153A80
	private void ResetLavaState()
	{
		this.reliableState = default(InfectionLavaController.LavaSyncData);
		this.lavaProgressLinear = 0f;
		this.lavaProgressSmooth = 0f;
		this.localLagLavaProgressOffset = 0f;
		this.activationProgessSmooth = 0f;
		this.currentTime = 0.0;
		this.prevTime = 0.0;
		this.lastSyncSendTime = 0.0;
		this.residuePlaneY = this.GetMinLavaY();
		Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, Vector4.zero);
		for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
		{
			this.lavaActivationVotePlayerIds[i] = 0;
		}
		this.lavaActivationVoteCount = 0;
		for (int j = 0; j < this.volcanoEffects.Length; j++)
		{
			VolcanoEffects volcanoEffects = this.volcanoEffects[j];
			if (volcanoEffects != null)
			{
				volcanoEffects.SetDrainedState();
			}
		}
		this.UpdateLava(0f);
		ZoneShaderSettings.ActivateDefaultSettings();
		if (this.localPlayerInZone && this.baseZoneShaderSettings != null)
		{
			this.baseZoneShaderSettings.BecomeActiveInstance(false);
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003FED RID: 16365 RVA: 0x00155988 File Offset: 0x00153B88
	// (set) Token: 0x06003FEE RID: 16366 RVA: 0x00155990 File Offset: 0x00153B90
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x06003FEF RID: 16367 RVA: 0x0015599C File Offset: 0x00153B9C
	void ITickSystemPost.PostTick()
	{
		this.prevTime = this.currentTime;
		this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
		bool flag = this.localPlayerInZone;
		this.localPlayerInZone = this.CheckLocalPlayerInZone();
		if (this.IsAuthority)
		{
			InfectionLavaController.RisingLavaState state = this.reliableState.state;
			this.UpdateReliableState(this.currentTime, ref this.reliableState);
			bool flag2 = this.reliableState.state != state;
			bool flag3 = this.reliableState.state != InfectionLavaController.RisingLavaState.Drained && this.currentTime - this.lastSyncSendTime > 2.0;
			if (flag2 || flag3)
			{
				this.SendSyncEvent();
			}
		}
		else
		{
			this.AdvanceLavaPhaseByTime(this.currentTime, ref this.reliableState);
			this.DrainActivationProgressLocally();
		}
		this.UpdateLocalState(this.currentTime, this.reliableState);
		if (this.localPlayerInZone && !flag)
		{
			if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
			{
				if (this.lavaZoneShaderSettings != null)
				{
					this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				}
			}
			else if (this.baseZoneShaderSettings != null)
			{
				this.baseZoneShaderSettings.BecomeActiveInstance(false);
			}
		}
		else if (!this.localPlayerInZone && flag)
		{
			ZoneShaderSettings.ActivateDefaultSettings();
			Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, Vector4.zero);
		}
		this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
		this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
		this.UpdateResidueState();
		this.UpdateVolcanoActivationLava(this.reliableState.activationProgress);
		this.CheckLocalPlayerAgainstLava(this.currentTime);
	}

	// Token: 0x06003FF0 RID: 16368 RVA: 0x00155B4C File Offset: 0x00153D4C
	private void JumpToState(InfectionLavaController.RisingLavaState state)
	{
		this.reliableState.state = state;
		switch (state)
		{
		case InfectionLavaController.RisingLavaState.Drained:
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				VolcanoEffects volcanoEffects = this.volcanoEffects[i];
				if (volcanoEffects != null)
				{
					volcanoEffects.SetDrainedState();
				}
			}
			if (this.localPlayerInZone)
			{
				ZoneShaderSettings.ActivateDefaultSettings();
				if (this.baseZoneShaderSettings != null)
				{
					this.baseZoneShaderSettings.BecomeActiveInstance(false);
					return;
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Erupting:
			for (int j = 0; j < this.volcanoEffects.Length; j++)
			{
				VolcanoEffects volcanoEffects2 = this.volcanoEffects[j];
				if (volcanoEffects2 != null)
				{
					volcanoEffects2.SetEruptingState();
				}
			}
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			for (int k = 0; k < this.volcanoEffects.Length; k++)
			{
				VolcanoEffects volcanoEffects3 = this.volcanoEffects[k];
				if (volcanoEffects3 != null)
				{
					volcanoEffects3.SetRisingState();
				}
			}
			return;
		case InfectionLavaController.RisingLavaState.Full:
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			for (int l = 0; l < this.volcanoEffects.Length; l++)
			{
				VolcanoEffects volcanoEffects4 = this.volcanoEffects[l];
				if (volcanoEffects4 != null)
				{
					volcanoEffects4.SetFullState();
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			for (int m = 0; m < this.volcanoEffects.Length; m++)
			{
				VolcanoEffects volcanoEffects5 = this.volcanoEffects[m];
				if (volcanoEffects5 != null)
				{
					volcanoEffects5.SetDrainingState();
				}
			}
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003FF1 RID: 16369 RVA: 0x00155D08 File Offset: 0x00153F08
	private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
	{
		if (syncData.stateStartTime - currentTime > (double)this.latencyBuffer + 1.0)
		{
			syncData.stateStartTime = currentTime;
		}
		switch (syncData.state)
		{
		default:
			if (syncData.activationProgress > 1f)
			{
				float playerCount = (float)this.PlayerCount;
				float num = this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue;
				int num2 = Mathf.RoundToInt(playerCount * num);
				if (this.lavaActivationVoteCount >= num2)
				{
					for (int i = 0; i < this.lavaActivationVoteCount; i++)
					{
						this.lavaActivationVotePlayerIds[i] = 0;
					}
					this.lavaActivationVoteCount = 0;
					syncData.stateStartTime = currentTime + (double)this.latencyBuffer;
					syncData.activationProgress = 1f;
					this.JumpToState(InfectionLavaController.RisingLavaState.Erupting);
					return;
				}
			}
			else
			{
				float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
				float activationProgress = syncData.activationProgress;
				syncData.activationProgress = Mathf.MoveTowards(syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
				if (activationProgress > 0f && syncData.activationProgress <= 1E-45f)
				{
					VolcanoEffects[] array = this.volcanoEffects;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].OnVolcanoBellyEmpty();
					}
					return;
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Erupting:
			if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
			{
				syncData.stateStartTime += (double)this.eruptTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Rising);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (currentTime > syncData.stateStartTime + (double)this.riseTime)
			{
				syncData.stateStartTime += (double)this.riseTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Full);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Full:
			if (currentTime > syncData.stateStartTime + (double)this.fullTime)
			{
				syncData.stateStartTime += (double)this.fullTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Draining);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
		{
			float num4 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
			syncData.activationProgress = Mathf.MoveTowards(syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num4);
			if (currentTime > syncData.stateStartTime + (double)this.drainTime)
			{
				syncData.stateStartTime += (double)this.drainTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Drained);
			}
			break;
		}
		}
	}

	// Token: 0x06003FF2 RID: 16370 RVA: 0x00155F60 File Offset: 0x00154160
	private void AdvanceLavaPhaseByTime(double time, ref InfectionLavaController.LavaSyncData syncData)
	{
		if (syncData.stateStartTime - time > (double)this.latencyBuffer + 1.0)
		{
			syncData.stateStartTime = time;
		}
		switch (syncData.state)
		{
		case InfectionLavaController.RisingLavaState.Erupting:
			if (time > syncData.stateStartTime + (double)this.eruptTime)
			{
				syncData.stateStartTime += (double)this.eruptTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Rising);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (time > syncData.stateStartTime + (double)this.riseTime)
			{
				syncData.stateStartTime += (double)this.riseTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Full);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Full:
			if (time > syncData.stateStartTime + (double)this.fullTime)
			{
				syncData.stateStartTime += (double)this.fullTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Draining);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			if (time > syncData.stateStartTime + (double)this.drainTime)
			{
				syncData.stateStartTime += (double)this.drainTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Drained);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003FF3 RID: 16371 RVA: 0x0015605C File Offset: 0x0015425C
	private void DrainActivationProgressLocally()
	{
		if (this.reliableState.activationProgress <= 0f)
		{
			return;
		}
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained && this.reliableState.state != InfectionLavaController.RisingLavaState.Draining)
		{
			return;
		}
		float num = Mathf.Clamp((float)(this.currentTime - this.prevTime), 0f, 0.1f);
		float activationProgress = this.reliableState.activationProgress;
		this.reliableState.activationProgress = Mathf.MoveTowards(this.reliableState.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num);
		if (activationProgress > 0f && this.reliableState.activationProgress <= 1E-45f)
		{
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				VolcanoEffects volcanoEffects = this.volcanoEffects[i];
				if (volcanoEffects != null)
				{
					volcanoEffects.OnVolcanoBellyEmpty();
				}
			}
		}
	}

	// Token: 0x06003FF4 RID: 16372 RVA: 0x00156134 File Offset: 0x00154334
	private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
	{
		switch (syncData.state)
		{
		default:
		{
			this.lavaProgressLinear = 0f;
			this.lavaProgressSmooth = 0f;
			float time = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
			{
				if (volcanoEffects != null)
				{
					volcanoEffects.UpdateDrainedState(time);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		{
			this.lavaProgressLinear = 0f;
			this.lavaProgressSmooth = 0f;
			float num = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float progress = Mathf.Clamp01(num / this.eruptTime);
			foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
			{
				if (volcanoEffects2 != null)
				{
					volcanoEffects2.UpdateEruptingState(num, this.eruptTime - num, progress);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Rising:
		{
			float num2 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float value = num2 / this.riseTime;
			this.lavaProgressLinear = Mathf.Clamp01(value);
			this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
			foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
			{
				if (volcanoEffects3 != null)
				{
					volcanoEffects3.UpdateRisingState(num2, this.riseTime - num2, this.lavaProgressLinear);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Full:
		{
			this.lavaProgressLinear = 1f;
			this.lavaProgressSmooth = 1f;
			float num3 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float progress2 = Mathf.Clamp01(num3 / this.fullTime);
			foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
			{
				if (volcanoEffects4 != null)
				{
					volcanoEffects4.UpdateFullState(num3, this.fullTime - num3, progress2);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Draining:
		{
			float num4 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float num5 = Mathf.Clamp01(num4 / this.drainTime);
			this.lavaProgressLinear = 1f - num5;
			this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
			foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
			{
				if (volcanoEffects5 != null)
				{
					volcanoEffects5.UpdateDrainingState(num4, this.riseTime - num4, num5);
				}
			}
			return;
		}
		}
	}

	// Token: 0x06003FF5 RID: 16373 RVA: 0x001563A4 File Offset: 0x001545A4
	private void UpdateLava(float fillProgress)
	{
		this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
		if (this.lavaMeshTransform != null)
		{
			this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
		}
	}

	// Token: 0x06003FF6 RID: 16374 RVA: 0x00156410 File Offset: 0x00154610
	private float GetMinLavaY()
	{
		if (this.lavaSurfacePlaneTransform == null || this.lavaMeshTransform == null)
		{
			return 0f;
		}
		float z = this.lavaMeshTransform.localScale.z;
		if (z < 0.001f)
		{
			return this.lavaSurfacePlaneTransform.position.y;
		}
		float y = this.lavaMeshTransform.position.y;
		float num = (this.lavaSurfacePlaneTransform.position.y - y) * (this.lavaMeshMinScale / z);
		return y + num;
	}

	// Token: 0x06003FF7 RID: 16375 RVA: 0x0015649C File Offset: 0x0015469C
	private void UpdateResidueState()
	{
		float num = (this.lavaSurfacePlaneTransform != null) ? this.lavaSurfacePlaneTransform.position.y : 0f;
		switch (this.reliableState.state)
		{
		case InfectionLavaController.RisingLavaState.Drained:
		{
			float minLavaY = this.GetMinLavaY();
			this.residuePlaneY = Mathf.MoveTowards(this.residuePlaneY, minLavaY, this.residueDrainSpeed * Time.deltaTime);
			break;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		case InfectionLavaController.RisingLavaState.Rising:
		case InfectionLavaController.RisingLavaState.Full:
			this.residuePlaneY = num;
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			this.residuePlaneY = Mathf.MoveTowards(this.residuePlaneY, num, this.residueDrainSpeed * Time.deltaTime);
			this.residuePlaneY = Mathf.Max(this.residuePlaneY, num);
			break;
		}
		if (this.localPlayerInZone)
		{
			float minLavaY2 = this.GetMinLavaY();
			float y = (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained || this.residuePlaneY > minLavaY2 + 0.01f) ? this.residueIntensity : 0f;
			Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, new Vector4(this.residuePlaneY + this.residueOffset, y, this.residueUVScale, 0f));
		}
	}

	// Token: 0x06003FF8 RID: 16376 RVA: 0x001565C0 File Offset: 0x001547C0
	private void UpdateVolcanoActivationLava(float activationProgress)
	{
		if (this.lavaActivationRenderer == null)
		{
			return;
		}
		this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
		this.lavaActivationMPB.SetColor(ShaderProps._BaseColor, this.lavaActivationGradient.Evaluate(this.activationProgessSmooth));
		this.lavaActivationRenderer.SetPropertyBlock(this.lavaActivationMPB);
		this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
	}

	// Token: 0x06003FF9 RID: 16377 RVA: 0x0015665D File Offset: 0x0015485D
	private void CheckLocalPlayerAgainstLava(double currentTime)
	{
		if (GTPlayer.Instance.InWater && GTPlayer.Instance.CurrentWaterVolume == this.lavaVolume)
		{
			this.LocalPlayerInLava(currentTime, false);
		}
	}

	// Token: 0x06003FFA RID: 16378 RVA: 0x0015668A File Offset: 0x0015488A
	private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
	{
		if (collider == GTPlayer.Instance.bodyCollider)
		{
			this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
		}
	}

	// Token: 0x06003FFB RID: 16379 RVA: 0x001566C4 File Offset: 0x001548C4
	private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
	{
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null && instance.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
		{
			this.lastTagSelfRPCTime = currentTime;
			GameMode.ReportHit();
		}
	}

	// Token: 0x06003FFC RID: 16380 RVA: 0x00156718 File Offset: 0x00154918
	public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!projectile.gameObject.CompareTag("LavaRockProjectile"))
		{
			return;
		}
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			return;
		}
		if (this.IsAuthority)
		{
			this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			return;
		}
		this.reliableState.activationProgress = this.reliableState.activationProgress + this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.volcanoEffects[i].OnStoneAccepted(this.reliableState.activationProgress);
		}
	}

	// Token: 0x06003FFD RID: 16381 RVA: 0x001567AC File Offset: 0x001549AC
	private void AddLavaRock(int playerId)
	{
		float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
		this.reliableState.activationProgress = this.reliableState.activationProgress + num;
		this.AddVoteForVolcanoActivation(playerId);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.volcanoEffects[i].OnStoneAccepted(this.reliableState.activationProgress);
		}
		this.SendSyncEvent();
	}

	// Token: 0x06003FFE RID: 16382 RVA: 0x00156814 File Offset: 0x00154A14
	private void AddVoteForVolcanoActivation(int playerId)
	{
		if (this.IsAuthority && this.lavaActivationVoteCount < 20)
		{
			bool flag = false;
			for (int i = 0; i < this.lavaActivationVoteCount; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] == playerId)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
				this.lavaActivationVoteCount++;
			}
		}
	}

	// Token: 0x06003FFF RID: 16383 RVA: 0x00156874 File Offset: 0x00154A74
	private void RemoveVoteForVolcanoActivation(int playerId)
	{
		if (this.IsAuthority)
		{
			for (int i = 0; i < this.lavaActivationVoteCount; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] == playerId)
				{
					this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
					this.lavaActivationVoteCount--;
					return;
				}
			}
		}
	}

	// Token: 0x06004000 RID: 16384 RVA: 0x001568CC File Offset: 0x00154ACC
	private void SendSyncEvent()
	{
		this.lastSyncSendTime = this.currentTime;
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		RoomSystem.SendLavaSync((byte)this.zone, (byte)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.activationProgress, this.lavaActivationVoteCount, this.lavaActivationVotePlayerIds);
	}

	// Token: 0x06004001 RID: 16385 RVA: 0x0015692C File Offset: 0x00154B2C
	private void SendSyncEventToPlayer(NetPlayer target)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		RoomSystem.SendLavaSyncToPlayer((byte)this.zone, (byte)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.activationProgress, this.lavaActivationVoteCount, this.lavaActivationVotePlayerIds, target);
	}

	// Token: 0x06004002 RID: 16386 RVA: 0x00156984 File Offset: 0x00154B84
	private unsafe void OnLavaSyncReceived(RoomSystem.LavaSyncEventData data)
	{
		if (data.zone != (byte)this.zone || this.IsAuthority)
		{
			return;
		}
		int zoneAuthorityActorNumber = this.GetZoneAuthorityActorNumber();
		if (zoneAuthorityActorNumber != 2147483647 && data.senderActorNumber != zoneAuthorityActorNumber)
		{
			return;
		}
		InfectionLavaController.RisingLavaState state = (InfectionLavaController.RisingLavaState)data.state;
		float num = this.lavaProgressSmooth;
		this.reliableState.stateStartTime = data.stateStartTime;
		this.reliableState.activationProgress = data.activationProgress;
		this.lavaActivationVoteCount = data.voteCount;
		for (int i = 0; i < 20; i++)
		{
			this.lavaActivationVotePlayerIds[i] = *(ref data.votes.FixedElementField + (IntPtr)i * 4);
		}
		if (state != this.reliableState.state)
		{
			this.JumpToState(state);
		}
		this.UpdateLocalState(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, this.reliableState);
		this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
	}

	// Token: 0x06004003 RID: 16387 RVA: 0x00156A71 File Offset: 0x00154C71
	private void OnPlayerJoinedRoom(NetPlayer player)
	{
		if (!this.IsAuthority)
		{
			return;
		}
		this.SendSyncEventToPlayer(player);
	}

	// Token: 0x06004004 RID: 16388 RVA: 0x00156A84 File Offset: 0x00154C84
	public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
	{
		this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			if (this.IsAuthority)
			{
				this.SendSyncEvent();
			}
		}
	}

	// Token: 0x06004005 RID: 16389 RVA: 0x00156ADC File Offset: 0x00154CDC
	private void OnLeftRoom()
	{
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			double num = this.currentTime - this.reliableState.stateStartTime;
			double timeAsDouble = Time.timeAsDouble;
			this.reliableState.stateStartTime = timeAsDouble - num;
			this.currentTime = timeAsDouble;
			this.prevTime = timeAsDouble;
			this.lastSyncSendTime = 0.0;
			for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
			{
				this.lavaActivationVotePlayerIds[i] = 0;
			}
			this.lavaActivationVoteCount = 0;
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
		if (this.baseZoneShaderSettings != null)
		{
			this.baseZoneShaderSettings.BecomeActiveInstance(false);
		}
		this.ResetLavaState();
	}

	// Token: 0x06004006 RID: 16390 RVA: 0x00156B84 File Offset: 0x00154D84
	private int CountRigsInZone()
	{
		int num = 0;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if (activeRigs[i] != null && activeRigs[i].zoneEntity.currentZone == this.zone)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06004007 RID: 16391 RVA: 0x00156BD8 File Offset: 0x00154DD8
	private bool CheckLocalPlayerInZone()
	{
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if (activeRigs[i] != null && activeRigs[i].isLocal)
			{
				return activeRigs[i].zoneEntity.currentZone == this.zone;
			}
		}
		return false;
	}

	// Token: 0x06004008 RID: 16392 RVA: 0x00156C34 File Offset: 0x00154E34
	private int GetZoneAuthorityActorNumber()
	{
		int num = int.MaxValue;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			VRRig vrrig = activeRigs[i];
			if (!(vrrig == null) && vrrig.zoneEntity.currentZone == this.zone)
			{
				int actorNumber;
				if (vrrig.isLocal)
				{
					actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
				}
				else
				{
					NetPlayer creator = vrrig.Creator;
					if (creator == null)
					{
						goto IL_6C;
					}
					actorNumber = creator.ActorNumber;
				}
				if (actorNumber < num)
				{
					num = actorNumber;
				}
			}
			IL_6C:;
		}
		return num;
	}

	// Token: 0x0400504B RID: 20555
	[OnEnterPlay_SetNew]
	private static readonly List<InfectionLavaController> activeControllers = new List<InfectionLavaController>();

	// Token: 0x0400504C RID: 20556
	[SerializeField]
	private GTZone zone;

	// Token: 0x0400504D RID: 20557
	[SerializeField]
	private float lavaMeshMinScale = 3.17f;

	// Token: 0x0400504E RID: 20558
	[Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
	[SerializeField]
	private float lavaMeshMaxScale = 8.941086f;

	// Token: 0x0400504F RID: 20559
	[SerializeField]
	private float eruptTime = 3f;

	// Token: 0x04005050 RID: 20560
	[SerializeField]
	private float riseTime = 10f;

	// Token: 0x04005051 RID: 20561
	[SerializeField]
	private float fullTime = 240f;

	// Token: 0x04005052 RID: 20562
	[SerializeField]
	private float drainTime = 10f;

	// Token: 0x04005053 RID: 20563
	[Tooltip("Delay added when starting the eruption cycle so the sync event has time to reach other clients before visuals begin.")]
	[SerializeField]
	private float latencyBuffer = 0.5f;

	// Token: 0x04005054 RID: 20564
	[SerializeField]
	private float lagResolutionLavaProgressPerSecond = 0.2f;

	// Token: 0x04005055 RID: 20565
	[SerializeField]
	private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005056 RID: 20566
	[Header("Volcano Activation")]
	[SerializeField]
	[Range(0f, 1f)]
	private float activationVotePercentageDefaultQueue = 0.42f;

	// Token: 0x04005057 RID: 20567
	[SerializeField]
	[Range(0f, 1f)]
	private float activationVotePercentageCompetitiveQueue = 0.6f;

	// Token: 0x04005058 RID: 20568
	[SerializeField]
	private Gradient lavaActivationGradient;

	// Token: 0x04005059 RID: 20569
	[SerializeField]
	private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400505A RID: 20570
	[SerializeField]
	private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400505B RID: 20571
	[SerializeField]
	private float lavaActivationVisualMovementProgressPerSecond = 1f;

	// Token: 0x0400505C RID: 20572
	[SerializeField]
	private bool debugLavaActivationVotes;

	// Token: 0x0400505D RID: 20573
	[Header("Scene References")]
	[SerializeField]
	private Transform lavaMeshTransform;

	// Token: 0x0400505E RID: 20574
	[SerializeField]
	private Transform lavaSurfacePlaneTransform;

	// Token: 0x0400505F RID: 20575
	[SerializeField]
	private WaterVolume lavaVolume;

	// Token: 0x04005060 RID: 20576
	[SerializeField]
	private MeshRenderer lavaActivationRenderer;

	// Token: 0x04005061 RID: 20577
	[SerializeField]
	private Transform lavaActivationStartPos;

	// Token: 0x04005062 RID: 20578
	[SerializeField]
	private Transform lavaActivationEndPos;

	// Token: 0x04005063 RID: 20579
	[SerializeField]
	private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

	// Token: 0x04005064 RID: 20580
	[SerializeField]
	private VolcanoEffects[] volcanoEffects;

	// Token: 0x04005065 RID: 20581
	[SerializeField]
	private ZoneShaderSettings lavaZoneShaderSettings;

	// Token: 0x04005066 RID: 20582
	[SerializeField]
	private ZoneShaderSettings baseZoneShaderSettings;

	// Token: 0x04005067 RID: 20583
	[DebugReadout]
	private InfectionLavaController.LavaSyncData reliableState;

	// Token: 0x04005068 RID: 20584
	private readonly int[] lavaActivationVotePlayerIds = new int[20];

	// Token: 0x04005069 RID: 20585
	private int lavaActivationVoteCount;

	// Token: 0x0400506A RID: 20586
	private float localLagLavaProgressOffset;

	// Token: 0x0400506B RID: 20587
	[DebugReadout]
	private float lavaProgressLinear;

	// Token: 0x0400506C RID: 20588
	[DebugReadout]
	private float lavaProgressSmooth;

	// Token: 0x0400506D RID: 20589
	private double lastTagSelfRPCTime;

	// Token: 0x0400506E RID: 20590
	private const string lavaRockProjectileTag = "LavaRockProjectile";

	// Token: 0x0400506F RID: 20591
	private double currentTime;

	// Token: 0x04005070 RID: 20592
	private double prevTime;

	// Token: 0x04005071 RID: 20593
	private float activationProgessSmooth;

	// Token: 0x04005072 RID: 20594
	private float lavaScale;

	// Token: 0x04005073 RID: 20595
	private MaterialPropertyBlock lavaActivationMPB;

	// Token: 0x04005074 RID: 20596
	private double lastSyncSendTime;

	// Token: 0x04005075 RID: 20597
	private const double syncInterval = 2.0;

	// Token: 0x04005076 RID: 20598
	private bool localPlayerInZone;

	// Token: 0x04005077 RID: 20599
	private static readonly int _shaderProp_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

	// Token: 0x04005078 RID: 20600
	private static readonly int _shaderProp_GlobalLavaResidueParams = Shader.PropertyToID("_GlobalLavaResidueParams");

	// Token: 0x04005079 RID: 20601
	[Header("Lava Residue")]
	[SerializeField]
	[Range(0f, 1f)]
	private float residueIntensity = 0.85f;

	// Token: 0x0400507A RID: 20602
	[Tooltip("How fast the residue plane trails behind the lava when draining (world units/sec).")]
	[SerializeField]
	private float residueDrainSpeed = 1.5f;

	// Token: 0x0400507B RID: 20603
	[Tooltip("UV scale for the residue texture in world space.")]
	[SerializeField]
	private float residueUVScale = 0.25f;

	// Token: 0x0400507C RID: 20604
	[SerializeField]
	private float residueOffset = 2f;

	// Token: 0x0400507D RID: 20605
	private float residuePlaneY;

	// Token: 0x020009C1 RID: 2497
	public enum RisingLavaState
	{
		// Token: 0x04005080 RID: 20608
		Drained,
		// Token: 0x04005081 RID: 20609
		Erupting,
		// Token: 0x04005082 RID: 20610
		Rising,
		// Token: 0x04005083 RID: 20611
		Full,
		// Token: 0x04005084 RID: 20612
		Draining
	}

	// Token: 0x020009C2 RID: 2498
	private struct LavaSyncData
	{
		// Token: 0x04005085 RID: 20613
		public InfectionLavaController.RisingLavaState state;

		// Token: 0x04005086 RID: 20614
		public double stateStartTime;

		// Token: 0x04005087 RID: 20615
		public float activationProgress;
	}
}
