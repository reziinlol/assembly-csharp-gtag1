using System;
using System.Collections;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x02000A30 RID: 2608
public class CustomMapsLoadRoomMapButton : GorillaPressableButton
{
	// Token: 0x060042B0 RID: 17072 RVA: 0x001647AF File Offset: 0x001629AF
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (CustomMapManager.CanLoadRoomMap())
		{
			CustomMapManager.ApproveAndLoadRoomMap();
		}
	}

	// Token: 0x060042B1 RID: 17073 RVA: 0x001647D0 File Offset: 0x001629D0
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x040054A3 RID: 21667
	[SerializeField]
	private float pressedTime = 0.2f;
}
