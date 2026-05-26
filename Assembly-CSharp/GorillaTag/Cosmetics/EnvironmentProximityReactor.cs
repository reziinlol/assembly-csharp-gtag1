using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001273 RID: 4723
	public class EnvironmentProximityReactor : MonoBehaviour
	{
		// Token: 0x0600765E RID: 30302 RVA: 0x0026D0C1 File Offset: 0x0026B2C1
		private void OnEnable()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			EnvironmentProximityReactorManager.Register(this);
		}

		// Token: 0x0600765F RID: 30303 RVA: 0x0026D0D8 File Offset: 0x0026B2D8
		private void OnDisable()
		{
			EnvironmentProximityReactorManager.Unregister(this);
			this.ResetBlockState();
		}

		// Token: 0x06007660 RID: 30304 RVA: 0x0026D0E8 File Offset: 0x0026B2E8
		private void Update()
		{
			if (CosmeticsProximityReactorManager.Instance == null)
			{
				return;
			}
			IReadOnlyList<CosmeticsProximityReactor> cosmetics = CosmeticsProximityReactorManager.Instance.Cosmetics;
			float time = Time.time;
			for (int i = 0; i < this.blocks.Count; i++)
			{
				EnvironmentProximityReactor.InteractionBlock interactionBlock = this.blocks[i];
				bool flag = false;
				Vector3 arg = base.transform.position;
				for (int j = 0; j < cosmetics.Count; j++)
				{
					CosmeticsProximityReactor cosmeticsProximityReactor = cosmetics[j];
					if (!(cosmeticsProximityReactor == null))
					{
						VRRig ownerRig = cosmeticsProximityReactor.GetOwnerRig();
						Vector3 vector;
						if (!(ownerRig == null) && ownerRig.isLocal && interactionBlock.CanTriggerFrom(cosmeticsProximityReactor) && this.AreWithinThreshold(cosmeticsProximityReactor, interactionBlock.proximityThreshold, out vector))
						{
							flag = true;
							arg = vector;
							break;
						}
					}
				}
				if (flag)
				{
					if (!interactionBlock.wasBelow && interactionBlock.CanPlay(time))
					{
						interactionBlock.wasBelow = true;
						interactionBlock.wasSharedBelow = true;
						interactionBlock.lastTriggerTime = time;
						UnityEvent<Vector3> onBelowLocal = interactionBlock.onBelowLocal;
						if (onBelowLocal != null)
						{
							onBelowLocal.Invoke(arg);
						}
						UnityEvent<Vector3> onBelowShared = interactionBlock.onBelowShared;
						if (onBelowShared != null)
						{
							onBelowShared.Invoke(arg);
						}
						EnvironmentProximityReactorManager instance = EnvironmentProximityReactorManager.Instance;
						if (instance != null)
						{
							instance.BroadcastProximityState(this.reactorId, i, true);
						}
					}
					else if (interactionBlock.wasBelow)
					{
						UnityEvent<Vector3> whileBelowLocal = interactionBlock.whileBelowLocal;
						if (whileBelowLocal != null)
						{
							whileBelowLocal.Invoke(arg);
						}
						UnityEvent<Vector3> whileBelowShared = interactionBlock.whileBelowShared;
						if (whileBelowShared != null)
						{
							whileBelowShared.Invoke(arg);
						}
						if (!interactionBlock.wasSharedBelow)
						{
							interactionBlock.wasSharedBelow = true;
							UnityEvent<Vector3> onBelowShared2 = interactionBlock.onBelowShared;
							if (onBelowShared2 != null)
							{
								onBelowShared2.Invoke(arg);
							}
							EnvironmentProximityReactorManager instance2 = EnvironmentProximityReactorManager.Instance;
							if (instance2 != null)
							{
								instance2.BroadcastProximityState(this.reactorId, i, true);
							}
						}
					}
				}
				else if (interactionBlock.wasBelow)
				{
					interactionBlock.wasBelow = false;
					interactionBlock.wasSharedBelow = false;
					UnityEvent onAboveLocal = interactionBlock.onAboveLocal;
					if (onAboveLocal != null)
					{
						onAboveLocal.Invoke();
					}
					UnityEvent onAboveShared = interactionBlock.onAboveShared;
					if (onAboveShared != null)
					{
						onAboveShared.Invoke();
					}
					EnvironmentProximityReactorManager instance3 = EnvironmentProximityReactorManager.Instance;
					if (instance3 != null)
					{
						instance3.BroadcastProximityState(this.reactorId, i, false);
					}
				}
				if (interactionBlock.wasSharedBelow && !interactionBlock.wasBelow)
				{
					UnityEvent<Vector3> whileBelowShared2 = interactionBlock.whileBelowShared;
					if (whileBelowShared2 != null)
					{
						whileBelowShared2.Invoke(base.transform.position);
					}
				}
			}
		}

		// Token: 0x06007661 RID: 30305 RVA: 0x0026D318 File Offset: 0x0026B518
		public void SyncStateTo(NetPlayer newPlayer, EnvironmentProximityReactorManager manager)
		{
			for (int i = 0; i < this.blocks.Count; i++)
			{
				if (this.blocks[i].wasBelow)
				{
					manager.BroadcastProximityStateTo(newPlayer, this.reactorId, i, true);
				}
			}
		}

		// Token: 0x06007662 RID: 30306 RVA: 0x0026D360 File Offset: 0x0026B560
		public void ApplySharedProximity(int blockIndex, bool isBelow)
		{
			if (blockIndex < 0 || blockIndex >= this.blocks.Count)
			{
				return;
			}
			EnvironmentProximityReactor.InteractionBlock interactionBlock = this.blocks[blockIndex];
			if (isBelow)
			{
				interactionBlock.wasSharedBelow = true;
				UnityEvent<Vector3> onBelowShared = interactionBlock.onBelowShared;
				if (onBelowShared == null)
				{
					return;
				}
				onBelowShared.Invoke(base.transform.position);
				return;
			}
			else
			{
				interactionBlock.wasSharedBelow = false;
				UnityEvent onAboveShared = interactionBlock.onAboveShared;
				if (onAboveShared == null)
				{
					return;
				}
				onAboveShared.Invoke();
				return;
			}
		}

		// Token: 0x06007663 RID: 30307 RVA: 0x0026D3CC File Offset: 0x0026B5CC
		private bool AreWithinThreshold(CosmeticsProximityReactor cosmetic, float threshold, out Vector3 contactPoint)
		{
			Vector3 vector = (cosmetic.collider == null) ? cosmetic.transform.position : cosmetic.collider.ClosestPoint(base.transform.position);
			Vector3 b = (this.proximityCollider == null) ? base.transform.position : this.proximityCollider.ClosestPoint(vector);
			contactPoint = (vector + b) * 0.5f;
			return Vector3.Distance(vector, b) <= threshold;
		}

		// Token: 0x06007664 RID: 30308 RVA: 0x0026D458 File Offset: 0x0026B658
		private void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
			int staticHash = base.GetType().Name.GetStaticHash();
			int hashCode2 = transform.position.QuantizedId128().GetHashCode();
			int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					int instanceID = transform.GetInstanceID();
					int num2 = StaticHash.Compute(num, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num2);
				}
				this.reactorId = this.staticId.GetStaticHash();
				return;
			}
			this.reactorId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x06007665 RID: 30309 RVA: 0x0026D524 File Offset: 0x0026B724
		private void ResetBlockState()
		{
			foreach (EnvironmentProximityReactor.InteractionBlock interactionBlock in this.blocks)
			{
				interactionBlock.wasBelow = false;
				interactionBlock.wasSharedBelow = false;
				interactionBlock.lastTriggerTime = -9999f;
			}
		}

		// Token: 0x06007666 RID: 30310 RVA: 0x0026D588 File Offset: 0x0026B788
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x04008871 RID: 34929
		public List<EnvironmentProximityReactor.InteractionBlock> blocks = new List<EnvironmentProximityReactor.InteractionBlock>();

		// Token: 0x04008872 RID: 34930
		[Tooltip("Optional collider for precise proximity measurement. If unassigned, the transform position is used.")]
		public Collider proximityCollider;

		// Token: 0x04008873 RID: 34931
		public int reactorId;

		// Token: 0x04008874 RID: 34932
		public string staticId;

		// Token: 0x04008875 RID: 34933
		[Tooltip("Leave off for most objects- the ID is computed automatically from the hierarchy path, type name, and world position, so no manual setup is needed.\n\nEnable only if this object is expected to move or be renamed in the editor after the ID has already been referenced elsewhere When enabled, the ID is pinned to the Static ID string above so it stays stable across repositions. Hit Recalculate once to generate it, then leave it alone.")]
		public bool useStaticId;

		// Token: 0x02001274 RID: 4724
		[Serializable]
		public class InteractionBlock
		{
			// Token: 0x06007668 RID: 30312 RVA: 0x0026D5A4 File Offset: 0x0026B7A4
			public bool CanTriggerFrom(CosmeticsProximityReactor cosmetic)
			{
				foreach (CosmeticsProximityReactor.InteractionSetting interactionSetting in cosmetic.blocks)
				{
					if ((interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToCosmetic || interactionSetting.mode == CosmeticsProximityReactor.InteractionMode.CosmeticToEnvironment) && interactionSetting.interactionKeys != null && interactionSetting.interactionKeys.Count != 0)
					{
						if (this.ignoreKeys != null && this.ignoreKeys.Count > 0)
						{
							bool flag = false;
							foreach (string text in interactionSetting.interactionKeys)
							{
								if (!string.IsNullOrEmpty(text) && this.ignoreKeys.Contains(text))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								continue;
							}
						}
						foreach (string text2 in interactionSetting.interactionKeys)
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
					}
				}
				return false;
			}

			// Token: 0x06007669 RID: 30313 RVA: 0x0026D740 File Offset: 0x0026B940
			public bool CanPlay(float now)
			{
				return now - this.lastTriggerTime >= this.cooldownTime;
			}

			// Token: 0x04008876 RID: 34934
			[Tooltip("Keys this block broadcasts. Cosmetics whose Key List or Listener List contains a matching key can trigger this block.")]
			public List<string> interactionKeys = new List<string>();

			// Token: 0x04008877 RID: 34935
			[Tooltip("If the cosmetic broadcasts any of these keys this block will not fire, even if another key matches.")]
			public List<string> ignoreKeys = new List<string>();

			// Token: 0x04008878 RID: 34936
			[Tooltip("React when a cosmetic broadcasts one of these keys. Listener keys are never broadcast outward, so two Listener-only objects will never trigger each other.")]
			public List<string> listenerKeys = new List<string>();

			// Token: 0x04008879 RID: 34937
			[Tooltip("Distance (m) at which a cosmetic triggers this block.")]
			public float proximityThreshold = 0.3f;

			// Token: 0x0400887A RID: 34938
			[Tooltip("Minimum seconds between consecutive OnBelow triggers for this block.")]
			[SerializeField]
			private float cooldownTime = 0.5f;

			// Token: 0x0400887B RID: 34939
			[Tooltip("Fires immediately on the client whose cosmetic crossed below the threshold. Local-only")]
			public UnityEvent<Vector3> onBelowLocal;

			// Token: 0x0400887C RID: 34940
			[Tooltip("Fires on aLL clients when any player's cosmetic crosses below the threshold.")]
			public UnityEvent<Vector3> onBelowShared;

			// Token: 0x0400887D RID: 34941
			[Tooltip("Fires every frame on the triggering client while the cosmetic remains below the threshold. Local-only")]
			public UnityEvent<Vector3> whileBelowLocal;

			// Token: 0x0400887E RID: 34942
			[Tooltip("Fires every frame on ALL clients while any player's cosmetic remains below the threshold.")]
			public UnityEvent<Vector3> whileBelowShared;

			// Token: 0x0400887F RID: 34943
			[Tooltip("Fires on the triggering client when the cosmetic goes back above the threshold.")]
			public UnityEvent onAboveLocal;

			// Token: 0x04008880 RID: 34944
			[Tooltip("Fires on aLL clients when the cosmetic goes back above the threshold.")]
			public UnityEvent onAboveShared;

			// Token: 0x04008881 RID: 34945
			[NonSerialized]
			public bool wasBelow;

			// Token: 0x04008882 RID: 34946
			[NonSerialized]
			public bool wasSharedBelow;

			// Token: 0x04008883 RID: 34947
			[NonSerialized]
			public float lastTriggerTime = -9999f;
		}
	}
}
