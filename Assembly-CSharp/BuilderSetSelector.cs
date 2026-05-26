using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000657 RID: 1623
public class BuilderSetSelector : MonoBehaviour
{
	// Token: 0x06002870 RID: 10352 RVA: 0x000DB208 File Offset: 0x000D9408
	private void Start()
	{
		this.zoneRenderers.Clear();
		foreach (GorillaPressableButton gorillaPressableButton in this.groupButtons)
		{
			this.zoneRenderers.Add(gorillaPressableButton.buttonRenderer);
			TMP_Text myTmpText = gorillaPressableButton.myTmpText;
			Renderer renderer = (myTmpText != null) ? myTmpText.GetComponent<Renderer>() : null;
			if (renderer != null)
			{
				this.zoneRenderers.Add(renderer);
			}
		}
		this.zoneRenderers.Add(this.previousPageButton.buttonRenderer);
		this.zoneRenderers.Add(this.nextPageButton.buttonRenderer);
		TMP_Text myTmpText2 = this.previousPageButton.myTmpText;
		Renderer renderer2 = (myTmpText2 != null) ? myTmpText2.GetComponent<Renderer>() : null;
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		TMP_Text myTmpText3 = this.nextPageButton.myTmpText;
		renderer2 = ((myTmpText3 != null) ? myTmpText3.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		foreach (Renderer renderer3 in this.zoneRenderers)
		{
			renderer3.enabled = false;
		}
		this.inBuilderZone = false;
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000DB374 File Offset: 0x000D9574
	public void Setup(List<BuilderPieceSet.BuilderPieceCategory> categories)
	{
		List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups = BuilderSetManager.instance.GetLiveDisplayGroups();
		this.numLiveDisplayGroups = liveDisplayGroups.Count;
		this.includedGroups = new List<BuilderPieceSet.BuilderDisplayGroup>(liveDisplayGroups.Count);
		this._includedCategories = categories;
		foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in liveDisplayGroups)
		{
			if (this.DoesDisplayGroupHaveIncludedCategories(builderDisplayGroup))
			{
				this.includedGroups.Add(builderDisplayGroup);
			}
		}
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedGroups));
		BuilderSetManager.instance.OnLiveSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedGroups));
		this.groupsPerPage = this.groupButtons.Length;
		this.totalPages = this.includedGroups.Count / this.groupsPerPage;
		if (this.includedGroups.Count % this.groupsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.pageIndex = 0;
		this.currentGroup = this.includedGroups[this.includedGroupIndex];
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.groupButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += this.OnSetButtonPressed;
		}
		this.UpdateLabels();
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000DB574 File Offset: 0x000D9774
	private void OnDestroy()
	{
		if (this.previousPageButton != null)
		{
			this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
		}
		if (this.nextPageButton != null)
		{
			this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedGroups));
			BuilderSetManager.instance.OnLiveSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedGroups));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.groupButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnSetButtonPressed;
			}
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000DB688 File Offset: 0x000D9888
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Renderer renderer = enumerator.Current;
					renderer.enabled = true;
				}
				goto IL_8B;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			foreach (Renderer renderer2 in this.zoneRenderers)
			{
				renderer2.enabled = false;
			}
		}
		IL_8B:
		this.inBuilderZone = flag;
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000DB744 File Offset: 0x000D9944
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		int num = 0;
		for (int i = 0; i < this.groupButtons.Length; i++)
		{
			if (button.Equals(this.groupButtons[i]))
			{
				num = i;
				break;
			}
		}
		int num2 = this.pageIndex * this.groupsPerPage + num;
		if (num2 < this.includedGroups.Count)
		{
			BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.includedGroups[num2];
			if (this.currentGroup == null || builderDisplayGroup.displayName != this.currentGroup.displayName)
			{
				UnityEvent<int> onSelectedGroup = this.OnSelectedGroup;
				if (onSelectedGroup == null)
				{
					return;
				}
				onSelectedGroup.Invoke(builderDisplayGroup.GetDisplayGroupIdentifier());
			}
		}
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x000DB7DC File Offset: 0x000D99DC
	private void RefreshUnlockedGroups()
	{
		List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups = BuilderSetManager.instance.GetLiveDisplayGroups();
		if (liveDisplayGroups.Count != this.numLiveDisplayGroups)
		{
			string value = (this.currentGroup != null) ? this.currentGroup.displayName : "";
			this.numLiveDisplayGroups = liveDisplayGroups.Count;
			this.includedGroups.EnsureCapacity(this.numLiveDisplayGroups);
			this.includedGroups.Clear();
			int num = 0;
			foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in liveDisplayGroups)
			{
				if (this.DoesDisplayGroupHaveIncludedCategories(builderDisplayGroup))
				{
					if (builderDisplayGroup.displayName.Equals(value))
					{
						num = this.includedGroups.Count;
					}
					this.includedGroups.Add(builderDisplayGroup);
				}
			}
			if (this.includedGroups.Count < 1)
			{
				this.currentGroup = null;
			}
			else
			{
				this.includedGroupIndex = num;
				this.currentGroup = this.includedGroups[this.includedGroupIndex];
			}
			this.totalPages = this.includedGroups.Count / this.groupsPerPage;
			if (this.includedGroups.Count % this.groupsPerPage > 0)
			{
				this.totalPages++;
			}
			this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
			this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
			this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
			this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		}
		this.UpdateLabels();
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x000DB994 File Offset: 0x000D9B94
	private void OnPreviousPageClicked()
	{
		this.RefreshUnlockedGroups();
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000DB9D4 File Offset: 0x000D9BD4
	private void OnNextPageClicked()
	{
		this.RefreshUnlockedGroups();
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000DBA14 File Offset: 0x000D9C14
	public void SetSelection(int groupID)
	{
		if (BuilderSetManager.instance == null)
		{
			return;
		}
		BuilderPieceSet.BuilderDisplayGroup newGroup = BuilderSetManager.instance.GetDisplayGroupFromIndex(groupID);
		if (newGroup == null)
		{
			return;
		}
		this.currentGroup = newGroup;
		this.includedGroupIndex = this.includedGroups.FindIndex((BuilderPieceSet.BuilderDisplayGroup x) => x.displayName == newGroup.displayName);
		this.UpdateLabels();
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000DBA84 File Offset: 0x000D9C84
	private void UpdateLabels()
	{
		for (int i = 0; i < this.groupLabels.Length; i++)
		{
			int num = this.pageIndex * this.groupsPerPage + i;
			if (num < this.includedGroups.Count && this.includedGroups[num] != null)
			{
				if (!this.groupButtons[i].gameObject.activeSelf)
				{
					this.groupButtons[i].gameObject.SetActive(true);
					this.groupButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.groupButtons[i].myTmpText.text != this.includedGroups[num].displayName)
				{
					this.groupButtons[i].myTmpText.text = this.includedGroups[num].displayName;
				}
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.includedGroups[num].setID))
				{
					bool flag = this.currentGroup != null && this.includedGroups[num].displayName == this.currentGroup.displayName;
					if (flag != this.groupButtons[i].isOn || !this.groupButtons[i].enabled)
					{
						this.groupButtons[i].isOn = flag;
						this.groupButtons[i].buttonRenderer.material = (flag ? this.groupButtons[i].pressedMaterial : this.groupButtons[i].unpressedMaterial);
					}
					this.groupButtons[i].enabled = true;
				}
				else
				{
					if (this.groupButtons[i].enabled)
					{
						this.groupButtons[i].buttonRenderer.material = this.disabledMaterial;
					}
					this.groupButtons[i].enabled = false;
				}
			}
			else
			{
				if (this.groupButtons[i].gameObject.activeSelf)
				{
					this.groupButtons[i].gameObject.SetActive(false);
					this.groupButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.groupButtons[i].isOn || this.groupButtons[i].enabled)
				{
					this.groupButtons[i].isOn = false;
					this.groupButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000DBD58 File Offset: 0x000D9F58
	public bool DoesDisplayGroupHaveIncludedCategories(BuilderPieceSet.BuilderDisplayGroup set)
	{
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in set.pieceSubsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x000DBDC0 File Offset: 0x000D9FC0
	public BuilderPieceSet.BuilderDisplayGroup GetSelectedGroup()
	{
		return this.currentGroup;
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000DBDC8 File Offset: 0x000D9FC8
	public int GetDefaultGroupID()
	{
		if (this.includedGroups == null || this.includedGroups.Count < 1)
		{
			return -1;
		}
		BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.includedGroups[0];
		if (!BuilderSetManager.instance.IsPieceSetOwnedLocally(builderDisplayGroup.setID))
		{
			foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup2 in this.includedGroups)
			{
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(builderDisplayGroup2.setID))
				{
					return builderDisplayGroup2.GetDisplayGroupIdentifier();
				}
			}
			Debug.LogWarning("No default group available for shelf");
			return -1;
		}
		return builderDisplayGroup.GetDisplayGroupIdentifier();
	}

	// Token: 0x040034BD RID: 13501
	private List<BuilderPieceSet.BuilderDisplayGroup> includedGroups;

	// Token: 0x040034BE RID: 13502
	private int numLiveDisplayGroups;

	// Token: 0x040034BF RID: 13503
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x040034C0 RID: 13504
	[Header("UI")]
	[FormerlySerializedAs("setLabels")]
	[SerializeField]
	private Text[] groupLabels;

	// Token: 0x040034C1 RID: 13505
	[Header("Buttons")]
	[FormerlySerializedAs("setButtons")]
	[SerializeField]
	private GorillaPressableButton[] groupButtons;

	// Token: 0x040034C2 RID: 13506
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x040034C3 RID: 13507
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x040034C4 RID: 13508
	private List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x040034C5 RID: 13509
	private int includedGroupIndex;

	// Token: 0x040034C6 RID: 13510
	private BuilderPieceSet.BuilderDisplayGroup currentGroup;

	// Token: 0x040034C7 RID: 13511
	private int pageIndex;

	// Token: 0x040034C8 RID: 13512
	private int groupsPerPage = 3;

	// Token: 0x040034C9 RID: 13513
	private int totalPages = 1;

	// Token: 0x040034CA RID: 13514
	private List<Renderer> zoneRenderers = new List<Renderer>(10);

	// Token: 0x040034CB RID: 13515
	private bool inBuilderZone;

	// Token: 0x040034CC RID: 13516
	[HideInInspector]
	public UnityEvent<int> OnSelectedGroup;
}
