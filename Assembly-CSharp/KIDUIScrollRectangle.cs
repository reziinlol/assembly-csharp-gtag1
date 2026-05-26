using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x020003E2 RID: 994
[AddComponentMenu("UI/KIDUI Scroll Rect", 37)]
public class KIDUIScrollRectangle : ScrollRect, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06001799 RID: 6041 RVA: 0x00087432 File Offset: 0x00085632
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x000877A8 File Offset: 0x000859A8
	protected override void OnEnable()
	{
		base.OnEnable();
		this.thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x000877E8 File Offset: 0x000859E8
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

	// Token: 0x0600179C RID: 6044 RVA: 0x00087824 File Offset: 0x00085A24
	private void PostUpdate()
	{
		if (this._currentPointerData == null || this.InputModule == null)
		{
			return;
		}
		if (this._currentPointerData.hovered.Contains(base.viewport.gameObject) && !this._currentPointerData.hovered.Contains(base.verticalScrollbar.gameObject))
		{
			this._isPointerInside = true;
		}
		else
		{
			this._isPointerInside = false;
		}
		if (!ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = false;
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(this._currentPointerData.pointerId) as XRRayInteractor;
		if (xrrayInteractor == null)
		{
			return;
		}
		XRRayInteractor xrrayInteractor2 = xrrayInteractor;
		RaycastResult raycastResult;
		if (!xrrayInteractor2.TryGetCurrentUIRaycastResult(out raycastResult))
		{
			return;
		}
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.viewRect, raycastResult.screenPosition, this.thirdPersonCamera, out vector);
		if (!this._isHolding && this._isPointerInside && ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = true;
			this.m_PointerStartLocalCursor = vector;
			this.m_ContentStartPosition = base.content.anchoredPosition;
		}
		if (!this._isHolding)
		{
			return;
		}
		base.UpdateBounds();
		Vector2 b = vector - this.m_PointerStartLocalCursor;
		Vector2 contentAnchoredPosition = this.m_ContentStartPosition + b;
		this.SetContentAnchoredPosition(contentAnchoredPosition);
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x00087960 File Offset: 0x00085B60
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.hovered.Contains(base.viewport.gameObject))
		{
			this._isPointerInside = true;
			this._currentPointerData = eventData;
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x00087988 File Offset: 0x00085B88
	public void OnPointerExit(PointerEventData eventData)
	{
		this._isPointerInside = false;
	}

	// Token: 0x040022D8 RID: 8920
	private bool _isPointerInside;

	// Token: 0x040022D9 RID: 8921
	private bool _isHolding;

	// Token: 0x040022DA RID: 8922
	private PointerEventData _currentPointerData;

	// Token: 0x040022DB RID: 8923
	private Vector2 m_PointerStartLocalCursor = Vector2.zero;

	// Token: 0x040022DC RID: 8924
	private Camera thirdPersonCamera;
}
