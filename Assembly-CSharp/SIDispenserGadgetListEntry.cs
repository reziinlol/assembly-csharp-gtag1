using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200013A RID: 314
public class SIDispenserGadgetListEntry : MonoBehaviour
{
	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060007D6 RID: 2006 RVA: 0x0002ACDE File Offset: 0x00028EDE
	public SITouchscreenButtonContainer DispenseButton
	{
		get
		{
			return this.dispenseButton;
		}
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0002ACE8 File Offset: 0x00028EE8
	public void SetStation(ITouchScreenStation station, Transform imageTarget, Transform textTarget)
	{
		this.dispenseButton.button.buttonPressed.RemoveAllListeners();
		this.dispenseButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		this.infoButton.button.buttonPressed.RemoveAllListeners();
		this.infoButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		station.AddButton(this.dispenseButton.button, false);
		station.AddButton(this.infoButton.button, false);
		this.image1.overrideParentTransform = imageTarget;
		this.image2.overrideParentTransform = imageTarget;
		this.text1.overrideParentTransform = textTarget;
		this.text2.overrideParentTransform = textTarget;
		this.image1.enabled = true;
		this.image2.enabled = true;
		this.text1.enabled = true;
		this.text2.enabled = true;
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002ADE8 File Offset: 0x00028FE8
	public void SetTechTreeNode(SITechTreeNode node)
	{
		base.name = (this.gadgetText.text = node.nickName);
		int nodeId = node.upgradeType.GetNodeId();
		SIDispenserGadgetListEntry.<SetTechTreeNode>g__ConfigureButton|10_0(this.dispenseButton.button, SITouchscreenButton.SITouchscreenButtonType.Dispense, nodeId);
		SIDispenserGadgetListEntry.<SetTechTreeNode>g__ConfigureButton|10_0(this.infoButton.button, SITouchscreenButton.SITouchscreenButtonType.Select, nodeId);
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0002AE3F File Offset: 0x0002903F
	[CompilerGenerated]
	internal static void <SetTechTreeNode>g__ConfigureButton|10_0(SITouchscreenButton button, SITouchscreenButton.SITouchscreenButtonType type, int data)
	{
		button.buttonType = type;
		button.data = data;
	}

	// Token: 0x040009EB RID: 2539
	[SerializeField]
	private TextMeshProUGUI gadgetText;

	// Token: 0x040009EC RID: 2540
	[SerializeField]
	private SITouchscreenButtonContainer dispenseButton;

	// Token: 0x040009ED RID: 2541
	[SerializeField]
	private SITouchscreenButtonContainer infoButton;

	// Token: 0x040009EE RID: 2542
	public ObjectHierarchyFlattener image1;

	// Token: 0x040009EF RID: 2543
	public ObjectHierarchyFlattener image2;

	// Token: 0x040009F0 RID: 2544
	public ObjectHierarchyFlattener text1;

	// Token: 0x040009F1 RID: 2545
	public ObjectHierarchyFlattener text2;
}
