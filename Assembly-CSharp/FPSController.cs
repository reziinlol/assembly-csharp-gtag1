using System;
using UnityEngine;

// Token: 0x02000D10 RID: 3344
public class FPSController : MonoBehaviour
{
	// Token: 0x14000092 RID: 146
	// (add) Token: 0x060052B7 RID: 21175 RVA: 0x001B235C File Offset: 0x001B055C
	// (remove) Token: 0x060052B8 RID: 21176 RVA: 0x001B2394 File Offset: 0x001B0594
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x14000093 RID: 147
	// (add) Token: 0x060052B9 RID: 21177 RVA: 0x001B23CC File Offset: 0x001B05CC
	// (remove) Token: 0x060052BA RID: 21178 RVA: 0x001B2404 File Offset: 0x001B0604
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x040063DD RID: 25565
	public float baseMoveSpeed = 4f;

	// Token: 0x040063DE RID: 25566
	public float shiftMoveSpeed = 8f;

	// Token: 0x040063DF RID: 25567
	public float ctrlMoveSpeed = 1f;

	// Token: 0x040063E0 RID: 25568
	public float lookHorizontal = 0.4f;

	// Token: 0x040063E1 RID: 25569
	public float lookVertical = 0.25f;

	// Token: 0x040063E2 RID: 25570
	[SerializeField]
	private Vector3 leftControllerPosOffset = new Vector3(-0.2f, -0.25f, 0.3f);

	// Token: 0x040063E3 RID: 25571
	[SerializeField]
	private Vector3 leftControllerRotationOffset = new Vector3(265f, -82f, 28f);

	// Token: 0x040063E4 RID: 25572
	[SerializeField]
	private Vector3 rightControllerPosOffset = new Vector3(0.2f, -0.25f, 0.3f);

	// Token: 0x040063E5 RID: 25573
	[SerializeField]
	private Vector3 rightControllerRotationOffset = new Vector3(263f, 318f, 485f);

	// Token: 0x040063E6 RID: 25574
	[SerializeField]
	private Vector3 noclipLeftControllerPosOffset = new Vector3(-0.3f, -0.1f, 0.65f);

	// Token: 0x040063E7 RID: 25575
	[SerializeField]
	private Vector3 noclipLeftControllerRotationOffset = new Vector3(180f, -90f, -90f);

	// Token: 0x040063E8 RID: 25576
	[SerializeField]
	private Vector3 noclipRightControllerPosOffset = new Vector3(0.3f, -0.1f, 0.65f);

	// Token: 0x040063E9 RID: 25577
	[SerializeField]
	private Vector3 noclipRightControllerRotationOffset = new Vector3(0f, -90f, -90f);

	// Token: 0x040063EA RID: 25578
	[SerializeField]
	private bool toggleGrab;

	// Token: 0x040063EB RID: 25579
	[SerializeField]
	private bool clampGrab;

	// Token: 0x040063EE RID: 25582
	private bool controlRightHand;

	// Token: 0x040063EF RID: 25583
	public LayerMask HandMask;

	// Token: 0x02000D11 RID: 3345
	// (Invoke) Token: 0x060052BD RID: 21181
	public delegate void OnStateChangeEventHandler();
}
