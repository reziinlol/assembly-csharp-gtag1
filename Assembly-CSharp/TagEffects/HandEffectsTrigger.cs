using System;
using GorillaExtensions;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010CE RID: 4302
	public class HandEffectsTrigger : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06006BB8 RID: 27576 RVA: 0x0022E4D7 File Offset: 0x0022C6D7
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06006BB9 RID: 27577 RVA: 0x0022E4E0 File Offset: 0x0022C6E0
		public bool FingersDown
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFistRight()) || (!this.rightHand && this.rig.IsMakingFistLeft()));
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06006BBA RID: 27578 RVA: 0x0022E52C File Offset: 0x0022C72C
		public bool FingersUp
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFiveRight()) || (!this.rightHand && this.rig.IsMakingFiveLeft()));
			}
		}

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06006BBB RID: 27579 RVA: 0x0022E578 File Offset: 0x0022C778
		public Vector3 Velocity
		{
			get
			{
				if (this.velocityEstimator != null && this.rig != null && this.rig.scaleFactor > 0.001f)
				{
					return this.velocityEstimator.linearVelocity / this.rig.scaleFactor;
				}
				return Vector3.zero;
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06006BBC RID: 27580 RVA: 0x0022E5D4 File Offset: 0x0022C7D4
		bool IHandEffectsTrigger.RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06006BBD RID: 27581 RVA: 0x0022E5DC File Offset: 0x0022C7DC
		// (set) Token: 0x06006BBE RID: 27582 RVA: 0x0022E5E4 File Offset: 0x0022C7E4
		public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06006BBF RID: 27583 RVA: 0x0022E5ED File Offset: 0x0022C7ED
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06006BC0 RID: 27584 RVA: 0x00086271 File Offset: 0x00084471
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06006BC1 RID: 27585 RVA: 0x0022E5F5 File Offset: 0x0022C7F5
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06006BC2 RID: 27586 RVA: 0x0022E5FD File Offset: 0x0022C7FD
		public TagEffectPack CosmeticEffectPack
		{
			get
			{
				if (this.rig == null)
				{
					return null;
				}
				return this.rig.CosmeticEffectPack;
			}
		}

		// Token: 0x06006BC3 RID: 27587 RVA: 0x0022E61C File Offset: 0x0022C81C
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.velocityEstimator == null)
			{
				this.velocityEstimator = base.GetComponentInParent<GorillaVelocityEstimator>();
			}
			for (int i = 0; i < this.debugVisuals.Length; i++)
			{
				this.debugVisuals[i].SetActive(TagEffectsLibrary.DebugMode);
			}
		}

		// Token: 0x06006BC4 RID: 27588 RVA: 0x000862D5 File Offset: 0x000844D5
		private void OnEnable()
		{
			if (!HandEffectsTriggerRegistry.HasInstance)
			{
				HandEffectsTriggerRegistry.FindInstance();
			}
			HandEffectsTriggerRegistry.Instance.Register(this);
		}

		// Token: 0x06006BC5 RID: 27589 RVA: 0x000862EE File Offset: 0x000844EE
		private void OnDisable()
		{
			HandEffectsTriggerRegistry.Instance.Unregister(this);
		}

		// Token: 0x06006BC6 RID: 27590 RVA: 0x0022E674 File Offset: 0x0022C874
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
			if (this.rig == other.Rig)
			{
				return;
			}
			if (this.FingersDown && other.FingersDown && (other.Static || (Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.up) * base.transform.up - Vector3.Dot(other.Velocity, other.Transform.up) * other.Transform.up, -other.Transform.up) > TagEffectsLibrary.FistBumpSpeedThreshold && Vector3.Dot(base.transform.up, other.Transform.up) < -0.01f)))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.FIST_BUMP, other);
			}
			if (this.FingersUp && other.FingersUp && (other.Static || Mathf.Abs(Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.right) * base.transform.right - Vector3.Dot(other.Velocity, other.Transform.right) * other.Transform.right, other.Transform.right)) > TagEffectsLibrary.HighFiveSpeedThreshold))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.HIGH_FIVE, other);
			}
		}

		// Token: 0x06006BC7 RID: 27591 RVA: 0x0022E7E0 File Offset: 0x0022C9E0
		private void PlayHandEffects(TagEffectsLibrary.EffectType effectType, IHandEffectsTrigger other)
		{
			if (this.rig.IsNull())
			{
				return;
			}
			bool flag = false;
			if (this.rig.isOfflineVRRig)
			{
				PlayerGameEvents.TriggerHandEffect(effectType.ToString());
			}
			if (this.OnTrigger != null || (other != null && other.OnTrigger != null))
			{
				switch (effectType)
				{
				case TagEffectsLibrary.EffectType.FIRST_PERSON:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger = this.OnTrigger;
					if (onTrigger != null)
					{
						onTrigger(IHandEffectsTrigger.Mode.Tag1P);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger2 = other.OnTrigger;
						if (onTrigger2 != null)
						{
							onTrigger2(IHandEffectsTrigger.Mode.Tag1P);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.THIRD_PERSON:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger3 = this.OnTrigger;
					if (onTrigger3 != null)
					{
						onTrigger3(IHandEffectsTrigger.Mode.Tag3P);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger4 = other.OnTrigger;
						if (onTrigger4 != null)
						{
							onTrigger4(IHandEffectsTrigger.Mode.Tag3P);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.HIGH_FIVE:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger5 = this.OnTrigger;
					if (onTrigger5 != null)
					{
						onTrigger5(IHandEffectsTrigger.Mode.HighFive);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger6 = other.OnTrigger;
						if (onTrigger6 != null)
						{
							onTrigger6(IHandEffectsTrigger.Mode.HighFive);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.FIST_BUMP:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger7 = this.OnTrigger;
					if (onTrigger7 != null)
					{
						onTrigger7(IHandEffectsTrigger.Mode.FistBump);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger8 = other.OnTrigger;
						if (onTrigger8 != null)
						{
							onTrigger8(IHandEffectsTrigger.Mode.FistBump);
						}
					}
					break;
				}
				}
			}
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic = null;
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic2 = null;
			foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic3 in (this.rightHand ? this.rig.CosmeticHandEffectsOverride_Right : this.rig.CosmeticHandEffectsOverride_Left))
			{
				if (handEffectsOverrideCosmetic3.handEffectType == this.MapEnum(effectType))
				{
					handEffectsOverrideCosmetic2 = handEffectsOverrideCosmetic3;
					break;
				}
			}
			if (this.rig.isOfflineVRRig && GorillaTagger.Instance != null)
			{
				if (other.Rig)
				{
					foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic4 in ((other.Rig.CosmeticHandEffectsOverride_Right != null) ? other.Rig.CosmeticHandEffectsOverride_Right : other.Rig.CosmeticHandEffectsOverride_Left))
					{
						if (handEffectsOverrideCosmetic4.handEffectType == this.MapEnum(effectType))
						{
							handEffectsOverrideCosmetic = handEffectsOverrideCosmetic4;
							break;
						}
					}
					if (handEffectsOverrideCosmetic && handEffectsOverrideCosmetic.handEffectType == this.MapEnum(effectType) && ((!handEffectsOverrideCosmetic.isLeftHand && other.RightHand) || (handEffectsOverrideCosmetic.isLeftHand && !other.RightHand)))
					{
						if (handEffectsOverrideCosmetic.thirdPerson.playHaptics)
						{
							GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic.thirdPerson.hapticStrength, handEffectsOverrideCosmetic.thirdPerson.hapticDuration);
						}
						TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic.thirdPerson.effectVFX, base.transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic.thirdPerson.parentEffect, base.transform.rotation);
						flag = true;
					}
				}
				if (handEffectsOverrideCosmetic2 && handEffectsOverrideCosmetic2.handEffectType == this.MapEnum(effectType) && ((handEffectsOverrideCosmetic2.isLeftHand && !this.rightHand) || (!handEffectsOverrideCosmetic2.isLeftHand && this.rightHand)))
				{
					if (handEffectsOverrideCosmetic2.firstPerson.playHaptics)
					{
						GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic2.firstPerson.hapticStrength, handEffectsOverrideCosmetic2.firstPerson.hapticDuration);
					}
					TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic2.firstPerson.effectVFX, other.Transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic2.firstPerson.parentEffect, other.Transform.rotation);
					flag = true;
				}
			}
			if (!flag)
			{
				if (this.rig.isOfflineVRRig)
				{
					GorillaTagger.Instance.StartVibration(!this.rightHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
				}
				TagEffectsLibrary.PlayEffect(base.transform, !this.rightHand, this.rig.scaleFactor, effectType, this.CosmeticEffectPack, other.CosmeticEffectPack, base.transform.rotation);
			}
		}

		// Token: 0x06006BC8 RID: 27592 RVA: 0x0022EBE4 File Offset: 0x0022CDE4
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return (base.transform.position - t.Transform.position).IsShorterThan(this.triggerRadius * this.rig.scaleFactor);
		}

		// Token: 0x06006BC9 RID: 27593 RVA: 0x0022EC18 File Offset: 0x0022CE18
		private HandEffectsOverrideCosmetic.HandEffectType MapEnum(TagEffectsLibrary.EffectType oldEnum)
		{
			return HandEffectsTrigger.mappingArray[(int)oldEnum];
		}

		// Token: 0x04007BF2 RID: 31730
		[SerializeField]
		private float triggerRadius = 0.07f;

		// Token: 0x04007BF3 RID: 31731
		[SerializeField]
		private bool rightHand;

		// Token: 0x04007BF4 RID: 31732
		[SerializeField]
		private bool isStatic;

		// Token: 0x04007BF5 RID: 31733
		private VRRig rig;

		// Token: 0x04007BF6 RID: 31734
		public GorillaVelocityEstimator velocityEstimator;

		// Token: 0x04007BF7 RID: 31735
		[SerializeField]
		private GameObject[] debugVisuals;

		// Token: 0x04007BFA RID: 31738
		private static HandEffectsOverrideCosmetic.HandEffectType[] mappingArray = new HandEffectsOverrideCosmetic.HandEffectType[]
		{
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.HighFive,
			HandEffectsOverrideCosmetic.HandEffectType.FistBump
		};
	}
}
