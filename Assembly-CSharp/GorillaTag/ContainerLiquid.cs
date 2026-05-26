using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001135 RID: 4405
	[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
	[ExecuteInEditMode]
	public class ContainerLiquid : MonoBehaviour
	{
		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x06006FD5 RID: 28629 RVA: 0x002482A0 File Offset: 0x002464A0
		[DebugReadout]
		public bool isEmpty
		{
			get
			{
				return this.fillAmount <= this.refillThreshold;
			}
		}

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x06006FD6 RID: 28630 RVA: 0x002482B3 File Offset: 0x002464B3
		// (set) Token: 0x06006FD7 RID: 28631 RVA: 0x002482BB File Offset: 0x002464BB
		public Vector3 cupTopWorldPos { get; private set; }

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x06006FD8 RID: 28632 RVA: 0x002482C4 File Offset: 0x002464C4
		// (set) Token: 0x06006FD9 RID: 28633 RVA: 0x002482CC File Offset: 0x002464CC
		public Vector3 bottomLipWorldPos { get; private set; }

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x06006FDA RID: 28634 RVA: 0x002482D5 File Offset: 0x002464D5
		// (set) Token: 0x06006FDB RID: 28635 RVA: 0x002482DD File Offset: 0x002464DD
		public Vector3 liquidPlaneWorldPos { get; private set; }

		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x06006FDC RID: 28636 RVA: 0x002482E6 File Offset: 0x002464E6
		// (set) Token: 0x06006FDD RID: 28637 RVA: 0x002482EE File Offset: 0x002464EE
		public Vector3 liquidPlaneWorldNormal { get; private set; }

		// Token: 0x06006FDE RID: 28638 RVA: 0x002482F8 File Offset: 0x002464F8
		protected bool IsValidLiquidSurfaceValues()
		{
			return this.meshRenderer != null && this.meshFilter != null && this.spillParticleSystem != null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
		}

		// Token: 0x06006FDF RID: 28639 RVA: 0x0024835C File Offset: 0x0024655C
		protected void InitializeLiquidSurface()
		{
			this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
			this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
			this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
			this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
		}

		// Token: 0x06006FE0 RID: 28640 RVA: 0x002483B4 File Offset: 0x002465B4
		protected void InitializeParticleSystem()
		{
			this.spillParticleSystem.main.startColor = this.liquidColor;
		}

		// Token: 0x06006FE1 RID: 28641 RVA: 0x002483DF File Offset: 0x002465DF
		protected void Awake()
		{
			this.matPropBlock = new MaterialPropertyBlock();
			this.topVerts = this.GetTopVerts();
		}

		// Token: 0x06006FE2 RID: 28642 RVA: 0x002483F8 File Offset: 0x002465F8
		protected void OnEnable()
		{
			if (Application.isPlaying)
			{
				base.enabled = (this.useLiquidShader && this.IsValidLiquidSurfaceValues());
				if (base.enabled)
				{
					this.InitializeLiquidSurface();
				}
				this.InitializeParticleSystem();
				this.useFloater = (this.floater != null);
			}
		}

		// Token: 0x06006FE3 RID: 28643 RVA: 0x0024844C File Offset: 0x0024664C
		protected void LateUpdate()
		{
			this.UpdateRefillTimer();
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Bounds bounds = this.meshRenderer.bounds;
			Vector3 a = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
			Vector3 b = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
			this.liquidPlaneWorldPos = Vector3.Lerp(a, b, this.fillAmount);
			Vector3 v = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
			float deltaTime = Time.deltaTime;
			this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
			float num = 6.2831855f * this.wobbleFrequency;
			float d = Mathf.Lerp(this.lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
			Vector2 vector = this.temporalWobbleAmp * d;
			this.liquidPlaneWorldNormal = new Vector3(vector.x, -1f, vector.y).normalized;
			Vector3 v2 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
			if (this.useLiquidShader)
			{
				this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, v2);
				this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, v);
				this.matPropBlock.SetVector(this.liquidColorShaderProp, this.liquidColor.linear);
				if (this.useLiquidVolume)
				{
					float value = MathUtils.Linear(this.fillAmount, 0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
					this.matPropBlock.SetFloat(ShaderProps._LiquidFill, value);
				}
				this.meshRenderer.SetPropertyBlock(this.matPropBlock);
			}
			if (this.useFloater)
			{
				float y = Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount);
				this.floater.localPosition = this.floater.localPosition.WithY(y);
			}
			Vector3 vector2 = (this.lastPos - position) / deltaTime;
			Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
			this.temporalWobbleAmp.x = this.temporalWobbleAmp.x + Mathf.Clamp((vector2.x + vector2.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.temporalWobbleAmp.y = this.temporalWobbleAmp.y + Mathf.Clamp((vector2.z + vector2.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.lastPos = position;
			this.lastRot = rotation;
			this.lastSineWave = d;
			this.lastVelocity = vector2;
			this.lastAngularVelocity = angularVelocity;
			this.meshRenderer.enabled = (!this.keepMeshHidden && !this.isEmpty);
			float x = transform.lossyScale.x;
			float num2 = this.localMeshBounds.extents.x * x;
			float y2 = this.localMeshBounds.extents.y;
			Vector3 position2 = this.localMeshBounds.center + new Vector3(0f, y2, 0f);
			this.cupTopWorldPos = transform.TransformPoint(position2);
			Vector3 up = transform.up;
			Vector3 rhs = transform.InverseTransformDirection(Vector3.down);
			float num3 = float.MinValue;
			Vector3 position3 = Vector3.zero;
			for (int i = 0; i < this.topVerts.Length; i++)
			{
				float num4 = Vector3.Dot(this.topVerts[i], rhs);
				if (num4 > num3)
				{
					num3 = num4;
					position3 = this.topVerts[i];
				}
			}
			this.bottomLipWorldPos = transform.TransformPoint(position3);
			float num5 = Mathf.Clamp01((this.liquidPlaneWorldPos.y - this.bottomLipWorldPos.y) / (num2 * 2f));
			bool flag = num5 > 1E-05f;
			ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission;
			emission.enabled = flag;
			if (flag)
			{
				if (!this.spillSoundBankPlayer.isPlaying)
				{
					this.spillSoundBankPlayer.Play();
				}
				this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, num5);
				this.spillParticleSystem.shape.radius = num2 * num5;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				float num6 = num5 * this.maxSpillRate;
				rateOverTime.constant = num6;
				emission.rateOverTime = rateOverTime;
				this.fillAmount -= num6 * deltaTime * 0.01f;
			}
			if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
			{
				this.emptySoundBankPlayer.Play();
			}
			else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
			{
				this.refillSoundBankPlayer.Play();
			}
			this.wasEmptyLastFrame = this.isEmpty;
		}

		// Token: 0x06006FE4 RID: 28644 RVA: 0x002489D8 File Offset: 0x00246BD8
		public void UpdateRefillTimer()
		{
			if (this.refillDelay < 0f || !this.isEmpty)
			{
				return;
			}
			if (this.refillTimer < 0f)
			{
				this.refillTimer = this.refillDelay;
				this.fillAmount = this.refillAmount;
				return;
			}
			this.refillTimer -= Time.deltaTime;
		}

		// Token: 0x06006FE5 RID: 28645 RVA: 0x00248A34 File Offset: 0x00246C34
		private Vector3[] GetTopVerts()
		{
			Vector3[] vertices = this.meshFilter.sharedMesh.vertices;
			List<Vector3> list = new List<Vector3>(vertices.Length);
			float num = float.MinValue;
			foreach (Vector3 vector in vertices)
			{
				if (vector.y > num)
				{
					num = vector.y;
				}
			}
			foreach (Vector3 vector2 in vertices)
			{
				if (Mathf.Abs(vector2.y - num) < 0.001f)
				{
					list.Add(vector2);
				}
			}
			return list.ToArray();
		}

		// Token: 0x04007FCC RID: 32716
		[Tooltip("Used to determine the world space bounds of the container.")]
		public MeshRenderer meshRenderer;

		// Token: 0x04007FCD RID: 32717
		[Tooltip("Used to determine the local space bounds of the container.")]
		public MeshFilter meshFilter;

		// Token: 0x04007FCE RID: 32718
		[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
		public bool keepMeshHidden;

		// Token: 0x04007FCF RID: 32719
		[Tooltip("The object that will float on top of the liquid.")]
		public Transform floater;

		// Token: 0x04007FD0 RID: 32720
		public bool useLiquidShader = true;

		// Token: 0x04007FD1 RID: 32721
		public bool useLiquidVolume;

		// Token: 0x04007FD2 RID: 32722
		public Vector2 liquidVolumeMinMax = Vector2.up;

		// Token: 0x04007FD3 RID: 32723
		public string liquidColorShaderPropertyName = "_BaseColor";

		// Token: 0x04007FD4 RID: 32724
		public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

		// Token: 0x04007FD5 RID: 32725
		public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

		// Token: 0x04007FD6 RID: 32726
		[Tooltip("Emits drips when pouring.")]
		public ParticleSystem spillParticleSystem;

		// Token: 0x04007FD7 RID: 32727
		[SoundBankInfo]
		public SoundBankPlayer emptySoundBankPlayer;

		// Token: 0x04007FD8 RID: 32728
		[SoundBankInfo]
		public SoundBankPlayer refillSoundBankPlayer;

		// Token: 0x04007FD9 RID: 32729
		[SoundBankInfo]
		public SoundBankPlayer spillSoundBankPlayer;

		// Token: 0x04007FDA RID: 32730
		public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

		// Token: 0x04007FDB RID: 32731
		[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
		[Range(0f, 1f)]
		public float fillAmount = 0.85f;

		// Token: 0x04007FDC RID: 32732
		[Tooltip("This is what fillAmount will be after automatic refilling.")]
		public float refillAmount = 0.85f;

		// Token: 0x04007FDD RID: 32733
		[Tooltip("Set to a negative value to disable.")]
		public float refillDelay = 10f;

		// Token: 0x04007FDE RID: 32734
		[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
		public float refillThreshold = 0.1f;

		// Token: 0x04007FDF RID: 32735
		public float wobbleMax = 0.2f;

		// Token: 0x04007FE0 RID: 32736
		public float wobbleFrequency = 1f;

		// Token: 0x04007FE1 RID: 32737
		public float recovery = 1f;

		// Token: 0x04007FE2 RID: 32738
		public float thickness = 1f;

		// Token: 0x04007FE3 RID: 32739
		public float maxSpillRate = 100f;

		// Token: 0x04007FE8 RID: 32744
		[DebugReadout]
		private bool wasEmptyLastFrame;

		// Token: 0x04007FE9 RID: 32745
		private int liquidColorShaderProp;

		// Token: 0x04007FEA RID: 32746
		private int liquidPlaneNormalShaderProp;

		// Token: 0x04007FEB RID: 32747
		private int liquidPlanePositionShaderProp;

		// Token: 0x04007FEC RID: 32748
		private float refillTimer;

		// Token: 0x04007FED RID: 32749
		private float lastSineWave;

		// Token: 0x04007FEE RID: 32750
		private float lastWobble;

		// Token: 0x04007FEF RID: 32751
		private Vector2 temporalWobbleAmp;

		// Token: 0x04007FF0 RID: 32752
		private Vector3 lastPos;

		// Token: 0x04007FF1 RID: 32753
		private Vector3 lastVelocity;

		// Token: 0x04007FF2 RID: 32754
		private Vector3 lastAngularVelocity;

		// Token: 0x04007FF3 RID: 32755
		private Quaternion lastRot;

		// Token: 0x04007FF4 RID: 32756
		private MaterialPropertyBlock matPropBlock;

		// Token: 0x04007FF5 RID: 32757
		private Bounds localMeshBounds;

		// Token: 0x04007FF6 RID: 32758
		private bool useFloater;

		// Token: 0x04007FF7 RID: 32759
		private Vector3[] topVerts;
	}
}
