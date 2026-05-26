using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200081A RID: 2074
public class GRUIBuyItem : MonoBehaviour
{
	// Token: 0x06003541 RID: 13633 RVA: 0x00126D4A File Offset: 0x00124F4A
	public void Setup(int standId)
	{
		this.standId = standId;
		this.buyItemButton.onPressButton.AddListener(new UnityAction(this.OnBuyItem));
		this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
	}

	// Token: 0x06003542 RID: 13634 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnBuyItem()
	{
	}

	// Token: 0x06003543 RID: 13635 RVA: 0x00126D8A File Offset: 0x00124F8A
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x040045C8 RID: 17864
	[SerializeField]
	private GorillaPressableButton buyItemButton;

	// Token: 0x040045C9 RID: 17865
	[SerializeField]
	private Text itemInfoLabel;

	// Token: 0x040045CA RID: 17866
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x040045CB RID: 17867
	[SerializeField]
	private GameEntity entityPrefab;

	// Token: 0x040045CC RID: 17868
	private int entityTypeId;

	// Token: 0x040045CD RID: 17869
	private int standId;
}
