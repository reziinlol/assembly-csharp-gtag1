using System;
using UnityEngine;

// Token: 0x02000176 RID: 374
public interface ITouchScreenStation
{
	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060009D1 RID: 2513
	GameObject gameObject { get; }

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060009D2 RID: 2514
	SIScreenRegion ScreenRegion { get; }

	// Token: 0x060009D3 RID: 2515
	void AddButton(SITouchscreenButton button, bool isPopupButton = false);

	// Token: 0x060009D4 RID: 2516
	void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr);

	// Token: 0x060009D5 RID: 2517
	void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn);
}
