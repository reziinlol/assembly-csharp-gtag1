using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AF9 RID: 2809
public class AgeSlider : MonoBehaviour, IBuildValidation
{
	// Token: 0x170006B2 RID: 1714
	// (get) Token: 0x060047ED RID: 18413 RVA: 0x00181D55 File Offset: 0x0017FF55
	// (set) Token: 0x060047EE RID: 18414 RVA: 0x00181D5D File Offset: 0x0017FF5D
	public AgeSlider.SliderHeldEvent onHoldComplete
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

	// Token: 0x060047EF RID: 18415 RVA: 0x00181D66 File Offset: 0x0017FF66
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060047F0 RID: 18416 RVA: 0x00181D8A File Offset: 0x0017FF8A
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x060047F1 RID: 18417 RVA: 0x00181DB0 File Offset: 0x0017FFB0
	protected void Update()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.ButtonDown && this._confirmButton.activeInHierarchy)
		{
			this.progress += Time.deltaTime / this.holdTime;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			if (this.progress >= 1f)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				return;
			}
		}
		else
		{
			this.progress = 0f;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
		}
	}

	// Token: 0x060047F2 RID: 18418 RVA: 0x00181EBC File Offset: 0x001800BC
	private void PostUpdate()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.IsLeftStick || ControllerBehaviour.Instance.IsUpStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
		if (ControllerBehaviour.Instance.IsRightStick || ControllerBehaviour.Instance.IsDownStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
	}

	// Token: 0x060047F3 RID: 18419 RVA: 0x00181FA9 File Offset: 0x001801A9
	public static void ToggleAgeGate(bool state)
	{
		AgeSlider._ageGateActive = state;
	}

	// Token: 0x060047F4 RID: 18420 RVA: 0x00181FB1 File Offset: 0x001801B1
	public bool BuildValidationCheck()
	{
		if (this._confirmButton == null)
		{
			Debug.LogError("[KID] Object [_confirmButton] is NULL. Must be assigned in editor");
			return false;
		}
		return true;
	}

	// Token: 0x04005A0B RID: 23051
	private const int MIN_AGE = 13;

	// Token: 0x04005A0C RID: 23052
	[SerializeField]
	private AgeSlider.SliderHeldEvent m_OnHoldComplete = new AgeSlider.SliderHeldEvent();

	// Token: 0x04005A0D RID: 23053
	[SerializeField]
	private int _maxAge = 99;

	// Token: 0x04005A0E RID: 23054
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04005A0F RID: 23055
	[SerializeField]
	private GameObject _confirmButton;

	// Token: 0x04005A10 RID: 23056
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x04005A11 RID: 23057
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x04005A12 RID: 23058
	private int _currentAge;

	// Token: 0x04005A13 RID: 23059
	private static bool _ageGateActive;

	// Token: 0x04005A14 RID: 23060
	private float progress;

	// Token: 0x02000AFA RID: 2810
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
