using System;

// Token: 0x02000609 RID: 1545
public class BuilderPieceInteractorFindNearby : MonoBehaviourPostTick
{
	// Token: 0x0600268C RID: 9868 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x000CC39E File Offset: 0x000CA59E
	public override void PostTick()
	{
		if (this.pieceInteractor != null)
		{
			this.pieceInteractor.StartFindNearbyPieces();
		}
	}

	// Token: 0x04003202 RID: 12802
	public BuilderPieceInteractor pieceInteractor;
}
