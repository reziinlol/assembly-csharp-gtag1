using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000B80 RID: 2944
public class KIDUIHoldableButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x170006FE RID: 1790
	// (get) Token: 0x06004A26 RID: 18982 RVA: 0x0018CD6A File Offset: 0x0018AF6A
	// (set) Token: 0x06004A27 RID: 18983 RVA: 0x0018CD72 File Offset: 0x0018AF72
	public KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete
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

	// Token: 0x170006FF RID: 1791
	// (get) Token: 0x06004A28 RID: 18984 RVA: 0x0018CD7B File Offset: 0x0018AF7B
	public float HoldPercentage
	{
		get
		{
			return this._elapsedTime / this._holdDuration;
		}
	}

	// Token: 0x06004A29 RID: 18985 RVA: 0x0018CD8C File Offset: 0x0018AF8C
	private void OnEnable()
	{
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004A2A RID: 18986 RVA: 0x0018CDDF File Offset: 0x0018AFDF
	private void Update()
	{
		this.ManageButtonInteraction(false);
	}

	// Token: 0x06004A2B RID: 18987 RVA: 0x0018CDE8 File Offset: 0x0018AFE8
	public void OnPointerDown(PointerEventData eventData)
	{
		this._isHoldingMouse = true;
		this.ToggleHoldingButton(true);
	}

	// Token: 0x06004A2C RID: 18988 RVA: 0x0018CDF8 File Offset: 0x0018AFF8
	public void OnPointerUp(PointerEventData eventData)
	{
		this._isHoldingMouse = false;
		this.ManageButtonInteraction(true);
		this.ToggleHoldingButton(false);
	}

	// Token: 0x06004A2D RID: 18989 RVA: 0x0018CE10 File Offset: 0x0018B010
	private void ToggleHoldingButton(bool isPointerDown)
	{
		this._isHoldingButton = (isPointerDown && this._button.interactable);
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (isPointerDown)
		{
			this._elapsedTime = 0f;
			KIDUIHoldableButton.ButtonHoldStartEvent onHoldStart = this.m_OnHoldStart;
			if (onHoldStart != null)
			{
				onHoldStart.Invoke();
			}
			KIDAudioManager.Instance.StartButtonHeldSound();
			return;
		}
		KIDUIHoldableButton.ButtonHoldReleaseEvent onHoldRelease = this.m_OnHoldRelease;
		if (onHoldRelease != null)
		{
			onHoldRelease.Invoke();
		}
		KIDAudioManager.Instance.StopButtonHeldSound();
	}

	// Token: 0x06004A2E RID: 18990 RVA: 0x0018CEA0 File Offset: 0x0018B0A0
	private void ManageButtonInteraction(bool isPointerUp = false)
	{
		if (!this._isHoldingButton)
		{
			return;
		}
		if (isPointerUp)
		{
			return;
		}
		if (this._holdDuration <= 0f)
		{
			this.HoldComplete();
			return;
		}
		this._elapsedTime += Time.deltaTime;
		bool flag = this._elapsedTime > this._holdDuration;
		float num = this._elapsedTime / this._holdDuration;
		this._holdProgressFill.rectTransform.localScale = new Vector3(num, 1f, 1f);
		HandRayController.Instance.PulseActiveHandray(num, 0.1f);
		if (flag)
		{
			this.HoldComplete();
		}
	}

	// Token: 0x06004A2F RID: 18991 RVA: 0x0018CF34 File Offset: 0x0018B134
	private void HoldComplete()
	{
		this.ToggleHoldingButton(false);
		KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete = this.m_OnHoldComplete;
		if (onHoldComplete != null)
		{
			onHoldComplete.Invoke();
		}
		Debug.Log("[HOLD_BUTTON " + base.name + " ]: Hold Complete");
		this.ResetButton();
	}

	// Token: 0x06004A30 RID: 18992 RVA: 0x0018CF6E File Offset: 0x0018B16E
	private void ResetButton()
	{
		this._elapsedTime = 0f;
		this.inside = false;
		KIDUIHoldableButton._triggeredThisFrame = false;
		this._button.ResetButton();
	}

	// Token: 0x06004A31 RID: 18993 RVA: 0x0018CF93 File Offset: 0x0018B193
	protected void Awake()
	{
		if (this._button != null)
		{
			return;
		}
		this._button = base.GetComponentInChildren<KIDUIButton>();
		if (this._button == null)
		{
			Debug.LogError("[KID::UI_BUTTON] Could not find [KIDUIButton] in children, trying to create a new one.");
			return;
		}
	}

	// Token: 0x06004A32 RID: 18994 RVA: 0x0018CFCC File Offset: 0x0018B1CC
	private void PostUpdate()
	{
		if (!KIDUIHoldableButton._canTrigger)
		{
			KIDUIHoldableButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!this._button.interactable || !KIDUIHoldableButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && this.inside)
			{
				if (!this._isHoldingButton)
				{
					string text = string.Concat(new string[]
					{
						"[",
						base.transform.parent.parent.parent.name,
						".",
						base.transform.parent.parent.name,
						".",
						base.transform.parent.name,
						".",
						base.transform.name,
						"]"
					});
					Debug.Log(string.Concat(new string[]
					{
						"[KID::UIBUTTON::DEBUG] ",
						text,
						" - STEAM - OnClick is pressed. Time: [",
						Time.time.ToString(),
						"]"
					}), this);
					this.ToggleHoldingButton(true);
					KIDUIHoldableButton._triggeredThisFrame = true;
					KIDUIHoldableButton._canTrigger = false;
					return;
				}
			}
			else if (this._isHoldingButton && !this._isHoldingMouse)
			{
				this.ToggleHoldingButton(false);
			}
		}
	}

	// Token: 0x06004A33 RID: 18995 RVA: 0x0018D130 File Offset: 0x0018B330
	private void LateUpdate()
	{
		if (KIDUIHoldableButton._triggeredThisFrame)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnLateUpdate triggered and Triggered Frame Reset. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
		}
		KIDUIHoldableButton._triggeredThisFrame = false;
	}

	// Token: 0x06004A34 RID: 18996 RVA: 0x0018D215 File Offset: 0x0018B415
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
	}

	// Token: 0x06004A35 RID: 18997 RVA: 0x0018D21E File Offset: 0x0018B41E
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06004A36 RID: 18998 RVA: 0x0018D227 File Offset: 0x0018B427
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x04005CF5 RID: 23797
	public KIDUIButton _button;

	// Token: 0x04005CF6 RID: 23798
	[SerializeField]
	private float _holdDuration;

	// Token: 0x04005CF7 RID: 23799
	[SerializeField]
	private Image _holdProgressFill;

	// Token: 0x04005CF8 RID: 23800
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005CF9 RID: 23801
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldCompleteEvent m_OnHoldComplete = new KIDUIHoldableButton.ButtonHoldCompleteEvent();

	// Token: 0x04005CFA RID: 23802
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldStartEvent m_OnHoldStart = new KIDUIHoldableButton.ButtonHoldStartEvent();

	// Token: 0x04005CFB RID: 23803
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldReleaseEvent m_OnHoldRelease = new KIDUIHoldableButton.ButtonHoldReleaseEvent();

	// Token: 0x04005CFC RID: 23804
	private bool _isHoldingButton;

	// Token: 0x04005CFD RID: 23805
	private float _elapsedTime;

	// Token: 0x04005CFE RID: 23806
	private bool inside;

	// Token: 0x04005CFF RID: 23807
	private bool _isHoldingMouse;

	// Token: 0x04005D00 RID: 23808
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005D01 RID: 23809
	private static bool _canTrigger = true;

	// Token: 0x02000B81 RID: 2945
	[Serializable]
	public class ButtonHoldCompleteEvent : UnityEvent
	{
	}

	// Token: 0x02000B82 RID: 2946
	[Serializable]
	public class ButtonHoldStartEvent : UnityEvent
	{
	}

	// Token: 0x02000B83 RID: 2947
	[Serializable]
	public class ButtonHoldReleaseEvent : UnityEvent
	{
	}
}
