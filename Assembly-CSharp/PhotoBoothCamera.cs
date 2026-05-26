using System;
using System.Collections.Generic;
using System.IO;
using GorillaNetworking;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Token: 0x0200048F RID: 1167
public class PhotoBoothCamera : MonoBehaviour
{
	// Token: 0x06001C55 RID: 7253 RVA: 0x00099966 File Offset: 0x00097B66
	public void SetSaveImageToDevice(bool b)
	{
		this.saveImageToDevice = b;
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x0009996F File Offset: 0x00097B6F
	public void Clear()
	{
		this.rt.Clear();
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x0009997C File Offset: 0x00097B7C
	public void Capture(float FOV)
	{
		this.cam.fieldOfView = FOV;
		this.cam.Render();
		this.rt.Add(new RenderTexture(this.renderTexture.width, this.renderTexture.height, 1));
		Graphics.Blit(this.renderTexture, this.rt[this.rt.Count - 1]);
		this.OnCapture(this.rt[this.rt.Count - 1], this.rt.Count - 1);
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x00099A1C File Offset: 0x00097C1C
	public void Print()
	{
		if (this.saveImageToDevice)
		{
			string fileName = this.saveName;
			if (this.appendDateToFile)
			{
				DateTime dateTime = DateTime.UtcNow;
				if (GorillaComputer.instance != null)
				{
					dateTime = GorillaComputer.instance.GetServerTime();
				}
				fileName += dateTime.ToString("yyyyMMddHHmmss");
			}
			RenderTexture print = new RenderTexture(this.renderTexture.width, this.renderTexture.height * this.rt.Count, 1);
			for (int i = 0; i < this.rt.Count; i++)
			{
				Graphics.CopyTexture(this.rt[i], 0, 0, 0, 0, this.rt[i].width, this.rt[i].height, print, 0, 0, 0, this.rt[i].height * i);
			}
			NativeArray<byte> narray = new NativeArray<byte>(print.width * print.height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			AsyncGPUReadback.RequestIntoNativeArray<byte>(ref narray, print, 0, delegate(AsyncGPUReadbackRequest request)
			{
				if (!request.hasError)
				{
					this.SaveImage(print, narray, fileName, this.imageDescription);
				}
				narray.Dispose();
			});
		}
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x00099B70 File Offset: 0x00097D70
	private void SaveImage(RenderTexture rt, NativeArray<byte> narray, string fileName, string desc)
	{
		NativeArray<byte> nativeArray = ImageConversion.EncodeNativeArrayToJPG<byte>(narray, rt.graphicsFormat, (uint)rt.width, (uint)rt.height, 0U, 75);
		File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName + ".jpg"), nativeArray.ToArray());
		nativeArray.Dispose();
	}

	// Token: 0x04002669 RID: 9833
	[SerializeField]
	private Camera cam;

	// Token: 0x0400266A RID: 9834
	[SerializeField]
	private RenderTexture renderTexture;

	// Token: 0x0400266B RID: 9835
	[SerializeField]
	private TMP_Text imageLabel;

	// Token: 0x0400266C RID: 9836
	[SerializeField]
	private Image imageImage;

	// Token: 0x0400266D RID: 9837
	[SerializeField]
	private string saveName = "img";

	// Token: 0x0400266E RID: 9838
	[SerializeField]
	private bool appendDateToFile;

	// Token: 0x0400266F RID: 9839
	[SerializeField]
	private string imageDescription = "";

	// Token: 0x04002670 RID: 9840
	private List<RenderTexture> rt = new List<RenderTexture>();

	// Token: 0x04002671 RID: 9841
	public Action<Texture, int> OnCapture;

	// Token: 0x04002672 RID: 9842
	private bool saveImageToDevice;
}
