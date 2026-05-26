using System;
using System.Collections;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class PropHuntDebugHelper : MonoBehaviour
{
	// Token: 0x060010E9 RID: 4329 RVA: 0x0005A8D0 File Offset: 0x00058AD0
	protected void Awake()
	{
		if (PropHuntDebugHelper.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		PropHuntDebugHelper.instance = this;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0005A8EC File Offset: 0x00058AEC
	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		this._propHuntManager = Object.FindAnyObjectByType<GorillaPropHuntGameManager>();
		if (this._propHuntManager != null)
		{
			Debug.Log("PropHuntDebugHelper :: Found number of props " + PropHuntPools.AllPropCosmeticIds.Length.ToString());
			this._cachedAllPropIDs = PropHuntPools.AllPropCosmeticIds;
			this._localPropHuntHandFollower = VRRig.LocalRig.GetComponent<PropHuntHandFollower>();
			this.UpdatePropsText();
		}
		yield break;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x0005A8FC File Offset: 0x00058AFC
	public void UpdatePropsText()
	{
		string selectedPropID = this.GetSelectedPropID(this._selectedPropIndex);
		string text = string.Empty;
		if (this._selectedPropIndex != -1)
		{
			CosmeticSO cosmeticSO = this._allCosmetics.SearchForCosmeticSO(selectedPropID);
			if (cosmeticSO != null)
			{
				text = cosmeticSO.info.displayName;
			}
		}
		this._propsText.text = "Current Prop: " + this.GetCurrentPropInfo() + "\n" + string.Format("Selected Prop: {0} - {1} ({2}/{3})", new object[]
		{
			selectedPropID,
			text,
			this._selectedPropIndex,
			this._cachedAllPropIDs.Length
		});
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0005A99D File Offset: 0x00058B9D
	private string GetCurrentPropInfo()
	{
		return string.Empty;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0005A9A4 File Offset: 0x00058BA4
	private string GetSelectedPropID(int index)
	{
		if (index <= -1)
		{
			return "None";
		}
		return this._cachedAllPropIDs[index];
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0005A9B8 File Offset: 0x00058BB8
	[ContextMenu("Prev Prop")]
	public void PrevProp()
	{
		this._selectedPropIndex--;
		if (this._selectedPropIndex < -1)
		{
			this._selectedPropIndex = this._cachedAllPropIDs.Length - 1;
		}
		string newPropId = (this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty;
		this.SendForcePropHandRPC(newPropId);
		this.UpdatePropsText();
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0005AA18 File Offset: 0x00058C18
	[ContextMenu("Next Prop")]
	public void NextProp()
	{
		this._selectedPropIndex++;
		if (this._selectedPropIndex >= this._cachedAllPropIDs.Length)
		{
			this._selectedPropIndex = -1;
		}
		string newPropId = (this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty;
		this.SendForcePropHandRPC(newPropId);
		this.UpdatePropsText();
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void SendForcePropHandRPC(string newPropId)
	{
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x000028C5 File Offset: 0x00000AC5
	[ContextMenu("Toggle Round")]
	public void ToggleRound()
	{
	}

	// Token: 0x04001423 RID: 5155
	[OnEnterPlay_SetNull]
	public static PropHuntDebugHelper instance;

	// Token: 0x04001424 RID: 5156
	[SerializeField]
	private GorillaPropHuntGameManager _propHuntManager;

	// Token: 0x04001425 RID: 5157
	[SerializeField]
	private PropHuntHandFollower _localPropHuntHandFollower;

	// Token: 0x04001426 RID: 5158
	[SerializeField]
	private TextMeshPro _propsText;

	// Token: 0x04001427 RID: 5159
	[SerializeField]
	private AllCosmeticsArraySO _allCosmetics;

	// Token: 0x04001428 RID: 5160
	private string[] _cachedAllPropIDs;

	// Token: 0x04001429 RID: 5161
	private int _selectedPropIndex = -1;
}
