using System;

// Token: 0x02000629 RID: 1577
public interface IBuilderPieceFunctional
{
	// Token: 0x06002741 RID: 10049
	void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x06002742 RID: 10050
	void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x06002743 RID: 10051
	bool IsStateValid(byte state);

	// Token: 0x06002744 RID: 10052
	void FunctionalPieceUpdate();

	// Token: 0x06002745 RID: 10053 RVA: 0x00002AF8 File Offset: 0x00000CF8
	void FunctionalPieceFixedUpdate()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x000D0353 File Offset: 0x000CE553
	float GetInteractionDistace()
	{
		return 2.5f;
	}
}
