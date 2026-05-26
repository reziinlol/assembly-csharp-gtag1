using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x020003E0 RID: 992
[AddComponentMenu("UI/KIDUI Scrollbar", 37)]
public class KIDUIScrollbar : Scrollbar, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06001790 RID: 6032 RVA: 0x00087432 File Offset: 0x00085632
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06001791 RID: 6033 RVA: 0x00087443 File Offset: 0x00085643
	private KIDUIScrollbar.Axis axis
	{
		get
		{
			if (base.direction != Scrollbar.Direction.LeftToRight && base.direction != Scrollbar.Direction.RightToLeft)
			{
				return KIDUIScrollbar.Axis.Vertical;
			}
			return KIDUIScrollbar.Axis.Horizontal;
		}
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x0008745C File Offset: 0x0008565C
	protected override void OnEnable()
	{
		base.OnEnable();
		this.containerRect = base.handleRect.parent.GetComponent<RectTransform>();
		if (GorillaTagger.Instance)
		{
			this.thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		}
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x000874C9 File Offset: 0x000856C9
	protected override void OnDisable()
	{
		base.OnDisable();
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this._isPointerInside = false;
		this._currentPointerData = null;
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x00087504 File Offset: 0x00085704
	private void PostUpdate()
	{
		if (!this._isPointerInside && !ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = false;
			return;
		}
		if (!base.interactable || !ControllerBehaviour.Instance.TriggerDown || this._currentPointerData == null)
		{
			return;
		}
		if (!this._isHolding && this._isPointerInside && ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = true;
		}
		if (!this._isHolding || !this.IsInteractable() || this.InputModule == null)
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(this._currentPointerData.pointerId) as XRRayInteractor;
		RaycastResult raycastResult;
		if (xrrayInteractor != null && xrrayInteractor.TryGetCurrentUIRaycastResult(out raycastResult))
		{
			Vector2 a;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.containerRect, raycastResult.screenPosition, this.thirdPersonCamera, out a);
			Vector2 zero = Vector2.zero;
			Vector2 handleCorner = a - zero - this.containerRect.rect.position - (base.handleRect.rect.size - base.handleRect.sizeDelta) * 0.5f;
			float num = ((this.axis == KIDUIScrollbar.Axis.Horizontal) ? this.containerRect.rect.width : this.containerRect.rect.height) * (1f - base.size);
			if (num <= 0f)
			{
				return;
			}
			this.UpdateDrag(handleCorner, num);
		}
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x00087688 File Offset: 0x00085888
	private void UpdateDrag(Vector2 handleCorner, float remainingSize)
	{
		switch (base.direction)
		{
		case Scrollbar.Direction.LeftToRight:
			base.value = Mathf.Clamp01(handleCorner.x / remainingSize);
			return;
		case Scrollbar.Direction.RightToLeft:
			base.value = Mathf.Clamp01(1f - handleCorner.x / remainingSize);
			return;
		case Scrollbar.Direction.BottomToTop:
			base.value = Mathf.Clamp01(handleCorner.y / remainingSize);
			return;
		case Scrollbar.Direction.TopToBottom:
			base.value = Mathf.Clamp01(1f - handleCorner.y / remainingSize);
			return;
		default:
			return;
		}
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x00087710 File Offset: 0x00085910
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this._isPointerInside = true;
		this._currentPointerData = eventData;
		if (this.IsInteractable() && this.InputModule != null)
		{
			XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
			if (xrrayInteractor != null)
			{
				xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
			}
		}
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x0008777A File Offset: 0x0008597A
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this._isPointerInside = false;
	}

	// Token: 0x040022CE RID: 8910
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x040022CF RID: 8911
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x040022D0 RID: 8912
	private RectTransform containerRect;

	// Token: 0x040022D1 RID: 8913
	private bool _isPointerInside;

	// Token: 0x040022D2 RID: 8914
	private bool _isHolding;

	// Token: 0x040022D3 RID: 8915
	private PointerEventData _currentPointerData;

	// Token: 0x040022D4 RID: 8916
	private Camera thirdPersonCamera;

	// Token: 0x020003E1 RID: 993
	private enum Axis
	{
		// Token: 0x040022D6 RID: 8918
		Horizontal,
		// Token: 0x040022D7 RID: 8919
		Vertical
	}
}
