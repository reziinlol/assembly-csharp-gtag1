using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class HideFirstFrame : MonoBehaviour
{
	// Token: 0x0600009F RID: 159 RVA: 0x000052A2 File Offset: 0x000034A2
	private void Awake()
	{
		this._cam = base.GetComponent<Camera>();
		this._farClipPlane = this._cam.farClipPlane;
		this._cam.farClipPlane = this._cam.nearClipPlane + 0.1f;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000052DD File Offset: 0x000034DD
	public IEnumerator Start()
	{
		int num;
		for (int i = 0; i < this._frameDelay; i = num + 1)
		{
			yield return null;
			num = i;
		}
		this._cam.farClipPlane = this._farClipPlane;
		yield break;
	}

	// Token: 0x040000C0 RID: 192
	[SerializeField]
	private int _frameDelay = 1;

	// Token: 0x040000C1 RID: 193
	private Camera _cam;

	// Token: 0x040000C2 RID: 194
	private float _farClipPlane;
}
