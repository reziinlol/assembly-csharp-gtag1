using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200085D RID: 2141
public sealed class GorillaGuardianManager : GorillaGameManager
{
	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x06003799 RID: 14233 RVA: 0x0012FDAB File Offset: 0x0012DFAB
	// (set) Token: 0x0600379A RID: 14234 RVA: 0x0012FDB3 File Offset: 0x0012DFB3
	public bool isPlaying { get; private set; }

	// Token: 0x0600379B RID: 14235 RVA: 0x0012FDBC File Offset: 0x0012DFBC
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.isPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StartPlaying();
			}
		}
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x0012FE20 File Offset: 0x0012E020
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.isPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StopPlaying();
			}
		}
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x0012FE84 File Offset: 0x0012E084
	public override void ResetGame()
	{
		base.ResetGame();
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x0012FE8C File Offset: 0x0012E08C
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GuardianRPCs>();
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x00035D0D File Offset: 0x00033F0D
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x0012FE9C File Offset: 0x0012E09C
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this.IsPlayerGuardian(myPlayer) && !this.IsHoldingPlayer();
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalIsTagged(NetPlayer player)
	{
		return false;
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x0012FEB2 File Offset: 0x0012E0B2
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return player != null && !this.IsPlayerGuardian(player);
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x0012FEC4 File Offset: 0x0012E0C4
	public bool IsPlayerGuardian(NetPlayer player)
	{
		using (List<GorillaGuardianZoneManager>.Enumerator enumerator = GorillaGuardianZoneManager.zoneManagers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsPlayerGuardian(player))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x0012FF20 File Offset: 0x0012E120
	public void RequestEjectGuardian(NetPlayer player)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.EjectGuardian(player);
			return;
		}
		GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianRequestEject", false, Array.Empty<object>());
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0012FF48 File Offset: 0x0012E148
	public void EjectGuardian(NetPlayer player)
	{
		foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
		{
			if (gorillaGuardianZoneManager.IsPlayerGuardian(player))
			{
				gorillaGuardianZoneManager.SetGuardian(null);
			}
		}
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x0012FFA4 File Offset: 0x0012E1A4
	public void LaunchPlayer(NetPlayer launcher, Vector3 velocity)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(launcher, out rigContainer))
		{
			return;
		}
		if (Vector3.Magnitude(VRRigCache.Instance.localRig.Rig.transform.position - rigContainer.Rig.transform.position) > this.requiredGuardianDistance + Mathf.Epsilon)
		{
			return;
		}
		if (velocity.sqrMagnitude > this.maxLaunchVelocity * this.maxLaunchVelocity)
		{
			return;
		}
		GTPlayer.Instance.DoLaunch(velocity);
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x00130028 File Offset: 0x0012E228
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		base.LocalTag(taggedPlayer, taggingPlayer, bodyHit, leftHand);
		if (bodyHit)
		{
			return;
		}
		RigContainer rigContainer;
		Vector3 vector;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && this.CheckSlap(taggingPlayer, taggedPlayer, leftHand, out vector))
		{
			GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", taggedPlayer, new object[]
			{
				vector
			});
			rigContainer.Rig.ApplyLocalTrajectoryOverride(vector);
			GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlapEffects", true, new object[]
			{
				rigContainer.Rig.transform.position,
				vector.normalized
			});
			this.LocalPlaySlapEffect(rigContainer.Rig.transform.position, vector.normalized);
		}
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x001300EC File Offset: 0x0012E2EC
	private bool CheckSlap(NetPlayer slapper, NetPlayer target, bool leftHand, out Vector3 velocity)
	{
		velocity = Vector3.zero;
		if (this.IsHoldingPlayer(leftHand))
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(slapper, out rigContainer))
		{
			return false;
		}
		Vector3 vector = GTPlayer.Instance.GetHandVelocityTracker(leftHand).GetAverageVelocity(true, 0.15f, false);
		Vector3 rhs = leftHand ? rigContainer.Rig.leftHandHoldsPlayer.transform.right : rigContainer.Rig.rightHandHoldsPlayer.transform.right;
		if (Vector3.Dot(vector.normalized, rhs) < this.slapFrontAlignmentThreshold && Vector3.Dot(vector.normalized, rhs) > this.slapBackAlignmentThreshold)
		{
			return false;
		}
		if (vector.magnitude < this.launchMinimumStrength)
		{
			return false;
		}
		vector = Vector3.ClampMagnitude(vector, this.maxLaunchVelocity);
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(target, out rigContainer2))
		{
			return false;
		}
		if (this.IsRigBeingHeld(rigContainer2.Rig) || rigContainer2.Rig.IsLocalTrajectoryOverrideActive())
		{
			return false;
		}
		if (!this.CheckLaunchRetriggerDelay(rigContainer2.Rig))
		{
			return false;
		}
		vector *= this.launchStrengthMultiplier;
		Vector3 vector2;
		if (rigContainer2.Rig.IsOnGround(this.launchGroundHeadCheckDist, this.launchGroundHandCheckDist, out vector2))
		{
			vector += vector2 * this.launchGroundKickup * Mathf.Clamp01(1f - Vector3.Dot(vector2, vector.normalized));
		}
		velocity = vector;
		return true;
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x00130258 File Offset: 0x0012E458
	public override void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
		base.HandleHandTap(tappingPlayer, hitTappable, leftHand, handVelocity, tapSurfaceNormal);
		if (hitTappable != null)
		{
			TappableGuardianIdol tappableGuardianIdol = hitTappable as TappableGuardianIdol;
			if (tappableGuardianIdol != null && tappableGuardianIdol.isActivationReady)
			{
				tappableGuardianIdol.isActivationReady = false;
				GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tapHapticStrength * this.hapticStrength, GorillaTagger.Instance.tapHapticDuration * this.hapticDuration);
			}
		}
		if (!this.IsPlayerGuardian(tappingPlayer))
		{
			return;
		}
		if (this.IsHoldingPlayer(leftHand))
		{
			return;
		}
		float num = Vector3.Dot(Vector3.down, handVelocity);
		if (num < this.slamTriggerTapSpeed || Vector3.Dot(Vector3.down, handVelocity.normalized) < this.slamTriggerAngle)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(tappingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand;
		Vector3 b = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * rigContainer.Rig.scaleFactor;
		Vector3 vector = vrmap.rigTarget.position - b;
		float num2 = Mathf.Clamp01((num - this.slamTriggerTapSpeed) / (this.slamMaxTapSpeed - this.slamTriggerTapSpeed));
		num2 = Mathf.Lerp(this.slamMinStrengthMultiplier, this.slamMaxStrengthMultiplier, num2);
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer2;
			if (RoomSystem.PlayersInRoom[i] != tappingPlayer && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer2))
			{
				VRRig rig = rigContainer2.Rig;
				if (!this.IsRigBeingHeld(rig) && this.CheckLaunchRetriggerDelay(rig))
				{
					Vector3 position = rig.transform.position;
					if (Vector3.SqrMagnitude(position - vector) < this.slamRadius * this.slamRadius)
					{
						Vector3 vector2 = (position - vector).normalized * num2;
						vector2 = Vector3.ClampMagnitude(vector2, this.maxLaunchVelocity);
						GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", RoomSystem.PlayersInRoom[i], new object[]
						{
							vector2
						});
					}
				}
			}
		}
		this.LocalPlaySlamEffect(vector, Vector3.up);
		GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlamEffect", true, new object[]
		{
			vector,
			Vector3.up
		});
	}

	// Token: 0x060037AC RID: 14252 RVA: 0x001304CA File Offset: 0x0012E6CA
	private bool CheckLaunchRetriggerDelay(VRRig launchedRig)
	{
		return launchedRig.fxSettings.callSettings[7].CallLimitSettings.CheckCallTime(Time.time);
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x001304E8 File Offset: 0x0012E6E8
	private bool IsHoldingPlayer()
	{
		return this.IsHoldingPlayer(true) || this.IsHoldingPlayer(false);
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x001304FC File Offset: 0x0012E6FC
	private bool IsHoldingPlayer(bool leftHand)
	{
		return (leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment is HoldableHand) || (!leftHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment is HoldableHand);
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x00130558 File Offset: 0x0012E758
	private bool IsRigBeingHeld(VRRig rig)
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment != null)
		{
			HoldableHand holdableHand = EquipmentInteractor.instance.leftHandHeldEquipment as HoldableHand;
			if (holdableHand != null && holdableHand.Rig == rig)
			{
				return true;
			}
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null)
		{
			HoldableHand holdableHand2 = EquipmentInteractor.instance.rightHandHeldEquipment as HoldableHand;
			if (holdableHand2 != null && holdableHand2.Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x001305CC File Offset: 0x0012E7CC
	public override GameModeType GameType()
	{
		return GameModeType.Guardian;
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x001305CF File Offset: 0x0012E7CF
	public override string GameModeName()
	{
		return "GUARDIAN";
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x001305D8 File Offset: 0x0012E7D8
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_GUARDIAN_ROOM_LABEL", out result, "(GUARDIAN GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_GUARDIAN_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x060037B5 RID: 14261 RVA: 0x00130603 File Offset: 0x0012E803
	public void PlaySlapEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlapEffect(location, direction);
	}

	// Token: 0x060037B6 RID: 14262 RVA: 0x0013060D File Offset: 0x0012E80D
	private void LocalPlaySlapEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slapImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x060037B7 RID: 14263 RVA: 0x00130628 File Offset: 0x0012E828
	public void PlaySlamEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlamEffect(location, direction);
	}

	// Token: 0x060037B8 RID: 14264 RVA: 0x00130632 File Offset: 0x0012E832
	private void LocalPlaySlamEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slamImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x0400479C RID: 18332
	[Space]
	[SerializeField]
	private float slapFrontAlignmentThreshold = 0.7f;

	// Token: 0x0400479D RID: 18333
	[SerializeField]
	private float slapBackAlignmentThreshold = 0.7f;

	// Token: 0x0400479E RID: 18334
	[SerializeField]
	private float launchMinimumStrength = 6f;

	// Token: 0x0400479F RID: 18335
	[SerializeField]
	private float launchStrengthMultiplier = 1f;

	// Token: 0x040047A0 RID: 18336
	[SerializeField]
	private float launchGroundHeadCheckDist = 1.2f;

	// Token: 0x040047A1 RID: 18337
	[SerializeField]
	private float launchGroundHandCheckDist = 0.4f;

	// Token: 0x040047A2 RID: 18338
	[SerializeField]
	private float launchGroundKickup = 3f;

	// Token: 0x040047A3 RID: 18339
	[Space]
	[SerializeField]
	private float slamTriggerTapSpeed = 7f;

	// Token: 0x040047A4 RID: 18340
	[SerializeField]
	private float slamMaxTapSpeed = 16f;

	// Token: 0x040047A5 RID: 18341
	[SerializeField]
	private float slamTriggerAngle = 0.7f;

	// Token: 0x040047A6 RID: 18342
	[SerializeField]
	private float slamRadius = 2.4f;

	// Token: 0x040047A7 RID: 18343
	[SerializeField]
	private float slamMinStrengthMultiplier = 3f;

	// Token: 0x040047A8 RID: 18344
	[SerializeField]
	private float slamMaxStrengthMultiplier = 10f;

	// Token: 0x040047A9 RID: 18345
	[Space]
	[SerializeField]
	private GameObject slapImpactPrefab;

	// Token: 0x040047AA RID: 18346
	[SerializeField]
	private GameObject slamImpactPrefab;

	// Token: 0x040047AB RID: 18347
	[Space]
	[SerializeField]
	private float hapticStrength = 1f;

	// Token: 0x040047AC RID: 18348
	[SerializeField]
	private float hapticDuration = 1f;

	// Token: 0x040047AE RID: 18350
	private float requiredGuardianDistance = 10f;

	// Token: 0x040047AF RID: 18351
	private float maxLaunchVelocity = 20f;
}
