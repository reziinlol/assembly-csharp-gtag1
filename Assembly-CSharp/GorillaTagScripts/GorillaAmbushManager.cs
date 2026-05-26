using System;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F0A RID: 3850
	public sealed class GorillaAmbushManager : GorillaTagManager
	{
		// Token: 0x06005FF7 RID: 24567 RVA: 0x001EF448 File Offset: 0x001ED648
		public override GameModeType GameType()
		{
			if (!this.isGhostTag)
			{
				return GameModeType.Ambush;
			}
			return GameModeType.Ghost;
		}

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06005FF8 RID: 24568 RVA: 0x001EF455 File Offset: 0x001ED655
		public static int HandEffectHash
		{
			get
			{
				return GorillaAmbushManager.handTapHash;
			}
		}

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06005FF9 RID: 24569 RVA: 0x001EF45C File Offset: 0x001ED65C
		// (set) Token: 0x06005FFA RID: 24570 RVA: 0x001EF463 File Offset: 0x001ED663
		public static float HandFXScaleModifier { get; private set; }

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06005FFB RID: 24571 RVA: 0x001EF46B File Offset: 0x001ED66B
		// (set) Token: 0x06005FFC RID: 24572 RVA: 0x001EF473 File Offset: 0x001ED673
		public bool isGhostTag { get; private set; }

		// Token: 0x06005FFD RID: 24573 RVA: 0x001EF47C File Offset: 0x001ED67C
		public override void Awake()
		{
			base.Awake();
			if (this.handTapFX != null)
			{
				GorillaAmbushManager.handTapHash = PoolUtils.GameObjHashCode(this.handTapFX);
			}
			GorillaAmbushManager.HandFXScaleModifier = this.handTapScaleFactor;
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x001EF4AD File Offset: 0x001ED6AD
		private void Start()
		{
			this.hasScryingPlane = this.scryingPlaneRef.TryResolve<MeshRenderer>(out this.scryingPlane);
			this.hasScryingPlane3p = this.scryingPlane3pRef.TryResolve<MeshRenderer>(out this.scryingPlane3p);
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x001EF4DD File Offset: 0x001ED6DD
		public override string GameModeName()
		{
			if (!this.isGhostTag)
			{
				return "AMBUSH";
			}
			return "GHOST";
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x001EF4F4 File Offset: 0x001ED6F4
		public override string GameModeNameRoomLabel()
		{
			string text = this.isGhostTag ? "GAME_MODE_GHOST_ROOM_LABEL" : "GAME_MODE_AMBUSH_ROOM_LABEL";
			string defaultResult = this.isGhostTag ? "(GHOST GAME)" : "(AMBUSH GAME)";
			string result;
			if (!LocalisationManager.TryGetKeyForCurrentLocale(text, out result, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [" + text + "]");
			}
			return result;
		}

		// Token: 0x06006001 RID: 24577 RVA: 0x001EF54C File Offset: 0x001ED74C
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			int materialIndex = this.MyMatIndex(rig.creator);
			rig.ChangeMaterialLocal(materialIndex);
			bool flag = base.IsInfected(rig.Creator);
			bool flag2 = base.IsInfected(NetworkSystem.Instance.LocalPlayer);
			rig.bodyRenderer.SetGameModeBodyType(flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default);
			rig.SetInvisibleToLocalPlayer(flag && !flag2);
			if (this.isGhostTag && rig.isOfflineVRRig)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(flag);
				if (this.hasScryingPlane)
				{
					this.scryingPlane.enabled = flag2;
				}
				if (this.hasScryingPlane3p)
				{
					this.scryingPlane3p.enabled = flag2;
				}
			}
		}

		// Token: 0x06006002 RID: 24578 RVA: 0x001EF5F2 File Offset: 0x001ED7F2
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (!base.IsInfected(forPlayer))
			{
				return 0;
			}
			return 13;
		}

		// Token: 0x06006003 RID: 24579 RVA: 0x001EF604 File Offset: 0x001ED804
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				GorillaSkin.ApplyToRig(rig, null, GorillaSkin.SkinType.gameMode);
				rig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
				rig.SetInvisibleToLocalPlayer(false);
			}
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (this.hasScryingPlane)
			{
				this.scryingPlane.enabled = false;
			}
			if (this.hasScryingPlane3p)
			{
				this.scryingPlane3p.enabled = false;
			}
		}

		// Token: 0x04006E8F RID: 28303
		public GameObject handTapFX;

		// Token: 0x04006E90 RID: 28304
		public GorillaSkin ambushSkin;

		// Token: 0x04006E91 RID: 28305
		[SerializeField]
		private AudioClip[] firstPersonTaggedSounds;

		// Token: 0x04006E92 RID: 28306
		[SerializeField]
		private float firstPersonTaggedSoundVolume;

		// Token: 0x04006E93 RID: 28307
		private static int handTapHash = -1;

		// Token: 0x04006E94 RID: 28308
		public float handTapScaleFactor = 0.5f;

		// Token: 0x04006E96 RID: 28310
		public float crawlingSpeedForMaxVolume;

		// Token: 0x04006E98 RID: 28312
		[SerializeField]
		private XSceneRef scryingPlaneRef;

		// Token: 0x04006E99 RID: 28313
		[SerializeField]
		private XSceneRef scryingPlane3pRef;

		// Token: 0x04006E9A RID: 28314
		private const int STEALTH_MATERIAL_INDEX = 13;

		// Token: 0x04006E9B RID: 28315
		private MeshRenderer scryingPlane;

		// Token: 0x04006E9C RID: 28316
		private bool hasScryingPlane;

		// Token: 0x04006E9D RID: 28317
		private MeshRenderer scryingPlane3p;

		// Token: 0x04006E9E RID: 28318
		private bool hasScryingPlane3p;
	}
}
