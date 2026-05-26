using System;

// Token: 0x02000298 RID: 664
public interface IGameStateProvider
{
	// Token: 0x06001186 RID: 4486
	void GameStateReceiverRegister(IGameStateReceiver receiver);

	// Token: 0x06001187 RID: 4487
	void GameStateReceiverUnregister(IGameStateReceiver receiver);
}
