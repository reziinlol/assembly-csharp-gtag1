using System;
using PlayFab.Internal;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001ED RID: 493
public class VoxelActions : SingletonMonoBehaviour<VoxelActions>
{
	// Token: 0x06000CEC RID: 3308 RVA: 0x00046B5C File Offset: 0x00044D5C
	public void PlayDigFX(Vector3 position, Vector3 normal, int dirtAmount, int stoneAmount)
	{
		if (dirtAmount > 0)
		{
			Object.Instantiate<GameObject>((dirtAmount >= 20) ? this._dirtDigBigFX : this._dirtDigFX, position, Quaternion.LookRotation(normal));
		}
		if (stoneAmount > 0)
		{
			Object.Instantiate<GameObject>((dirtAmount >= 20) ? this._stoneDigBigFX : this._stoneDigFX, position, Quaternion.LookRotation(normal));
		}
	}

	// Token: 0x04000F8E RID: 3982
	[SerializeField]
	private GameObject _hitFX;

	// Token: 0x04000F8F RID: 3983
	[FormerlySerializedAs("_digFX")]
	[SerializeField]
	private GameObject _dirtDigFX;

	// Token: 0x04000F90 RID: 3984
	[FormerlySerializedAs("_bigDigFX")]
	[SerializeField]
	private GameObject _dirtDigBigFX;

	// Token: 0x04000F91 RID: 3985
	[SerializeField]
	private GameObject _stoneDigFX;

	// Token: 0x04000F92 RID: 3986
	[SerializeField]
	private GameObject _stoneDigBigFX;
}
