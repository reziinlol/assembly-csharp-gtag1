using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001269 RID: 4713
	public class CosmeticSwapper : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x06007629 RID: 30249 RVA: 0x0026B3C7 File Offset: 0x002695C7
		private int CosmeticStepIndex
		{
			get
			{
				return this.newSwappedCosmetics.Count;
			}
		}

		// Token: 0x0600762A RID: 30250 RVA: 0x0026B3D4 File Offset: 0x002695D4
		private void Awake()
		{
			this.controller = CosmeticsController.instance;
		}

		// Token: 0x0600762B RID: 30251 RVA: 0x0026B3E3 File Offset: 0x002695E3
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsGlobal(this.cosmeticIDs);
		}

		// Token: 0x0600762C RID: 30252 RVA: 0x0026B3F6 File Offset: 0x002695F6
		private void OnDisable()
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticsGlobal(this.cosmeticIDs);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x0600762D RID: 30253 RVA: 0x0026B409 File Offset: 0x00269609
		public void SwapInCosmetic(VRRig vrRig)
		{
			this.TriggerSwap(vrRig);
		}

		// Token: 0x0600762E RID: 30254 RVA: 0x0026B412 File Offset: 0x00269612
		private CosmeticSwapper.SwapMode GetCurrentMode()
		{
			return this.swapMode;
		}

		// Token: 0x0600762F RID: 30255 RVA: 0x0026B41A File Offset: 0x0026961A
		private bool ShouldHoldFinalStep()
		{
			return this.holdFinalStep;
		}

		// Token: 0x06007630 RID: 30256 RVA: 0x0026B422 File Offset: 0x00269622
		public int GetCurrentStepIndex(VRRig rig)
		{
			if (rig == null)
			{
				return 0;
			}
			return this.CosmeticStepIndex;
		}

		// Token: 0x06007631 RID: 30257 RVA: 0x0026B435 File Offset: 0x00269635
		public int GetNumberOfSteps()
		{
			if (this.cycleController != null)
			{
				return this.cycleController.Count;
			}
			return this.cosmeticIDs.Count;
		}

		// Token: 0x06007632 RID: 30258 RVA: 0x0026B45C File Offset: 0x0026965C
		private void TriggerSwap(VRRig rig)
		{
			if (GorillaGameManager.instance != null && this.gameModeExclusion.Contains(GorillaGameManager.instance.GameType()))
			{
				return;
			}
			if (rig == null || this.controller == null)
			{
				return;
			}
			if (rig != GorillaTagger.Instance.offlineVRRig)
			{
				return;
			}
			if (this.cycleController != null)
			{
				if (this.swapMode == CosmeticSwapper.SwapMode.Random)
				{
					this.cycleController.CycleRandom();
				}
				string appliedCosmeticID = this.cycleController.GetAppliedCosmeticID();
				if (string.IsNullOrEmpty(appliedCosmeticID))
				{
					return;
				}
				CosmeticSwapper.CosmeticState? cosmeticState = this.SwapInCosmeticWithReturn(appliedCosmeticID, rig);
				if (cosmeticState != null)
				{
					this.AddNewSwappedCosmetic(cosmeticState.Value);
				}
				return;
			}
			else
			{
				if (this.cosmeticIDs.Count == 0)
				{
					return;
				}
				if (this.swapMode == CosmeticSwapper.SwapMode.Random)
				{
					string nameOrId = this.cosmeticIDs[Random.Range(0, this.cosmeticIDs.Count)];
					CosmeticSwapper.CosmeticState? cosmeticState2 = this.SwapInCosmeticWithReturn(nameOrId, rig);
					if (cosmeticState2 != null)
					{
						this.AddNewSwappedCosmetic(cosmeticState2.Value);
					}
					return;
				}
				if (this.swapMode == CosmeticSwapper.SwapMode.AllAtOnce)
				{
					foreach (string nameOrId2 in this.cosmeticIDs)
					{
						CosmeticSwapper.CosmeticState? cosmeticState3 = this.SwapInCosmeticWithReturn(nameOrId2, rig);
						if (cosmeticState3 != null)
						{
							this.AddNewSwappedCosmetic(cosmeticState3.Value);
						}
					}
					return;
				}
				int cosmeticStepIndex = this.CosmeticStepIndex;
				if (cosmeticStepIndex < 0 || cosmeticStepIndex >= this.cosmeticIDs.Count)
				{
					return;
				}
				string nameOrId3 = this.cosmeticIDs[cosmeticStepIndex];
				CosmeticSwapper.CosmeticState? cosmeticState4 = this.SwapInCosmeticWithReturn(nameOrId3, rig);
				if (cosmeticState4 != null)
				{
					this.AddNewSwappedCosmetic(cosmeticState4.Value);
					if (cosmeticStepIndex == this.cosmeticIDs.Count - 1)
					{
						if (this.holdFinalStep)
						{
							this.MarkFinalCosmeticStep();
						}
						if (this.OnSwappingSequenceCompleted != null)
						{
							this.OnSwappingSequenceCompleted.Invoke(rig);
							return;
						}
					}
					else
					{
						this.UnmarkFinalCosmeticStep();
					}
				}
				return;
			}
		}

		// Token: 0x06007633 RID: 30259 RVA: 0x0026B654 File Offset: 0x00269854
		private CosmeticSwapper.CosmeticState? SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
		{
			if (this.controller == null)
			{
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(nameOrId);
			if (cosmeticItem.isNullItem)
			{
				Debug.LogWarning("Cosmetic not found: " + nameOrId);
				return null;
			}
			bool isLeftHand;
			CosmeticsController.CosmeticSlots cosmeticSlot = this.GetCosmeticSlot(cosmeticItem, out isLeftHand);
			if (cosmeticSlot == CosmeticsController.CosmeticSlots.Count)
			{
				Debug.LogWarning("Could not determine slot for: " + cosmeticItem.displayName);
				return null;
			}
			CosmeticsController.CosmeticItem cosmeticItem2 = this.controller.currentWornSet.items[(int)cosmeticSlot];
			if (!cosmeticItem2.isNullItem && cosmeticItem2.itemName == cosmeticItem.itemName)
			{
				return null;
			}
			this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, cosmeticItem, isLeftHand, false);
			this.controller.UpdateWornCosmetics(true);
			return new CosmeticSwapper.CosmeticState?(new CosmeticSwapper.CosmeticState
			{
				cosmeticId = nameOrId,
				replacedItem = cosmeticItem2,
				slot = cosmeticSlot,
				isLeftHand = isLeftHand
			});
		}

		// Token: 0x06007634 RID: 30260 RVA: 0x0026B768 File Offset: 0x00269968
		private void RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state)
		{
			if (this.controller == null)
			{
				return;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(state.cosmeticId);
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			this.controller.RemoveCosmeticItemFromSet(this.controller.tempUnlockedSet, cosmeticItem.displayName, false);
			if (!state.replacedItem.isNullItem)
			{
				this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, state.replacedItem, state.isLeftHand, false);
			}
			this.controller.UpdateWornCosmetics(true);
		}

		// Token: 0x06007635 RID: 30261 RVA: 0x0026B7F4 File Offset: 0x002699F4
		private CosmeticsController.CosmeticItem FindItem(string nameOrId)
		{
			CosmeticsController.CosmeticItem result;
			if (this.controller.allCosmeticsDict.TryGetValue(nameOrId, out result))
			{
				return result;
			}
			string itemID;
			if (this.controller.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(nameOrId, out itemID))
			{
				return this.controller.GetItemFromDict(itemID);
			}
			return this.controller.nullItem;
		}

		// Token: 0x06007636 RID: 30262 RVA: 0x0026B848 File Offset: 0x00269A48
		private CosmeticsController.CosmeticSlots GetCosmeticSlot(CosmeticsController.CosmeticItem item, out bool isLeftHand)
		{
			isLeftHand = false;
			if (!item.isHoldable)
			{
				return CosmeticsController.CategoryToNonTransferrableSlot(item.itemCategory);
			}
			CosmeticsController.CosmeticSet currentWornSet = this.controller.currentWornSet;
			CosmeticsController.CosmeticItem cosmeticItem = currentWornSet.items[7];
			CosmeticsController.CosmeticItem cosmeticItem2 = currentWornSet.items[8];
			if (cosmeticItem.isNullItem || (!cosmeticItem2.isNullItem && item.itemName == cosmeticItem.itemName))
			{
				isLeftHand = true;
			}
			if (!isLeftHand)
			{
				return CosmeticsController.CosmeticSlots.HandRight;
			}
			return CosmeticsController.CosmeticSlots.HandLeft;
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06007637 RID: 30263 RVA: 0x0026B8BD File Offset: 0x00269ABD
		// (set) Token: 0x06007638 RID: 30264 RVA: 0x0026B8C5 File Offset: 0x00269AC5
		public bool TickRunning { get; set; }

		// Token: 0x06007639 RID: 30265 RVA: 0x0026B8D0 File Offset: 0x00269AD0
		public void Tick()
		{
			if (this.newSwappedCosmetics.Count > 0)
			{
				if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.StepByStep)
				{
					if (this.isAtFinalCosmeticStep && this.ShouldHoldFinalStep())
					{
						if (Time.time - this.lastCosmeticSwapTime <= this.stepTimeout)
						{
							return;
						}
						this.isAtFinalCosmeticStep = false;
					}
					if (Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
					{
						while (this.newSwappedCosmetics.Count > 0)
						{
							CosmeticSwapper.CosmeticState state = this.newSwappedCosmetics.Pop();
							this.RestorePreviousCosmetic(state);
						}
						this.isAtFinalCosmeticStep = false;
						this.lastCosmeticSwapTime = float.PositiveInfinity;
						return;
					}
				}
				else if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.AllAtOnce && Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
				{
					while (this.newSwappedCosmetics.Count > 0)
					{
						CosmeticSwapper.CosmeticState state2 = this.newSwappedCosmetics.Pop();
						this.RestorePreviousCosmetic(state2);
					}
					this.lastCosmeticSwapTime = float.PositiveInfinity;
					this.isAtFinalCosmeticStep = false;
				}
			}
		}

		// Token: 0x0600763A RID: 30266 RVA: 0x0026B9C1 File Offset: 0x00269BC1
		private void AddNewSwappedCosmetic(CosmeticSwapper.CosmeticState state)
		{
			this.newSwappedCosmetics.Push(state);
			this.lastCosmeticSwapTime = Time.time;
		}

		// Token: 0x0600763B RID: 30267 RVA: 0x0026B9DA File Offset: 0x00269BDA
		private void MarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = true;
			this.lastCosmeticSwapTime = Time.time;
		}

		// Token: 0x0600763C RID: 30268 RVA: 0x0026B9EE File Offset: 0x00269BEE
		private void UnmarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = false;
		}

		// Token: 0x040087EE RID: 34798
		[SerializeField]
		private List<string> cosmeticIDs = new List<string>();

		// Token: 0x040087EF RID: 34799
		[SerializeField]
		private CosmeticSwapper.SwapMode swapMode = CosmeticSwapper.SwapMode.StepByStep;

		// Token: 0x040087F0 RID: 34800
		[Tooltip("Optional. When assigned, TriggerSwap sources the cosmetic ID from the cycle controller's active sub-item instead of the cosmeticIDs list. Use SwapMode.Random to call CycleRandom() automatically on each hit before reading the active sub-item.")]
		[SerializeField]
		private SubCosmeticCycleController cycleController;

		// Token: 0x040087F1 RID: 34801
		[SerializeField]
		private float stepTimeout = 10f;

		// Token: 0x040087F2 RID: 34802
		[Tooltip("Hold final step as long as the swapper is being called within the timeframe")]
		[SerializeField]
		private bool holdFinalStep = true;

		// Token: 0x040087F3 RID: 34803
		[SerializeField]
		private UnityEvent<VRRig> OnSwappingSequenceCompleted;

		// Token: 0x040087F4 RID: 34804
		[SerializeField]
		private List<GameModeType> gameModeExclusion = new List<GameModeType>();

		// Token: 0x040087F5 RID: 34805
		private CosmeticsController controller;

		// Token: 0x040087F6 RID: 34806
		private Stack<CosmeticSwapper.CosmeticState> newSwappedCosmetics = new Stack<CosmeticSwapper.CosmeticState>();

		// Token: 0x040087F7 RID: 34807
		private float lastCosmeticSwapTime = float.PositiveInfinity;

		// Token: 0x040087F8 RID: 34808
		private bool isAtFinalCosmeticStep;

		// Token: 0x0200126A RID: 4714
		private enum SwapMode
		{
			// Token: 0x040087FB RID: 34811
			AllAtOnce,
			// Token: 0x040087FC RID: 34812
			StepByStep,
			// Token: 0x040087FD RID: 34813
			Random
		}

		// Token: 0x0200126B RID: 4715
		private struct CosmeticState
		{
			// Token: 0x040087FE RID: 34814
			public string cosmeticId;

			// Token: 0x040087FF RID: 34815
			public CosmeticsController.CosmeticItem replacedItem;

			// Token: 0x04008800 RID: 34816
			public CosmeticsController.CosmeticSlots slot;

			// Token: 0x04008801 RID: 34817
			public bool isLeftHand;
		}
	}
}
