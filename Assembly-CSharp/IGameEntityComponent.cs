using System;

// Token: 0x020006AB RID: 1707
public interface IGameEntityComponent
{
	// Token: 0x06002A94 RID: 10900
	void OnEntityInit();

	// Token: 0x06002A95 RID: 10901
	void OnEntityDestroy();

	// Token: 0x06002A96 RID: 10902
	void OnEntityStateChange(long prevState, long newState);
}
