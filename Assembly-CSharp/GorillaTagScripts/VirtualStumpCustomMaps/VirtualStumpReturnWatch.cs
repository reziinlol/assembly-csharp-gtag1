using System;
using System.Collections;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F51 RID: 3921
	public class VirtualStumpReturnWatch : MonoBehaviour
	{
		// Token: 0x060061D6 RID: 25046 RVA: 0x001F9034 File Offset: 0x001F7234
		private void Start()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.AddListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.AddListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x060061D7 RID: 25047 RVA: 0x001F90A4 File Offset: 0x001F72A4
		private void OnDestroy()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.RemoveListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.RemoveListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x060061D8 RID: 25048 RVA: 0x001F9114 File Offset: 0x001F7314
		public static void SetWatchProperties(VirtualStumpReturnWatchProps props)
		{
			VirtualStumpReturnWatch.currentCustomMapProps = props;
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom, 0.5f, 5f);
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x001F9190 File Offset: 0x001F7390
		private float GetCurrentHoldDuration()
		{
			if (GorillaGameManager.instance.IsNull())
			{
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			switch (GorillaGameManager.instance.GameType())
			{
			case GameModeType.Infection:
				if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			case GameModeType.Custom:
				if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x001F9239 File Offset: 0x001F7439
		private void OnStartedPressingButton()
		{
			this.startPressingButtonTime = Time.time;
			this.currentlyBeingPressed = true;
			this.returnButton.pressDuration = this.GetCurrentHoldDuration();
			this.ShowCountdownText();
			this.updateCountdownCoroutine = base.StartCoroutine(this.UpdateCountdownText());
		}

		// Token: 0x060061DB RID: 25051 RVA: 0x001F9276 File Offset: 0x001F7476
		private void OnStoppedPressingButton()
		{
			this.currentlyBeingPressed = false;
			this.HideCountdownText();
			if (this.updateCountdownCoroutine != null)
			{
				base.StopCoroutine(this.updateCountdownCoroutine);
				this.updateCountdownCoroutine = null;
			}
		}

		// Token: 0x060061DC RID: 25052 RVA: 0x001F92A0 File Offset: 0x001F74A0
		private void OnButtonPressed()
		{
			this.currentlyBeingPressed = false;
			if (ZoneManagement.IsInZone(GTZone.customMaps) && !CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				bool flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer;
				bool flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer;
				if (GorillaGameManager.instance.IsNotNull())
				{
					switch (GorillaGameManager.instance.GameType())
					{
					case GameModeType.Infection:
						if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
						{
							flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_Infection;
							flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_Infection;
						}
						break;
					case GameModeType.Custom:
						if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
						{
							flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_CustomMode;
							flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_CustomMode;
						}
						break;
					}
				}
				if (flag2 && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else if (flag)
				{
					GameMode.ReportHit();
				}
				CustomMapManager.ReturnToVirtualStump();
			}
		}

		// Token: 0x060061DD RID: 25053 RVA: 0x001F93A0 File Offset: 0x001F75A0
		private void ShowCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			int num = 1 + Mathf.FloorToInt(this.GetCurrentHoldDuration());
			this.countdownText.text = num.ToString();
			this.countdownText.gameObject.SetActive(true);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(false);
			}
		}

		// Token: 0x060061DE RID: 25054 RVA: 0x001F940C File Offset: 0x001F760C
		private void HideCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			this.countdownText.text = "";
			this.countdownText.gameObject.SetActive(false);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(true);
			}
		}

		// Token: 0x060061DF RID: 25055 RVA: 0x001F9466 File Offset: 0x001F7666
		private IEnumerator UpdateCountdownText()
		{
			while (this.currentlyBeingPressed)
			{
				if (this.countdownText.IsNull())
				{
					yield break;
				}
				float f = this.GetCurrentHoldDuration() - (Time.time - this.startPressingButtonTime);
				int num = 1 + Mathf.FloorToInt(f);
				this.countdownText.text = num.ToString();
				yield return null;
			}
			yield break;
		}

		// Token: 0x04007068 RID: 28776
		[SerializeField]
		private HeldButton returnButton;

		// Token: 0x04007069 RID: 28777
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x0400706A RID: 28778
		[SerializeField]
		private TMP_Text countdownText;

		// Token: 0x0400706B RID: 28779
		private static VirtualStumpReturnWatchProps currentCustomMapProps;

		// Token: 0x0400706C RID: 28780
		private float startPressingButtonTime = -1f;

		// Token: 0x0400706D RID: 28781
		private bool currentlyBeingPressed;

		// Token: 0x0400706E RID: 28782
		private Coroutine updateCountdownCoroutine;
	}
}
