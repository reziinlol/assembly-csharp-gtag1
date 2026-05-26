using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;

namespace FXP
{
	// Token: 0x02000FF2 RID: 4082
	[Obsolete("CosmeticItemPrefab is deprecated, if we want to use this we need services to re-activate a webservice that was called gt-featureditem-dev.")]
	public class CosmeticItemPrefab : MonoBehaviour
	{
		// Token: 0x0600660C RID: 26124 RVA: 0x0020E383 File Offset: 0x0020C583
		private void Awake()
		{
			this.JonsAwakeCode();
		}

		// Token: 0x0600660D RID: 26125 RVA: 0x0020E38C File Offset: 0x0020C58C
		private void JonsAwakeCode()
		{
			this.lastUpdated = -this.updateClock;
			this.isValid = (this.goPedestal && this.goMannequin && this.goCosmeticItem && this.goCosmeticItemNameplate && this.goClock && this.goPreviewMode && this.goAttractMode && this.goPurchaseMode);
			this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			this.goAttractModeSFX = this.goAttractMode.transform.FindChildRecursive("SFXAttractMode").GetComponent<AudioSource>();
			this.goPurchaseModeSFX = this.goPurchaseMode.transform.FindChildRecursive("SFXPurchaseMode").GetComponent<AudioSource>();
			this.goAttractModeVFX = this.goAttractMode.transform.FindChildRecursive("VFXAttractMode").GetComponent<ParticleSystem>();
			this.goPurchaseModeVFX = this.goPurchaseMode.transform.FindChildRecursive("VFXPurchaseMode").GetComponent<ParticleSystem>();
			this.clockTextMesh = this.goClock.GetComponent<TextMeshPro>();
			this.clockTextMeshIsValid = (this.clockTextMesh != null);
			if (this.clockTextMeshIsValid)
			{
				this.defaultCountdownTextTemplate = this.clockTextMesh.text;
			}
			this.isValid = (this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX);
		}

		// Token: 0x0600660E RID: 26126 RVA: 0x0020E515 File Offset: 0x0020C715
		private void OnDisable()
		{
			if (StoreUpdater.instance != null)
			{
				this.countdownTimerCoRoutine = null;
				this.StopCountdownCoroutine();
				StoreUpdater.instance.PedestalAsleep(this);
			}
		}

