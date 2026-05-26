using System;
using UnityEngine;

// Token: 0x02000B38 RID: 2872
public class KIDHandReference : MonoBehaviour
{
	// Token: 0x170006D9 RID: 1753
	// (get) Token: 0x060048B8 RID: 18616 RVA: 0x00184735 File Offset: 0x00182935
	public static GameObject LeftHand
	{
		get
		{
			return KIDHandReference._leftHandRef;
		}
	}

	// Token: 0x170006DA RID: 1754
	// (get) Token: 0x060048B9 RID: 18617 RVA: 0x0018473C File Offset: 0x0018293C
	public static GameObject RightHand
	{
		get
		{
			return KIDHandReference._rightHandRef;
		}
	}

	// Token: 0x060048BA RID: 18618 RVA: 0x00184743 File Offset: 0x00182943
	private void Awake()
	{
		KIDHandReference._leftHandRef = this._leftHand;
		KIDHandReference._rightHandRef = this._rightHand;
	}

	// Token: 0x04005B2A RID: 23338
	[SerializeField]
	private GameObject _leftHand;

	// Token: 0x04005B2B RID: 23339
	[SerializeField]
	private GameObject _rightHand;

	// Token: 0x04005B2C RID: 23340
	private static GameObject _leftHandRef;

	// Token: 0x04005B2D RID: 23341
	private static GameObject _rightHandRef;
}
