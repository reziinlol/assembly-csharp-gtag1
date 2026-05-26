using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Valve.VR;

// Token: 0x02000BA8 RID: 2984
public class KIDUI_InputFieldController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1700070C RID: 1804
	// (get) Token: 0x06004AEC RID: 19180 RVA: 0x00087432 File Offset: 0x00085632
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06004AED RID: 19181 RVA: 0x001903DC File Offset: 0x0018E5DC
	protected void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
		SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x06004AEE RID: 19182 RVA: 0x00190444 File Offset: 0x0018E644
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Remove(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Remove(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x06004AEF RID: 19183 RVA: 0x001904AC File Offset: 0x0018E6AC
	private void Update()
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		SteamVR.instance.overlay.GetKeyboardText(this._inputStringBuilder, 1024U);
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] String BUilder Says: [" + this._inputStringBuilder.ToString() + "]");
		this._inputField.text = this._inputBuffer;
		this._inputField.stringPosition = this._inputBuffer.Length;
	}

	// Token: 0x06004AF0 RID: 19184 RVA: 0x00190524 File Offset: 0x0018E724
	private void PostUpdate()
	{
		if (!this._inputField.interactable || !this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown)
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
			this.OnClickedInputField("");
		}
	}

	// Token: 0x06004AF1 RID: 19185 RVA: 0x00190638 File Offset: 0x0018E838
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
		if (!this._inputField.IsInteractable() || !this._inputField.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06004AF2 RID: 19186 RVA: 0x0019069F File Offset: 0x0018E89F
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06004AF3 RID: 19187 RVA: 0x001906A8 File Offset: 0x0018E8A8
	private void OnClickedInputField(string _ = "")
	{
		if (this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUT_FIELD_CONTROLLER] Selecting and Activating Input Field");
		EVROverlayError evroverlayError = OpenVR.Overlay.ShowKeyboard(0, 0, 1U, "Enter Email", 1024U, this._inputField.text ?? "", 0UL);
		if (evroverlayError != EVROverlayError.None)
		{
			Debug.LogError("[KID::INPUT_FIELD_CONTROLLER] Failed to open keyboard. Resulted with error: [" + evroverlayError.ToString() + "]");
			return;
		}
		this._inputBuffer = (this._inputField.text ?? "");
		this.keyboardShowing = true;
		HandRayController.Instance.DisableHandRays();
	}

	// Token: 0x06004AF4 RID: 19188 RVA: 0x00190748 File Offset: 0x0018E948
	private void OnChar(VREvent_t ev)
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		char c = ev.data.keyboard.cNewInput[0];
		if (c == '\b')
		{
			this._inputBuffer = this._inputBuffer.Remove(this._inputBuffer.Length - 1, 1);
			return;
		}
		if (this.IsIllegalChar(c))
		{
			return;
		}
		this._inputBuffer += c.ToString();
	}

	// Token: 0x06004AF5 RID: 19189 RVA: 0x001907BC File Offset: 0x0018E9BC
	private void OnKeyboardClosed(VREvent_t ev)
	{
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Trying to close Keyboard");
		if (!this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Closing Keyboard");
		OpenVR.Overlay.HideKeyboard();
		this._inputField.text = this._inputBuffer;
		this._inputField.DeactivateInputField(false);
		HandRayController.Instance.EnableHandRays();
		this.keyboardShowing = false;
	}

	// Token: 0x06004AF6 RID: 19190 RVA: 0x0019081E File Offset: 0x0018EA1E
	private bool IsIllegalChar(char c)
	{
		return c == '\t' || c == '\n';
	}

	// Token: 0x04005DD2 RID: 24018
	[Header("Haptics")]
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04005DD3 RID: 24019
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04005DD4 RID: 24020
	[Header("Steam Settings")]
	[SerializeField]
	private TMP_InputField _inputField;

	// Token: 0x04005DD5 RID: 24021
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005DD6 RID: 24022
	public bool testMinimal;

	// Token: 0x04005DD7 RID: 24023
	public bool minimalMode;

	// Token: 0x04005DD8 RID: 24024
	private bool inside;

	// Token: 0x04005DD9 RID: 24025
	private bool keyboardShowing;

	// Token: 0x04005DDA RID: 24026
	private bool _canTrigger = true;

	// Token: 0x04005DDB RID: 24027
	private string _testStr = string.Empty;

	// Token: 0x04005DDC RID: 24028
	private string previousStr = string.Empty;

	// Token: 0x04005DDD RID: 24029
	private StringBuilder _inputStringBuilder = new StringBuilder(1024);

	// Token: 0x04005DDE RID: 24030
	private string _inputBuffer = "";
}
