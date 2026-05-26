using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class BootscreenPositioner : MonoBehaviour
{
	// Token: 0x0600009C RID: 156 RVA: 0x00005194 File Offset: 0x00003394
	private void Awake()
	{
		base.transform.position = this._pov.position;
		base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x0600009D RID: 157 RVA: 0x000051EC File Offset: 0x000033EC
	private void LateUpdate()
	{
		if (Vector3.Distance(base.transform.position, this._pov.position) > this._distanceThreshold)
		{
			base.transform.position = this._pov.position;
		}
		if (Mathf.Abs(this._pov.rotation.eulerAngles.y - base.transform.rotation.eulerAngles.y) > this._rotationThreshold)
		{
			base.transform.rotation = Quaternion.Euler(0f, this._pov.rotation.eulerAngles.y, 0f);
		}
	}

	// Token: 0x040000BD RID: 189
	[SerializeField]
	private Transform _pov;

	// Token: 0x040000BE RID: 190
	[SerializeField]
	private float _distanceThreshold;

	// Token: 0x040000BF RID: 191
	[SerializeField]
	private float _rotationThreshold;
}
