using System;
using System.Collections;
using System.Threading;
using UnityEngine;

// Token: 0x02000855 RID: 2133
public class GorillaDayNight : MonoBehaviour
{
	// Token: 0x06003732 RID: 14130 RVA: 0x0012ED3C File Offset: 0x0012CF3C
	public void Awake()
	{
		if (GorillaDayNight.instance == null)
		{
			GorillaDayNight.instance = this;
		}
		else if (GorillaDayNight.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.test = false;
		this.working = false;
		this.lerpValue = 0.5f;
		this.workingLightMapDatas = new LightmapData[3];
		this.workingLightMapData = new LightmapData();
		this.workingLightMapData.lightmapColor = this.lightmapDatas[0].lightTextures[0];
		this.workingLightMapData.lightmapDir = this.lightmapDatas[0].dirTextures[0];
	}

	// Token: 0x06003733 RID: 14131 RVA: 0x0012EDE0 File Offset: 0x0012CFE0
	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

	// Token: 0x06003734 RID: 14132 RVA: 0x0012EE10 File Offset: 0x0012D010
	public void DoWork()
	{
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		this.done = true;
	}

	// Token: 0x06003735 RID: 14133 RVA: 0x0012F01C File Offset: 0x0012D21C
	public void DoLightsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x0012F0E0 File Offset: 0x0012D2E0
	public void DoDirsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x0012F1A3 File Offset: 0x0012D3A3
	private IEnumerator LightMapSet(int setFirstData, int setSecondData, float setLerp)
	{
		this.working = true;
		this.firstData = setFirstData;
		this.secondData = setSecondData;
		this.lerpValue = setLerp;
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.lightsThread = new Thread(new ThreadStart(this.DoLightsStep));
			this.lightsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapColor.Apply(false);
			this.dirsThread = new Thread(new ThreadStart(this.DoDirsStep));
			this.dirsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		LightmapSettings.lightmaps = this.workingLightMapDatas;
		this.working = false;
		this.done = true;
		yield break;
	}

	// Token: 0x0400474B RID: 18251
	[OnEnterPlay_SetNull]
	public static volatile GorillaDayNight instance;

	// Token: 0x0400474C RID: 18252
	public GorillaLightmapData[] lightmapDatas;

	// Token: 0x0400474D RID: 18253
	private LightmapData[] workingLightMapDatas;

	// Token: 0x0400474E RID: 18254
	private LightmapData workingLightMapData;

	// Token: 0x0400474F RID: 18255
	public float lerpValue;

	// Token: 0x04004750 RID: 18256
	public bool done;

	// Token: 0x04004751 RID: 18257
	public bool finishedStep;

	// Token: 0x04004752 RID: 18258
	private Color[] fromPixels;

	// Token: 0x04004753 RID: 18259
	private Color[] toPixels;

	// Token: 0x04004754 RID: 18260
	private Color[] mixedPixels;

	// Token: 0x04004755 RID: 18261
	public int firstData;

	// Token: 0x04004756 RID: 18262
	public int secondData;

	// Token: 0x04004757 RID: 18263
	public int i;

	// Token: 0x04004758 RID: 18264
	public int j;

	// Token: 0x04004759 RID: 18265
	public int k;

	// Token: 0x0400475A RID: 18266
	public int l;

	// Token: 0x0400475B RID: 18267
	private Thread lightsThread;

	// Token: 0x0400475C RID: 18268
	private Thread dirsThread;

	// Token: 0x0400475D RID: 18269
	public bool test;

	// Token: 0x0400475E RID: 18270
	public bool working;
}
