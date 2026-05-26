using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001242 RID: 4674
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x06007502 RID: 29954 RVA: 0x00265550 File Offset: 0x00263750
		protected void Awake()
		{
			if (!DrillFX.appIsQuittingHandlerIsSubscribed)
			{
				DrillFX.appIsQuittingHandlerIsSubscribed = true;
				Application.quitting += DrillFX.HandleApplicationQuitting;
			}
			this.hasFX = (this.fx != null);
			if (this.hasFX)
			{
				this.fxEmissionModule = this.fx.emission;
				this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
				this.fxShapeModule = this.fx.shape;
				this.fxShapeMaxRadius = this.fxShapeModule.radius;
			}
			this.hasAudio = (this.loopAudio != null);
			if (this.hasAudio)
			{
				this.audioMaxVolume = this.loopAudio.volume;
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
		}

		// Token: 0x06007503 RID: 29955 RVA: 0x0026562C File Offset: 0x0026382C
		protected void OnEnable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
			this.ValidateLineCastPositions();
		}

		// Token: 0x06007504 RID: 29956 RVA: 0x00265690 File Offset: 0x00263890
		protected void OnDisable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.GTStop();
			}
		}

		// Token: 0x06007505 RID: 29957 RVA: 0x002656E0 File Offset: 0x002638E0
		protected void LateUpdate()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			Transform transform = base.transform;
			RaycastHit raycastHit;
			Vector3 position = Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), out raycastHit, this.lineCastLayerMask, QueryTriggerInteraction.Ignore) ? raycastHit.point : this.lineCastEnd;
			Vector3 vector = transform.InverseTransformPoint(position);
			float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, vector) / this.maxDepth);
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
				this.fxShapeModule.position = vector;
				this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06007506 RID: 29958 RVA: 0x002657F6 File Offset: 0x002639F6
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x06007507 RID: 29959 RVA: 0x00265800 File Offset: 0x00263A00
		private bool ValidateLineCastPositions()
		{
			this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
			if (this.maxDepth > 1E-45f)
			{
				return true;
			}
			if (Application.isPlaying)
			{
				Debug.Log("DrillFX: lineCastStart and End are too close together. Disabling component.", this);
				base.enabled = false;
			}
			return false;
		}

		// Token: 0x0400867F RID: 34431
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x04008680 RID: 34432
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x04008681 RID: 34433
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x04008682 RID: 34434
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x04008683 RID: 34435
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x04008684 RID: 34436
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x04008685 RID: 34437
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x04008686 RID: 34438
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x04008687 RID: 34439
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x04008688 RID: 34440
		private static bool appIsQuitting;

		// Token: 0x04008689 RID: 34441
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x0400868A RID: 34442
		private float maxDepth;

		// Token: 0x0400868B RID: 34443
		private bool hasFX;

		// Token: 0x0400868C RID: 34444
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x0400868D RID: 34445
		private float fxEmissionMaxRate;

		// Token: 0x0400868E RID: 34446
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x0400868F RID: 34447
		private float fxShapeMaxRadius;

		// Token: 0x04008690 RID: 34448
		private bool hasAudio;

		// Token: 0x04008691 RID: 34449
		private float audioMaxVolume;
	}
}
