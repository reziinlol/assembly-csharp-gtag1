using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FB9 RID: 4025
	public class BuilderScaleParticles : MonoBehaviour
	{
		// Token: 0x06006497 RID: 25751 RVA: 0x0020696C File Offset: 0x00204B6C
		private void OnEnable()
		{
			if (this.useLossyScale)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x00206988 File Offset: 0x00204B88
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScale)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x06006499 RID: 25753 RVA: 0x002069C4 File Offset: 0x00204BC4
		private void OnDisable()
		{
			if (this.useLossyScale)
			{
				this.RevertScale();
			}
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x002069D4 File Offset: 0x00204BD4
		public void SetScale(float inScale)
		{
			bool isPlaying = this.system.isPlaying;
			if (isPlaying)
			{
				this.system.Stop();
				this.system.Clear();
			}
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			this.gravityMod = main.gravityModifierMultiplier;
			main.gravityModifierMultiplier = this.gravityMod * this.scale;
			if (main.startSize3D)
			{
				ParticleSystem.MinMaxCurve startSizeX = main.startSizeX;
				this.sizeCurveXCache = main.startSizeX;
				this.ScaleCurve(ref startSizeX, this.scale);
				main.startSizeX = startSizeX;
				ParticleSystem.MinMaxCurve startSizeY = main.startSizeY;
				this.sizeCurveYCache = main.startSizeY;
				this.ScaleCurve(ref startSizeY, this.scale);
				main.startSizeY = startSizeY;
				ParticleSystem.MinMaxCurve startSizeZ = main.startSizeZ;
				this.sizeCurveZCache = main.startSizeZ;
				this.ScaleCurve(ref startSizeZ, this.scale);
				main.startSizeZ = startSizeZ;
			}
			else
			{
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				this.sizeCurveCache = main.startSize;
				this.ScaleCurve(ref startSize, this.scale);
				main.startSize = startSize;
			}
			ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
			this.speedCurveCache = main.startSpeed;
			this.ScaleCurve(ref startSpeed, this.scale);
			main.startSpeed = startSpeed;
			if (this.scaleShape)
			{
				ParticleSystem.ShapeModule shape = this.system.shape;
				this.shapeScale = shape.scale;
				shape.scale = this.shapeScale * this.scale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				this.lifetimeVelocityX = velocityOverLifetime.x;
				this.lifetimeVelocityY = velocityOverLifetime.y;
				this.lifetimeVelocityZ = velocityOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve = velocityOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.x = minMaxCurve;
				minMaxCurve = velocityOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.y = minMaxCurve;
				minMaxCurve = velocityOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.z = minMaxCurve;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = this.system.limitVelocityOverLifetime;
				this.limitMultiplier = limitVelocityOverLifetime.limitMultiplier;
				limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier * this.scale;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				this.forceX = forceOverLifetime.x;
				this.forceY = forceOverLifetime.y;
				this.forceZ = forceOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve2 = forceOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.x = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.y = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.z = minMaxCurve2;
			}
			if (this.autoPlay || isPlaying)
			{
				this.system.Play(true);
			}
			this.shouldRevert = true;
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x00206D44 File Offset: 0x00204F44
		private void ScaleCurve(ref ParticleSystem.MinMaxCurve curve, float scale)
		{
			switch (curve.mode)
			{
			case ParticleSystemCurveMode.Constant:
				curve.constant *= scale;
				return;
			case ParticleSystemCurveMode.Curve:
			case ParticleSystemCurveMode.TwoCurves:
				curve.curveMultiplier *= scale;
				return;
			case ParticleSystemCurveMode.TwoConstants:
				curve.constantMin *= scale;
				curve.constantMax *= scale;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600649C RID: 25756 RVA: 0x00206DAC File Offset: 0x00204FAC
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			main.gravityModifierMultiplier = this.gravityMod;
			main.startSpeed = this.speedCurveCache;
			if (main.startSize3D)
			{
				main.startSizeX = this.sizeCurveXCache;
				main.startSizeY = this.sizeCurveYCache;
				main.startSizeZ = this.sizeCurveZCache;
			}
			else
			{
				main.startSize = this.sizeCurveCache;
			}
			if (this.scaleShape)
			{
				this.system.shape.scale = this.shapeScale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				velocityOverLifetime.x = this.lifetimeVelocityX;
				velocityOverLifetime.y = this.lifetimeVelocityY;
				velocityOverLifetime.z = this.lifetimeVelocityZ;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				this.system.limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				forceOverLifetime.x = this.forceX;
				forceOverLifetime.y = this.forceY;
				forceOverLifetime.z = this.forceZ;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x0400736A RID: 29546
		private float scale = 1f;

		// Token: 0x0400736B RID: 29547
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScale;

		// Token: 0x0400736C RID: 29548
		[Tooltip("Play particles after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x0400736D RID: 29549
		[SerializeField]
		private ParticleSystem system;

		// Token: 0x0400736E RID: 29550
		[SerializeField]
		private bool scaleShape;

		// Token: 0x0400736F RID: 29551
		[SerializeField]
		private bool scaleVelocityLifetime;

		// Token: 0x04007370 RID: 29552
		[SerializeField]
		private bool scaleVelocityLimitLifetime;

		// Token: 0x04007371 RID: 29553
		[SerializeField]
		private bool scaleForceOverLife;

		// Token: 0x04007372 RID: 29554
		private float gravityMod = 1f;

		// Token: 0x04007373 RID: 29555
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x04007374 RID: 29556
		private ParticleSystem.MinMaxCurve sizeCurveCache;

		// Token: 0x04007375 RID: 29557
		private ParticleSystem.MinMaxCurve sizeCurveXCache;

		// Token: 0x04007376 RID: 29558
		private ParticleSystem.MinMaxCurve sizeCurveYCache;

		// Token: 0x04007377 RID: 29559
		private ParticleSystem.MinMaxCurve sizeCurveZCache;

		// Token: 0x04007378 RID: 29560
		private ParticleSystem.MinMaxCurve forceX;

		// Token: 0x04007379 RID: 29561
		private ParticleSystem.MinMaxCurve forceY;

		// Token: 0x0400737A RID: 29562
		private ParticleSystem.MinMaxCurve forceZ;

		// Token: 0x0400737B RID: 29563
		private Vector3 shapeScale = Vector3.one;

		// Token: 0x0400737C RID: 29564
		private ParticleSystem.MinMaxCurve lifetimeVelocityX;

		// Token: 0x0400737D RID: 29565
		private ParticleSystem.MinMaxCurve lifetimeVelocityY;

		// Token: 0x0400737E RID: 29566
		private ParticleSystem.MinMaxCurve lifetimeVelocityZ;

		// Token: 0x0400737F RID: 29567
		private float limitMultiplier = 1f;

		// Token: 0x04007380 RID: 29568
		private bool shouldRevert;

		// Token: 0x04007381 RID: 29569
		private bool setScaleNextFrame;

		// Token: 0x04007382 RID: 29570
		private int enableFrame;
	}
}
