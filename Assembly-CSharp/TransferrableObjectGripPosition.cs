using System;
using UnityEngine;

// Token: 0x0200054D RID: 1357
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x06002289 RID: 8841 RVA: 0x000B9A0F File Offset: 0x000B7C0F
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x000B9A47 File Offset: 0x000B7C47
	public SubGrabPoint CreateSubGrabPoint(SlotTransformOverride overrideContainer)
	{
		return new SubGrabPoint();
	}

	// Token: 0x04002D9D RID: 11677
	[SerializeField]
	private TransferrableItemSlotTransformOverride parentObject;

	// Token: 0x04002D9E RID: 11678
	[SerializeField]
	private TransferrableObject.PositionState attachmentType;
}
