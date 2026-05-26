using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class SlingshotLifeIndicator : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001D71 RID: 7537 RVA: 0x0009F37F File Offset: 0x0009D57F
	// (set) Token: 0x06001D72 RID: 7538 RVA: 0x0009F387 File Offset: 0x0009D587
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06001D73 RID: 7539 RVA: 0x0009F390 File Offset: 0x0009D590
	// (set) Token: 0x06001D74 RID: 7540 RVA: 0x0009F398 File Offset: 0x0009D598
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001D75 RID: 7541 RVA: 0x0009F3A1 File Offset: 0x0009D5A1
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x0009F3AA File Offset: 0x0009D5AA
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x0009F3CE File Offset: 0x0009D5CE
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.Reset();
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x0009F3F9 File Offset: 0x0009D5F9
	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(false);
		}
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x0009F424 File Offset: 0x0009D624
	public void SliceUpdate()
	{
		if (!NetworkSystem.Instance.InRoom || (this.checkedBattle && !this.inBattle))
		{
			if (this.indicator1.activeSelf)
			{
				this.indicator1.SetActive(false);
			}
			if (this.indicator2.activeSelf)
			{
				this.indicator2.SetActive(false);
			}
			if (this.indicator3.activeSelf)
			{
				this.indicator3.SetActive(false);
			}
			return;
		}
		if (this.bMgr == null)
		{
			this.checkedBattle = true;
			this.inBattle = true;
			if (GorillaGameManager.instance == null)
			{
				return;
			}
			this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (this.bMgr == null)
			{
				this.inBattle = false;
				return;
			}
		}
		VRRig vrrig = this.myRig;
		if (((vrrig != null) ? vrrig.creator : null) == null)
		{
			return;
		}
		int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
		this.SetActive(this.indicator1, playerLives >= 1);
		this.SetActive(this.indicator2, playerLives >= 2);
		this.SetActive(this.indicator3, playerLives >= 3);
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x0009F559 File Offset: 0x0009D759
	public void OnLeftRoom()
	{
		this.Reset();
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x0009F561 File Offset: 0x0009D761
	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	// Token: 0x040027C0 RID: 10176
	private VRRig myRig;

	// Token: 0x040027C1 RID: 10177
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x040027C2 RID: 10178
	public bool checkedBattle;

	// Token: 0x040027C3 RID: 10179
	public bool inBattle;

	// Token: 0x040027C4 RID: 10180
	public GameObject indicator1;

	// Token: 0x040027C5 RID: 10181
	public GameObject indicator2;

	// Token: 0x040027C6 RID: 10182
	public GameObject indicator3;
}
