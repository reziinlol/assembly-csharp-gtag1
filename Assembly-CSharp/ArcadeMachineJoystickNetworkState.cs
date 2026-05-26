using System;
using Fusion;
using Photon.Pun;

// Token: 0x0200000F RID: 15
[NetworkBehaviourWeaved(0)]
public class ArcadeMachineJoystickNetworkState : NetworkComponent
{
	// Token: 0x0600003F RID: 63 RVA: 0x00002AEA File Offset: 0x00000CEA
	private new void Awake()
	{
		this.joystick = base.GetComponent<ArcadeMachineJoystick>();
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public override void ReadDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public override void WriteDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000042 RID: 66 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000043 RID: 67 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000027 RID: 39
	private ArcadeMachineJoystick joystick;
}
