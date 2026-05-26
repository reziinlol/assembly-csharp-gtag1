using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000140 RID: 320
public class SIGadgetListEntry : MonoBehaviour
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000804 RID: 2052 RVA: 0x0002C033 File Offset: 0x0002A233
	public SITouchscreenButtonContainer ButtonContainer
	{
		get
		{
			return this.buttonContainer;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000805 RID: 2053 RVA: 0x0002C03B File Offset: 0x0002A23B
	// (set) Token: 0x06000806 RID: 2054 RVA: 0x0002C043 File Offset: 0x0002A243
	public int Id { get; private set; } = -1;

	// Token: 0x06000807 RID: 2055 RVA: 0x0002C04C File Offset: 0x0002A24C
	public void Configure(ITouchScreenStation station, SITechTreePage page, Transform imageTarget, Transform textTarget, SITouchscreenButton.SITouchscreenButtonType buttonType = SITouchscreenButton.SITouchscreenButtonType.Select, int index = 0, float positionInterval = 0f, int listSize = 0)
	{
		base.name = (this.gadgetText.text = page.nickName);
		SITouchscreenButton button = this.buttonContainer.button;
		button.buttonType = buttonType;
		this.Id = (button.data = (int)page.pageId);
		button.buttonPressed.RemoveAllListeners();
		button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		station.AddButton(button, false);
		float num = (float)Mathf.Max(listSize - 1, 0) * -(positionInterval / 2f);
		base.transform.localPosition += new Vector3(0f, num + (float)index * positionInterval, 0f);
		this.imageFlattener.overrideParentTransform = imageTarget;
		this.textFlattener.overrideParentTransform = textTarget;
		this.imageFlattener.enabled = true;
		this.textFlattener.enabled = true;
		this.buttonContainer.SetUsable(page.IsAllowed);
	}

	// Token: 0x04000A25 RID: 2597
	[SerializeField]
	private TextMeshProUGUI gadgetText;

	// Token: 0x04000A26 RID: 2598
	[SerializeField]
	private SITouchscreenButtonContainer buttonContainer;

	// Token: 0x04000A27 RID: 2599
	public ObjectHierarchyFlattener imageFlattener;

	// Token: 0x04000A28 RID: 2600
	public ObjectHierarchyFlattener textFlattener;

	// Token: 0x04000A29 RID: 2601
	public GameObject selectionIndicator;
}
