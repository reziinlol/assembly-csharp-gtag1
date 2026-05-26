using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x020009FA RID: 2554
public class DayNightCycle : MonoBehaviour
{
	// Token: 0x0600415A RID: 16730 RVA: 0x0015D9D0 File Offset: 0x0015BBD0
	public void Awake()
	{
		this.fromMap = new Texture2D(this._sunriseMap.width, this._sunriseMap.height);
		this.fromMap = LightmapSettings.lightmaps[0].lightmapColor;
		this.toMap = new Texture2D(this._dayMap.width, this._dayMap.height);
		this.toMap.SetPixels(this._dayMap.GetPixels());
		this.toMap.Apply();
		this.workBlockMix = new Color[this.subTextureSize * this.subTextureSize];
		this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height, this.fromMap.graphicsFormat, TextureCreationFlags.None);
		this.newData = new LightmapData();
		this.textureHeight = this.fromMap.height;
		this.textureWidth = this.fromMap.width;
		this.subTextureArray = new Texture2D[(int)Mathf.Pow((float)(this.textureHeight / this.subTextureSize), 2f)];
		Debug.Log("aaaa " + this.fromMap.format.ToString());
		Debug.Log("aaaa " + this.fromMap.graphicsFormat.ToString());
		this.startJob = false;
		this.startCoroutine = false;
		this.startedCoroutine = false;
		this.finishedCoroutine = false;
	}

	// Token: 0x0600415B RID: 16731 RVA: 0x0015DB54 File Offset: 0x0015BD54
	public void Update()
	{
		if (this.startJob)
		{
			this.startJob = false;
			this.startTime = Time.realtimeSinceStartup;
			base.StartCoroutine(this.UpdateWork());
			this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
		}
		if (this.jobStarted && this.jobHandle.IsCompleted)
		{
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.jobHandle.Complete();
			this.jobStarted = false;
			this.newTexture.SetPixels(this.job.mixedPixels.ToArray());
			this.newData.lightmapDir = LightmapSettings.lightmaps[0].lightmapDir;
			LightmapSettings.lightmaps = new LightmapData[]
			{
				this.newData
			};
			this.job.fromPixels.Dispose();
			this.job.toPixels.Dispose();
			this.job.mixedPixels.Dispose();
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
		if (this.startCoroutine)
		{
			this.startCoroutine = false;
			this.startTime = Time.realtimeSinceStartup;
			this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height);
			base.StartCoroutine(this.UpdateWork());
		}
		if (this.startedCoroutine && this.finishedCoroutine)
		{
			this.startedCoroutine = false;
			this.finishedCoroutine = false;
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.newData = LightmapSettings.lightmaps[0];
			this.newData.lightmapColor = this.fromMap;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			lightmaps[0].lightmapColor = this.fromMap;
			LightmapSettings.lightmaps = lightmaps;
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
	}

	// Token: 0x0600415C RID: 16732 RVA: 0x0015DD42 File Offset: 0x0015BF42
	public IEnumerator UpdateWork()
	{
		yield return 0;
		this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
		this.startTime = Time.realtimeSinceStartup;
		this.startedCoroutine = true;
		this.currentSubTexture = 0;
		int num;
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.subTextureArray[i] = new Texture2D(this.subTextureSize, this.subTextureSize, this.fromMap.graphicsFormat, TextureCreationFlags.None);
			yield return 0;
			num = i;
		}
		for (int i = 0; i < this.textureWidth / this.subTextureSize; i = num + 1)
		{
			this.currentColumn = i;
			for (int j = 0; j < this.textureHeight / this.subTextureSize; j = num + 1)
			{
				this.currentRow = j;
				this.workBlockFrom = this.fromMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				this.workBlockTo = this.toMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				for (int k = 0; k < this.subTextureSize * this.subTextureSize - 1; k++)
				{
					this.workBlockMix[k] = Color.Lerp(this.workBlockFrom[k], this.workBlockTo[k], this.lerpAmount);
				}
				this.subTextureArray[j * (this.textureWidth / this.subTextureSize) + i].SetPixels(0, 0, this.subTextureSize, this.subTextureSize, this.workBlockMix);
				yield return 0;
				num = j;
			}
			num = i;
		}
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.currentSubTexture = i;
			this.subTextureArray[i].Apply();
			yield return 0;
			Graphics.CopyTexture(this.subTextureArray[i], 0, 0, 0, 0, this.subTextureSize, this.subTextureSize, this.newTexture, 0, 0, i * this.subTextureSize % this.textureHeight, (int)Mathf.Floor((float)(this.subTextureSize * i / this.textureHeight)) * this.subTextureSize);
			yield return 0;
			num = i;
		}
		this.finishedCoroutine = true;
		yield break;
	}

	// Token: 0x040052E0 RID: 21216
	public Texture2D _dayMap;

	// Token: 0x040052E1 RID: 21217
	private Texture2D fromMap;

	// Token: 0x040052E2 RID: 21218
	public Texture2D _sunriseMap;

	// Token: 0x040052E3 RID: 21219
	private Texture2D toMap;

	// Token: 0x040052E4 RID: 21220
	public DayNightCycle.LerpBakedLightingJob job;

	// Token: 0x040052E5 RID: 21221
	public JobHandle jobHandle;

	// Token: 0x040052E6 RID: 21222
	public bool isComplete;

	// Token: 0x040052E7 RID: 21223
	private float startTime;

	// Token: 0x040052E8 RID: 21224
	public float timeTakenStartingJob;

	// Token: 0x040052E9 RID: 21225
	public float timeTakenPostJob;

	// Token: 0x040052EA RID: 21226
	public float timeTakenDuringJob;

	// Token: 0x040052EB RID: 21227
	public LightmapData newData;

	// Token: 0x040052EC RID: 21228
	private Color[] fromPixels;

	// Token: 0x040052ED RID: 21229
	private Color[] toPixels;

	// Token: 0x040052EE RID: 21230
	private Color[] mixedPixels;

	// Token: 0x040052EF RID: 21231
	private LightmapData[] newDatas;

	// Token: 0x040052F0 RID: 21232
	public Texture2D newTexture;

	// Token: 0x040052F1 RID: 21233
	public int textureWidth;

	// Token: 0x040052F2 RID: 21234
	public int textureHeight;

	// Token: 0x040052F3 RID: 21235
	private Color[] workBlockFrom;

	// Token: 0x040052F4 RID: 21236
	private Color[] workBlockTo;

	// Token: 0x040052F5 RID: 21237
	private Color[] workBlockMix;

	// Token: 0x040052F6 RID: 21238
	public int subTextureSize = 1024;

	// Token: 0x040052F7 RID: 21239
	public Texture2D[] subTextureArray;

	// Token: 0x040052F8 RID: 21240
	public bool startCoroutine;

	// Token: 0x040052F9 RID: 21241
	public bool startedCoroutine;

	// Token: 0x040052FA RID: 21242
	public bool finishedCoroutine;

	// Token: 0x040052FB RID: 21243
	public bool startJob;

	// Token: 0x040052FC RID: 21244
	public float switchTimeTaken;

	// Token: 0x040052FD RID: 21245
	public bool jobStarted;

	// Token: 0x040052FE RID: 21246
	public float lerpAmount;

	// Token: 0x040052FF RID: 21247
	public int currentRow;

	// Token: 0x04005300 RID: 21248
	public int currentColumn;

	// Token: 0x04005301 RID: 21249
	public int currentSubTexture;

	// Token: 0x04005302 RID: 21250
	public int currentRowInSubtexture;

	// Token: 0x020009FB RID: 2555
	public struct LerpBakedLightingJob : IJob
	{
		// Token: 0x0600415E RID: 16734 RVA: 0x0015DD64 File Offset: 0x0015BF64
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		// Token: 0x04005303 RID: 21251
		public NativeArray<Color> fromPixels;

		// Token: 0x04005304 RID: 21252
		public NativeArray<Color> toPixels;

		// Token: 0x04005305 RID: 21253
		public NativeArray<Color> mixedPixels;

		// Token: 0x04005306 RID: 21254
		public float lerpValue;
	}
}
