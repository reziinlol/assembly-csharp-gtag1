using System;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class CosmeticCritterButterfly : CosmeticCritter
{
	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060004B1 RID: 1201 RVA: 0x0001A487 File Offset: 0x00018687
	public ParticleSystem.EmitParams GetEmitParams
	{
		get
		{
			return this.emitParams;
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001A48F File Offset: 0x0001868F
	public void SetStartPos(Vector3 initialPos)
	{
		this.startPosition = initialPos;
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001A498 File Offset: 0x00018698
	public override void SetRandomVariables()
	{
		this.direction = Random.insideUnitSphere;
		this.emitParams.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
		this.particleSystem.Emit(this.emitParams, 1);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001A4F5 File Offset: 0x000186F5
	public override void Tick()
	{
		base.transform.position = this.startPosition + (float)base.GetAliveTime() * this.speed * this.direction;
	}

	// Token: 0x0400052C RID: 1324
	[Tooltip("The speed this Butterfly will move at.")]
	[SerializeField]
	private float speed = 1f;

	// Token: 0x0400052D RID: 1325
	[Tooltip("Emit one particle from this particle system when spawning.")]
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x0400052E RID: 1326
	private Vector3 startPosition;

	// Token: 0x0400052F RID: 1327
	private Vector3 direction;

	// Token: 0x04000530 RID: 1328
	private ParticleSystem.EmitParams emitParams;
}
