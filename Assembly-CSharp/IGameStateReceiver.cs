using System;

// Token: 0x02000299 RID: 665
public interface IGameStateReceiver
{
	// Token: 0x06001188 RID: 4488
	void GameStateReceiverOnStateChanged(long oldState, long newState);
}
