using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Token: 0x02000B75 RID: 2933
public class HandRayController : MonoBehaviour
{
	// Token: 0x170006FA RID: 1786
	// (get) Token: 0x060049D2 RID: 18898 RVA: 0x0018B6EE File Offset: 0x001898EE
	public static HandRayController Instance
	{
		get
		{
			if (HandRayController.instance == null)
			{
				HandRayController.instance = Object.FindAnyObjectByType<HandRayController>();
				if (HandRayController.instance == null)
				{
					Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Not found in scene", Array.Empty<object>());
				}
			}
			return HandRayController.instance;
		}
	}

	// Token: 0x060049D3 RID: 18899 RVA: 0x0018B728 File Offset: 0x00189928
	private void Awake()
	{
		if (HandRayController.instance != null && HandRayController.instance != this)
		{
			Debug.LogErrorFormat(base.gameObject, "[KID::UI::HAND_RAY_CONTROLLER] Duplicate instance of HandRayController", Array.Empty<object>());
			Object.DestroyImmediate(this);
			return;
		}
		HandRayController.instance = this;
	}

	// Token: 0x060049D4 RID: 18900 RVA: 0x0018B768 File Offset: 0x00189968
	private void Start()
	{
		this._leftHandRay.attachTransform = (this._leftHandRay.rayOriginTransform = KIDHandReference.LeftHand.transform);
		this._rightHandRay.attachTransform = (this._rightHandRay.rayOriginTransform = KIDHandReference.RightHand.transform);
		this.DisableHandRays();
		this._activationCounter = 0;
	}

	// Token: 0x060049D5 RID: 18901 RVA: 0x0018B7C8 File Offset: 0x001899C8
	private void OnDisable()
	{
		this.DisableHandRays();
	}

	// Token: 0x060049D6 RID: 18902 RVA: 0x0018B7D0 File Offset: 0x001899D0
	public void EnableHandRays()
	{
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			this.ToggleHands();
		}
		this._activationCounter++;
	}

	// Token: 0x060049D7 RID: 18903 RVA: 0x0018B810 File Offset: 0x00189A10
	public void DisableHandRays()
	{
		this._activationCounter--;
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.HideHands();
		}
	}

	// Token: 0x060049D8 RID: 18904 RVA: 0x0018B850 File Offset: 0x00189A50
	public void PulseActiveHandray(float vibrationStrength, float vibrationDuration)
	{
		if (this._activeHandRay == null)
		{
			return;
		}
		this._activeHandRay.SendHapticImpulse(vibrationStrength, vibrationDuration);
	}

	// Token: 0x060049D9 RID: 18905 RVA: 0x0018B86F File Offset: 0x00189A6F
	private void PostUpdate()
	{
		if (!this._hasInitialised)
		{
			return;
		}
		if (this.ActiveHand == HandRayController.HandSide.Left)
		{
			if (ControllerBehaviour.Instance.RightButtonDown)
			{
				this.ToggleHands();
			}
			return;
		}
		if (ControllerBehaviour.Instance.LeftButtonDown)
		{
			this.ToggleHands();
		}
	}

	// Token: 0x060049DA RID: 18906 RVA: 0x0018B8A8 File Offset: 0x00189AA8
	private void ToggleRightHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] RIGHT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._rightHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._rightHandRay;
		}
	}

	// Token: 0x060049DB RID: 18907 RVA: 0x0018B904 File Offset: 0x00189B04
	private void ToggleLeftHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] LEFT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._leftHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._leftHandRay;
		}
	}

	// Token: 0x060049DC RID: 18908 RVA: 0x0018B960 File Offset: 0x00189B60
	private void InitialiseHands()
	{
		Debug.Log("[KID::UI::HAND_RAY_CONTROLLER] Initialising Hands");
		this.ToggleRightHandRay(this.ActiveHand == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(this.ActiveHand == HandRayController.HandSide.Left);
		this._hasInitialised = true;
	}

	// Token: 0x060049DD RID: 18909 RVA: 0x0018B994 File Offset: 0x00189B94
	private void ToggleHands()
	{
		if (!this._hasInitialised)
		{
			this.InitialiseHands();
			return;
		}
		HandRayController.HandSide handSide = (this.ActiveHand == HandRayController.HandSide.Left) ? HandRayController.HandSide.Right : HandRayController.HandSide.Left;
		Debug.LogFormat(string.Concat(new string[]
		{
			"[KID::UI::HAND_RAY_CONTROLLER] Setting ActiveHand FROM: [",
			this.ActiveHand.ToString(),
			"] TO: [",
			handSide.ToString(),
			"]"
		}), Array.Empty<object>());
		this.ActiveHand = handSide;
		this.ToggleRightHandRay(handSide == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(handSide == HandRayController.HandSide.Left);
	}

	// Token: 0x060049DE RID: 18910 RVA: 0x0018BA29 File Offset: 0x00189C29
	private void HideHands()
	{
		this.ToggleRightHandRay(false);
		this.ToggleLeftHandRay(false);
		this._hasInitialised = false;
		this._activeHandRay = null;
	}

	// Token: 0x04005C9A RID: 23706
	[OnEnterPlay_SetNull]
	private static HandRayController instance;

	// Token: 0x04005C9B RID: 23707
	[SerializeField]
	private XRRayInteractor _leftHandRay;

	// Token: 0x04005C9C RID: 23708
	[SerializeField]
	private XRRayInteractor _rightHandRay;

	// Token: 0x04005C9D RID: 23709
	private bool _hasInitialised;

	// Token: 0x04005C9E RID: 23710
	private HandRayController.HandSide ActiveHand = HandRayController.HandSide.Right;

	// Token: 0x04005C9F RID: 23711
	private XRRayInteractor _activeHandRay;

	// Token: 0x04005CA0 RID: 23712
	private int _activationCounter;

	// Token: 0x02000B76 RID: 2934
	private enum HandSide
	{
		// Token: 0x04005CA2 RID: 23714
		Left,
		// Token: 0x04005CA3 RID: 23715
		Right
	}
}
