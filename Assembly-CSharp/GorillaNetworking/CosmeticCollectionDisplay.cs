using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorillaNetworking
{
	// Token: 0x02001016 RID: 4118
	public class CosmeticCollectionDisplay : MonoBehaviour
	{
		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x060066B7 RID: 26295 RVA: 0x00210634 File Offset: 0x0020E834
		// (set) Token: 0x060066B8 RID: 26296 RVA: 0x0021063C File Offset: 0x0020E83C
		public string ParentPlayFabID { get; private set; }

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x060066B9 RID: 26297 RVA: 0x00210645 File Offset: 0x0020E845
		public int ActiveIndex
		{
			get
			{
				return this.activeIndex;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x060066BA RID: 26298 RVA: 0x0021064D File Offset: 0x0020E84D
		public int Count
		{
			get
			{
				return this.spawnedAnchors.Count;
			}
		}

		// Token: 0x060066BB RID: 26299 RVA: 0x0021065A File Offset: 0x0020E85A
		public static void Register(int rigID, string parentID, CosmeticCollectionDisplay display)
		{
			display.registeredRigID = rigID;
			display.registeredParentID = parentID;
			display.ParentPlayFabID = parentID;
			CosmeticCollectionDisplay.Registered[new ValueTuple<int, string>(rigID, parentID)] = display;
		}

		// Token: 0x060066BC RID: 26300 RVA: 0x00210684 File Offset: 0x0020E884
		public static CosmeticCollectionDisplay FindForRig(int rigID, string parentID)
		{
			CosmeticCollectionDisplay result;
			CosmeticCollectionDisplay.Registered.TryGetValue(new ValueTuple<int, string>(rigID, parentID), out result);
			return result;
		}

		// Token: 0x060066BD RID: 26301 RVA: 0x002106A8 File Offset: 0x0020E8A8
		public static void GetDisplaysForRig(int rigID, List<CosmeticCollectionDisplay> result)
		{
			result.Clear();
			foreach (KeyValuePair<ValueTuple<int, string>, CosmeticCollectionDisplay> keyValuePair in CosmeticCollectionDisplay.Registered)
			{
				if (keyValuePair.Key.Item1 == rigID)
				{
					result.Add(keyValuePair.Value);
				}
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x060066BE RID: 26302 RVA: 0x00210718 File Offset: 0x0020E918
		public CosmeticsController.CosmeticItem? ActiveCollectable
		{
			get
			{
				if (this.placedCollectables.Count <= 0)
				{
					return null;
				}
				return new CosmeticsController.CosmeticItem?(this.placedCollectables[this.activeIndex]);
			}
		}

		// Token: 0x060066BF RID: 26303 RVA: 0x00210754 File Offset: 0x0020E954
		public CosmeticsController.CosmeticItem? GetCollectableAt(int index)
		{
			if (index < 0 || index >= this.placedCollectables.Count)
			{
				return null;
			}
			return new CosmeticsController.CosmeticItem?(this.placedCollectables[index]);
		}

		// Token: 0x060066C0 RID: 26304 RVA: 0x00210790 File Offset: 0x0020E990
		public bool ContentMatches(IReadOnlyList<CosmeticsController.CosmeticItem> items)
		{
			if (this.placedCollectables.Count != items.Count)
			{
				return false;
			}
			for (int i = 0; i < this.placedCollectables.Count; i++)
			{
				if (this.placedCollectables[i].itemName != items[i].itemName)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060066C1 RID: 26305 RVA: 0x002107F0 File Offset: 0x0020E9F0
		public void Populate(IReadOnlyList<CosmeticsController.CosmeticItem> ownedCollectables, CosmeticInfoV2 parentInfo, Transform rootXform)
		{
			this.ClearSpawnedAnchors();
			this.placedCollectables.Clear();
			this.isCycling = parentInfo.collectionIsCycling;
			bool collectionUsesIndexTargeting = parentInfo.collectionUsesIndexTargeting;
			if (this.isCycling)
			{
				CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition = parentInfo.collectionSlots[0];
				Vector3 vector = cosmeticCollectionSlotDefinition.offset.scale;
				if (Mathf.Abs(vector.x) < 0.001f || Mathf.Abs(vector.y) < 0.001f || Mathf.Abs(vector.z) < 0.001f)
				{
					vector = Vector3.one;
				}
				for (int i = 0; i < ownedCollectables.Count; i++)
				{
					GameObject gameObject = new GameObject(string.Format("CollectionSlot_{0}", i));
					gameObject.transform.SetParent(rootXform, false);
					gameObject.transform.localPosition = cosmeticCollectionSlotDefinition.offset.pos;
					gameObject.transform.localRotation = cosmeticCollectionSlotDefinition.offset.rot;
					gameObject.transform.localScale = vector;
					this.spawnedAnchors.Add(gameObject);
					this.placedCollectables.Add(ownedCollectables[i]);
					this.InstantiateIntoAnchor(ownedCollectables[i], gameObject.transform);
				}
			}
			else
			{
				int num = 0;
				for (int j = 0; j < parentInfo.collectionSlots.Length; j++)
				{
					CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition2 = parentInfo.collectionSlots[j];
					CosmeticsController.CosmeticItem? cosmeticItem = null;
					if (collectionUsesIndexTargeting)
					{
						for (int k = 0; k < ownedCollectables.Count; k++)
						{
							if (ownedCollectables[k].collectionTargetSlotIndex == j)
							{
								cosmeticItem = new CosmeticsController.CosmeticItem?(ownedCollectables[k]);
								break;
							}
						}
					}
					else if (num < ownedCollectables.Count)
					{
						cosmeticItem = new CosmeticsController.CosmeticItem?(ownedCollectables[num++]);
					}
					if (cosmeticItem != null)
					{
						Vector3 vector2 = cosmeticCollectionSlotDefinition2.offset.scale;
						if (Mathf.Abs(vector2.x) < 0.001f || Mathf.Abs(vector2.y) < 0.001f || Mathf.Abs(vector2.z) < 0.001f)
						{
							vector2 = Vector3.one;
						}
						GameObject gameObject2 = new GameObject(string.Format("CollectionSlot_{0}", j));
						gameObject2.transform.SetParent(rootXform, false);
						gameObject2.transform.localPosition = cosmeticCollectionSlotDefinition2.offset.pos;
						gameObject2.transform.localRotation = cosmeticCollectionSlotDefinition2.offset.rot;
						gameObject2.transform.localScale = vector2;
						this.spawnedAnchors.Add(gameObject2);
						this.placedCollectables.Add(cosmeticItem.Value);
						this.InstantiateIntoAnchor(cosmeticItem.Value, gameObject2.transform);
					}
				}
			}
			this.activeIndex = 0;
			this.ApplyCyclingVisibility();
		}

		// Token: 0x060066C2 RID: 26306 RVA: 0x00210AC0 File Offset: 0x0020ECC0
		public void SetActiveIndex(int index)
		{
			if (this.spawnedAnchors.Count == 0)
			{
				return;
			}
			this.activeIndex = Mathf.Clamp(index, 0, this.spawnedAnchors.Count - 1);
			this.RefreshAnchorVisibility();
		}

		// Token: 0x060066C3 RID: 26307 RVA: 0x00210AF0 File Offset: 0x0020ECF0
		public void CycleActive(int direction)
		{
			if (!this.isCycling || this.spawnedAnchors.Count == 0)
			{
				return;
			}
			this.activeIndex = (this.activeIndex + direction + this.spawnedAnchors.Count) % this.spawnedAnchors.Count;
			this.RefreshAnchorVisibility();
		}

		// Token: 0x060066C4 RID: 26308 RVA: 0x00210B3F File Offset: 0x0020ED3F
		public void SetVisible(bool visible)
		{
			this.isVisible = visible;
			this.RefreshAnchorVisibility();
		}

		// Token: 0x060066C5 RID: 26309 RVA: 0x00210B50 File Offset: 0x0020ED50
		private void InstantiateIntoAnchor(CosmeticsController.CosmeticItem collectable, Transform anchor)
		{
			CosmeticInfoV2 cosmeticInfoV;
			if (!CosmeticsController.instance.TryGetCosmeticInfoV2(collectable.itemName, out cosmeticInfoV))
			{
				return;
			}
			CosmeticPart[] array = cosmeticInfoV.hasStoreParts ? cosmeticInfoV.storeParts : cosmeticInfoV.functionalParts;
			if (array == null || array.Length == 0)
			{
				return;
			}
			GTAssetRef<GameObject> prefabAssetRef = array[0].prefabAssetRef;
			if (prefabAssetRef == null || !prefabAssetRef.RuntimeKeyIsValid())
			{
				return;
			}
			Vector3 attachScale = Vector3.one;
			CosmeticPart[] functionalParts = cosmeticInfoV.functionalParts;
			if (functionalParts != null && functionalParts.Length != 0)
			{
				CosmeticAttachInfo[] attachAnchors = functionalParts[0].attachAnchors;
				if (attachAnchors != null && attachAnchors.Length != 0)
				{
					Vector3 scale = attachAnchors[0].offset.scale;
					if (Mathf.Abs(scale.x) >= 0.001f && Mathf.Abs(scale.y) >= 0.001f && Mathf.Abs(scale.z) >= 0.001f)
					{
						attachScale = scale;
					}
				}
			}
			AsyncOperationHandle<GameObject> item = prefabAssetRef.InstantiateAsync(anchor, false);
			this.loadOps.Add(item);
			item.Completed += delegate(AsyncOperationHandle<GameObject> handle)
			{
				if (handle.Status != AsyncOperationStatus.Succeeded)
				{
					return;
				}
				if (anchor == null || handle.Result == null)
				{
					Addressables.ReleaseInstance(handle);
					return;
				}
				handle.Result.transform.localPosition = Vector3.zero;
				handle.Result.transform.localRotation = Quaternion.identity;
				handle.Result.transform.localScale = attachScale;
			};
		}

		// Token: 0x060066C6 RID: 26310 RVA: 0x00210C72 File Offset: 0x0020EE72
		private void ApplyCyclingVisibility()
		{
			this.RefreshAnchorVisibility();
		}

		// Token: 0x060066C7 RID: 26311 RVA: 0x00210C7C File Offset: 0x0020EE7C
		private void RefreshAnchorVisibility()
		{
			for (int i = 0; i < this.spawnedAnchors.Count; i++)
			{
				if (this.spawnedAnchors[i] != null)
				{
					bool active = this.isVisible && (!this.isCycling || i == this.activeIndex);
					this.spawnedAnchors[i].SetActive(active);
				}
			}
		}

		// Token: 0x060066C8 RID: 26312 RVA: 0x00210CE8 File Offset: 0x0020EEE8
		private void ClearSpawnedAnchors()
		{
			for (int i = 0; i < this.loadOps.Count; i++)
			{
				if (this.loadOps[i].IsValid())
				{
					Addressables.ReleaseInstance(this.loadOps[i]);
				}
			}
			this.loadOps.Clear();
			for (int j = 0; j < this.spawnedAnchors.Count; j++)
			{
				if (this.spawnedAnchors[j] != null)
				{
					Object.Destroy(this.spawnedAnchors[j]);
				}
			}
			this.spawnedAnchors.Clear();
			this.placedCollectables.Clear();
		}

		// Token: 0x060066C9 RID: 26313 RVA: 0x00210D8F File Offset: 0x0020EF8F
		private void OnDisable()
		{
			CosmeticCollectionDisplay.Registered.Remove(new ValueTuple<int, string>(this.registeredRigID, this.registeredParentID));
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x00210DAD File Offset: 0x0020EFAD
		private void OnEnable()
		{
			if (!string.IsNullOrEmpty(this.registeredParentID))
			{
				CosmeticCollectionDisplay.Registered[new ValueTuple<int, string>(this.registeredRigID, this.registeredParentID)] = this;
			}
		}

		// Token: 0x060066CB RID: 26315 RVA: 0x00210DD8 File Offset: 0x0020EFD8
		private void OnDestroy()
		{
			CosmeticCollectionDisplay.Registered.Remove(new ValueTuple<int, string>(this.registeredRigID, this.registeredParentID));
			this.ClearSpawnedAnchors();
		}

		// Token: 0x04007628 RID: 30248
		private static readonly Dictionary<ValueTuple<int, string>, CosmeticCollectionDisplay> Registered = new Dictionary<ValueTuple<int, string>, CosmeticCollectionDisplay>();

		// Token: 0x04007629 RID: 30249
		private bool isCycling;

		// Token: 0x0400762A RID: 30250
		private bool isVisible = true;

		// Token: 0x0400762B RID: 30251
		private int activeIndex;

		// Token: 0x0400762C RID: 30252
		private int registeredRigID;

		// Token: 0x0400762D RID: 30253
		private string registeredParentID;

		// Token: 0x0400762E RID: 30254
		private readonly List<GameObject> spawnedAnchors = new List<GameObject>();

		// Token: 0x0400762F RID: 30255
		private readonly List<AsyncOperationHandle<GameObject>> loadOps = new List<AsyncOperationHandle<GameObject>>();

		// Token: 0x04007630 RID: 30256
		private readonly List<CosmeticsController.CosmeticItem> placedCollectables = new List<CosmeticsController.CosmeticItem>();
	}
}