		// Token: 0x0600660F RID: 26127 RVA: 0x0020E540 File Offset: 0x0020C740
		private void OnEnable()
		{
			if (this.goPreviewModeSFX == null)
			{
				this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goAttractModeSFX == null)
			{
				this.goAttractModeSFX = this.goAttractMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goPurchaseModeSFX == null)
			{
				this.goPurchaseModeSFX = this.goPurchaseMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			this.isValid = (this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX);
			if (StoreUpdater.instance != null)
			{
				StoreUpdater.instance.PedestalAwakened(this);
			}
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x0020E610 File Offset: 0x0020C810
		public void SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode NewDisplayMode)
		{
			if (!this.isValid)
			{
				return;
			}
			if (NewDisplayMode.Equals(CosmeticItemPrefab.EDisplayMode.NULL))
			{
				return;
			}
			if (NewDisplayMode == this.currentDisplayMode)
			{
				return;
			}
			switch (NewDisplayMode)
			{
			case CosmeticItemPrefab.EDisplayMode.HIDDEN:
			{
				this.goPedestal.SetActive(false);
				this.goMannequin.SetActive(false);
				this.goCosmeticItem.SetActive(false);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				AudioSource audioSource = this.goPreviewModeSFX;
				if (audioSource != null)
				{
					audioSource.GTStop();
				}
				this.goAttractMode.SetActive(false);
				AudioSource audioSource2 = this.goAttractModeSFX;
				if (audioSource2 != null)
				{
					audioSource2.GTStop();
				}
				this.goPurchaseMode.SetActive(false);
				AudioSource audioSource3 = this.goPurchaseModeSFX;
				if (audioSource3 != null)
				{
					audioSource3.GTStop();
				}
				this.StopPreviewTimer();
				this.StopAttractTimer();
				break;
			}
			case CosmeticItemPrefab.EDisplayMode.PREVIEW:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(true);
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goPreviewMode.SetActive(true);
				this.goPreviewModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.ATTRACT:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(true);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goAttractMode.SetActive(true);
				this.goAttractModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartAttractTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.PURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(true);
				this.goPurchaseModeSFX.GTPlay();
				this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = "Purchased!";
				this.StopPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.POSTPURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.StopPreviewTimer();
				break;
			}
			this.currentDisplayMode = NewDisplayMode;
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x0020E95E File Offset: 0x0020CB5E
		private void Update()
		{
			if (Time.time > this.lastUpdated + this.updateClock)
			{
				this.lastUpdated = Time.time;
				this.UpdateClock();
			}
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x0020E988 File Offset: 0x0020CB88
		private void UpdateClock()
		{
			if (this.currentUpdateEvent != null && this.clockTextMeshIsValid && this.clockTextMesh.isActiveAndEnabled)
			{
				TimeSpan ts = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				this.clockTextMesh.text = CountdownText.GetTimeDisplay(ts, this.defaultCountdownTextTemplate);
			}
		}

		// Token: 0x06006613 RID: 26131 RVA: 0x0020E9EC File Offset: 0x0020CBEC
		public void SetDefaultProperties()
		{
			if (!this.isValid)
			{
				return;
			}
			this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.defaultPedestalMesh;
			this.goPedestal.GetComponent<MeshRenderer>().sharedMaterial = this.defaultPedestalMaterial;
			this.goMannequin.GetComponent<MeshFilter>().sharedMesh = this.defaultMannequinMesh;
			this.goMannequin.GetComponent<MeshRenderer>().sharedMaterial = this.defaultMannequinMaterial;
			this.goCosmeticItem.GetComponent<MeshFilter>().sharedMesh = this.defaultCosmeticMesh;
			this.goCosmeticItem.GetComponent<MeshRenderer>().sharedMaterial = this.defaultCosmeticMaterial;
			this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = this.defaultItemText;
			this.goPreviewModeSFX.clip = this.defaultSFXPreviewMode;
			this.goAttractModeSFX.clip = this.defaultSFXAttractMode;
			this.goPurchaseModeSFX.clip = this.defaultSFXPurchaseMode;
		}

		// Token: 0x06006614 RID: 26132 RVA: 0x0020EACF File Offset: 0x0020CCCF
		private void ClearCosmeticMesh()
		{
			Object.Destroy(this.goCosmeticItemGameObject);
		}

		// Token: 0x06006615 RID: 26133 RVA: 0x0020EADC File Offset: 0x0020CCDC
		private void ClearCosmeticAtlas()
		{
			if (this.goCosmeticItemMeshAtlas.IsNotNull())
			{
				Object.Destroy(this.goCosmeticItemMeshAtlas);
			}
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x0020EAF8 File Offset: 0x0020CCF8
		public void SetCosmeticItemFromCosmeticController(CosmeticsController.CosmeticItem item)
		{
			if (!this.isValid)
			{
				return;
			}
			this.ClearCosmeticAtlas();
			this.ClearCosmeticMesh();
			this.oldItemID = this.itemID;
			this.itemID = item.itemName;
			this.itemName = item.displayName;
			if (item.overrideDisplayName != string.Empty)
			{
				this.itemName = item.overrideDisplayName;
			}
			this.HeadModel.SetCosmeticActive(this.itemID, false);
			this.SetCosmeticStand();
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x0020EB74 File Offset: 0x0020CD74
		public void SetCosmeticStand()
		{
			this.cosmeticStand.thisCosmeticName = this.itemID;
			this.cosmeticStand.InitializeCosmetic();
			if (this.oldItemID.Length > 0)
			{
				if (this.oldItemID != this.itemID)
				{
					this.cosmeticStand.isOn = false;
				}
				this.cosmeticStand.UpdateColor();
			}
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x0020EBD8 File Offset: 0x0020CDD8
		public void SetStoreUpdateEvent(StoreUpdateEvent storeUpdateEvent, bool playFX)
		{
			if (!this.isValid || !this.AffectedByStoreUpdateEvents)
			{
				return;
			}
			if (playFX)
			{
				this.goAttractMode.SetActive(true);
				this.goAttractModeVFX.Play();
			}
			this.currentUpdateEvent = storeUpdateEvent;
			this.SetCosmeticItemFromCosmeticController(CosmeticsController.instance.GetItemFromDict(storeUpdateEvent.ItemName));
			if (base.isActiveAndEnabled)
			{
				this.countdownTimerCoRoutine = base.StartCoroutine(this.PlayCountdownTimer());
			}
			this.UpdateClock();
		}

		// Token: 0x06006619 RID: 26137 RVA: 0x0020EC4F File Offset: 0x0020CE4F
		private IEnumerator PlayCountdownTimer()
		{
			yield return new WaitForSeconds(Mathf.Clamp((float)((this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted).TotalSeconds - 10.0), 0f, float.MaxValue));
			this.PlaySFX();
			yield break;
		}

		// Token: 0x0600661A RID: 26138 RVA: 0x0020EC5E File Offset: 0x0020CE5E
		public void StopCountdownCoroutine()
		{
			this.CountdownSFX.GTStop();
			this.goAttractModeVFX.Stop();
			if (this.countdownTimerCoRoutine != null)
			{
				base.StopCoroutine(this.countdownTimerCoRoutine);
				this.countdownTimerCoRoutine = null;
			}
		}

		// Token: 0x0600661B RID: 26139 RVA: 0x0020EC94 File Offset: 0x0020CE94
		private void PlaySFX()
		{
			if (this.currentUpdateEvent != null)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				if (timeSpan.TotalSeconds >= 10.0)
				{
					this.CountdownSFX.time = 0f;
					this.CountdownSFX.GTPlay();
					return;
				}
				this.CountdownSFX.time = 10f - (float)timeSpan.TotalSeconds;
				this.CountdownSFX.GTPlay();
			}
		}

		// Token: 0x0600661C RID: 26140 RVA: 0x0020ED20 File Offset: 0x0020CF20
		public void SetCosmeticItemProperties(string WhichGUID, string Name, List<Transform> SocketsList, int Socket, string PedestalMesh = null, string MannequinMesh = null)
		{
			if (!this.isValid)
			{
				return;
			}
			Guid guid;
			if (!Guid.TryParse(WhichGUID, out guid))
			{
				return;
			}
			this.itemName = Name;
			this.itemSocket = Socket;
			if (this.pedestalMesh != null)
			{
				this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.pedestalMesh;
			}
		}

		// Token: 0x0600661D RID: 26141 RVA: 0x0020ED74 File Offset: 0x0020CF74
		private void StartPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.coroutinePreviewTimer = this.DoPreviewTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInPreviewMode ?? this.defaultHoursInPreviewMode) * 60 * 60)));
			base.StartCoroutine(this.coroutinePreviewTimer);
		}

