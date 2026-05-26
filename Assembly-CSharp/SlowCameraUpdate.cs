using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009CF RID: 2511
public class SlowCameraUpdate : MonoBehaviour
{
	// Token: 0x0600404E RID: 16462 RVA: 0x00157E21 File Offset: 0x00156021
	public void Awake()
	{
		this.frameRate = 30f;
		this.timeToNextFrame = 1f / this.frameRate;
		this.myCamera = base.GetComponent<Camera>();
	}

	// Token: 0x0600404F RID: 16463 RVA: 0x00157E4C File Offset: 0x0015604C
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateMirror());
	}

	// Token: 0x06004050 RID: 16464 RVA: 0x00005511 File Offset: 0x00003711
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06004051 RID: 16465 RVA: 0x00157E5B File Offset: 0x0015605B
	public IEnumerator UpdateMirror()
	{
		for (;;)
		{
			if (base.gameObject.activeSelf)
			{
				Debug.Log("rendering camera!");
				this.myCamera.Render();
			}
			yield return new WaitForSeconds(this.timeToNextFrame);
		}
		yield break;
	}

	// Token: 0x040050DE RID: 20702
	private Camera myCamera;

	// Token: 0x040050DF RID: 20703
	private float frameRate;

	// Token: 0x040050E0 RID: 20704
	private float timeToNextFrame;
}
