using System;
using UnityEngine;

// Token: 0x0200090A RID: 2314
public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x1400006F RID: 111
	// (add) Token: 0x06003C7D RID: 15485 RVA: 0x00149BC4 File Offset: 0x00147DC4
	// (remove) Token: 0x06003C7E RID: 15486 RVA: 0x00149BFC File Offset: 0x00147DFC
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	// Token: 0x14000070 RID: 112
	// (add) Token: 0x06003C7F RID: 15487 RVA: 0x00149C34 File Offset: 0x00147E34
	// (remove) Token: 0x06003C80 RID: 15488 RVA: 0x00149C6C File Offset: 0x00147E6C
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	// Token: 0x06003C81 RID: 15489 RVA: 0x00149CA1 File Offset: 0x00147EA1
	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06003C82 RID: 15490 RVA: 0x00149CAF File Offset: 0x00147EAF
	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	// Token: 0x06003C83 RID: 15491 RVA: 0x00149CC5 File Offset: 0x00147EC5
	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	// Token: 0x06003C84 RID: 15492 RVA: 0x00149CDB File Offset: 0x00147EDB
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	// Token: 0x06003C85 RID: 15493 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x06003C86 RID: 15494 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPieceDestroy()
	{
	}

	// Token: 0x06003C87 RID: 15495 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06003C88 RID: 15496 RVA: 0x00149CE9 File Offset: 0x00147EE9
	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x06003C89 RID: 15497 RVA: 0x00149CE9 File Offset: 0x00147EE9
	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x04004D2D RID: 19757
	private Collider myCollider;

	// Token: 0x04004D30 RID: 19760
	public bool builderEnterTrigger;

	// Token: 0x04004D31 RID: 19761
	public bool builderExitOnEnterTrigger;

	// Token: 0x0200090B RID: 2315
	// (Invoke) Token: 0x06003C8C RID: 15500
	public delegate void SizeChangerTriggerEvent(Collider other);
}
