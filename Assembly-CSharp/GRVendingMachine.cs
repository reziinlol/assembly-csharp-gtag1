using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000827 RID: 2087
public class GRVendingMachine : MonoBehaviour
{
	// Token: 0x0600359C RID: 13724 RVA: 0x00129181 File Offset: 0x00127381
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x0600359D RID: 13725 RVA: 0x0012918A File Offset: 0x0012738A
	public Transform GetSpawnMarker()
	{
		return this.itemSpawnLocation;
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x00129192 File Offset: 0x00127392
	public void NavButtonPressedLeft()
	{
		this.hIndex = Mathf.Max(0, this.hIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x0600359F RID: 13727 RVA: 0x001291AE File Offset: 0x001273AE
	public void NavButtonPressedRight()
	{
		this.hIndex = Mathf.Min(this.hIndex + 1, this.horizontalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x060035A0 RID: 13728 RVA: 0x001291D1 File Offset: 0x001273D1
	public void NavButtonPressedUp()
	{
		this.vIndex = Mathf.Max(0, this.vIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x001291ED File Offset: 0x001273ED
	public void NavButtonPressedDown()
	{
		this.vIndex = Mathf.Min(this.vIndex + 1, this.verticalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x00129210 File Offset: 0x00127410
	public void RequestPurchase()
	{
		if (!this.currentlyVending)
		{
			int num = this.vIndex * this.horizontalSteps + this.hIndex;
			if (num >= 0 && num < this.vendingEntries.Count)
			{
				this.vendingIndex = num;
				if (this.vendingCoroutine != null)
				{
					base.StopCoroutine(this.vendingCoroutine);
				}
				this.vendingCoroutine = base.StartCoroutine(this.VendingCoroutine());
			}
		}
	}

	// Token: 0x060035A3 RID: 13731 RVA: 0x0012927C File Offset: 0x0012747C
	private void RefreshCardReaderDisplay()
	{
		int num = this.vIndex * this.horizontalSteps + this.hIndex;
		if (num >= 0 && num < this.vendingEntries.Count)
		{
			int entityTypeId = this.vendingEntries[num].GetEntityTypeId();
			int itemCost = this.reactor.GetItemCost(entityTypeId);
			this.cardDisplayText.text = this.vendingEntries[num].itemName + "\n" + itemCost.ToString();
		}
	}

	// Token: 0x060035A4 RID: 13732 RVA: 0x001292FF File Offset: 0x001274FF
	private void Update()
	{
		if (!this.currentlyVending)
		{
			this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime);
		}
	}

	// Token: 0x060035A5 RID: 13733 RVA: 0x0012933C File Offset: 0x0012753C
	private bool MoveTransportToSlot(int x, int y, int rows, int cols, float xSpeed, float ySpeed, float dt)
	{
		Vector3 vector = Vector3.Lerp(this.horizontalMin.position, this.horizontalMax.position, (float)x / (float)(rows - 1));
		Vector3 vector2 = Vector3.Lerp(this.verticalMin.position, this.verticalMax.position, (float)y / (float)(cols - 1));
		this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, vector, xSpeed * dt);
		this.verticalTransport.position = Vector3.MoveTowards(this.verticalTransport.position, vector2, ySpeed * dt);
		float sqrMagnitude = (this.horizontalTransport.position - vector).sqrMagnitude;
		float sqrMagnitude2 = (this.verticalTransport.position - vector2).sqrMagnitude;
		return sqrMagnitude > 0.001f || sqrMagnitude2 > 0.001f;
	}

	// Token: 0x060035A6 RID: 13734 RVA: 0x00129416 File Offset: 0x00127616
	private IEnumerator VendingCoroutine()
	{
		this.currentlyVending = true;
		while (this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
		{
			yield return null;
		}
		int entityTypeId = this.vendingEntries[this.vendingIndex].GetEntityTypeId();
		int itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugUnlimitedPurchasing || VRRig.LocalRig.GetComponent<GRPlayer>().ShiftCredits >= itemCost)
		{
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(true);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
			float depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
			while (depositPosSqDist > 0.001f)
			{
				this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, this.depositLocation.position, this.horizontalSpeed * Time.deltaTime);
				depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
				yield return null;
			}
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(false);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
		}
		this.currentlyVending = false;
		yield break;
	}

	// Token: 0x04004639 RID: 17977
	[SerializeField]
	private Transform horizontalTransport;

	// Token: 0x0400463A RID: 17978
	[SerializeField]
	private Transform verticalTransport;

	// Token: 0x0400463B RID: 17979
	[SerializeField]
	private Transform horizontalMin;

	// Token: 0x0400463C RID: 17980
	[SerializeField]
	private Transform horizontalMax;

	// Token: 0x0400463D RID: 17981
	[SerializeField]
	private Transform verticalMin;

	// Token: 0x0400463E RID: 17982
	[SerializeField]
	private Transform verticalMax;

	// Token: 0x0400463F RID: 17983
	[SerializeField]
	private Transform depositLocation;

	// Token: 0x04004640 RID: 17984
	[SerializeField]
	private Transform itemSpawnLocation;

	// Token: 0x04004641 RID: 17985
	[SerializeField]
	private TMP_Text cardDisplayText;

	// Token: 0x04004642 RID: 17986
	[SerializeField]
	private int horizontalSteps = 4;

	// Token: 0x04004643 RID: 17987
	[SerializeField]
	private int verticalSteps = 3;

	// Token: 0x04004644 RID: 17988
	[SerializeField]
	private float horizontalSpeed = 0.25f;

	// Token: 0x04004645 RID: 17989
	[SerializeField]
	private float verticalSpeed = 0.25f;

	// Token: 0x04004646 RID: 17990
	[SerializeField]
	private bool debugUnlimitedPurchasing;

	// Token: 0x04004647 RID: 17991
	[SerializeField]
	private List<GRVendingMachine.VendingEntry> vendingEntries = new List<GRVendingMachine.VendingEntry>();

	// Token: 0x04004648 RID: 17992
	private int hIndex;

	// Token: 0x04004649 RID: 17993
	private int vIndex;

	// Token: 0x0400464A RID: 17994
	private bool currentlyVending;

	// Token: 0x0400464B RID: 17995
	private int vendingIndex;

	// Token: 0x0400464C RID: 17996
	private Coroutine vendingCoroutine;

	// Token: 0x0400464D RID: 17997
	public int VendingMachineId;

	// Token: 0x0400464E RID: 17998
	private GhostReactor reactor;

	// Token: 0x02000828 RID: 2088
	[Serializable]
	public struct VendingEntry
	{
		// Token: 0x060035A8 RID: 13736 RVA: 0x0012945C File Offset: 0x0012765C
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x0400464F RID: 17999
		public Transform transportVisual;

		// Token: 0x04004650 RID: 18000
		public GameEntity entityPrefab;

		// Token: 0x04004651 RID: 18001
		public string itemName;

		// Token: 0x04004652 RID: 18002
		private int entityTypeId;

		// Token: 0x04004653 RID: 18003
		private bool entityTypeIdSet;
	}
}
