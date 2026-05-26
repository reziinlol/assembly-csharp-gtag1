using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class GhostLabButton : GorillaPressableButton, IBuildValidation
{
	// Token: 0x06000C0B RID: 3083 RVA: 0x00041760 File Offset: 0x0003F960
	public bool BuildValidationCheck()
	{
		if (this.ghostLab == null)
		{
			Debug.LogError("ghostlab is missing", this);
			return false;
		}
		return true;
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x0004177E File Offset: 0x0003F97E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.ghostLab.DoorButtonPress(this.buttonIndex, this.forSingleDoor);
	}

	// Token: 0x04000EAB RID: 3755
	public GhostLab ghostLab;

	// Token: 0x04000EAC RID: 3756
	public int buttonIndex;

	// Token: 0x04000EAD RID: 3757
	public bool forSingleDoor;
}
