using System;

// Token: 0x020006AC RID: 1708
public interface IGameEntityCustomStateChange
{
	// Token: 0x06002A97 RID: 10903
	bool CanChangeState(long newState, int playerId);
}
