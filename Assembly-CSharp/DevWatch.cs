using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000AF RID: 175
public class DevWatch : MonoBehaviour
{
	// Token: 0x06000436 RID: 1078 RVA: 0x00018A10 File Offset: 0x00016C10
	private void Awake()
	{
		this.SearchButton.SearchEvent.AddListener(new UnityAction(this.SearchItems));
		this.TakeOwnershipButton.onClick.AddListener(new UnityAction(this.TakeOwneshipOfItem));
		this.DestroyObjectButton.onClick.AddListener(new UnityAction(this.TryDestroyItem));
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00018A74 File Offset: 0x00016C74
	public void SearchItems()
	{
		this.FoundNetworkObjects.Clear();
		RaycastHit[] array = Physics.SphereCastAll(new Ray(this.RayCastStartPos.position, this.RayCastDirection.position - this.RayCastStartPos.position), 0.3f, 100f);
		if (array.Length != 0)
		{
			foreach (RaycastHit raycastHit in array)
			{
				NetworkObject item;
				if (raycastHit.collider.gameObject.TryGetComponent<NetworkObject>(out item))
				{
					this.FoundNetworkObjects.Add(item);
				}
			}
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00018B08 File Offset: 0x00016D08
	public void Cleanup()
	{
		this.FoundNetworkObjects.Clear();
		if (this.Items.Count > 0)
		{
			for (int i = this.Items.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.Items[i]);
			}
		}
		this.Items.Clear();
		this.Panel1.SetActive(true);
		this.Panel2.SetActive(false);
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00018B7A File Offset: 0x00016D7A
	public void ItemSelected(DevWatchSelectableItem item)
	{
		this.Panel1.SetActive(false);
		this.Panel2.SetActive(true);
		this.SelectedItem = item;
		this.SelectedItemName.text = item.ItemName.text;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void TryDestroyItem()
	{
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void TakeOwneshipOfItem()
	{
	}

	// Token: 0x0400049B RID: 1179
	public DevWatchButton SearchButton;

	// Token: 0x0400049C RID: 1180
	public GameObject Panel1;

	// Token: 0x0400049D RID: 1181
	public GameObject Panel2;

	// Token: 0x0400049E RID: 1182
	public DevWatchSelectableItem SelectableItemPrefab;

	// Token: 0x0400049F RID: 1183
	public List<DevWatchSelectableItem> Items;

	// Token: 0x040004A0 RID: 1184
	public Transform RayCastStartPos;

	// Token: 0x040004A1 RID: 1185
	public Transform RayCastDirection;

	// Token: 0x040004A2 RID: 1186
	public Transform ItemsFoundContainer;

	// Token: 0x040004A3 RID: 1187
	public Button TakeOwnershipButton;

	// Token: 0x040004A4 RID: 1188
	public Button DestroyObjectButton;

	// Token: 0x040004A5 RID: 1189
	public List<NetworkObject> FoundNetworkObjects = new List<NetworkObject>();

	// Token: 0x040004A6 RID: 1190
	public TextMeshProUGUI SelectedItemName;

	// Token: 0x040004A7 RID: 1191
	public DevWatchSelectableItem SelectedItem;
}
