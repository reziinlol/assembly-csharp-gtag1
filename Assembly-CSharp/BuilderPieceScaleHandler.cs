using System;
using System.Collections.Generic;
using GorillaTagScripts.Builder;
using UnityEngine;

// Token: 0x0200060B RID: 1547
public class BuilderPieceScaleHandler : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x06002692 RID: 9874 RVA: 0x000CC3BC File Offset: 0x000CA5BC
	public void OnPieceCreate(int pieceType, int pieceId)
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.SetScale(this.myPiece.GetScale());
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.SetScale(this.myPiece.GetScale());
		}
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x000CC464 File Offset: 0x000CA664
	public void OnPieceDestroy()
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.RevertScale();
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.RevertScale();
		}
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPieceActivate()
	{
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPieceDeactivate()
	{
	}

	// Token: 0x04003204 RID: 12804
	[SerializeField]
	private BuilderPiece myPiece;

	// Token: 0x04003205 RID: 12805
	[SerializeField]
	private List<BuilderScaleAudioRadius> audioScalers = new List<BuilderScaleAudioRadius>();

	// Token: 0x04003206 RID: 12806
	[SerializeField]
	private List<BuilderScaleParticles> particleScalers = new List<BuilderScaleParticles>();
}
