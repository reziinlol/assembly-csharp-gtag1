using System;
using UnityEngine;

// Token: 0x02000D28 RID: 3368
public abstract class BasePageHandler : MonoBehaviour
{
	// Token: 0x170007DA RID: 2010
	// (get) Token: 0x06005317 RID: 21271 RVA: 0x001B3270 File Offset: 0x001B1470
	// (set) Token: 0x06005318 RID: 21272 RVA: 0x001B3278 File Offset: 0x001B1478
	private protected int selectedIndex { protected get; private set; }

	// Token: 0x170007DB RID: 2011
	// (get) Token: 0x06005319 RID: 21273 RVA: 0x001B3281 File Offset: 0x001B1481
	// (set) Token: 0x0600531A RID: 21274 RVA: 0x001B3289 File Offset: 0x001B1489
	private protected int currentPage { protected get; private set; }

	// Token: 0x170007DC RID: 2012
	// (get) Token: 0x0600531B RID: 21275 RVA: 0x001B3292 File Offset: 0x001B1492
	// (set) Token: 0x0600531C RID: 21276 RVA: 0x001B329A File Offset: 0x001B149A
	private protected int pages { protected get; private set; }

	// Token: 0x170007DD RID: 2013
	// (get) Token: 0x0600531D RID: 21277 RVA: 0x001B32A3 File Offset: 0x001B14A3
	// (set) Token: 0x0600531E RID: 21278 RVA: 0x001B32AB File Offset: 0x001B14AB
	private protected int maxEntires { protected get; private set; }

	// Token: 0x170007DE RID: 2014
	// (get) Token: 0x0600531F RID: 21279
	protected abstract int pageSize { get; }

	// Token: 0x170007DF RID: 2015
	// (get) Token: 0x06005320 RID: 21280
	protected abstract int entriesCount { get; }

	// Token: 0x06005321 RID: 21281 RVA: 0x001B32B4 File Offset: 0x001B14B4
	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	// Token: 0x06005322 RID: 21282 RVA: 0x001B331C File Offset: 0x001B151C
	public void SelectEntryOnPage(int entryIndex)
	{
		int num = entryIndex + this.pageSize * this.currentPage;
		if (num > this.entriesCount)
		{
			return;
		}
		this.selectedIndex = num;
		this.PageEntrySelected(entryIndex, this.selectedIndex);
	}

	// Token: 0x06005323 RID: 21283 RVA: 0x001B3358 File Offset: 0x001B1558
	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int pageEntry = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(pageEntry, index);
		this.SetPage(this.currentPage);
	}

	// Token: 0x06005324 RID: 21284 RVA: 0x001B33A4 File Offset: 0x001B15A4
	public void ChangePage(bool left)
	{
		int num = left ? -1 : 1;
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	// Token: 0x06005325 RID: 21285 RVA: 0x001B33D4 File Offset: 0x001B15D4
	public void SetPage(int page)
	{
		if (page > this.pages)
		{
			return;
		}
		this.currentPage = page;
		int num = this.pageSize * page;
		this.ShowPage(this.currentPage, num, Mathf.Min(num + this.pageSize, this.entriesCount));
	}

	// Token: 0x06005326 RID: 21286
	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	// Token: 0x06005327 RID: 21287
	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);
}
