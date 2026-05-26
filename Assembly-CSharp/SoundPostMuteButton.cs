using System;
using UnityEngine;

// Token: 0x02000A27 RID: 2599
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x0600427D RID: 17021 RVA: 0x0016362C File Offset: 0x0016182C
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.IsDummyButton)
		{
			SynchedMusicController[] array = this.musicControllers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MuteAudio(this);
			}
			return;
		}
		if (this._targetMuteButton != null)
		{
			this._targetMuteButton.ButtonActivation();
		}
	}

	// Token: 0x04005470 RID: 21616
	public SynchedMusicController[] musicControllers;

	// Token: 0x04005471 RID: 21617
	[Tooltip("If true, then this button will passthrough clicks to a connected SoundPostMuteButton.")]
	public bool IsDummyButton;

	// Token: 0x04005472 RID: 21618
	[SerializeField]
	[Tooltip("The targetted SoundPostMuteButton if this is a dummy button.")]
	private SoundPostMuteButton _targetMuteButton;
}
