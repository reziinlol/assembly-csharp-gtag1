using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001262 RID: 4706
	public class CosmeticsProximityReactor : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x060075EA RID: 30186 RVA: 0x00269DF8 File Offset: 0x00267FF8
		// (set) Token: 0x060075EB RID: 30187 RVA: 0x00269E00 File Offset: 0x00268000
		public bool IsMatched { get; set; }

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x060075EC RID: 30188 RVA: 0x00269E09 File Offset: 0x00268009
		// (set) Token: 0x060075ED RID: 30189 RVA: 0x00269E11 File Offset: 0x00268011
		private VRRig MyRig { get; set; }

		// Token: 0x060075EE RID: 30190 RVA: 0x00269E1A File Offset: 0x0026801A
		public VRRig GetOwnerRig()
		{
			return this.MyRig;
		}

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x060075EF RID: 30191 RVA: 0x00269E22 File Offset: 0x00268022
		// (set) Token: 0x060075F0 RID: 30192 RVA: 0x00269E2A File Offset: 0x0026802A
		public bool IsSpawned { get; set; }

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x060075F1 RID: 30193 RVA: 0x00269E33 File Offset: 0x00268033
		// (set) Token: 0x060075F2 RID: 30194 RVA: 0x00269E3B File Offset: 0x0026803B
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x060075F3 RID: 30195 RVA: 0x00269E44 File Offset: 0x00268044
		public bool IsBelow
		{
			get
			{
				using (List<CosmeticsProximityReactor.InteractionSetting>.Enumerator enumerator = this.blocks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.wasBelow)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x060075F4 RID: 30196 RVA: 0x00269EA0 File Offset: 0x002680A0
		public void OnSpawn(VRRig rig)
		{
			if (this.MyRig == null)
			{
				this.MyRig = rig;
			}
		}

		// Token: 0x060075F5 RID: 30197 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x060075F6 RID: 30198 RVA: 0x00269EB7 File Offset: 0x002680B7
		private void Start()
		{
			this.IsMatched = false;
			if (CosmeticsProximityReactorManager.Instance != null)
			{
				CosmeticsProximityReactorManager.Instance.Register(this);
			}
		}

		// Token: 0x060075F7 RID: 30199 RVA: 0x00269ED8 File Offset: 0x002680D8
		private void OnEnable()
		{
			if (this.MyRig == null)
			{
				this.MyRig = base.GetComponentInParent<VRRig>();
			}
			if (CosmeticsProximityReactorManager.Instance != null)
			{
				CosmeticsProximityReactorManager.Instance.Register(this);
			}
		}

		// Token: 0x060075F8 RID: 30200 RVA: 0x00269F0C File Offset: 0x0026810C
		private void OnDisable()
		{
			if (CosmeticsProximityReactorManager.Instance)
			{
				CosmeticsProximityReactorManager.Instance.Unregister(this);
			}
		}

		// Token: 0x060075F9 RID: 30201 RVA: 0x00269F28 File Offset: 0x00268128
		public IReadOnlyList<string> GetTypes()
		{
			List<string> sharedKeysCache = CosmeticsProximityReactorManager.SharedKeysCache;
			sharedKeysCache.Clear();
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic)
				{
					if (interactionSetting.interactionKeys != null)
					{
						foreach (string text in interactionSetting.interactionKeys)
						{
							if (!string.IsNullOrEmpty(text) && !sharedKeysCache.Contains(text))
							{
								sharedKeysCache.Add(text);
							}
						}
					}
					if (interactionSetting.listenerKeys != null)
					{
						foreach (string text2 in interactionSetting.listenerKeys)
						{
							if (!string.IsNullOrEmpty(text2) && !sharedKeysCache.Contains(text2))
							{
								sharedKeysCache.Add(text2);
							}
						}
					}
				}
			}
			return sharedKeysCache;
		}

		// Token: 0x060075FA RID: 30202 RVA: 0x0026A050 File Offset: 0x00268250
		public bool IsGorillaBody()
		{
			return this.itemKind == CosmeticsProximityReactor.ItemKind.GorillaBody;
		}

		// Token: 0x060075FB RID: 30203 RVA: 0x0026A05B File Offset: 0x0026825B
		public bool IsCosmeticItem()
		{
			return this.itemKind == CosmeticsProximityReactor.ItemKind.Cosmetic;
		}

		// Token: 0x060075FC RID: 30204 RVA: 0x0026A068 File Offset: 0x00268268
		public bool AcceptsAnySource()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.gorillaBodyMask != CosmeticsProximityReactor.GorillaBodyPart.None)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060075FD RID: 30205 RVA: 0x0026A0CC File Offset: 0x002682CC
		public bool AcceptsThisSource(CosmeticsProximityReactor.GorillaBodyPart kind)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060075FE RID: 30206 RVA: 0x0026A134 File Offset: 0x00268334
		public float GetCosmeticPairThresholdWith(CosmeticsProximityReactor other, out bool any)
		{
			any = false;
			float num = float.MaxValue;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.CanTriggerFrom(interactionSetting2))
						{
							any = true;
							if (interactionSetting.proximityThreshold < num)
							{
								num = interactionSetting.proximityThreshold;
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060075FF RID: 30207 RVA: 0x0026A224 File Offset: 0x00268424
		public float GetSourceThresholdFor(CosmeticsProximityReactor gorillaBody, out bool any)
		{
			any = false;
			float num = float.MaxValue;
			CosmeticsProximityReactor.GorillaBodyPart kind = gorillaBody.gorillaBodyParts;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.AllowsRig(this.MyRig, gorillaBody.MyRig))
				{
					any = true;
					if (interactionSetting.proximityThreshold < num)
					{
						num = interactionSetting.proximityThreshold;
					}
				}
			}
			return num;
		}

		// Token: 0x06007600 RID: 30208 RVA: 0x0026A2BC File Offset: 0x002684BC
		public void OnCosmeticBelowWith(CosmeticsProximityReactor other, Vector3 contact)
		{
			float time = Time.time;
			bool flag = false;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					bool flag2 = false;
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.CanTriggerFrom(interactionSetting2))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						interactionSetting.FireBelow(this.MyRig, contact, time);
						if (interactionSetting.wasBelow)
						{
							interactionSetting.isMatched = true;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				this.IsMatched = true;
			}
		}

		// Token: 0x06007601 RID: 30209 RVA: 0x0026A3D4 File Offset: 0x002685D4
		public void WhileCosmeticBelowWith(CosmeticsProximityReactor other, Vector3 contact)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched && interactionSetting.AllowsRig(this.MyRig, other.MyRig))
				{
					bool flag = false;
					foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting2 in other.blocks)
					{
						if (interactionSetting2.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting2.AllowsRig(other.MyRig, this.MyRig) && interactionSetting.CanTriggerFrom(interactionSetting2))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						interactionSetting.FireWhile(this.MyRig, contact);
					}
				}
			}
		}

		// Token: 0x06007602 RID: 30210 RVA: 0x0026A4CC File Offset: 0x002686CC
		public void OnCosmeticAboveAll()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched)
				{
					interactionSetting.FireAbove(this.MyRig);
				}
			}
			this.RefreshAggregateMatched();
		}

		// Token: 0x06007603 RID: 30211 RVA: 0x0026A53C File Offset: 0x0026873C
		public void OnSourceBelow(Vector3 contact, CosmeticsProximityReactor.GorillaBodyPart kind, VRRig sourceRig)
		{
			float time = Time.time;
			bool flag = false;
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.AllowsRig(this.MyRig, sourceRig))
				{
					interactionSetting.FireBelow(this.MyRig, contact, time);
					if (interactionSetting.wasBelow)
					{
						interactionSetting.isMatched = true;
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.RefreshAggregateMatched();
			}
		}

		// Token: 0x06007604 RID: 30212 RVA: 0x0026A5DC File Offset: 0x002687DC
		public void WhileSourceBelow(Vector3 contact, CosmeticsProximityReactor.GorillaBodyPart kind, VRRig sourceRig)
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.AcceptsGorillaBodyPart(kind) && interactionSetting.isMatched && interactionSetting.AllowsRig(this.MyRig, sourceRig))
				{
					interactionSetting.FireWhile(this.MyRig, contact);
				}
			}
		}

		// Token: 0x06007605 RID: 30213 RVA: 0x0026A660 File Offset: 0x00268860
		public void OnSourceAboveAll()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.isMatched)
				{
					interactionSetting.FireAbove(this.MyRig);
				}
			}
			this.RefreshAggregateMatched();
		}

		// Token: 0x06007606 RID: 30214 RVA: 0x0026A6D0 File Offset: 0x002688D0
		public bool HasAnyCosmeticMatch()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic && interactionSetting.isMatched)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007607 RID: 30215 RVA: 0x0026A734 File Offset: 0x00268934
		private bool HasAnyGorillaBodyPartMatch()
		{
			foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in this.blocks)
			{
				if (interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && interactionSetting.isMatched)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007608 RID: 30216 RVA: 0x0026A798 File Offset: 0x00268998
		public void RefreshAggregateMatched()
		{
			this.IsMatched = (this.HasAnyCosmeticMatch() || this.HasAnyGorillaBodyPartMatch());
		}

		// Token: 0x040087B8 RID: 34744
		[Tooltip("Is this object a Cosmetic or a gorilla body part like hand? (gorilla body slot is reserved for Gorilla Player Networked)")]
		public CosmeticsProximityReactor.ItemKind itemKind;

		// Token: 0x040087B9 RID: 34745
		[FormerlySerializedAs("sourceKinds")]
		public CosmeticsProximityReactor.GorillaBodyPart gorillaBodyParts;

		// Token: 0x040087BA RID: 34746
		public List<CosmeticsProximityReactor.InteractionSetting> blocks = new List<CosmeticsProximityReactor.InteractionSetting>();

		// Token: 0x040087BB RID: 34747
		[Tooltip("If enabled, this cosmetic ignores other instances that share the same PlayFabID.")]
		public bool ignoreSameCosmeticInstances;

		// Token: 0x040087BC RID: 34748
		public string PlayFabID = "";

		// Token: 0x040087BD RID: 34749
		[Tooltip("If collider is not assigned, we will use the position of this object to find the distance between two cosmetic/body part")]
		public Collider collider;

		// Token: 0x040087C0 RID: 34752
		private RubberDuckEvents _events;

		// Token: 0x02001263 RID: 4707
		public enum ItemKind
		{
			// Token: 0x040087C4 RID: 34756
			Cosmetic,
			// Token: 0x040087C5 RID: 34757
			GorillaBody
		}

		// Token: 0x02001264 RID: 4708
		[Flags]
		public enum GorillaBodyPart
		{
			// Token: 0x040087C7 RID: 34759
			None = 0,
			// Token: 0x040087C8 RID: 34760
			HandLeft = 1,
			// Token: 0x040087C9 RID: 34761
			HandRight = 2,
			// Token: 0x040087CA RID: 34762
			Mouth = 4
		}

		// Token: 0x02001265 RID: 4709
		public enum InteractionMode
		{
			// Token: 0x040087CC RID: 34764
			CosmeticToCosmetic,
			// Token: 0x040087CD RID: 34765
			CosmeticToEnvironment,
			// Token: 0x040087CE RID: 34766
			GorillaBodyToCosmetic
		}

		// Token: 0x02001266 RID: 4710
		public enum TargetType
		{
			// Token: 0x040087D0 RID: 34768
			Owner,
			// Token: 0x040087D1 RID: 34769
			Others,
			// Token: 0x040087D2 RID: 34770
			All
		}

		// Token: 0x02001267 RID: 4711
		[Serializable]
		public class InteractionSetting
		{
			// Token: 0x0600760A RID: 30218 RVA: 0x0026A7CF File Offset: 0x002689CF
			public bool IsCosmeticToCosmetic()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic;
			}

			// Token: 0x0600760B RID: 30219 RVA: 0x0026A7DA File Offset: 0x002689DA
			public bool IsCosmeticToEnvironment()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToEnvironment;
			}

			// Token: 0x0600760C RID: 30220 RVA: 0x0026A7E5 File Offset: 0x002689E5
			public bool IsGorillaBodyToCosmetic()
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic;
			}

			// Token: 0x0600760D RID: 30221 RVA: 0x0026A7F0 File Offset: 0x002689F0
			public bool AcceptsGorillaBodyPart(CosmeticsProximityReactor.GorillaBodyPart kind)
			{
				return this.mode == CosmeticsProximityReactor.InteractionMode.GorillaBodyToCosmetic && (this.gorillaBodyMask & kind) > CosmeticsProximityReactor.GorillaBodyPart.None;
			}

			// Token: 0x0600760E RID: 30222 RVA: 0x0026A808 File Offset: 0x00268A08
			public bool CanTriggerFrom(CosmeticsProximityReactor.InteractionSetting other)
			{
				if (this.mode != CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic || other == null || other.mode != CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic)
				{
					return false;
				}
				if (other.interactionKeys == null || other.interactionKeys.Count == 0)
				{
					return false;
				}
				if (this.ignoreKeys != null && this.ignoreKeys.Count > 0)
				{
					foreach (string text in other.interactionKeys)
					{
						if (!string.IsNullOrEmpty(text) && this.ignoreKeys.Contains(text))
						{
							return false;
						}
					}
				}
				foreach (string text2 in other.interactionKeys)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						if (this.interactionKeys != null && this.interactionKeys.Contains(text2))
						{
							return true;
						}
						if (this.listenerKeys != null && this.listenerKeys.Contains(text2))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x0600760F RID: 30223 RVA: 0x0026A930 File Offset: 0x00268B30
			public bool CanPlay(float now)
			{
				return now - this.lastEffectTime >= this.cooldownTime;
			}

			// Token: 0x06007610 RID: 30224 RVA: 0x0026A948 File Offset: 0x00268B48
			public void FireBelow(VRRig rig, Vector3 contact, float now)
			{
				if (!this.wasBelow && this.CanPlay(now))
				{
					if (rig != null && rig.isLocal)
					{
						UnityEvent<Vector3> unityEvent = this.onBelowLocal;
						if (unityEvent != null)
						{
							unityEvent.Invoke(contact);
						}
					}
					UnityEvent<Vector3> unityEvent2 = this.onBelowShared;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(contact);
					}
					this.wasBelow = true;
					this.lastEffectTime = now;
				}
			}

			// Token: 0x06007611 RID: 30225 RVA: 0x0026A9A9 File Offset: 0x00268BA9
			public void FireWhile(VRRig rig, Vector3 contact)
			{
				if (rig != null && rig.isLocal)
				{
					UnityEvent<Vector3> unityEvent = this.whileBelowLocal;
					if (unityEvent != null)
					{
						unityEvent.Invoke(contact);
					}
				}
				UnityEvent<Vector3> unityEvent2 = this.whileBelowShared;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke(contact);
			}

			// Token: 0x06007612 RID: 30226 RVA: 0x0026A9E0 File Offset: 0x00268BE0
			public void FireAbove(VRRig rig)
			{
				if (this.wasBelow)
				{
					if (rig != null && rig.isLocal)
					{
						UnityEvent unityEvent = this.onAboveLocal;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					UnityEvent unityEvent2 = this.onAboveShared;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasBelow = false;
					this.isMatched = false;
				}
			}

			// Token: 0x06007613 RID: 30227 RVA: 0x0026AA38 File Offset: 0x00268C38
			public bool AllowsRig(VRRig myRig, VRRig otherRig)
			{
				if (myRig == null || otherRig == null)
				{
					return true;
				}
				switch (this.targetType)
				{
				case CosmeticsProximityReactor.TargetType.Owner:
					return myRig == otherRig;
				case CosmeticsProximityReactor.TargetType.Others:
					return myRig != otherRig;
				}
				return true;
			}

			// Token: 0x040087D3 RID: 34771
			[Tooltip("Determines what type of interaction this block handles.\n• CosmeticToCosmetic: triggers when two cosmetics with matching keys are nearby.\n• CosmeticToEnvironment: broadcasts keys that EnvironmentProximityReactor objects listen for. Use this to mark a cosmetic as a trigger for scene objects.\n• GorillaBodyToCosmetic: triggers when a Gorilla body part (hand, head, etc.) is near this cosmetic.")]
			public CosmeticsProximityReactor.InteractionMode mode;

			// Token: 0x040087D4 RID: 34772
			[Tooltip("Keys this block broadcasts. Other cosmetics or environment objects whose Key list or Listener list contain a matching key can react to this block.")]
			public List<string> interactionKeys = new List<string>();

			// Token: 0x040087D5 RID: 34773
			[Tooltip("If the other side is broadcasting any of these keys, this block will not fire, even if another key matches.")]
			public List<string> ignoreKeys = new List<string>();

			// Token: 0x040087D6 RID: 34774
			[Tooltip("Keys this block silently listens for. When the other side broadcasts one of these keys, this block fires. Listener keys are never broadcast outward, so two Listener-only objects will never trigger each other.")]
			public List<string> listenerKeys = new List<string>();

			// Token: 0x040087D7 RID: 34775
			[Tooltip("Specifies which Gorilla body parts (e.g., Hands, Head) can trigger this interaction.\nUse this when the Mode is set to GorillaBodyToCosmetic.")]
			public CosmeticsProximityReactor.GorillaBodyPart gorillaBodyMask;

			// Token: 0x040087D8 RID: 34776
			[Tooltip("The distance threshold (in meters) for triggering the interaction.\nIf another object enters this range, the OnBelow and WhileBelow events are fired.")]
			public float proximityThreshold = 0.15f;

			// Token: 0x040087D9 RID: 34777
			[Tooltip("Minimum time (in seconds) between consecutive triggers for this interaction block.\nPrevents rapid re-triggering when objects remain within proximity.")]
			[SerializeField]
			private float cooldownTime = 0.5f;

			// Token: 0x040087DA RID: 34778
			[Tooltip("Who is allowed to trigger this block (if gorilla body part is selected).\n• Owner: only this cosmetic's own rig/body can trigger this.\n• Others: only other players' rigs/bodies can trigger this.\n• All: anyone can trigger.\n\nNote: everyone will still be able to see the result when it triggers.")]
			public CosmeticsProximityReactor.TargetType targetType = CosmeticsProximityReactor.TargetType.All;

			// Token: 0x040087DB RID: 34779
			public UnityEvent<Vector3> onBelowLocal;

			// Token: 0x040087DC RID: 34780
			public UnityEvent<Vector3> onBelowShared;

			// Token: 0x040087DD RID: 34781
			public UnityEvent<Vector3> whileBelowLocal;

			// Token: 0x040087DE RID: 34782
			public UnityEvent<Vector3> whileBelowShared;

			// Token: 0x040087DF RID: 34783
			public UnityEvent onAboveLocal;

			// Token: 0x040087E0 RID: 34784
			public UnityEvent onAboveShared;

			// Token: 0x040087E1 RID: 34785
			[NonSerialized]
			public bool wasBelow;

			// Token: 0x040087E2 RID: 34786
			[NonSerialized]
			public bool isMatched;

			// Token: 0x040087E3 RID: 34787
			[NonSerialized]
			public float lastEffectTime = -9999f;
		}
	}
}
