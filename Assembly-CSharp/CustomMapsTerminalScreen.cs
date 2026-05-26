using System;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AA5 RID: 2725
public abstract class CustomMapsTerminalScreen : MonoBehaviour
{
	// Token: 0x060045A3 RID: 17827
	public abstract void Initialize();

	// Token: 0x060045A4 RID: 17828 RVA: 0x00178D5C File Offset: 0x00176F5C
	public virtual void Show()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard != null)
			{
				customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
			}
		}
		this.showTime = Time.time;
	}

	// Token: 0x060045A5 RID: 17829 RVA: 0x00178DB0 File Offset: 0x00176FB0
	public virtual void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard != null)
			{
				customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
			}
		}
		this.showTime = 0f;
	}

	// Token: 0x060045A6 RID: 17830 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void PressButton(CustomMapKeyboardBinding pressedButton)
	{
	}

	// Token: 0x04005817 RID: 22551
	public CustomMapsKeyboard terminalKeyboard;

	// Token: 0x04005818 RID: 22552
	[SerializeField]
	protected float activationTime = 0.25f;

	// Token: 0x04005819 RID: 22553
	protected float showTime;
}
