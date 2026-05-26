using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200026F RID: 623
public class PlayableBoundaryManager : MonoBehaviour
{
	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060010D2 RID: 4306 RVA: 0x0005A222 File Offset: 0x00058422
	// (set) Token: 0x060010D3 RID: 4307 RVA: 0x0005A23A File Offset: 0x0005843A
	public static bool ShouldRender
	{
		get
		{
			return Shader.GetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled) > 0f;
		}
		set
		{
			Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled, (float)(value ? 1 : 0));
		}
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x0005A253 File Offset: 0x00058453
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x0005A264 File Offset: 0x00058464
	public void Setup()
	{
		Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
		Vector3 position = base.transform.position;
		SRand srand = new SRand(StaticHash.Compute(position.x, position.y, position.z));
		this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
		this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
		for (int i = 1; i < 8; i++)
		{
			Vector3 vector = position + srand.NextPointInsideSphere(this.m_bigCylinderRadius * this.radiusScale);
			this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
			this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
		}
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0005A37C File Offset: 0x0005857C
	private void OnEnable()
	{
		PlayableBoundaryManager.ShouldRender = true;
		this.Setup();
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0005A38A File Offset: 0x0005858A
	private void OnDisable()
	{
		PlayableBoundaryManager.ShouldRender = false;
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0005A394 File Offset: 0x00058594
	public unsafe void UpdateSim()
	{
		if (Time.frameCount == this._lastFrameUpdated)
		{
			return;
		}
		this._lastFrameUpdated = Time.frameCount;
		Vector4[] array = this._cylinders_centers;
		if (array != null && array.Length == 8)
		{
			array = this._cylinders_radiusHeights;
			if (array != null && array.Length == 8)
			{
				if (this.m_smallCylindersMoveTimeScale > 0.0)
				{
					Vector3 position = base.transform.position;
					float d = (float)((double)(GTTime.TimeAsMilliseconds() % 86400000L) * this.m_smallCylindersMoveTimeScale / 1000.0);
					this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
					this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
					for (int i = 1; i < 8; i++)
					{
						float num = (float)i * 0.125f;
						Vector3 v = *PlayableBoundaryManager.Hash3(num * 1.17f) + *PlayableBoundaryManager.Hash3(num * 13.7f) * d;
						Vector3 vector = position + v.Sin() * this.m_bigCylinderRadius * this.radiusScale;
						this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
						this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
					}
				}
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_Centers, this._cylinders_centers);
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_RadiusHeights, this._cylinders_radiusHeights);
				for (int j = 0; j < this.tracked.Count; j++)
				{
					PlayableBoundaryTracker playableBoundaryTracker = this.tracked[j];
					if (playableBoundaryTracker)
					{
						playableBoundaryTracker.UpdateSignedDistanceToBoundary(this._GetSignedDistanceToBoundary(playableBoundaryTracker.transform.position, playableBoundaryTracker.radius), Time.deltaTime);
					}
				}
				Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
				return;
			}
		}
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0005A5D0 File Offset: 0x000587D0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float _GetSignedDistanceToBoundary(float3 tracked_center, float tracked_radius)
	{
		float num = float.MaxValue;
		float smoothFactor = this.GetSmoothFactor();
		for (int i = 0; i < 8; i++)
		{
			float3 @float = this._cylinders_centers[i].xyz - tracked_center;
			float x = this._cylinders_radiusHeights[i].x;
			float signedDist = math.length(@float.xz) - x;
			num = this.SDFSmoothMerge(num, signedDist, smoothFactor);
		}
		return num - tracked_radius;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x0005A64C File Offset: 0x0005884C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float SDFSmoothMerge(float signedDist1, float signedDist2, float smoothRadius)
	{
		float num = -math.length(math.min(new float2(signedDist1 - smoothRadius, signedDist2 - smoothRadius), new float2(0f, 0f)));
		float num2 = math.max(math.min(signedDist1, signedDist2), smoothRadius);
		return num + num2;
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0005A690 File Offset: 0x00058890
	private static ref Vector3 Hash3(float n)
	{
		PlayableBoundaryManager.kHashVec.x = Mathf.Sin(n) * 43758.547f % 1f;
		PlayableBoundaryManager.kHashVec.y = Mathf.Sin(n + 1f) * 22578.146f % 1f;
		PlayableBoundaryManager.kHashVec.z = Mathf.Sin(n + 2f) * 19642.35f % 1f;
		return ref PlayableBoundaryManager.kHashVec;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x0005A704 File Offset: 0x00058904
	private float GetSmoothFactor()
	{
		float num = this.m_smoothFactor;
		if (this.m_bigCylinderRadius <= 1f)
		{
			num *= math.max(this.m_bigCylinderRadius, 0f);
		}
		return math.max(num, 1E-06f);
	}

	// Token: 0x04001410 RID: 5136
	public List<PlayableBoundaryTracker> tracked = new List<PlayableBoundaryTracker>(10);

	// Token: 0x04001411 RID: 5137
	[Space]
	[Range(0f, 128f)]
	public float m_bigCylinderRadius = 8f;

	// Token: 0x04001412 RID: 5138
	public float m_smoothFactor = 1.5f;

	// Token: 0x04001413 RID: 5139
	public float m_smallCylindersRadius = 3f;

	// Token: 0x04001414 RID: 5140
	[SerializeField]
	private double m_smallCylindersMoveTimeScale = 0.25;

	// Token: 0x04001415 RID: 5141
	[Space]
	private readonly Vector4[] _cylinders_centers = new Vector4[8];

	// Token: 0x04001416 RID: 5142
	private readonly Vector4[] _cylinders_radiusHeights = new Vector4[8];

	// Token: 0x04001417 RID: 5143
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_Centers = "_GTGameModes_PlayableBoundary_Cylinders_Centers";

	// Token: 0x04001418 RID: 5144
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_RadiusHeights = "_GTGameModes_PlayableBoundary_Cylinders_RadiusHeights";

	// Token: 0x04001419 RID: 5145
	private static ShaderHashId _GTGameModes_PlayableBoundary_NonZeroSmoothRadius = "_GTGameModes_PlayableBoundary_NonZeroSmoothRadius";

	// Token: 0x0400141A RID: 5146
	private static ShaderHashId _GTGameModes_PlayableBoundary_IsEnabled = "_GTGameModes_PlayableBoundary_IsEnabled";

	// Token: 0x0400141B RID: 5147
	private const int _k_cylinders_count = 8;

	// Token: 0x0400141C RID: 5148
	[NonSerialized]
	public float radiusScale = 1f;

	// Token: 0x0400141D RID: 5149
	private int _lastFrameUpdated = -1;

	// Token: 0x0400141E RID: 5150
	private static Vector3 kHashVec = Vector3.zero;
}
