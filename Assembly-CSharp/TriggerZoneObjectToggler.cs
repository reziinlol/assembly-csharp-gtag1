using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001EA RID: 490
public class TriggerZoneObjectToggler : MonoBehaviour
{
	// Token: 0x06000CE2 RID: 3298 RVA: 0x00046A0A File Offset: 0x00044C0A
	private void Awake()
	{
		this.ToggleObject.SetActive(false);
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00046A18 File Offset: 0x00044C18
	private void OnDisable()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			this.HandleExit();
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00046A27 File Offset: 0x00044C27
	private void OnTriggerEnter(Collider other)
	{
		if (this.IsMatchingTrigger(other))
		{
			this.HandleEnter();
		}
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00046A38 File Offset: 0x00044C38
	private void OnTriggerExit(Collider other)
	{
		if (this.IsMatchingTrigger(other))
		{
			this.HandleExit();
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00046A49 File Offset: 0x00044C49
	private void HandleEnter()
	{
		if (this._inTriggerZone)
		{
			return;
		}
		this._inTriggerZone = true;
		this.ToggleObject.SetActive(true);
		UnityEvent onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter.Invoke();
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00046A77 File Offset: 0x00044C77
	private void HandleExit()
	{
		if (!this._inTriggerZone)
		{
			return;
		}
		this._inTriggerZone = false;
		this.ToggleObject.SetActive(false);
		UnityEvent onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit.Invoke();
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00046AA8 File Offset: 0x00044CA8
	private bool IsMatchingTrigger(Collider other)
	{
		NamedTriggerZone component = other.GetComponent<NamedTriggerZone>();
		return component != null && component.TriggerName == this.TriggerName;
	}

	// Token: 0x04000F82 RID: 3970
	public string TriggerName = "Trigger";

	// Token: 0x04000F83 RID: 3971
	public GameObject ToggleObject;

	// Token: 0x04000F84 RID: 3972
	public UnityEvent OnEnter;

	// Token: 0x04000F85 RID: 3973
	public UnityEvent OnExit;

	// Token: 0x04000F86 RID: 3974
	private bool _inTriggerZone;
}
