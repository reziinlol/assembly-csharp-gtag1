using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010A5 RID: 4261
	public class StoreController : MonoBehaviour
	{
		// Token: 0x06006AED RID: 27373 RVA: 0x00229758 File Offset: 0x00227958
		public void Awake()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = this;
			}
			else if (StoreController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
			this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
		}

		// Token: 0x06006AEE RID: 27374 RVA: 0x002297B0 File Offset: 0x002279B0
		public void RefreshCosmeticStandsDictionaryFromDepartments()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				if (!(storeDepartment == null) && !storeDepartment.departmentName.IsNullOrEmpty())
				{
					foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
					{
						if (!storeDisplay.displayName.IsNullOrEmpty())
						{
							foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
							{
								if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
								{
									string text = string.Concat(new string[]
									{
										storeDepartment.departmentName,
										"|",
										storeDisplay.displayName,
										"|",
										dynamicCosmeticStand.StandName
									});
									if (this.CosmeticStandsDict.ContainsKey(text))
									{
										Debug.LogError(string.Concat(new string[]
										{
											"StoreStuff: Duplicate Stand Name: ",
											text,
											" Please Fix Gameobject : ",
											dynamicCosmeticStand.gameObject.GetPath(),
											dynamicCosmeticStand.gameObject.name
										}), base.gameObject);
									}
									else
									{
										this.CosmeticStandsDict.Add(text, dynamicCosmeticStand);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006AEF RID: 27375 RVA: 0x00229938 File Offset: 0x00227B38
		public void AddStandToCosmeticStandsDictionary(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (this.CosmeticStandsDict.ContainsKey(text))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"StoreStuff: Duplicate Stand Name: ",
					text,
					" Please Fix Gameobject : ",
					stand.gameObject.GetPath(),
					stand.gameObject.name
				}), base.gameObject);
				return;
			}
			this.CosmeticStandsDict.Add(text, stand);
		}

		// Token: 0x06006AF0 RID: 27376 RVA: 0x00229A40 File Offset: 0x00227C40
		public void RemoveStandFromDynamicCosmeticStandsDictionary(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (!this.CosmeticStandsDict.ContainsKey(text))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"StoreStuff: StoreController doesn't have stand in its dict. that's weird!: ",
					text,
					" Please Fix Gameobject : ",
					stand.gameObject.GetPath(),
					stand.gameObject.name
				}), base.gameObject);
				return;
			}
			this.CosmeticStandsDict.Remove(text);
		}

		// Token: 0x06006AF1 RID: 27377 RVA: 0x00229B48 File Offset: 0x00227D48
		private void Create_StandsByPlayfabIDDictionary()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				this.AddStandToPlayfabIDDictionary(dynamicCosmeticStand);
			}
		}

		// Token: 0x06006AF2 RID: 27378 RVA: 0x00229BA0 File Offset: 0x00227DA0
		public void AddStandToPlayfabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
			{
				if (dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
				{
					return;
				}
				if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
				{
					this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
					return;
				}
				this.StandsByPlayfabID.Add(dynamicCosmeticStand.thisCosmeticName, new List<DynamicCosmeticStand>
				{
					dynamicCosmeticStand
				});
			}
		}

		// Token: 0x06006AF3 RID: 27379 RVA: 0x00229C10 File Offset: 0x00227E10
		public void RemoveStandFromPlayFabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			List<DynamicCosmeticStand> list;
			if (this.StandsByPlayfabID.TryGetValue(dynamicCosmeticStand.thisCosmeticName, out list))
			{
				list.Remove(dynamicCosmeticStand);
			}
		}

		// Token: 0x06006AF4 RID: 27380 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void ExportCosmeticStandLayoutWithItems()
		{
		}

		// Token: 0x06006AF5 RID: 27381 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void ExportCosmeticStandLayoutWITHOUTItems()
		{
		}

		// Token: 0x06006AF6 RID: 27382 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void ImportCosmeticStandLayout()
		{
		}

		// Token: 0x06006AF7 RID: 27383 RVA: 0x00229C3A File Offset: 0x00227E3A
		private void InitializeFromTitleData()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("StoreLayoutData", delegate(string data)
			{
				this.ImportCosmeticStandLayoutFromTitleData(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting StoreLayoutData data: {0}", e));
			}, false);
		}

		// Token: 0x06006AF8 RID: 27384 RVA: 0x00229C78 File Offset: 0x00227E78
		private void ImportCosmeticStandLayoutFromTitleData(string TSVData)
		{
			this.standImport = new StandImport();
			this.standImport.DecomposeFromTitleDataString(TSVData);
			foreach (StandTypeData standTypeData in this.standImport.standData)
			{
				string key = string.Concat(new string[]
				{
					standTypeData.departmentID,
					"|",
					standTypeData.displayID,
					"|",
					standTypeData.standID
				});
				this.standImport.standKeyToDataDict.Add(key, standTypeData);
				if (this.CosmeticStandsDict.ContainsKey(key))
				{
					this.CosmeticStandsDict[key].SetStandTypeString(standTypeData.bustType);
					this.CosmeticStandsDict[key].SpawnItemOntoStand(standTypeData.playFabID);
					this.CosmeticStandsDict[key].InitializeCosmetic();
				}
			}
		}

		// Token: 0x06006AF9 RID: 27385 RVA: 0x00229D7C File Offset: 0x00227F7C
		public void InitializeStandFromTitleData(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				Debug.LogError("Stand " + stand.name + " is missing important setup data somehow, please fix!", stand.gameObject);
				return;
			}
			string key = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (!this.CosmeticStandsDict.ContainsKey(key) || !this.standImport.standKeyToDataDict.ContainsKey(key))
			{
				return;
			}
			StandTypeData standTypeData = this.standImport.standKeyToDataDict[key];
			this.CosmeticStandsDict[key].SetStandTypeString(standTypeData.bustType);
			this.CosmeticStandsDict[key].SpawnItemOntoStand(standTypeData.playFabID);
			this.CosmeticStandsDict[key].InitializeCosmetic();
		}

		// Token: 0x06006AFA RID: 27386 RVA: 0x00229EB3 File Offset: 0x002280B3
		public void InitalizeCosmeticStands()
		{
			this.cosmeticsInitialized = true;
			this.RefreshCosmeticStandsDictionaryFromDepartments();
			if (this.LoadFromTitleData)
			{
				this.InitializeFromTitleData();
			}
		}

		// Token: 0x06006AFB RID: 27387 RVA: 0x00229ED0 File Offset: 0x002280D0
		public void LoadCosmeticOntoStand(string standID, string playFabId)
		{
			if (this.CosmeticStandsDict.ContainsKey(standID))
			{
				this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
				Debug.Log("StoreStuff: Cosmetic Loaded Onto Stand: " + standID + " | " + playFabId);
			}
		}

		// Token: 0x06006AFC RID: 27388 RVA: 0x00229F08 File Offset: 0x00228108
		public void ClearCosmetics()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				StoreDisplay[] displays = storeDepartment.Displays;
				for (int i = 0; i < displays.Length; i++)
				{
					DynamicCosmeticStand[] stands = displays[i].Stands;
					for (int j = 0; j < stands.Length; j++)
					{
						stands[j].ClearCosmetics();
					}
				}
			}
		}

		// Token: 0x06006AFD RID: 27389 RVA: 0x00229F8C File Offset: 0x0022818C
		public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
		}

		// Token: 0x06006AFE RID: 27390 RVA: 0x00229FBC File Offset: 0x002281BC
		public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (dynamicCosmeticStand.thisCosmeticName == PlayFabID)
				{
					return dynamicCosmeticStand;
				}
			}
			return null;
		}

		// Token: 0x06006AFF RID: 27391 RVA: 0x0022A024 File Offset: 0x00228224
		public void FindAllDepartments()
		{
			this.Departments = Object.FindObjectsByType<StoreDepartment>(FindObjectsSortMode.None).ToList<StoreDepartment>();
		}

		// Token: 0x06006B00 RID: 27392 RVA: 0x0022A038 File Offset: 0x00228238
		public void SaveAllCosmeticsPositions()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
					{
						Debug.Log(string.Concat(new string[]
						{
							"StoreStuff: Saving Items mount transform: ",
							storeDepartment.departmentName,
							"|",
							storeDisplay.displayName,
							"|",
							dynamicCosmeticStand.StandName,
							"|",
							dynamicCosmeticStand.DisplayHeadModel.bustType.ToString(),
							"|",
							dynamicCosmeticStand.thisCosmeticName
						}));
						dynamicCosmeticStand.UpdateCosmeticsMountPositions();
					}
				}
			}
		}

		// Token: 0x06006B01 RID: 27393 RVA: 0x0022A158 File Offset: 0x00228358
		public static void SetForGame()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			StoreController.instance.RefreshCosmeticStandsDictionaryFromDepartments();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
				dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
			}
		}

		// Token: 0x04007B17 RID: 31511
		[OnEnterPlay_Clear]
		public static volatile StoreController instance;

		// Token: 0x04007B18 RID: 31512
		public List<StoreDepartment> Departments;

		// Token: 0x04007B19 RID: 31513
		private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;

		// Token: 0x04007B1A RID: 31514
		public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;

		// Token: 0x04007B1B RID: 31515
		public AllCosmeticsArraySO AllCosmeticsArraySO;

		// Token: 0x04007B1C RID: 31516
		public bool cosmeticsInitialized;

		// Token: 0x04007B1D RID: 31517
		public bool LoadFromTitleData;

		// Token: 0x04007B1E RID: 31518
		private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";

		// Token: 0x04007B1F RID: 31519
		private StandImport standImport;
	}
}
