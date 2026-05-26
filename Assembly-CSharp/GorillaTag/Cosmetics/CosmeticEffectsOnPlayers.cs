using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200122D RID: 4653
	public class CosmeticEffectsOnPlayers : MonoBehaviour, ISpawnable
	{
		// Token: 0x06007459 RID: 29785 RVA: 0x002615E4 File Offset: 0x0025F7E4
		private bool ShouldAffectRig(VRRig rig, CosmeticEffectsOnPlayers.TargetType target)
		{
			bool flag = rig == this.myRig;
			bool result;
			switch (target)
			{
			case CosmeticEffectsOnPlayers.TargetType.Owner:
				result = flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.Others:
				result = !flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.All:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x0600745A RID: 29786 RVA: 0x00261624 File Offset: 0x0025F824
		private void Awake()
		{
			foreach (CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect in this.allEffects)
			{
				this.allEffectsDict.TryAdd(cosmeticEffect.effectType, cosmeticEffect);
			}
		}

		// Token: 0x0600745B RID: 29787 RVA: 0x00261660 File Offset: 0x0025F860
		public void SetKnockbackStrengthMultiplier(float value)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				keyValuePair.Value.knockbackStrengthMultiplier = value;
			}
		}

		// Token: 0x0600745C RID: 29788 RVA: 0x002616BC File Offset: 0x0025F8BC
		public void ApplyAllEffects()
		{
			this.ApplyAllEffectsByDistance(base.transform.position);
		}

		// Token: 0x0600745D RID: 29789 RVA: 0x002616CF File Offset: 0x0025F8CF
		public void ApplyAllEffectsByDistance(Transform _transform)
		{
			this.ApplyAllEffectsByDistance(_transform.position);
		}

		// Token: 0x0600745E RID: 29790 RVA: 0x002616E0 File Offset: 0x0025F8E0
		public void ApplyAllEffectsByDistance(Vector3 position)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
			{
				switch (effect.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxByDistance(effect, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
					this.PlayVFXByDistance(effect, position);
					break;
				}
			}
		}

		// Token: 0x0600745F RID: 29791 RVA: 0x00261788 File Offset: 0x0025F988
		public void ApplyAllEffectsForRig(VRRig rig)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
			{
				switch (effect.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride:
					this.ApplyVOForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxForRig(effect, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
					this.PlayVFXForRig(effect, rig);
					break;
				}
			}
		}

		// Token: 0x06007460 RID: 29792 RVA: 0x0026183C File Offset: 0x0025FA3C
		private void ApplySkinByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnSkinEffects(effect);
				}
			}
		}

		// Token: 0x06007461 RID: 29793 RVA: 0x00261928 File Offset: 0x0025FB28
		private void ApplySkinForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnSkinEffects(effect);
		}

		// Token: 0x06007462 RID: 29794 RVA: 0x00261998 File Offset: 0x0025FB98
		private void ApplyTagWithKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.EnableHitWithKnockBack(effect);
		}

		// Token: 0x06007463 RID: 29795 RVA: 0x00261A08 File Offset: 0x0025FC08
		private void ApplyTagWithKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.EnableHitWithKnockBack(effect);
				}
			}
		}

		// Token: 0x06007464 RID: 29796 RVA: 0x00261AF4 File Offset: 0x0025FCF4
		private void ApplyInstantKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = vrRig.transform.position - base.transform.position;
			float num = (1f / vector.magnitude * effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
			if (effect.Value.applyScaleToKnockbackStrength)
			{
				num *= vrRig.scaleFactor;
			}
			RoomSystem.HitPlayer(vrRig.creator, vector.normalized, num);
			vrRig.ApplyInstanceKnockBack(effect);
		}

		// Token: 0x06007465 RID: 29797 RVA: 0x00261BF0 File Offset: 0x0025FDF0
		private void ApplyInstantKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(GorillaTagger.Instance.offlineVRRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (GorillaTagger.Instance.offlineVRRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = GorillaTagger.Instance.offlineVRRig.transform.position - position;
			if (vector.IsShorterThan(effect.Value.effectDistanceRadius))
			{
				float magnitude = vector.magnitude;
				GTPlayer instance = GTPlayer.Instance;
				if (effect.Value.specialVerticalForce && (instance.IsHandTouching(true) || instance.IsHandTouching(false) || instance.BodyOnGround))
				{
					Vector3 vector2 = -Physics.gravity.normalized;
					Vector3 vector3 = Vector3.ProjectOnPlane(vector, vector2);
					vector = ((Vector3.Dot(vector / magnitude, vector2) > 0f) ? vector : vector3) + vector3.magnitude * vector2;
				}
				float num = (effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier / magnitude).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
				if (effect.Value.applyScaleToKnockbackStrength)
				{
					num *= instance.scale;
				}
				instance.ApplyKnockback(vector.normalized, num, effect.Value.forceOffTheGround);
				GorillaTagger.Instance.offlineVRRig.ApplyInstanceKnockBack(effect);
			}
		}

		// Token: 0x06007466 RID: 29798 RVA: 0x00261D9C File Offset: 0x0025FF9C
		private void ApplyVOForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(rig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			rig.ActivateVOEffect(effect);
		}

		// Token: 0x06007467 RID: 29799 RVA: 0x00261E0C File Offset: 0x0026000C
		private void PlaySfxForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.PlayCosmeticEffectSFX(effect);
		}

		// Token: 0x06007468 RID: 29800 RVA: 0x00261E7C File Offset: 0x0026007C
		private void PlaySfxByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.PlayCosmeticEffectSFX(effect);
				}
			}
		}

		// Token: 0x06007469 RID: 29801 RVA: 0x00261F68 File Offset: 0x00260168
		private void PlayVFXForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnVFXEffect(effect);
		}

		// Token: 0x0600746A RID: 29802 RVA: 0x00261FD8 File Offset: 0x002601D8
		private void PlayVFXByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[]
				{
					GorillaTagger.Instance.offlineVRRig
				};
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnVFXEffect(effect);
				}
			}
		}

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x0600746B RID: 29803 RVA: 0x002620C4 File Offset: 0x002602C4
		// (set) Token: 0x0600746C RID: 29804 RVA: 0x002620CC File Offset: 0x002602CC
		public bool IsSpawned { get; set; }

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x0600746D RID: 29805 RVA: 0x002620D5 File Offset: 0x002602D5
		// (set) Token: 0x0600746E RID: 29806 RVA: 0x002620DD File Offset: 0x002602DD
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600746F RID: 29807 RVA: 0x002620E6 File Offset: 0x002602E6
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06007470 RID: 29808 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x04008599 RID: 34201
		public CosmeticEffectsOnPlayers.CosmeticEffect[] allEffects = new CosmeticEffectsOnPlayers.CosmeticEffect[0];

		// Token: 0x0400859A RID: 34202
		private VRRig myRig;

		// Token: 0x0400859B RID: 34203
		private Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> allEffectsDict = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

		// Token: 0x0200122E RID: 4654
		[Serializable]
		public enum TargetType
		{
			// Token: 0x0400859F RID: 34207
			Owner,
			// Token: 0x040085A0 RID: 34208
			Others,
			// Token: 0x040085A1 RID: 34209
			All
		}

		// Token: 0x0200122F RID: 4655
		[Serializable]
		public class CosmeticEffect
		{
			// Token: 0x17000B24 RID: 2852
			// (get) Token: 0x06007472 RID: 29810 RVA: 0x0026210E File Offset: 0x0026030E
			// (set) Token: 0x06007473 RID: 29811 RVA: 0x00262116 File Offset: 0x00260316
			public float knockbackStrengthMultiplier { get; set; }

			// Token: 0x06007474 RID: 29812 RVA: 0x00262120 File Offset: 0x00260320
			public bool IsGameModeAllowed()
			{
				GameModeType value = (GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual;
				return !this.excludeForGameModes.Contains(value);
			}

			// Token: 0x17000B25 RID: 2853
			// (get) Token: 0x06007475 RID: 29813 RVA: 0x00262159 File Offset: 0x00260359
			// (set) Token: 0x06007476 RID: 29814 RVA: 0x00262161 File Offset: 0x00260361
			public float EffectDuration
			{
				get
				{
					return this.effectDurationOthers;
				}
				set
				{
					this.effectDurationOthers = value;
				}
			}

			// Token: 0x17000B26 RID: 2854
			// (get) Token: 0x06007477 RID: 29815 RVA: 0x0026216A File Offset: 0x0026036A
			// (set) Token: 0x06007478 RID: 29816 RVA: 0x00262172 File Offset: 0x00260372
			public float EffectStartedTime { get; set; }

			// Token: 0x06007479 RID: 29817 RVA: 0x0026217B File Offset: 0x0026037B
			private bool IsSkin()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin;
			}

			// Token: 0x0600747A RID: 29818 RVA: 0x00262186 File Offset: 0x00260386
			private bool IsTagKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback;
			}

			// Token: 0x0600747B RID: 29819 RVA: 0x00262191 File Offset: 0x00260391
			private bool IsInstantKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x0600747C RID: 29820 RVA: 0x0026219C File Offset: 0x0026039C
			private bool HasKnockback()
			{
				CosmeticEffectsOnPlayers.EFFECTTYPE effecttype = this.effectType;
				return effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback || effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x0600747D RID: 29821 RVA: 0x002621C1 File Offset: 0x002603C1
			private bool IsVO()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride;
			}

			// Token: 0x0600747E RID: 29822 RVA: 0x002621CC File Offset: 0x002603CC
			private bool IsSFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.SFX;
			}

			// Token: 0x0600747F RID: 29823 RVA: 0x002621D7 File Offset: 0x002603D7
			private bool IsVFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VFX;
			}

			// Token: 0x17000B27 RID: 2855
			// (get) Token: 0x06007480 RID: 29824 RVA: 0x002621E2 File Offset: 0x002603E2
			private HashSet<GameModeType> Modes
			{
				get
				{
					if (this.modesHash == null)
					{
						this.modesHash = new HashSet<GameModeType>(this.excludeForGameModes);
					}
					return this.modesHash;
				}
			}

			// Token: 0x040085A2 RID: 34210
			public GameModeType[] excludeForGameModes;

			// Token: 0x040085A3 RID: 34211
			public CosmeticEffectsOnPlayers.EFFECTTYPE effectType;

			// Token: 0x040085A4 RID: 34212
			public float effectDistanceRadius;

			// Token: 0x040085A5 RID: 34213
			public CosmeticEffectsOnPlayers.TargetType target = CosmeticEffectsOnPlayers.TargetType.All;

			// Token: 0x040085A6 RID: 34214
			public float effectDurationOthers;

			// Token: 0x040085A7 RID: 34215
			public float effectDurationOwner;

			// Token: 0x040085A8 RID: 34216
			public GorillaSkin newSkin;

			// Token: 0x040085A9 RID: 34217
			[Tooltip("Use object pools")]
			public GameObject knockbackVFX;

			// Token: 0x040085AA RID: 34218
			[FormerlySerializedAs("knockbackStrengthMultiplier")]
			public float knockbackStrength;

			// Token: 0x040085AB RID: 34219
			public bool applyScaleToKnockbackStrength;

			// Token: 0x040085AC RID: 34220
			[Tooltip("force pushing players with hands on the ground")]
			public bool forceOffTheGround;

			// Token: 0x040085AD RID: 34221
			[Tooltip("Take the horizontal magnitude of the knockback, and add it opposite gravity. For example, being hit sideways will also impart a large upwards force. Breaks conservation of energy, but feels better to the player.")]
			public bool specialVerticalForce;

			// Token: 0x040085AE RID: 34222
			[FormerlySerializedAs("minStrengthClamp")]
			public float minKnockbackStrength = 0.5f;

			// Token: 0x040085AF RID: 34223
			[FormerlySerializedAs("maxStrengthClamp")]
			public float maxKnockbackStrength = 6f;

			// Token: 0x040085B1 RID: 34225
			public AudioClip[] voiceOverrideNormalClips;

			// Token: 0x040085B2 RID: 34226
			public AudioClip[] voiceOverrideLoudClips;

			// Token: 0x040085B3 RID: 34227
			public float voiceOverrideNormalVolume = 0.5f;

			// Token: 0x040085B4 RID: 34228
			public float voiceOverrideLoudVolume = 0.8f;

			// Token: 0x040085B5 RID: 34229
			public float voiceOverrideLoudThreshold = 0.175f;

			// Token: 0x040085B6 RID: 34230
			[Tooltip("plays sfx on player")]
			public List<AudioClip> sfxAudioClip;

			// Token: 0x040085B7 RID: 34231
			[Tooltip("plays vfx on player, must be in the global object pool and have a tag.")]
			public GameObject VFXGameObject;

			// Token: 0x040085B8 RID: 34232
			private HashSet<GameModeType> modesHash;
		}

		// Token: 0x02001230 RID: 4656
		public enum EFFECTTYPE
		{
			// Token: 0x040085BB RID: 34235
			Skin,
			// Token: 0x040085BC RID: 34236
			[Obsolete("FPV has been removed, do not use, use Stick Object To Player instead")]
			TagWithKnockback = 2,
			// Token: 0x040085BD RID: 34237
			InstantKnockback,
			// Token: 0x040085BE RID: 34238
			VoiceOverride,
			// Token: 0x040085BF RID: 34239
			SFX,
			// Token: 0x040085C0 RID: 34240
			VFX
		}
	}
}
