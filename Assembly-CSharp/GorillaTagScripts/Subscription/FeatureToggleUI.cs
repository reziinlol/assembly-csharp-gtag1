using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000F65 RID: 3941
	[RequireComponent(typeof(SITouchscreenButtonContainer))]
	public class FeatureToggleUI : MonoBehaviour
	{
		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x0600622E RID: 25134 RVA: 0x001FAFB7 File Offset: 0x001F91B7
		// (set) Token: 0x0600622F RID: 25135 RVA: 0x001FAFBF File Offset: 0x001F91BF
		public SITouchscreenButtonContainer ButtonContainer { get; private set; }

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06006230 RID: 25136 RVA: 0x001FAFC8 File Offset: 0x001F91C8
		// (set) Token: 0x06006231 RID: 25137 RVA: 0x001FAFD5 File Offset: 0x001F91D5
		public string LabelText
		{
			get
			{
				return this._label.text;
			}
			set
			{
				this._label.text = value;
			}
		}

		// Token: 0x06006232 RID: 25138 RVA: 0x001FAFE3 File Offset: 0x001F91E3
		private void Awake()
		{
			this.ButtonContainer = base.gameObject.GetComponent<SITouchscreenButtonContainer>();
		}

		// Token: 0x06006233 RID: 25139 RVA: 0x001FAFF8 File Offset: 0x001F91F8
		public void AttachToFeature(FeatureTogglesScreen.Feature feature)
		{
			this.ButtonContainer.button.buttonPressed.RemoveAllListeners();
			this.ButtonContainer.button.buttonToggled.RemoveAllListeners();
			this.LabelText = feature.DisplayName;
			bool state2 = SubscriptionManager.GetSubscriptionSettingBool(feature.Value);
			bool flag = SubscriptionManager.IsSubscriptionFeatureAvailable(feature.Value);
			bool flag2 = true;
			if (flag && flag2)
			{
				this.ButtonContainer.button.buttonPressed.AddListener(delegate(SITouchscreenButton.SITouchscreenButtonType type, int data, int nr)
				{
					this.OnPressed(nr, feature);
				});
				this.ButtonContainer.button.buttonToggled.AddListener(delegate(SITouchscreenButton.SITouchscreenButtonType type, int data, int nr, bool state)
				{
					this.OnToggled(nr, feature, state);
				});
				this._unavailable.gameObject.SetActive(false);
			}
			else
			{
				state2 = false;
				this._unavailable.gameObject.SetActive(true);
				if (!flag2)
				{
					this._unavailable.text = "ENABLE PERMISSION IN QUEST SETTINGS";
				}
				else
				{
					this._unavailable.text = "NOT AVAILABLE ON THIS DEVICE";
				}
			}
			this.ButtonContainer.button.SetToggleState(state2, false);
			this.ButtonContainer.UpdateToggleVisual();
		}

		// Token: 0x06006234 RID: 25140 RVA: 0x001FB125 File Offset: 0x001F9325
		private void OnPressed(int actorNr, FeatureTogglesScreen.Feature feature)
		{
			if (Time.time < this._disableUntil)
			{
				return;
			}
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this._disableUntil = Time.time + 0.5f;
			feature.OnPressed.Invoke();
		}

		// Token: 0x06006235 RID: 25141 RVA: 0x001FB164 File Offset: 0x001F9364
		private void OnToggled(int actorNr, FeatureTogglesScreen.Feature feature, bool state)
		{
			if (Time.time < this._disableUntil)
			{
				return;
			}
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this._disableUntil = Time.time + 0.5f;
			feature.OnToggle.Invoke(state);
			this.ButtonContainer.button.SetToggleState(state, false);
			this.ButtonContainer.UpdateToggleVisual();
		}

		// Token: 0x04007102 RID: 28930
		[SerializeField]
		private TextMeshPro _label;

		// Token: 0x04007103 RID: 28931
		[SerializeField]
		private TextMeshPro _unavailable;

		// Token: 0x04007104 RID: 28932
		private const float DEBOUNCE_TIME = 0.5f;

		// Token: 0x04007105 RID: 28933
		private float _disableUntil = float.MinValue;
	}
}
