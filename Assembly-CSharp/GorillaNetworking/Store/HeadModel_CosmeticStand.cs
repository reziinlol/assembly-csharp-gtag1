using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorillaNetworking.Store
{
	// Token: 0x020010A3 RID: 4259
	public class HeadModel_CosmeticStand : HeadModel
	{
		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06006ADB RID: 27355 RVA: 0x00228B58 File Offset: 0x00226D58
		private string mountID
		{
			get
			{
				return "Mount_" + this.bustType.ToString();
			}
		}

		// Token: 0x06006ADC RID: 27356 RVA: 0x00228B78 File Offset: 0x00226D78
		public void LoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			if (cosmeticInfo == null)
			{
				Debug.LogWarning("Dynamic Cosmetics - LoadWardRobeParts -  No Cosmetic Info");
				return;
			}
			Debug.Log("Dynamic Cosmetics - Loading Wardrobe Parts for " + cosmeticInfo.info.playFabID);
			this.HandleLoadCosmeticParts(cosmeticInfo, forRightSide);
		}

		// Token: 0x06006ADD RID: 27357 RVA: 0x00228BC8 File Offset: 0x00226DC8
		private void ResetMannequinSkin()
		{
			SkinnedMeshRenderer skinnedMeshRenderer;
			if (this.mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
			{
				List<Material> list;
				using (ListPool<Material>.Get(out list))
				{
					list.Clear();
					list.EnsureCapacity(3);
					list.Add(this.defaultMannequinBody);
					list.Add(this.defaultMannequinChest);
					list.Add(this.defaultMannequinFace);
					skinnedMeshRenderer.SetSharedMaterials(list);
					return;
				}
			}
			MeshRenderer meshRenderer;
			if (this.mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				List<Material> list2;
				using (ListPool<Material>.Get(out list2))
				{
					list2.Clear();
					list2.EnsureCapacity(3);
					list2.Add(this.defaultMannequinBody);
					list2.Add(this.defaultMannequinChest);
					list2.Add(this.defaultMannequinFace);
					meshRenderer.SetSharedMaterials(list2);
				}
			}
		}

		// Token: 0x06006ADE RID: 27358 RVA: 0x00228CB8 File Offset: 0x00226EB8
		private void HandleLoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide)
		{
			if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Set && !cosmeticInfo.info.hasStoreParts)
			{
				foreach (CosmeticSO cosmeticInfo2 in cosmeticInfo.info.setCosmetics)
				{
					this.HandleLoadCosmeticParts(cosmeticInfo2, forRightSide);
				}
				return;
			}
			CosmeticPart[] array;
			if (cosmeticInfo.info.storeParts.Length != 0)
			{
				array = cosmeticInfo.info.storeParts;
			}
			else
			{
				if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Fur)
				{
					CosmeticPart[] array2 = cosmeticInfo.info.functionalParts;
					int i = 0;
					if (i < array2.Length)
					{
						CosmeticPart cosmeticPart = array2[i];
						GameObject gameObject = this.LoadAndInstantiatePrefab(cosmeticPart.prefabAssetRef, base.transform);
						gameObject.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin, false);
						Object.DestroyImmediate(gameObject);
						return;
					}
				}
				array = cosmeticInfo.info.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart2 in array)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart2.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo partLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = cosmeticInfo.info.playFabID,
							prefabAssetRef = cosmeticPart2.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							xform = null
						};
						GameObject gameObject2 = this.LoadAndInstantiatePrefab(cosmeticPart2.prefabAssetRef, base.transform);
						partLoadInfo.xform = gameObject2.transform;
						this._manuallySpawnedCosmeticParts.Add(gameObject2);
						gameObject2.SetActive(true);
						switch (this.bustType)
						{
						case HeadModel_CosmeticStand.BustType.Disabled:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaHead:
						case HeadModel_CosmeticStand.BustType.GorillaTorso:
						case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
						case HeadModel_CosmeticStand.BustType.GuitarStand:
						case HeadModel_CosmeticStand.BustType.JewelryBox:
						case HeadModel_CosmeticStand.BustType.Table:
						case HeadModel_CosmeticStand.BustType.PinDisplay:
						case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
							this.PositionWardRobeItems(gameObject2, partLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaMannequin:
							this._manuallySpawnedCosmeticParts.Remove(gameObject2);
							Object.DestroyImmediate(gameObject2);
							break;
						default:
							this.PositionWithWardRobeOffsets(partLoadInfo);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06006ADF RID: 27359 RVA: 0x00228F08 File Offset: 0x00227108
		public void LoadCosmeticPartsV2(string playFabId, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			CosmeticInfoV2 cosmeticInfo;
			if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfo))
			{
				if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
				{
					Debug.LogError("HeadModel.playFabId: Cosmetic id \"" + playFabId + "\" not found in `CosmeticsController`.", this);
				}
				return;
			}
			this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticInfo);
		}

		// Token: 0x06006AE0 RID: 27360 RVA: 0x00228F7C File Offset: 0x0022717C
		private void HandleLoadingAllPieces(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
		{
			CosmeticPart[] array;
			if (cosmeticInfo.storeParts.Length != 0)
			{
				array = cosmeticInfo.storeParts;
			}
			else
			{
				if (cosmeticInfo.category == CosmeticsController.CosmeticCategory.Fur)
				{
					this.HandleLoadingFur(playFabId, forRightSide, cosmeticInfo);
					return;
				}
				if (cosmeticInfo.category == CosmeticsController.CosmeticCategory.Set)
				{
					foreach (CosmeticSO cosmeticSO in cosmeticInfo.setCosmetics)
					{
						this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticSO.info);
					}
					return;
				}
				array = cosmeticInfo.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart in array)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadCosmeticPartsV2;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}

		// Token: 0x06006AE1 RID: 27361 RVA: 0x00229114 File Offset: 0x00227314
		private void _HandleLoadCosmeticPartsV2(AsyncOperationHandle<GameObject> loadOp)
		{
			int num;
			if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
			{
				if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
				{
					Object.Destroy(loadOp.Result);
				}
				return;
			}
			HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
			if (loadOp.Status == AsyncOperationStatus.Failed)
			{
				Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
				GTDelayedExec.Add(this, 10f, num);
				return;
			}
			cosmeticPartLoadInfo.xform = loadOp.Result.transform;
			this._manuallySpawnedCosmeticParts.Add(cosmeticPartLoadInfo.xform.gameObject);
			switch (this.bustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this._manuallySpawnedCosmeticParts.Remove(cosmeticPartLoadInfo.xform.gameObject);
				Object.DestroyImmediate(cosmeticPartLoadInfo.xform.gameObject);
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.PositionWardRobeItems(cosmeticPartLoadInfo);
				break;
			default:
				this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
				break;
			}
			cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
		}

		// Token: 0x06006AE2 RID: 27362 RVA: 0x0022928C File Offset: 0x0022748C
		private void HandleLoadingFur(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
		{
			foreach (CosmeticPart cosmeticPart in cosmeticInfo.functionalParts)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = playFabId,
							prefabAssetRef = cosmeticPart.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
							xform = null
						};
						cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadCosmeticPartsV2Fur;
						this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
						this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
					}
				}
			}
		}

		// Token: 0x06006AE3 RID: 27363 RVA: 0x002293AC File Offset: 0x002275AC
		private void _HandleLoadCosmeticPartsV2Fur(AsyncOperationHandle<GameObject> loadOp)
		{
			int num;
			if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
			{
				if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
				{
					Object.Destroy(loadOp.Result);
				}
				return;
			}
			HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
			if (loadOp.Status == AsyncOperationStatus.Failed)
			{
				Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
				GTDelayedExec.Add(this, 10f, num);
				return;
			}
			cosmeticPartLoadInfo.xform = loadOp.Result.transform;
			cosmeticPartLoadInfo.xform.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin, false);
			Object.DestroyImmediate(cosmeticPartLoadInfo.xform.gameObject);
		}

		// Token: 0x06006AE4 RID: 27364 RVA: 0x0022946C File Offset: 0x0022766C
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.bustType = newBustType;
		}

		// Token: 0x06006AE5 RID: 27365 RVA: 0x00229478 File Offset: 0x00227678
		private void PositionWardRobeItems(GameObject instantiateEdObject, HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = instantiateEdObject.transform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				instantiateEdObject.transform.localPosition = transform.localPosition;
				instantiateEdObject.transform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x06006AE6 RID: 27366 RVA: 0x0022954C File Offset: 0x0022774C
		private void PositionWardRobeItems(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = partLoadInfo.xform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				partLoadInfo.xform.position = base.transform.position;
				partLoadInfo.xform.rotation = base.transform.rotation;
				partLoadInfo.xform.localPosition = transform.localPosition;
				partLoadInfo.xform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				partLoadInfo.xform.position = base.transform.position;
				partLoadInfo.xform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x06006AE7 RID: 27367 RVA: 0x00229620 File Offset: 0x00227820
		private void PositionWithWardRobeOffsets(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Debug.Log("Dynamic Cosmetics - Mount Not Found: " + this.mountID);
			partLoadInfo.xform.localPosition = partLoadInfo.attachInfo.offset.pos;
			partLoadInfo.xform.localRotation = partLoadInfo.attachInfo.offset.rot;
			partLoadInfo.xform.localScale = partLoadInfo.attachInfo.offset.scale;
		}

		// Token: 0x06006AE8 RID: 27368 RVA: 0x00229694 File Offset: 0x00227894
		public void ClearManuallySpawnedCosmeticParts()
		{
			foreach (GameObject obj in this._manuallySpawnedCosmeticParts)
			{
				Object.DestroyImmediate(obj);
			}
			this._manuallySpawnedCosmeticParts.Clear();
		}

		// Token: 0x06006AE9 RID: 27369 RVA: 0x002296F0 File Offset: 0x002278F0
		public void ClearCosmetics()
		{
			this.ResetMannequinSkin();
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06006AEA RID: 27370 RVA: 0x00035D0D File Offset: 0x00033F0D
		private GameObject LoadAndInstantiatePrefab(GTAssetRef<GameObject> prefabAssetRef, Transform parent)
		{
			return null;
		}

		// Token: 0x06006AEB RID: 27371 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void UpdateCosmeticsMountPositions(CosmeticSO findCosmeticInAllCosmeticsArraySO)
		{
		}

		// Token: 0x04007B05 RID: 31493
		[ReadOnly]
		public HeadModel_CosmeticStand.BustType bustType = HeadModel_CosmeticStand.BustType.JewelryBox;

		// Token: 0x04007B06 RID: 31494
		[SerializeField]
		[ReadOnly]
		private List<GameObject> _manuallySpawnedCosmeticParts = new List<GameObject>();

		// Token: 0x04007B07 RID: 31495
		public GameObject mannequin;

		// Token: 0x04007B08 RID: 31496
		public Material defaultMannequinFace;

		// Token: 0x04007B09 RID: 31497
		public Material defaultMannequinChest;

		// Token: 0x04007B0A RID: 31498
		public Material defaultMannequinBody;

		// Token: 0x04007B0B RID: 31499
		[DebugReadout]
		private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

		// Token: 0x020010A4 RID: 4260
		public enum BustType
		{
			// Token: 0x04007B0D RID: 31501
			Disabled,
			// Token: 0x04007B0E RID: 31502
			GorillaHead,
			// Token: 0x04007B0F RID: 31503
			GorillaTorso,
			// Token: 0x04007B10 RID: 31504
			GorillaTorsoPost,
			// Token: 0x04007B11 RID: 31505
			GorillaMannequin,
			// Token: 0x04007B12 RID: 31506
			GuitarStand,
			// Token: 0x04007B13 RID: 31507
			JewelryBox,
			// Token: 0x04007B14 RID: 31508
			Table,
			// Token: 0x04007B15 RID: 31509
			PinDisplay,
			// Token: 0x04007B16 RID: 31510
			TagEffectDisplay
		}
	}
}
