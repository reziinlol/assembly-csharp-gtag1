using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000AFB RID: 2811
public class AgeSliderWithProgressBar : MonoBehaviourTick
{
	// Token: 0x170006B3 RID: 1715
	// (get) Token: 0x060047F7 RID: 18423 RVA: 0x00181FFC File Offset: 0x001801FC
	// (set) Token: 0x060047F8 RID: 18424 RVA: 0x00182004 File Offset: 0x00180204
	public AgeSliderWithProgressBar.SliderHeldEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x170006B4 RID: 1716
	// (get) Token: 0x060047F9 RID: 18425 RVA: 0x0018200D File Offset: 0x0018020D
	public bool AdjustAge
	{
		get
		{
			return this._adjustAge;
		}
	}

	// Token: 0x170006B5 RID: 1717
	// (get) Token: 0x060047FA RID: 18426 RVA: 0x00182015 File Offset: 0x00180215
	// (set) Token: 0x060047FB RID: 18427 RVA: 0x0018201D File Offset: 0x0018021D
	public bool ControllerActive
	{
		get
		{
			return this.controllerActive;
		}
		set
		{
			if (value)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			else
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.controllerActive = value;
		}
	}

	// Token: 0x170006B6 RID: 1718
	// (get) Token: 0x060047FC RID: 18428 RVA: 0x00182057 File Offset: 0x00180257
	// (set) Token: 0x060047FD RID: 18429 RVA: 0x0018205F File Offset: 0x0018025F
	public string LockMessage
	{
		get
		{
			return this._lockMessage;
		}
		set
		{
			this._lockMessage = value;
		}
	}

	// Token: 0x170006B7 RID: 1719
	// (get) Token: 0x060047FE RID: 18430 RVA: 0x00182068 File Offset: 0x00180268
	public int CurrentAge
	{
		get
		{
			return this._currentAge;
		}
	}

	// Token: 0x060047FF RID: 18431 RVA: 0x00182070 File Offset: 0x00180270
	private void Awake()
	{
		if (this._messageText)
		{
			this._originalText = this._messageText.text;
		}
	}

	// Token: 0x06004800 RID: 18432 RVA: 0x00182090 File Offset: 0x00180290
	public void SetOriginalText(string text)
	{
		this._originalText = text;
	}

	// Token: 0x06004801 RID: 18433 RVA: 0x0018209C File Offset: 0x0018029C
	private new void OnEnable()
	{
		base.OnEnable();
		if (this._progressBarContainer != null && this.progressBarFill != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
		}
	}

	// Token: 0x06004802 RID: 18434 RVA: 0x00182124 File Offset: 0x00180324
	public override void Tick()
	{
		if (!this._progressBarContainer)
		{
			return;
		}
		if (!this.ControllerActive)
		{
			return;
		}
		if (!this._lockMessage.IsNullOrEmpty())
		{
			this.progress = 0f;
			if (this._messageText)
			{
				this._messageText.text = this.LockMessage;
			}
		}
		else
		{
			if (this._messageText)
			{
				this._messageText.text = this._originalText;
			}
			if ((double)this.progress == 1.0)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				this.progress = 0f;
			}
			if (ControllerBehaviour.Instance.ButtonDown && this._progressBarContainer != null && (this._currentAge > 0 || !this.AdjustAge))
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progress = Mathf.Clamp01(this.progress);
			}
			else
			{
				this.progress = 0f;
			}
		}
		if (this._progressBarContainer != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(this.progress, 1f, 1f);
		}
	}

	// Token: 0x06004803 RID: 18435 RVA: 0x00182268 File Offset: 0x00180468
	private void PostUpdate()
	{
		if (this.ControllerActive && this._ageValueTxt && this._ageSlidable && !this._incrementButtonsLockingSlider)
		{
			if (ControllerBehaviour.Instance.IsLeftStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
			if (ControllerBehaviour.Instance.IsRightStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = this.GetAgeString();
			if (this._progressBarContainer != null)
			{
				this._progressBarContainer.SetActive(this._currentAge > 0);
			}
		}
	}

	// Token: 0x06004804 RID: 18436 RVA: 0x0018238C File Offset: 0x0018058C
	public void EnableEditing()
	{
		this._ageSlidable = true;
	}

	// Token: 0x06004805 RID: 18437 RVA: 0x00182395 File Offset: 0x00180595
	public void DisableEditing()
	{
		this._ageSlidable = false;
	}

	// Token: 0x06004806 RID: 18438 RVA: 0x001823A0 File Offset: 0x001805A0
	public string GetAgeString()
	{
		if (this._confirmButton)
		{
			this._confirmButton.interactable = true;
		}
		if (this._currentAge == 0)
		{
			if (this._confirmButton)
			{
				this._confirmButton.interactable = false;
			}
			return "?";
		}
		if (this._currentAge == this._maxAge)
		{
			return this._maxAge.ToString() + "+";
		}
		return this._currentAge.ToString();
	}

	// Token: 0x06004807 RID: 18439 RVA: 0x0018241C File Offset: 0x0018061C
	public void ForceAddAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Min(this._currentAge + number, this._maxAge);
	}

	// Token: 0x06004808 RID: 18440 RVA: 0x0018243E File Offset: 0x0018063E
	public void ForceSubtractAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Max(this._currentAge - number, 1);
	}

	// Token: 0x04005A15 RID: 23061
	private const int MIN_AGE = 13;

	// Token: 0x04005A16 RID: 23062
	[SerializeField]
	private AgeSliderWithProgressBar.SliderHeldEvent m_OnHoldComplete = new AgeSliderWithProgressBar.SliderHeldEvent();

	// Token: 0x04005A17 RID: 23063
	[SerializeField]
	private bool _adjustAge;

	// Token: 0x04005A18 RID: 23064
	[SerializeField]
	private int _maxAge = 25;

	// Token: 0x04005A19 RID: 23065
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04005A1A RID: 23066
	[Tooltip("Optional game object that should hold the Progress Bar Fill. Disables Hold functionality if null.")]
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x04005A1B RID: 23067
	[SerializeField]
	private float holdTime = 2.5f;

	// Token: 0x04005A1C RID: 23068
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x04005A1D RID: 23069
	[SerializeField]
	private TMP_Text _messageText;

	// Token: 0x04005A1E RID: 23070
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04005A1F RID: 23071
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x04005A20 RID: 23072
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005A21 RID: 23073
	private bool _ageSlidable = true;

	// Token: 0x04005A22 RID: 23074
	private bool _incrementButtonsLockingSlider;

	// Token: 0x04005A23 RID: 23075
	private bool controllerActive;

	// Token: 0x04005A24 RID: 23076
	[SerializeField]
	private string _lockMessage;

	// Token: 0x04005A25 RID: 23077
	private string _originalText;

	// Token: 0x04005A26 RID: 23078
	private int _currentAge;

	// Token: 0x04005A27 RID: 23079
	private float progress;

	// Token: 0x02000AFC RID: 2812
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
