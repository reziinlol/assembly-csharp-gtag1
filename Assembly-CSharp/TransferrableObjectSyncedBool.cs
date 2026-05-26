using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200054F RID: 1359
public class TransferrableObjectSyncedBool : TransferrableObject
{
	// Token: 0x06002295 RID: 8853 RVA: 0x000B9B5B File Offset: 0x000B7D5B
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnItemStateBoolFalse.AddListener(new UnityAction(this.OnItemStateChanged));
		this.OnItemStateBoolTrue.AddListener(new UnityAction(this.OnItemStateChanged));
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000B9B91 File Offset: 0x000B7D91
	internal override void OnDisable()
	{
		base.OnDisable();
		this.OnItemStateBoolFalse.RemoveListener(new UnityAction(this.OnItemStateChanged));
		this.OnItemStateBoolTrue.RemoveListener(new UnityAction(this.OnItemStateChanged));
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000B9BC8 File Offset: 0x000B7DC8
	public void SetItemState(bool state)
	{
		base.SetItemStateBool(state);
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000B9BDC File Offset: 0x000B7DDC
	public void ToggleItemState()
	{
		base.ToggleNetworkedItemStateBool();
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x000B9BEF File Offset: 0x000B7DEF
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			UnityEvent onItemStateSetFalse = this.OnItemStateSetFalse;
			if (onItemStateSetFalse == null)
			{
				return;
			}
			onItemStateSetFalse.Invoke();
			return;
		}
		else
		{
			UnityEvent onItemStateSetTrue = this.OnItemStateSetTrue;
			if (onItemStateSetTrue == null)
			{
				return;
			}
			onItemStateSetTrue.Invoke();
			return;
		}
	}

	// Token: 0x04002DA2 RID: 11682
	[SerializeField]
	private bool deprecatedWarning = true;

	// Token: 0x04002DA3 RID: 11683
	[SerializeField]
	private UnityEvent OnItemStateSetTrue;

	// Token: 0x04002DA4 RID: 11684
	[SerializeField]
	private UnityEvent OnItemStateSetFalse;
}
