using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000173 RID: 371
public class SITouchscreenButton : MonoBehaviour, IClickable
{
	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00034C34 File Offset: 0x00032E34
	private bool IsReady
	{
		get
		{
			bool flag = Time.time - this._enableTime >= 0.2f;
			if (this._screenRegion)
			{
				flag = (flag && !this._screenRegion.HasPressedButton);
			}
			return flag;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060009C9 RID: 2505 RVA: 0x00034C7B File Offset: 0x00032E7B
	public bool IsToggledOn
	{
		get
		{
			return this._isToggledOn;
		}
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00034C84 File Offset: 0x00032E84
	private void Awake()
	{
		ITouchScreenStation componentInParent = base.GetComponentInParent<ITouchScreenStation>();
		if (componentInParent != null)
		{
			this._screenRegion = componentInParent.ScreenRegion;
		}
		if (this.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			this._isToggledOn = this._startToggledOn;
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00034CBC File Offset: 0x00032EBC
	private void OnEnable()
	{
		this._enableTime = Time.time;
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00034CCC File Offset: 0x00032ECC
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton();
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00034D18 File Offset: 0x00032F18
	public void PressButton()
	{
		if (!this.IsReady || !this.isUsable)
		{
			return;
		}
		if (this._screenRegion)
		{
			this._screenRegion.RegisterButtonPress();
		}
		if (this.buttonMode == SITouchscreenButton.ButtonMode.Normal)
		{
			this.buttonPressed.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}
		else if (this.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			bool arg = !this._isToggledOn;
			this.buttonToggled.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber, arg);
		}
		if (this._pressSound != null)
		{
			GTAudioOneShot.Play(this._pressSound, base.transform.position, this._pressSoundVolume, 1f);
		}
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00034DE8 File Offset: 0x00032FE8
	public void SetToggleState(bool state, bool invokeEvent = false)
	{
		if (this.buttonMode != SITouchscreenButton.ButtonMode.Toggle)
		{
			return;
		}
		bool flag = this._isToggledOn != state;
		this._isToggledOn = state;
		if (invokeEvent && flag)
		{
			this.buttonToggled.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber, this._isToggledOn);
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00034E44 File Offset: 0x00033044
	public void Click(bool leftHand = false)
	{
		this.PressButton();
	}

	// Token: 0x04000BEB RID: 3051
	public SITouchscreenButton.ButtonMode buttonMode;

	// Token: 0x04000BEC RID: 3052
	public SITouchscreenButton.SITouchscreenButtonType buttonType;

	// Token: 0x04000BED RID: 3053
	public int data;

	// Token: 0x04000BEE RID: 3054
	[SerializeField]
	private AudioClip _pressSound;

	// Token: 0x04000BEF RID: 3055
	[SerializeField]
	private float _pressSoundVolume = 0.1f;

	// Token: 0x04000BF0 RID: 3056
	[SerializeField]
	private bool _isToggledOn;

	// Token: 0x04000BF1 RID: 3057
	[SerializeField]
	private bool _startToggledOn;

	// Token: 0x04000BF2 RID: 3058
	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int> buttonPressed;

	// Token: 0x04000BF3 RID: 3059
	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int, bool> buttonToggled;

	// Token: 0x04000BF4 RID: 3060
	private SIScreenRegion _screenRegion;

	// Token: 0x04000BF5 RID: 3061
	private const float DEBOUNCE_TIME = 0.2f;

	// Token: 0x04000BF6 RID: 3062
	private float _enableTime;

	// Token: 0x04000BF7 RID: 3063
	[NonSerialized]
	public bool isUsable = true;

	// Token: 0x02000174 RID: 372
	public enum ButtonMode
	{
		// Token: 0x04000BF9 RID: 3065
		Normal,
		// Token: 0x04000BFA RID: 3066
		Toggle
	}

	// Token: 0x02000175 RID: 373
	public enum SITouchscreenButtonType
	{
		// Token: 0x04000BFC RID: 3068
		Back,
		// Token: 0x04000BFD RID: 3069
		Next,
		// Token: 0x04000BFE RID: 3070
		Exit,
		// Token: 0x04000BFF RID: 3071
		Help,
		// Token: 0x04000C00 RID: 3072
		Select,
		// Token: 0x04000C01 RID: 3073
		Dispense,
		// Token: 0x04000C02 RID: 3074
		Research,
		// Token: 0x04000C03 RID: 3075
		Collect,
		// Token: 0x04000C04 RID: 3076
		Debug,
		// Token: 0x04000C05 RID: 3077
		PageSelect,
		// Token: 0x04000C06 RID: 3078
		Purchase,
		// Token: 0x04000C07 RID: 3079
		Confirm,
		// Token: 0x04000C08 RID: 3080
		Cancel,
		// Token: 0x04000C09 RID: 3081
		OverrideFailure,
		// Token: 0x04000C0A RID: 3082
		None,
		// Token: 0x04000C0B RID: 3083
		Subscribe
	}
}
