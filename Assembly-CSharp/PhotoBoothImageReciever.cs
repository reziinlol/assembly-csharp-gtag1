using System;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class PhotoBoothImageReciever : MonoBehaviour
{
	// Token: 0x06001C5D RID: 7261 RVA: 0x00099C28 File Offset: 0x00097E28
	private void OnEnable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Combine(photoBoothCamera.OnCapture, new Action<Texture, int>(this.photoBoothCamera_OnCapture));
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x00099C51 File Offset: 0x00097E51
	private void photoBoothCamera_OnCapture(Texture texture, int i)
	{
		if (this.index < 0 || this.index == i)
		{
			base.GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x00099C76 File Offset: 0x00097E76
	private void OnDisable()
	{
		PhotoBoothCamera photoBoothCamera = this.photoBoothCamera;
		photoBoothCamera.OnCapture = (Action<Texture, int>)Delegate.Remove(photoBoothCamera.OnCapture, new Action<Texture, int>(this.photoBoothCamera_OnCapture));
	}

	// Token: 0x04002677 RID: 9847
	[SerializeField]
	private PhotoBoothCamera photoBoothCamera;

	// Token: 0x04002678 RID: 9848
	[SerializeField]
	private int index = -1;
}
