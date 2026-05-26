using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000F56 RID: 3926
	public class CustomMapsKeyButton : GorillaKeyButton<CustomMapKeyboardBinding>
	{
		// Token: 0x060061EC RID: 25068 RVA: 0x001F9587 File Offset: 0x001F7787
		protected override void OnEnableEvents()
		{
			base.OnEnableEvents();
			if (!this._isLocalized)
			{
				return;
			}
			this.OnLanguageChanged();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x060061ED RID: 25069 RVA: 0x001F95AF File Offset: 0x001F77AF
		protected override void OnDisableEvents()
		{
			base.OnDisableEvents();
			if (!this._isLocalized)
			{
				return;
			}
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x060061EE RID: 25070 RVA: 0x001F95D4 File Offset: 0x001F77D4
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			if (binding < CustomMapKeyboardBinding.up || (binding > CustomMapKeyboardBinding.option3 && binding < CustomMapKeyboardBinding.at))
			{
				if (binding >= CustomMapKeyboardBinding.up)
				{
					return binding.ToString();
				}
				int num = (int)binding;
				return num.ToString();
			}
			else
			{
				switch (binding)
				{
				case CustomMapKeyboardBinding.at:
					return "@";
				case CustomMapKeyboardBinding.dash:
					return "-";
				case CustomMapKeyboardBinding.period:
					return ".";
				case CustomMapKeyboardBinding.underscore:
					return "_";
				case CustomMapKeyboardBinding.plus:
					return "+";
				case CustomMapKeyboardBinding.space:
					return " ";
				default:
					return "";
				}
			}
		}

		// Token: 0x060061EF RID: 25071 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected override void OnButtonPressedEvent()
		{
		}

		// Token: 0x060061F0 RID: 25072 RVA: 0x001F965C File Offset: 0x001F785C
		private void OnLanguageChanged()
		{
			if (!this._isLocalized)
			{
				return;
			}
			if (this._buttonDisplayNameTxt == null)
			{
				Debug.LogError("[LOCALIZATION::CUSTOM_MAPS_KEY_BUTTON] [_buttonDisplayNameTxt] has not been assigned and is NULL", this);
				return;
			}
			if (this._localizedName == null || this._localizedName.IsEmpty)
			{
				Debug.LogError("[LOCALIZATION::CUSTOM_MAPS_KEY_BUTTON] [_localizedName] has not been assigned", this);
				return;
			}
			this._buttonDisplayNameTxt.text = this._localizedName.GetLocalizedString();
		}

		// Token: 0x040070BA RID: 28858
		[SerializeField]
		private bool _isLocalized;

		// Token: 0x040070BB RID: 28859
		[SerializeField]
		private LocalizedString _localizedName;

		// Token: 0x040070BC RID: 28860
		[SerializeField]
		private TMP_Text _buttonDisplayNameTxt;
	}
}
