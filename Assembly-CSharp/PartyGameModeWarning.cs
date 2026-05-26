using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class PartyGameModeWarning : MonoBehaviour
{
	// Token: 0x17000210 RID: 528
	// (get) Token: 0x060014FA RID: 5370 RVA: 0x0006F910 File Offset: 0x0006DB10
	public bool ShouldShowWarning
	{
		get
		{
			return FriendshipGroupDetection.Instance.IsInParty && FriendshipGroupDetection.Instance.AnyPartyMembersOutsideFriendCollider();
		}
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x0006F92C File Offset: 0x0006DB2C
	private void Awake()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x0006F957 File Offset: 0x0006DB57
	public void Show()
	{
		this.visibleUntilTimestamp = Time.time + this.visibleDuration;
		if (this.hideCoroutine == null)
		{
			this.hideCoroutine = base.StartCoroutine(this.HideCo());
		}
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x0006F985 File Offset: 0x0006DB85
	private IEnumerator HideCo()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		float lastVisible;
		do
		{
			lastVisible = this.visibleUntilTimestamp;
			yield return new WaitForSeconds(this.visibleUntilTimestamp - Time.time);
		}
		while (lastVisible != this.visibleUntilTimestamp);
		array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x040019DA RID: 6618
	[SerializeField]
	private GameObject[] showParts;

	// Token: 0x040019DB RID: 6619
	[SerializeField]
	private GameObject[] hideParts;

	// Token: 0x040019DC RID: 6620
	[SerializeField]
	private float visibleDuration;

	// Token: 0x040019DD RID: 6621
	private float visibleUntilTimestamp;

	// Token: 0x040019DE RID: 6622
	private Coroutine hideCoroutine;
}
