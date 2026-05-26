using System;
using DefaultNamespace;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009C RID: 156
public class EvolvingCosmetic : MonoBehaviour, ICosmeticStateSync
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060003E1 RID: 993 RVA: 0x00017483 File Offset: 0x00015683
	public int StateValue
	{
		get
		{
			return this.SelectedObjectIndex;
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060003E2 RID: 994 RVA: 0x0001748B File Offset: 0x0001568B
	// (set) Token: 0x060003E3 RID: 995 RVA: 0x00017493 File Offset: 0x00015693
	public int SelectedObjectIndex { get; private set; } = -1;

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060003E4 RID: 996 RVA: 0x0001749C File Offset: 0x0001569C
	public string PlayfabId
	{
		get
		{
			return base.gameObject.name;
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x000174AC File Offset: 0x000156AC
	private void Awake()
	{
		int num;
		if (EvolvingCosmeticSaveData.Instance.SelectedIndices.TryGetValue(this.PlayfabId, out num) && this.IsIndexAvailable(num))
		{
			this.SelectedObjectIndex = num;
			this.ActivateSelectedIndex();
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x000174E8 File Offset: 0x000156E8
	private void OnEnable()
	{
		VRRig vrrig = base.GetComponentInParent<VRRig>();
		if (vrrig == null)
		{
			if (base.GetComponentInParent<GTPlayer>() == null)
			{
				return;
			}
			vrrig = VRRig.LocalRig;
		}
		if (vrrig == null)
		{
			return;
		}
		this._daysAccrued = new int?(0);
		this.UnselectAll();
		VRRigReliableState reliableState = vrrig.reliableState;
		if (reliableState != null)
		{
			reliableState.RegisterCosmeticStateSyncTarget(this.GetStateSyncSlot(), this);
		}
		SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(vrrig);
		switch (this.ageRule)
		{
		case EvolvingCosmetic.SubscriptionAgeRule.ItemAge:
			this._daysAccrued = new int?(vrrig.CheckCosmeticAge(base.name));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAge:
			this._daysAccrued = new int?(Mathf.Min(subscriptionDetails.daysAccrued, vrrig.CheckCosmeticAge(base.name)));
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAge:
			this._daysAccrued = new int?(subscriptionDetails.daysAccrued);
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.MinItemSubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				this._daysAccrued = new int?(Mathf.Min(subscriptionDetails.daysAccrued, vrrig.CheckCosmeticAge(base.name)));
			}
			break;
		case EvolvingCosmetic.SubscriptionAgeRule.SubscriptionAgeActive:
			if (subscriptionDetails.active)
			{
				this._daysAccrued = new int?(subscriptionDetails.daysAccrued);
			}
			break;
		}
		if (this._daysAccrued == null)
		{
			Debug.LogError("_daysAccrued was not set by end of OnEnable.");
			return;
		}
		int value = this._daysAccrued.Value;
		this.SelectedObjectIndex = this.FindAgeAwareIndex(value);
		this.ActivateSelectedIndex();
		UnityEvent<int> dispatchDaysOnEnable = this.DispatchDaysOnEnable;
		if (dispatchDaysOnEnable != null)
		{
			dispatchDaysOnEnable.Invoke(Mathf.Min(value, this.capDays));
		}
		if (this.maxDays > 0)
		{
			UnityEvent<float> dispatchDaysOnEnableNormalized = this.DispatchDaysOnEnableNormalized;
			if (dispatchDaysOnEnableNormalized == null)
			{
				return;
			}
			dispatchDaysOnEnableNormalized.Invoke(Mathf.Min((float)value / (float)this.maxDays, 1f) * (float)this.multiplier);
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x0001769C File Offset: 0x0001589C
	private int FindAgeAwareIndex(int daysAccrued)
	{
		if (this.ageAwareGameObjects.Length == 0)
		{
			return 0;
		}
		if (this.ageAwareGameObjects[0].minActiveDays > daysAccrued)
		{
			return -1;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			if (daysAccrued <= this.ageAwareGameObjects[i].maxActiveDays)
			{
				return i;
			}
		}
		return this.ageAwareGameObjects.Length - 1;
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00017700 File Offset: 0x00015900
	private void OnDisable()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		VRRigReliableState vrrigReliableState = (componentInParent != null) ? componentInParent.reliableState : null;
		if (vrrigReliableState != null)
		{
			vrrigReliableState.UnRegisterCosmeticStateSyncTarget(this.GetStateSyncSlot(), this);
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00017730 File Offset: 0x00015930
	private void ActivateSelectedIndex()
	{
		if (!this.IsSelectedIndexAvailable())
		{
			return;
		}
		for (int i = 0; i < this.ageAwareGameObjects.Length; i++)
		{
			this.ageAwareGameObjects[i].gameObject.SetActive(i == this.SelectedObjectIndex);
		}
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00017778 File Offset: 0x00015978
	private bool IsSelectedIndexAvailable()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex);
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x00017788 File Offset: 0x00015988
	private bool IsIndexAvailable(int index)
	{
		if (index < 0 || index >= this.ageAwareGameObjects.Length)
		{
			return false;
		}
		EvolvingCosmetic.AgeAwareGameObject ageAwareGameObject = this.ageAwareGameObjects[index];
		return this._daysAccrued.Value >= ageAwareGameObject.minActiveDays;
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x000177CC File Offset: 0x000159CC
	public void GoBack()
	{
		if (!this.CanGoBack())
		{
			return;
		}
		int selectedObjectIndex = this.SelectedObjectIndex;
		this.SelectedObjectIndex = selectedObjectIndex - 1;
		this.ActivateSelectedIndex();
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x000177F8 File Offset: 0x000159F8
	public void GoForward()
	{
		if (!this.CanGoForward())
		{
			return;
		}
		int selectedObjectIndex = this.SelectedObjectIndex;
		this.SelectedObjectIndex = selectedObjectIndex + 1;
		this.ActivateSelectedIndex();
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00017824 File Offset: 0x00015A24
	public void MatchStage(EvolvingCosmetic other)
	{
		while (this.SelectedObjectIndex > other.SelectedObjectIndex)
		{
			int selectedObjectIndex;
			if (!this.CanGoBack())
			{
				IL_42:
				while (this.SelectedObjectIndex < other.SelectedObjectIndex && this.CanGoForward())
				{
					selectedObjectIndex = this.SelectedObjectIndex;
					this.SelectedObjectIndex = selectedObjectIndex + 1;
				}
				this.ActivateSelectedIndex();
				return;
			}
			selectedObjectIndex = this.SelectedObjectIndex;
			this.SelectedObjectIndex = selectedObjectIndex - 1;
		}
		goto IL_42;
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00017888 File Offset: 0x00015A88
	private void UnselectAll()
	{
		this.SelectedObjectIndex = -1;
		EvolvingCosmetic.AgeAwareGameObject[] array = this.ageAwareGameObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000178C3 File Offset: 0x00015AC3
	public bool CanGoBack()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex - 1);
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x000178D3 File Offset: 0x00015AD3
	public bool CanGoForward()
	{
		return this.IsIndexAvailable(this.SelectedObjectIndex + 1);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x000178E3 File Offset: 0x00015AE3
	public void OnStateUpdate(int state)
	{
		if (!this.IsIndexAvailable(state))
		{
			return;
		}
		this.SelectedObjectIndex = state;
		this.ActivateSelectedIndex();
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x000178FC File Offset: 0x00015AFC
	private VRRigReliableState.StateSyncSlots GetStateSyncSlot()
	{
		CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(this.PlayfabId);
		CosmeticsController.CosmeticCategory value = cosmeticSOFromDisplayName.info.category.Value;
		VRRigReliableState.StateSyncSlots result;
		if (value != CosmeticsController.CosmeticCategory.Hat)
		{
			if (value != CosmeticsController.CosmeticCategory.Face)
			{
				if (value != CosmeticsController.CosmeticCategory.Shirt)
				{
					throw new Exception(string.Format("Unhandled CosmeticCategory {0}", cosmeticSOFromDisplayName.info.category.Value));
				}
				result = VRRigReliableState.StateSyncSlots.Shirt;
			}
			else
			{
				result = VRRigReliableState.StateSyncSlots.Face;
			}
		}
		else
		{
			result = VRRigReliableState.StateSyncSlots.Hat;
		}
		return result;
	}

	// Token: 0x04000450 RID: 1104
	[SerializeField]
	private EvolvingCosmetic.SubscriptionAgeRule ageRule;

	// Token: 0x04000451 RID: 1105
	[SerializeField]
	private EvolvingCosmetic.AgeAwareGameObject[] ageAwareGameObjects;

	// Token: 0x04000452 RID: 1106
	[SerializeField]
	private int capDays = 1;

	// Token: 0x04000453 RID: 1107
	[SerializeField]
	private UnityEvent<int> DispatchDaysOnEnable;

	// Token: 0x04000454 RID: 1108
	[SerializeField]
	private int maxDays = 1;

	// Token: 0x04000455 RID: 1109
	[SerializeField]
	private int multiplier = 1;

	// Token: 0x04000456 RID: 1110
	[SerializeField]
	private UnityEvent<float> DispatchDaysOnEnableNormalized;

	// Token: 0x04000458 RID: 1112
	private int? _daysAccrued;

	// Token: 0x0200009D RID: 157
	private enum SubscriptionAgeRule
	{
		// Token: 0x0400045A RID: 1114
		ItemAge,
		// Token: 0x0400045B RID: 1115
		MinItemSubscriptionAge,
		// Token: 0x0400045C RID: 1116
		SubscriptionAge,
		// Token: 0x0400045D RID: 1117
		MinItemSubscriptionAgeActive,
		// Token: 0x0400045E RID: 1118
		SubscriptionAgeActive
	}

	// Token: 0x0200009E RID: 158
	[Serializable]
	private struct AgeAwareGameObject
	{
		// Token: 0x0400045F RID: 1119
		public GameObject gameObject;

		// Token: 0x04000460 RID: 1120
		public int minActiveDays;

		// Token: 0x04000461 RID: 1121
		public int maxActiveDays;

		// Token: 0x04000462 RID: 1122
		public bool requireCurrentSubscription;
	}
}
