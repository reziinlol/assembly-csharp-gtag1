using System;

// Token: 0x02000274 RID: 628
internal class PropHuntGameModeRPCs : RPCNetworkBase
{
	// Token: 0x060010F9 RID: 4345 RVA: 0x0005AB59 File Offset: 0x00058D59
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.propHuntManager = (GorillaPropHuntGameManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x0400142E RID: 5166
	private GameModeSerializer serializer;

	// Token: 0x0400142F RID: 5167
	private GorillaPropHuntGameManager propHuntManager;
}
