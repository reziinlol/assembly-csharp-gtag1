using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000177 RID: 375
public class SITouchscreenButtonContainer : MonoBehaviour
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060009D6 RID: 2518 RVA: 0x00034E66 File Offset: 0x00033066
	// (set) Token: 0x060009D7 RID: 2519 RVA: 0x00034E6E File Offset: 0x0003306E
	public bool isUsable { get; private set; }

	// Token: 0x060009D8 RID: 2520 RVA: 0x00034E78 File Offset: 0x00033078
	private void Start()
	{
		if (Application.isPlaying && this.button != null && this.button.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			this.button.buttonToggled.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int, bool>(this.OnToggleStateChanged));
			this.UpdateToggleVisual(this.button.IsToggledOn);
		}
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00034ED5 File Offset: 0x000330D5
	private void OnToggleStateChanged(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr, bool isToggledOn)
	{
		this.UpdateToggleVisual(isToggledOn);
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00034EDF File Offset: 0x000330DF
	public void UpdateToggleVisual()
	{
		this.UpdateToggleVisual(this.button.IsToggledOn);
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00034EF4 File Offset: 0x000330F4
	private void UpdateToggleVisual(bool isToggledOn)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.foreGround.color = (isToggledOn ? this.toggleOnColor : this.toggleOffColor);
		this.buttonText.text = (isToggledOn ? this.toggleOnText : this.toggleOffText);
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00034F5C File Offset: 0x0003315C
	public void SetUsable(bool newIsUsable)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.isUsable = newIsUsable;
		if (this.button.buttonMode == SITouchscreenButton.ButtonMode.Normal)
		{
			this.foreGround.color = (newIsUsable ? this._cachedForegroundColor : Color.gray);
		}
		this.button.isUsable = newIsUsable;
	}

	// Token: 0x04000C0C RID: 3084
	public SITouchscreenButton.SITouchscreenButtonType type;

	// Token: 0x04000C0D RID: 3085
	public string buttonTextString;

	// Token: 0x04000C0E RID: 3086
	public int data;

	// Token: 0x04000C0F RID: 3087
	public RectTransform backGround;

	// Token: 0x04000C10 RID: 3088
	public RectTransform backgroundShadow;

	// Token: 0x04000C11 RID: 3089
	public Image foreGround;

	// Token: 0x04000C12 RID: 3090
	public TextMeshProUGUI buttonText;

	// Token: 0x04000C13 RID: 3091
	public ITouchScreenStation station;

	// Token: 0x04000C14 RID: 3092
	[Header("Toggle Visual Settings")]
	public Color toggleOnColor = new Color(0f, 1f, 0.345098f);

	// Token: 0x04000C15 RID: 3093
	public Color toggleOffColor = new Color(0.5f, 0.5f, 0.5f);

	// Token: 0x04000C16 RID: 3094
	[Header("Toggle Text Settings")]
	[Tooltip("Text to display when toggle is ON")]
	public string toggleOnText = "ON";

	// Token: 0x04000C17 RID: 3095
	[Tooltip("Text to display when toggle is OFF")]
	public string toggleOffText = "OFF";

	// Token: 0x04000C18 RID: 3096
	public SITouchscreenButton button;

	// Token: 0x04000C19 RID: 3097
	[SerializeField]
	private bool autoConfigure = true;

	// Token: 0x04000C1B RID: 3099
	[NonSerialized]
	private Color _cachedForegroundColor = new Color(-1f, -1f, -1f);
}
