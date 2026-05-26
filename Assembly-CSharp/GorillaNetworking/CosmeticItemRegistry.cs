using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001018 RID: 4120
	public class CosmeticItemRegistry
	{
		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x060066D0 RID: 26320 RVA: 0x00210EB8 File Offset: 0x0020F0B8
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x060066D1 RID: 26321 RVA: 0x00210EC0 File Offset: 0x0020F0C0
		public void RefreshRig()
		{
			this.rig.RefreshCosmetics();
		}

		// Token: 0x060066D2 RID: 26322 RVA: 0x00210ECD File Offset: 0x0020F0CD
		public CosmeticItemRegistry(VRRig _rig)
		{
			this.rig = _rig;
		}

		// Token: 0x060066D3 RID: 26323 RVA: 0x00210EF4 File Offset: 0x0020F0F4
		public void InitializeCosmetic(GameObject cosmeticGObj, bool isOverride)
		{
			if (this.initializedCosmetics.Contains(cosmeticGObj))
			{
				return;
			}
			this.initializedCosmetics.Add(cosmeticGObj);
			if (!isOverride)
			{
				foreach (GameObject gameObject in this.rig.overrideCosmetics)
				{
					if (cosmeticGObj.name == gameObject.name)
					{
						cosmeticGObj.name = "OVERRIDDEN";
						return;
					}
				}
			}
			string text = cosmeticGObj.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
			CosmeticItemInstance cosmeticItemInstance;
			if (this._nameToCosmeticMap.ContainsKey(text))
			{
				cosmeticItemInstance = this._nameToCosmeticMap[text];
			}
			else
			{
				cosmeticItemInstance = new CosmeticItemInstance();
				CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(text);
				cosmeticItemInstance.clippingOffsets = ((cosmeticSOFromDisplayName != null) ? cosmeticSOFromDisplayName.info.anchorAntiIntersectOffsets : CosmeticsController.instance.defaultClipOffsets);
				cosmeticItemInstance.isHoldableItem = (cosmeticSOFromDisplayName != null && cosmeticSOFromDisplayName.info.hasHoldableParts);
				this._nameToCosmeticMap.Add(text, cosmeticItemInstance);
			}
			HoldableObject component = cosmeticGObj.GetComponent<HoldableObject>();
			bool flag = cosmeticGObj.name.Contains("LEFT.");
			bool flag2 = cosmeticGObj.name.Contains("RIGHT.");
			if (cosmeticItemInstance.isHoldableItem && component != null)
			{
				if (component is SnowballThrowable || component is TransferrableObject)
				{
					cosmeticItemInstance.holdableObjects.Add(cosmeticGObj);
				}
				else if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(cosmeticGObj);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(cosmeticGObj);
				}
				else
				{
					cosmeticItemInstance.objects.Add(cosmeticGObj);
				}
			}
			else if (flag)
			{
				cosmeticItemInstance.leftObjects.Add(cosmeticGObj);
			}
			else if (flag2)
			{
				cosmeticItemInstance.rightObjects.Add(cosmeticGObj);
			}
			else
			{
				cosmeticItemInstance.objects.Add(cosmeticGObj);
			}
			cosmeticItemInstance.dbgname = text;
			Renderer[] componentsInChildren = cosmeticGObj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].enabled)
				{
					cosmeticItemInstance.allRenderers.Add(componentsInChildren[i]);
				}
			}
			ParticleSystem[] componentsInChildren2 = cosmeticGObj.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].emission.enabled)
				{
					cosmeticItemInstance.allParticles.Add(componentsInChildren2[j]);
				}
			}
		}

		// Token: 0x060066D4 RID: 26324 RVA: 0x00211178 File Offset: 0x0020F378
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance result;
			if (!this._nameToCosmeticMap.TryGetValue(itemName, out result))
			{
				CosmeticsV2Spawner_Dirty.ProcessLoadOpInfos(this.rig, itemName, this);
				return null;
			}
			return result;
		}

		// Token: 0x04007634 RID: 30260
		private Dictionary<string, CosmeticItemInstance> _nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x04007635 RID: 30261
		private HashSet<GameObject> initializedCosmetics = new HashSet<GameObject>();

		// Token: 0x04007636 RID: 30262
		private GameObject _nullItem;

		// Token: 0x04007637 RID: 30263
		private VRRig rig;
	}
}
