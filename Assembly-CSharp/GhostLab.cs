using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class GhostLab : MonoBehaviourTick, IBuildValidation
{
	// Token: 0x06000C02 RID: 3074 RVA: 0x00041392 File Offset: 0x0003F592
	private void Awake()
	{
		this.relState = Object.FindFirstObjectByType<GhostLabReliableState>();
		this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		this.doorOpen = new bool[this.relState.singleDoorCount];
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00023994 File Offset: 0x00021B94
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x000413BC File Offset: 0x0003F5BC
	public void DoorButtonPress(int buttonIndex, bool forSingleDoor)
	{
		if (!forSingleDoor)
		{
			this.UpdateEntranceDoorsState(buttonIndex);
			return;
		}
		this.UpdateDoorState(buttonIndex);
		this.relState.UpdateSingleDoorState(buttonIndex);
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x000413DC File Offset: 0x0003F5DC
	public void UpdateDoorState(int buttonIndex)
	{
		if ((this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == this.singleDoorTravelDistance) || (!this.doorOpen[buttonIndex] && this.slidingDoor[buttonIndex].localPosition == Vector3.zero))
		{
			this.doorOpen[buttonIndex] = !this.doorOpen[buttonIndex];
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00041444 File Offset: 0x0003F644
	public void UpdateEntranceDoorsState(int buttonIndex)
	{
		if (this.outerDoor == null || this.innerDoor == null)
		{
			return;
		}
		if (this.doorState == GhostLab.EntranceDoorsState.BothClosed)
		{
			if (!(this.innerDoor.localPosition != Vector3.zero) && !(this.outerDoor.localPosition != Vector3.zero))
			{
				if (buttonIndex == 0 || buttonIndex == 1)
				{
					this.doorState = GhostLab.EntranceDoorsState.OuterDoorOpen;
				}
				if (buttonIndex == 2 || buttonIndex == 3)
				{
					this.doorState = GhostLab.EntranceDoorsState.InnerDoorOpen;
				}
			}
		}
		else if (this.innerDoor.localPosition == this.doorTravelDistance || this.outerDoor.localPosition == this.doorTravelDistance)
		{
			this.doorState = GhostLab.EntranceDoorsState.BothClosed;
		}
		this.relState.UpdateEntranceDoorsState(this.doorState);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0004150C File Offset: 0x0003F70C
	public override void Tick()
	{
		this.SynchStates();
		if (this.innerDoor != null && this.outerDoor != null)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			switch (this.doorState)
			{
			case GhostLab.EntranceDoorsState.InnerDoorOpen:
				zero2 = this.doorTravelDistance;
				break;
			case GhostLab.EntranceDoorsState.OuterDoorOpen:
				zero = this.doorTravelDistance;
				break;
			}
			this.outerDoor.localPosition = Vector3.MoveTowards(this.outerDoor.localPosition, zero, this.doorMoveSpeed * Time.deltaTime);
			this.innerDoor.localPosition = Vector3.MoveTowards(this.innerDoor.localPosition, zero2, this.doorMoveSpeed * Time.deltaTime);
		}
		Vector3 zero3 = Vector3.zero;
		for (int i = 0; i < this.slidingDoor.Length; i++)
		{
			if (this.doorOpen[i])
			{
				zero3 = this.singleDoorTravelDistance;
			}
			else
			{
				zero3 = Vector3.zero;
			}
			this.slidingDoor[i].localPosition = Vector3.MoveTowards(this.slidingDoor[i].localPosition, zero3, this.singleDoorMoveSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00041630 File Offset: 0x0003F830
	private void SynchStates()
	{
		this.doorState = this.relState.doorState;
		for (int i = 0; i < this.doorOpen.Length; i++)
		{
			this.doorOpen[i] = this.relState.singleDoorOpen[i];
		}
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x00041678 File Offset: 0x0003F878
	public bool IsDoorMoving(bool singleDoor, int index)
	{
		if (singleDoor)
		{
			return (this.doorOpen[index] && this.slidingDoor[index].localPosition != this.singleDoorTravelDistance) || (!this.doorOpen[index] && this.slidingDoor[index].localPosition != Vector3.zero);
		}
		if (index == 0 || index == 1)
		{
			return (this.doorState == GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.OuterDoorOpen && this.outerDoor.localPosition != Vector3.zero);
		}
		return (this.doorState == GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != this.doorTravelDistance) || (this.doorState != GhostLab.EntranceDoorsState.InnerDoorOpen && this.innerDoor.localPosition != Vector3.zero);
	}

	// Token: 0x04000E9D RID: 3741
	public Transform outerDoor;

	// Token: 0x04000E9E RID: 3742
	public Transform innerDoor;

	// Token: 0x04000E9F RID: 3743
	public Vector3 doorTravelDistance;

	// Token: 0x04000EA0 RID: 3744
	public float doorMoveSpeed;

	// Token: 0x04000EA1 RID: 3745
	public float singleDoorMoveSpeed;

	// Token: 0x04000EA2 RID: 3746
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x04000EA3 RID: 3747
	public GhostLabReliableState relState;

	// Token: 0x04000EA4 RID: 3748
	public Transform[] slidingDoor;

	// Token: 0x04000EA5 RID: 3749
	public Vector3 singleDoorTravelDistance;

	// Token: 0x04000EA6 RID: 3750
	private bool[] doorOpen;

	// Token: 0x020001C6 RID: 454
	public enum EntranceDoorsState
	{
		// Token: 0x04000EA8 RID: 3752
		BothClosed,
		// Token: 0x04000EA9 RID: 3753
		InnerDoorOpen,
		// Token: 0x04000EAA RID: 3754
		OuterDoorOpen
	}
}
