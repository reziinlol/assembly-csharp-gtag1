using System;

// Token: 0x02000628 RID: 1576
public interface IBuilderPieceComponent
{
	// Token: 0x0600273C RID: 10044
	void OnPieceCreate(int pieceType, int pieceId);

	// Token: 0x0600273D RID: 10045
	void OnPieceDestroy();

	// Token: 0x0600273E RID: 10046
	void OnPiecePlacementDeserialized();

	// Token: 0x0600273F RID: 10047
	void OnPieceActivate();

	// Token: 0x06002740 RID: 10048
	void OnPieceDeactivate();
}
