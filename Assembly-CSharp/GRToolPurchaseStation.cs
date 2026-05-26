using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000806 RID: 2054
public class GRToolPurchaseStation : MonoBehaviour
{
	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x060034A7 RID: 13479 RVA: 0x00122029 File Offset: 0x00120229
	public int ActiveEntryIndex
	{
		get
		{
			return this.activeEntryIndex;
		}
	}

	// Token: 0x060034A8 RID: 13480 RVA: 0x00122031 File Offset: 0x00120231
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x00122041 File Offset: 0x00120241
	public void RequestPurchaseButton(int actorNumber)
	{
		if (actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
		}
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x00122067 File Offset: 0x00120267
	public void ShiftRightButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftRight);
	}

	// Token: 0x060034AB RID: 13483 RVA: 0x0012207B File Offset: 0x0012027B
	public void ShiftLeftButton()
	{
		this.grManager.ToolPurchaseStationRequest(this.PurchaseStationId, GhostReactorManager.ToolPurchaseStationAction.ShiftLeft);
	}

	// Token: 0x060034AC RID: 13484 RVA: 0x0012208F File Offset: 0x0012028F
	public void ShiftRightAuthority()
	{
		this.activeEntryIndex = (this.activeEntryIndex + 1) % this.toolEntries.Count;
	}

	// Token: 0x060034AD RID: 13485 RVA: 0x001220AB File Offset: 0x001202AB
	public void ShiftLeftAuthority()
	{
		this.activeEntryIndex = ((this.activeEntryIndex > 0) ? (this.activeEntryIndex - 1) : (this.toolEntries.Count - 1));
	}

	// Token: 0x060034AE RID: 13486 RVA: 0x001220D4 File Offset: 0x001202D4
	public void DebugPurchase()
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
		Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
		Quaternion rotation = this.depositTransform.rotation * localRotation;
		Vector3 position = this.depositTransform.position + this.depositTransform.rotation * localPosition;
		this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, position, rotation, 0L);
		this.OnPurchaseSucceeded();
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x00122194 File Offset: 0x00120394
	public bool TryPurchaseAuthority(GRPlayer player, out int itemCost)
	{
		int entityTypeId = this.toolEntries[this.activeEntryIndex].GetEntityTypeId();
		itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugIgnoreToolCost || player.ShiftCredits >= itemCost)
		{
			Vector3 localPosition = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localPosition;
			Quaternion localRotation = this.toolEntries[this.activeEntryIndex].displayToolParent.GetChild(0).localRotation;
			Quaternion rotation = this.depositTransform.rotation * localRotation;
			Vector3 position = this.depositTransform.position + this.depositTransform.rotation * localPosition;
			this.grManager.gameEntityManager.RequestCreateItem(entityTypeId, position, rotation, 0L);
			return true;
		}
		return false;
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x00122274 File Offset: 0x00120474
	public void OnSelectionUpdate(int newSelectedIndex)
	{
		this.activeEntryIndex = Mathf.Clamp(newSelectedIndex % this.toolEntries.Count, 0, this.toolEntries.Count - 1);
		this.audioSource.PlayOneShot(this.nextItemAudio, this.nextItemVolume);
		this.displayItemNameText.text = this.toolEntries[this.activeEntryIndex].toolName;
		this.displayItemCostText.text = this.toolEntries[this.activeEntryIndex].toolCost.ToString();
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x00122308 File Offset: 0x00120508
	public void OnPurchaseSucceeded()
	{
		this.animatingDeposit = true;
		this.animationStartTime = Time.time;
		this.audioSource.PlayOneShot(this.purchaseAudio, this.purchaseVolume);
		UnityEvent onSucceeded = this.idCardScanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		if (this.displayedEntryIndex < 0 || this.displayedEntryIndex >= this.toolEntries.Count)
		{
			this.displayedEntryIndex = this.activeEntryIndex;
		}
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x0012237C File Offset: 0x0012057C
	public void OnPurchaseFailed()
	{
		this.audioSource.PlayOneShot(this.purchaseFailedAudio, this.purchaseFailedVolume);
		UnityEvent onFailed = this.idCardScanner.onFailed;
		if (onFailed == null)
		{
			return;
		}
		onFailed.Invoke();
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x001223AA File Offset: 0x001205AA
	public Transform GetSpawnMarker()
	{
		return this.toolSpawnLocation;
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x001223B2 File Offset: 0x001205B2
	public string GetCurrentToolName()
	{
		return this.toolEntries[this.activeEntryIndex].toolName;
	}

	// Token: 0x060034B5 RID: 13493 RVA: 0x001223CA File Offset: 0x001205CA
	private void Awake()
	{
		this.depositLidOpenRot = Quaternion.Euler(this.depositLidOpenEuler);
		this.toolEntryRot = Quaternion.Euler(this.toolEntryRotEuler);
		this.toolExitRot = Quaternion.Euler(this.toolExitRotEuler);
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x00122400 File Offset: 0x00120600
	private void Update()
	{
		if (!this.animatingSwap && !this.animatingDeposit && this.activeEntryIndex != this.displayedEntryIndex)
		{
			this.animatingSwap = true;
			this.animationStartTime = Time.time;
			this.animPrevToolIndex = this.displayedEntryIndex;
			this.animNextToolIndex = this.activeEntryIndex;
			this.toolEntryRot = Quaternion.AngleAxis(this.toolEntryRotDegrees, Random.onUnitSphere);
		}
		if (this.animatingSwap)
		{
			float num = (Time.time - this.animationStartTime) / this.nextToolAnimationTime;
			Transform transform = null;
			if (this.animPrevToolIndex >= 0 && this.animPrevToolIndex < this.toolEntries.Count)
			{
				transform = this.toolEntries[this.animPrevToolIndex].displayToolParent;
				transform.localRotation = Quaternion.Slerp(Quaternion.identity, this.toolExitRot, this.toolExitRotTimingCurve.Evaluate(num));
				transform.localPosition = Vector3.Lerp(Vector3.zero, this.toolExitPosOffset, this.toolExitPosTimingCurve.Evaluate(num));
			}
			Transform displayToolParent = this.toolEntries[this.animNextToolIndex].displayToolParent;
			displayToolParent.localRotation = Quaternion.Slerp(this.toolEntryRot, Quaternion.identity, this.toolEntryRotTimingCurve.Evaluate(num));
			displayToolParent.localPosition = Vector3.Lerp(this.toolEntryPosOffset, Vector3.zero, this.toolEntryPosTimingCurve.Evaluate(num));
			displayToolParent.gameObject.SetActive(true);
			if (num >= 1f)
			{
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
				this.displayedEntryIndex = this.animNextToolIndex;
				this.animatingSwap = false;
				return;
			}
		}
		else if (this.animatingDeposit)
		{
			float num2 = (Time.time - this.animationStartTime) / this.toolDepositAnimationTime;
			Transform displayToolParent2 = this.toolEntries[this.displayedEntryIndex].displayToolParent;
			Vector3 localPosition = displayToolParent2.localPosition;
			localPosition.y = Mathf.Lerp(0f, this.depositTransform.localPosition.y, this.toolDepositMotionCurveY.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			localPosition.z = Mathf.Lerp(0f, this.depositTransform.localPosition.z, this.toolDepositMotionCurveZ.Evaluate(this.toolDepositTimingCurve.Evaluate(num2)));
			displayToolParent2.localPosition = localPosition;
			this.depositLidTransform.localRotation = Quaternion.Slerp(Quaternion.identity, this.depositLidOpenRot, this.depositLidTimingCurve.Evaluate(num2));
			if (num2 >= 1f)
			{
				this.depositLidTransform.localRotation = Quaternion.identity;
				displayToolParent2.gameObject.SetActive(false);
				this.displayedEntryIndex = -1;
				this.animatingDeposit = false;
			}
		}
	}

	// Token: 0x040044AC RID: 17580
	[SerializeField]
	private List<GRToolPurchaseStation.ToolEntry> toolEntries = new List<GRToolPurchaseStation.ToolEntry>();

	// Token: 0x040044AD RID: 17581
	[SerializeField]
	private Transform displayTransform;

	// Token: 0x040044AE RID: 17582
	[SerializeField]
	private Transform depositTransform;

	// Token: 0x040044AF RID: 17583
	[SerializeField]
	private Transform toolSpawnLocation;

	// Token: 0x040044B0 RID: 17584
	[SerializeField]
	private TMP_Text displayItemNameText;

	// Token: 0x040044B1 RID: 17585
	[SerializeField]
	private TMP_Text displayItemCostText;

	// Token: 0x040044B2 RID: 17586
	[SerializeField]
	private float nextToolAnimationTime = 0.5f;

	// Token: 0x040044B3 RID: 17587
	[SerializeField]
	private float toolDepositAnimationTime = 1f;

	// Token: 0x040044B4 RID: 17588
	[SerializeField]
	private Vector3 toolEntryPosOffset = new Vector3(0f, 0.25f, 0f);

	// Token: 0x040044B5 RID: 17589
	[SerializeField]
	private Vector3 toolEntryRotEuler = new Vector3(0f, 0f, 15f);

	// Token: 0x040044B6 RID: 17590
	[SerializeField]
	private float toolEntryRotDegrees = 15f;

	// Token: 0x040044B7 RID: 17591
	[SerializeField]
	private Vector3 toolExitPosOffset = new Vector3(0f, 0f, -0.25f);

	// Token: 0x040044B8 RID: 17592
	[SerializeField]
	private Vector3 toolExitRotEuler = new Vector3(180f, 0f, 0f);

	// Token: 0x040044B9 RID: 17593
	[SerializeField]
	private AnimationCurve toolEntryPosTimingCurve;

	// Token: 0x040044BA RID: 17594
	[SerializeField]
	private AnimationCurve toolEntryRotTimingCurve;

	// Token: 0x040044BB RID: 17595
	[SerializeField]
	private AnimationCurve toolExitPosTimingCurve;

	// Token: 0x040044BC RID: 17596
	[SerializeField]
	private AnimationCurve toolExitRotTimingCurve;

	// Token: 0x040044BD RID: 17597
	[SerializeField]
	private AnimationCurve toolDepositTimingCurve;

	// Token: 0x040044BE RID: 17598
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveY;

	// Token: 0x040044BF RID: 17599
	[SerializeField]
	private AnimationCurve toolDepositMotionCurveZ;

	// Token: 0x040044C0 RID: 17600
	[SerializeField]
	private Transform depositLidTransform;

	// Token: 0x040044C1 RID: 17601
	[SerializeField]
	private Vector3 depositLidOpenEuler = new Vector3(65f, 0f, 0f);

	// Token: 0x040044C2 RID: 17602
	[SerializeField]
	private AnimationCurve depositLidTimingCurve;

	// Token: 0x040044C3 RID: 17603
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040044C4 RID: 17604
	[SerializeField]
	private AudioClip nextItemAudio;

	// Token: 0x040044C5 RID: 17605
	[SerializeField]
	private float nextItemVolume = 0.5f;

	// Token: 0x040044C6 RID: 17606
	[SerializeField]
	private AudioClip purchaseAudio;

	// Token: 0x040044C7 RID: 17607
	[SerializeField]
	private float purchaseVolume = 0.5f;

	// Token: 0x040044C8 RID: 17608
	[SerializeField]
	private AudioClip purchaseFailedAudio;

	// Token: 0x040044C9 RID: 17609
	[SerializeField]
	private float purchaseFailedVolume = 0.5f;

	// Token: 0x040044CA RID: 17610
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x040044CB RID: 17611
	private int activeEntryIndex = 1;

	// Token: 0x040044CC RID: 17612
	private int displayedEntryIndex = -1;

	// Token: 0x040044CD RID: 17613
	private float animationStartTime;

	// Token: 0x040044CE RID: 17614
	private bool animatingDeposit;

	// Token: 0x040044CF RID: 17615
	private bool animatingSwap;

	// Token: 0x040044D0 RID: 17616
	private int animPrevToolIndex;

	// Token: 0x040044D1 RID: 17617
	private int animNextToolIndex;

	// Token: 0x040044D2 RID: 17618
	private Quaternion depositLidOpenRot = Quaternion.identity;

	// Token: 0x040044D3 RID: 17619
	private Quaternion toolEntryRot = Quaternion.identity;

	// Token: 0x040044D4 RID: 17620
	private Quaternion toolExitRot = Quaternion.identity;

	// Token: 0x040044D5 RID: 17621
	private Coroutine vendingCoroutine;

	// Token: 0x040044D6 RID: 17622
	private bool debugIgnoreToolCost;

	// Token: 0x040044D7 RID: 17623
	[HideInInspector]
	public int PurchaseStationId;

	// Token: 0x040044D8 RID: 17624
	private GhostReactorManager grManager;

	// Token: 0x040044D9 RID: 17625
	private GhostReactor reactor;

	// Token: 0x02000807 RID: 2055
	[Serializable]
	public struct ToolEntry
	{
		// Token: 0x060034B8 RID: 13496 RVA: 0x001227BD File Offset: 0x001209BD
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x040044DA RID: 17626
		public Transform displayToolParent;

		// Token: 0x040044DB RID: 17627
		public GameEntity entityPrefab;

		// Token: 0x040044DC RID: 17628
		public string toolName;

		// Token: 0x040044DD RID: 17629
		public int toolCost;

		// Token: 0x040044DE RID: 17630
		private int entityTypeId;

		// Token: 0x040044DF RID: 17631
		private bool entityTypeIdSet;
	}
}
