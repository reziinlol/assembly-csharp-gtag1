using System;
using UnityEngine;

// Token: 0x0200033A RID: 826
public class GTShaderGlobals : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001441 RID: 5185 RVA: 0x0006CCCC File Offset: 0x0006AECC
	public static Vector3 WorldSpaceCameraPos
	{
		get
		{
			return GTShaderGlobals.gMainCameraWorldPos;
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001442 RID: 5186 RVA: 0x0006CCD3 File Offset: 0x0006AED3
	public static float Time
	{
		get
		{
			return GTShaderGlobals.gTime;
		}
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001443 RID: 5187 RVA: 0x0006CCDA File Offset: 0x0006AEDA
	public static int Frame
	{
		get
		{
			return GTShaderGlobals.gIFrame;
		}
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0006CCE1 File Offset: 0x0006AEE1
	private void Awake()
	{
		GTShaderGlobals.gMainCamera = Camera.main;
		if (GTShaderGlobals.gMainCamera)
		{
			GTShaderGlobals.gMainCameraXform = GTShaderGlobals.gMainCamera.transform;
			GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		}
		this.SliceUpdate();
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x0006CD1D File Offset: 0x0006AF1D
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		GTShaderGlobals.InitBlueNoiseTex();
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0006CD24 File Offset: 0x0006AF24
	public void SliceUpdate()
	{
		GTShaderGlobals.UpdateTime();
		GTShaderGlobals.UpdateFrame();
		GTShaderGlobals.UpdateCamera();
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x0006CD35 File Offset: 0x0006AF35
	private static void UpdateFrame()
	{
		GTShaderGlobals.gIFrame = UnityEngine.Time.frameCount;
		Shader.SetGlobalInteger(GTShaderGlobals._GT_iFrame, GTShaderGlobals.gIFrame);
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x0006CD55 File Offset: 0x0006AF55
	private static void UpdateCamera()
	{
		if (!GTShaderGlobals.gMainCameraXform)
		{
			return;
		}
		GTShaderGlobals.gMainCameraWorldPos = GTShaderGlobals.gMainCameraXform.position;
		Shader.SetGlobalVector(GTShaderGlobals._GT_WorldSpaceCameraPos, GTShaderGlobals.gMainCameraWorldPos);
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0006CD8C File Offset: 0x0006AF8C
	private static void UpdateTime()
	{
		GTShaderGlobals.gTime = (float)(DateTime.UtcNow - GTShaderGlobals.gStartTime).TotalSeconds;
		Shader.SetGlobalFloat(GTShaderGlobals._GT_Time, GTShaderGlobals.gTime);
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0006CDCA File Offset: 0x0006AFCA
	private static void UpdatePawns()
	{
		GTShaderGlobals.gActivePawns = GorillaPawn.ActiveCount;
		GorillaPawn.SyncPawnData();
		Shader.SetGlobalMatrixArray(GTShaderGlobals._GT_PawnData, GTShaderGlobals.gPawnData);
		Shader.SetGlobalInteger(GTShaderGlobals._GT_PawnActiveCount, GTShaderGlobals.gActivePawns);
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0006CE04 File Offset: 0x0006B004
	private static void InitBlueNoiseTex()
	{
		GTShaderGlobals.gBlueNoiseTex = Resources.Load<Texture2D>("Graphics/Textures/noise_blue_rgba_128");
		GTShaderGlobals.gBlueNoiseTexWH = GTShaderGlobals.gBlueNoiseTex.GetTexelSize();
		Shader.SetGlobalTexture(GTShaderGlobals._GT_BlueNoiseTex, GTShaderGlobals.gBlueNoiseTex);
		Shader.SetGlobalVector(GTShaderGlobals._GT_BlueNoiseTex_WH, GTShaderGlobals.gBlueNoiseTexWH);
	}

	// Token: 0x04001912 RID: 6418
	private static Camera gMainCamera;

	// Token: 0x04001913 RID: 6419
	private static Transform gMainCameraXform;

	// Token: 0x04001914 RID: 6420
	private static Vector3 gMainCameraWorldPos;

	// Token: 0x04001915 RID: 6421
	[Space]
	private static int gIFrame;

	// Token: 0x04001916 RID: 6422
	private static float gTime;

	// Token: 0x04001917 RID: 6423
	[Space]
	private static Texture2D gBlueNoiseTex;

	// Token: 0x04001918 RID: 6424
	private static Vector4 gBlueNoiseTexWH;

	// Token: 0x04001919 RID: 6425
	[Space]
	private static int gActivePawns;

	// Token: 0x0400191A RID: 6426
	[Space]
	private static DateTime gStartTime = DateTime.Today.AddDays(-1.0).ToUniversalTime();

	// Token: 0x0400191B RID: 6427
	private static Matrix4x4[] gPawnData = GorillaPawn.ShaderData;

	// Token: 0x0400191C RID: 6428
	private static ShaderHashId _GT_WorldSpaceCameraPos = "_GT_WorldSpaceCameraPos";

	// Token: 0x0400191D RID: 6429
	private static ShaderHashId _GT_BlueNoiseTex = "_GT_BlueNoiseTex";

	// Token: 0x0400191E RID: 6430
	private static ShaderHashId _GT_BlueNoiseTex_WH = "_GT_BlueNoiseTex_WH";

	// Token: 0x0400191F RID: 6431
	private static ShaderHashId _GT_iFrame = "_GT_iFrame";

	// Token: 0x04001920 RID: 6432
	private static ShaderHashId _GT_Time = "_GT_Time";

	// Token: 0x04001921 RID: 6433
	private static ShaderHashId _GT_PawnData = "_GT_PawnData";

	// Token: 0x04001922 RID: 6434
	private static ShaderHashId _GT_PawnActiveCount = "_GT_PawnActiveCount";
}