		// Token: 0x0600661E RID: 26142 RVA: 0x0020EDF3 File Offset: 0x0020CFF3
		private void StopPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.clockTextMesh.text = "Clock";
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x0020EE29 File Offset: 0x0020D029
		private IEnumerator DoPreviewTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.clockTextMesh.text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.ATTRACT);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x06006620 RID: 26144 RVA: 0x0020EE40 File Offset: 0x0020D040
		public void StartAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.coroutineAttractTimer = this.DoAttractTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInAttractMode ?? this.defaultHoursInAttractMode) * 60 * 60)));
			base.StartCoroutine(this.coroutineAttractTimer);
		}

		// Token: 0x06006621 RID: 26145 RVA: 0x0020EEBF File Offset: 0x0020D0BF
		private void StopAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.goClock.GetComponent<TextMesh>().text = "Clock";
		}

		// Token: 0x06006622 RID: 26146 RVA: 0x0020EEFA File Offset: 0x0020D0FA
		private IEnumerator DoAttractTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.goClock.GetComponent<TextMesh>().text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.HIDDEN);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x0400756D RID: 30061
		public string PedestalID = "";

		// Token: 0x0400756E RID: 30062
		public HeadModel HeadModel;

		// Token: 0x0400756F RID: 30063
		public bool AffectedByStoreUpdateEvents = true;

		// Token: 0x04007570 RID: 30064
		[SerializeField]
		private Guid? itemGUID;

		// Token: 0x04007571 RID: 30065
		[SerializeField]
		private string itemName = string.Empty;

		// Token: 0x04007572 RID: 30066
		[SerializeField]
		private List<Transform> sockets = new List<Transform>();

		// Token: 0x04007573 RID: 30067
		[SerializeField]
		private int itemSocket = int.MinValue;

		// Token: 0x04007574 RID: 30068
		[SerializeField]
		private int? hoursInPreviewMode;

		// Token: 0x04007575 RID: 30069
		[SerializeField]
		private int? hoursInAttractMode;

		// Token: 0x04007576 RID: 30070
		[SerializeField]
		private Mesh pedestalMesh;

		// Token: 0x04007577 RID: 30071
		[SerializeField]
		private Mesh mannequinMesh;

		// Token: 0x04007578 RID: 30072
		[SerializeField]
		private Mesh cosmeticMesh;

		// Token: 0x04007579 RID: 30073
		[SerializeField]
		private AudioClip sfxPreviewMode;

		// Token: 0x0400757A RID: 30074
		[SerializeField]
		private AudioClip sfxAttractMode;

		// Token: 0x0400757B RID: 30075
		[SerializeField]
		private AudioClip sfxPurchaseMode;

		// Token: 0x0400757C RID: 30076
		[SerializeField]
		private ParticleSystem vfxPreviewMode;

		// Token: 0x0400757D RID: 30077
		[SerializeField]
		private ParticleSystem vfxAttractMode;

		// Token: 0x0400757E RID: 30078
		[SerializeField]
		private ParticleSystem vfxPurchaseMode;

		// Token: 0x0400757F RID: 30079
		[SerializeField]
		private GameObject goPedestal;

		// Token: 0x04007580 RID: 30080
		[SerializeField]
		private GameObject goMannequin;

		// Token: 0x04007581 RID: 30081
		[SerializeField]
		private GameObject goCosmeticItem;

		// Token: 0x04007582 RID: 30082
		[SerializeField]
		private GameObject goCosmeticItemGameObject;

		// Token: 0x04007583 RID: 30083
		[SerializeField]
		private GameObject goCosmeticItemNameplate;

		// Token: 0x04007584 RID: 30084
		[SerializeField]
		private GameObject goClock;

		// Token: 0x04007585 RID: 30085
		[SerializeField]
		private GameObject goPreviewMode;

		// Token: 0x04007586 RID: 30086
		[SerializeField]
		private GameObject goAttractMode;

		// Token: 0x04007587 RID: 30087
		[SerializeField]
		private GameObject goPurchaseMode;

		// Token: 0x04007588 RID: 30088
		[SerializeField]
		private Mesh defaultPedestalMesh;

		// Token: 0x04007589 RID: 30089
		[SerializeField]
		private Material defaultPedestalMaterial;

		// Token: 0x0400758A RID: 30090
		[SerializeField]
		private Mesh defaultMannequinMesh;

		// Token: 0x0400758B RID: 30091
		[SerializeField]
		private Material defaultMannequinMaterial;

		// Token: 0x0400758C RID: 30092
		[SerializeField]
		private Mesh defaultCosmeticMesh;

		// Token: 0x0400758D RID: 30093
		[SerializeField]
		private Material defaultCosmeticMaterial;

		// Token: 0x0400758E RID: 30094
		[SerializeField]
		private string defaultItemText;

		// Token: 0x0400758F RID: 30095
		[SerializeField]
		private int defaultHoursInPreviewMode;

		// Token: 0x04007590 RID: 30096
		[SerializeField]
		private int defaultHoursInAttractMode;

		// Token: 0x04007591 RID: 30097
		[SerializeField]
		private AudioClip defaultSFXPreviewMode;

		// Token: 0x04007592 RID: 30098
		[SerializeField]
		private AudioClip defaultSFXAttractMode;

		// Token: 0x04007593 RID: 30099
		[SerializeField]
		private AudioClip defaultSFXPurchaseMode;

		// Token: 0x04007594 RID: 30100
		private GameObject goCosmeticItemMeshAtlas;

		// Token: 0x04007595 RID: 30101
		public AudioSource CountdownSFX;

		// Token: 0x04007596 RID: 30102
		private CosmeticItemPrefab.EDisplayMode currentDisplayMode;

		// Token: 0x04007597 RID: 30103
		private bool isValid;

		// Token: 0x04007598 RID: 30104
		[Nullable(2)]
		private AudioSource goPreviewModeSFX;

		// Token: 0x04007599 RID: 30105
		[Nullable(2)]
		private AudioSource goAttractModeSFX;

		// Token: 0x0400759A RID: 30106
		[Nullable(2)]
		private AudioSource goPurchaseModeSFX;

		// Token: 0x0400759B RID: 30107
		[Nullable(2)]
		private ParticleSystem goAttractModeVFX;

		// Token: 0x0400759C RID: 30108
		[Nullable(2)]
		private ParticleSystem goPurchaseModeVFX;

		// Token: 0x0400759D RID: 30109
		private IEnumerator coroutinePreviewTimer;

		// Token: 0x0400759E RID: 30110
		private IEnumerator coroutineAttractTimer;

		// Token: 0x0400759F RID: 30111
		private DateTime startTime;

		// Token: 0x040075A0 RID: 30112
		private TextMeshPro clockTextMesh;

		// Token: 0x040075A1 RID: 30113
		private bool clockTextMeshIsValid;

		// Token: 0x040075A2 RID: 30114
		private StoreUpdateEvent currentUpdateEvent;

		// Token: 0x040075A3 RID: 30115
		private string defaultCountdownTextTemplate = "";

		// Token: 0x040075A4 RID: 30116
		public CosmeticStand cosmeticStand;

		// Token: 0x040075A5 RID: 30117
		public string itemID = "";

		// Token: 0x040075A6 RID: 30118
		public string oldItemID = "";

		// Token: 0x040075A7 RID: 30119
		private Coroutine countdownTimerCoRoutine;

		// Token: 0x040075A8 RID: 30120
		private float updateClock = 60f;

		// Token: 0x040075A9 RID: 30121
		private float lastUpdated;

		// Token: 0x02000FF3 RID: 4083
		[SerializeField]
		public enum EDisplayMode
		{
			// Token: 0x040075AB RID: 30123
			NULL,
			// Token: 0x040075AC RID: 30124
			HIDDEN,
			// Token: 0x040075AD RID: 30125
			PREVIEW,
			// Token: 0x040075AE RID: 30126
			ATTRACT,
			// Token: 0x040075AF RID: 30127
			PURCHASE,
			// Token: 0x040075B0 RID: 30128
			POSTPURCHASE
		}
	}
}
