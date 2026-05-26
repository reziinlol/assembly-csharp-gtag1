using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002F1 RID: 753
public class RadioButtonGroupWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x0600132A RID: 4906 RVA: 0x000656DE File Offset: 0x000638DE
	// (set) Token: 0x0600132B RID: 4907 RVA: 0x000656E6 File Offset: 0x000638E6
	public bool IsSpawned { get; set; }

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x0600132C RID: 4908 RVA: 0x000656EF File Offset: 0x000638EF
	// (set) Token: 0x0600132D RID: 4909 RVA: 0x000656F7 File Offset: 0x000638F7
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600132E RID: 4910 RVA: 0x00065700 File Offset: 0x00063900
	private void Start()
	{
		this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.assignedSlot];
		if (!this.ownerRig.isLocal)
		{
			GorillaPressableButton[] array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				Collider component = array[i].GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x0006575E File Offset: 0x0006395E
	private void OnEnable()
	{
		this.SharedRefreshState();
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00065766 File Offset: 0x00063966
	private int GetCurrentState()
	{
		return GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x0006578E File Offset: 0x0006398E
	private void Update()
	{
		if (this.ownerRig.isLocal)
		{
			return;
		}
		if (this.lastReportedState != this.GetCurrentState())
		{
			this.SharedRefreshState();
		}
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x000657B4 File Offset: 0x000639B4
	public void SharedRefreshState()
	{
		int currentState = this.GetCurrentState();
		int num = this.AllowSelectNone ? (currentState - 1) : currentState;
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].isOn = (num == i);
			this.buttons[i].UpdateColor();
		}
		if (this.lastReportedState != currentState)
		{
			this.lastReportedState = currentState;
			this.OnSelectionChanged.Invoke(currentState);
		}
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x00065824 File Offset: 0x00063A24
	public void OnPress(GorillaPressableButton button)
	{
		int currentState = this.GetCurrentState();
		int num = Array.IndexOf<GorillaPressableButton>(this.buttons, button);
		if (this.AllowSelectNone)
		{
			num++;
		}
		int value = num;
		if (this.AllowSelectNone && num == currentState)
		{
			value = 0;
		}
		this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, value);
		this.SharedRefreshState();
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x00065889 File Offset: 0x00063A89
	public void OnSpawn(VRRig rig)
	{
		this.ownerRig = rig;
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x0400176B RID: 5995
	[SerializeField]
	private bool AllowSelectNone = true;

	// Token: 0x0400176C RID: 5996
	[SerializeField]
	private GorillaPressableButton[] buttons;

	// Token: 0x0400176D RID: 5997
	[SerializeField]
	private UnityEvent<int> OnSelectionChanged;

	// Token: 0x0400176E RID: 5998
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlot = VRRig.WearablePackedStateSlots.Pants1;

	// Token: 0x0400176F RID: 5999
	private int lastReportedState;

	// Token: 0x04001770 RID: 6000
	private VRRig ownerRig;

	// Token: 0x04001771 RID: 6001
	private GTBitOps.BitWriteInfo stateBitsWriteInfo;
}
