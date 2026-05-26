using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200126F RID: 4719
	public class Dreidel : MonoBehaviour
	{
		// Token: 0x0600764E RID: 30286 RVA: 0x0026BE28 File Offset: 0x0026A028
		public bool TrySetIdle()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface || this.state == Dreidel.State.Fallen)
			{
				this.StartIdle();
				return true;
			}
			return false;
		}

		// Token: 0x0600764F RID: 30287 RVA: 0x0026BE4D File Offset: 0x0026A04D
		public bool TryCheckForSurfaces()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface)
			{
				this.StartFindingSurfaces();
				return true;
			}
			return false;
		}

		// Token: 0x06007650 RID: 30288 RVA: 0x0026BE69 File Offset: 0x0026A069
		public void Spin()
		{
			this.StartSpin();
		}

		// Token: 0x06007651 RID: 30289 RVA: 0x0026BE74 File Offset: 0x0026A074
		public bool TryGetSpinStartData(out Vector3 surfacePoint, out Vector3 surfaceNormal, out float randomDuration, out Dreidel.Side randomSide, out Dreidel.Variation randomVariation, out double startTime)
		{
			if (this.canStartSpin)
			{
				surfacePoint = this.surfacePlanePoint;
				surfaceNormal = this.surfacePlaneNormal;
				randomDuration = Random.Range(this.spinTimeRange.x, this.spinTimeRange.y);
				randomSide = (Dreidel.Side)Random.Range(0, 4);
				randomVariation = (Dreidel.Variation)Random.Range(0, 5);
				startTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0);
				return true;
			}
			surfacePoint = Vector3.zero;
			surfaceNormal = Vector3.zero;
			randomDuration = 0f;
			randomSide = Dreidel.Side.Shin;
			randomVariation = Dreidel.Variation.Tumble;
			startTime = -1.0;
			return false;
		}

		// Token: 0x06007652 RID: 30290 RVA: 0x0026BF20 File Offset: 0x0026A120
		public void SetSpinStartData(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			this.surfacePlanePoint = surfacePoint;
			this.surfacePlaneNormal = surfaceNormal;
			this.spinTime = duration;
			this.spinStartTime = startTime;
			this.spinCounterClockwise = counterClockwise;
			this.landingSide = side;
			this.landingVariation = variation;
		}

		// Token: 0x06007653 RID: 30291 RVA: 0x0026BF58 File Offset: 0x0026A158
		private void LateUpdate()
		{
			float deltaTime = Time.deltaTime;
			double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.canStartSpin = false;
			switch (this.state)
			{
			default:
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			case Dreidel.State.FindingSurface:
			{
				float num2 = (GTPlayer.Instance != null) ? GTPlayer.Instance.scale : 1f;
				Vector3 down = Vector3.down;
				Vector3 origin = base.transform.parent.position - down * 2f * this.surfaceCheckDistance * num2;
				float maxDistance = (3f * this.surfaceCheckDistance + -this.bottomPointOffset.y) * num2;
				RaycastHit raycastHit;
				if (Physics.Raycast(origin, down, out raycastHit, maxDistance, this.surfaceLayers.value, QueryTriggerInteraction.Ignore) && Vector3.Dot(raycastHit.normal, Vector3.up) > this.surfaceUprightThreshold && Vector3.Dot(raycastHit.normal, this.spinTransform.up) > this.surfaceDreidelAngleThreshold)
				{
					this.canStartSpin = true;
					this.surfacePlanePoint = raycastHit.point;
					this.surfacePlaneNormal = raycastHit.normal;
					this.AlignToSurfacePlane();
					this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
					this.UpdateSpinTransform();
					return;
				}
				this.canStartSpin = false;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			}
			case Dreidel.State.Spinning:
			{
				float num3 = Mathf.Clamp01((float)(num - this.stateStartTime) / this.spinTime);
				this.spinSpeed = Mathf.Lerp(this.spinSpeedStart, this.spinSpeedEnd, num3);
				float num4 = this.spinCounterClockwise ? -1f : 1f;
				this.spinAngle += num4 * this.spinSpeed * 360f * deltaTime;
				float num5 = this.tiltWobble;
				float num6 = Mathf.Sin(this.spinWobbleFrequency * 2f * 3.1415927f * (float)(num - this.stateStartTime));
				float t = 0.5f * num6 + 0.5f;
				this.tiltWobble = Mathf.Lerp(this.spinWobbleAmplitudeEndMin * num3, this.spinWobbleAmplitude * num3, t);
				if (this.landingTiltTarget.y == 0f)
				{
					if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltTarget.x) * this.tiltWobble;
					}
					else
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltLeadingTarget.x) * this.tiltWobble;
					}
				}
				else if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltTarget.y) * this.tiltWobble;
				}
				else
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltLeadingTarget.y) * this.tiltWobble;
				}
				float num7 = Mathf.Lerp(this.pathStartTurnRate, this.pathEndTurnRate, num3) + num6 * this.pathTurnRateSinOffset;
				if (this.spinCounterClockwise)
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num7 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				else
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num7 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				this.pathOffset += this.pathDir * this.pathMoveSpeed * deltaTime;
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				if (num3 - Mathf.Epsilon >= 1f && this.tiltWobble > 0.9f * this.spinWobbleAmplitude && num5 < this.tiltWobble)
				{
					this.StartFall();
					return;
				}
				break;
			}
			case Dreidel.State.Falling:
			{
				float num8 = this.fallTimeTumble;
				Dreidel.Variation variation = this.landingVariation;
				if (variation <= Dreidel.Variation.Smooth || variation - Dreidel.Variation.Bounce > 2)
				{
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num9 = this.spinCounterClockwise ? -1f : 1f;
					this.spinAngle += num9 * this.spinSpeed * 360f * deltaTime;
					float angularFrequency = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrontBackFrequency;
					float dampingRatio = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallFrontBackDampingRatio;
					float angularFrequency2 = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrequency;
					float dampingRatio2 = (this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallDampingRatio;
					this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, angularFrequency, dampingRatio, deltaTime);
					this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, angularFrequency2, dampingRatio2, deltaTime);
				}
				else
				{
					bool flag = this.landingVariation != Dreidel.Variation.Bounce;
					bool flag2 = this.landingVariation == Dreidel.Variation.FalseSlowTurn;
					float num10 = flag ? this.slowTurnSwitchTime : this.bounceFallSwitchTime;
					if (flag)
					{
						num8 = this.fallTimeSlowTurn;
					}
					if (num - this.stateStartTime < (double)num10)
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltLeadingTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
					}
					else
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						if (flag2)
						{
							if (!this.falseTargetReached && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.49f)
							{
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
							}
							else
							{
								this.falseTargetReached = true;
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
							}
						}
						else if (flag && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.45f)
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
						}
						else
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
						}
					}
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num11 = this.spinCounterClockwise ? -1f : 1f;
					this.spinAngle += num11 * this.spinSpeed * 360f * deltaTime;
				}
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				float num12 = (float)(num - this.stateStartTime);
				if (num12 > num8)
				{
					if (!this.hasLanded)
					{
						this.hasLanded = true;
						if (this.landingSide == Dreidel.Side.Gimel)
						{
							this.gimelConfetti.transform.position = this.spinTransform.position + Vector3.up * this.confettiHeight;
							this.gimelConfetti.gameObject.SetActive(true);
							this.audioSource.GTPlayOneShot(this.gimelConfettiSound, 1f);
						}
					}
					if (num12 > num8 + this.respawnTimeAfterLanding)
					{
						this.StartIdle();
					}
				}
				break;
			}
			case Dreidel.State.Fallen:
				break;
			}
		}

		// Token: 0x06007654 RID: 30292 RVA: 0x0026C784 File Offset: 0x0026A984
		private void StartIdle()
		{
			this.state = Dreidel.State.Idle;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x06007655 RID: 30293 RVA: 0x0026C860 File Offset: 0x0026AA60
		private void StartFindingSurfaces()
		{
			this.state = Dreidel.State.FindingSurface;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x06007656 RID: 30294 RVA: 0x0026C93C File Offset: 0x0026AB3C
		private void StartSpin()
		{
			this.state = Dreidel.State.Spinning;
			this.stateStartTime = ((this.spinStartTime > 0.0) ? this.spinStartTime : ((double)Time.time));
			this.canStartSpin = false;
			this.spinSpeed = this.spinSpeedStart;
			this.tiltWobble = 0f;
			this.audioSource.loop = true;
			this.audioSource.clip = this.spinLoopAudio;
			this.audioSource.GTPlay();
			this.gimelConfetti.gameObject.SetActive(false);
			this.AlignToSurfacePlane();
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
		}

		// Token: 0x06007657 RID: 30295 RVA: 0x0026CA04 File Offset: 0x0026AC04
		private void StartFall()
		{
			this.state = Dreidel.State.Falling;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.falseTargetReached = false;
			this.hasLanded = false;
			if (this.landingVariation == Dreidel.Variation.FalseSlowTurn)
			{
				if (this.spinCounterClockwise)
				{
					this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
				else
				{
					this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
			}
			else if (this.spinCounterClockwise)
			{
				this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			else
			{
				this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			this.spinSpeedSpring.Reset(this.spinSpeed, 0f);
			this.tiltFrontBackSpring.Reset(this.tiltFrontBack, 0f);
			this.tiltLeftRightSpring.Reset(this.tiltLeftRight, 0f);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.audioSource.loop = false;
			this.audioSource.GTPlayOneShot(this.fallSound, 1f);
			this.gimelConfetti.gameObject.SetActive(false);
		}

		// Token: 0x06007658 RID: 30296 RVA: 0x0026CB54 File Offset: 0x0026AD54
		private Vector3 GetGroundContactPoint()
		{
			Vector3 position = this.spinTransform.position;
			this.dreidelCollider.enabled = true;
			Vector3 vector = this.dreidelCollider.ClosestPoint(position - base.transform.up);
			this.dreidelCollider.enabled = false;
			float num = Vector3.Dot(vector - position, this.spinTransform.up);
			if (num > 0f)
			{
				vector -= num * this.spinTransform.up;
			}
			return this.spinTransform.InverseTransformPoint(vector);
		}

		// Token: 0x06007659 RID: 30297 RVA: 0x0026CBE8 File Offset: 0x0026ADE8
		private void GetTiltVectorsForSideWithPrev(Dreidel.Side side, out Vector2 sideTilt, out Vector2 prevSideTilt)
		{
			int num = (side <= Dreidel.Side.Shin) ? 3 : (side - Dreidel.Side.Hey);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				prevSideTilt = this.landingTiltValues[num];
				prevSideTilt.x = sideTilt.x;
				return;
			}
			prevSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = prevSideTilt.x;
		}

		// Token: 0x0600765A RID: 30298 RVA: 0x0026CC6C File Offset: 0x0026AE6C
		private void GetTiltVectorsForSideWithNext(Dreidel.Side side, out Vector2 sideTilt, out Vector2 nextSideTilt)
		{
			int num = (int)((side + 1) % Dreidel.Side.Count);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				nextSideTilt = this.landingTiltValues[num];
				nextSideTilt.x = sideTilt.x;
				return;
			}
			nextSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = nextSideTilt.x;
		}

		// Token: 0x0600765B RID: 30299 RVA: 0x0026CCE8 File Offset: 0x0026AEE8
		private void AlignToSurfacePlane()
		{
			Vector3 forward = Vector3.forward;
			if (Vector3.Dot(Vector3.up, this.surfacePlaneNormal) < 0.9999f)
			{
				Vector3 axis = Vector3.Cross(this.surfacePlaneNormal, Vector3.up);
				forward = Quaternion.AngleAxis(90f, axis) * this.surfacePlaneNormal;
			}
			Quaternion rotation = Quaternion.LookRotation(forward, this.surfacePlaneNormal);
			base.transform.position = this.surfacePlanePoint;
			base.transform.rotation = rotation;
		}

		// Token: 0x0600765C RID: 30300 RVA: 0x0026CD64 File Offset: 0x0026AF64
		private void UpdateSpinTransform()
		{
			Vector3 position = this.spinTransform.position;
			Vector3 groundContactPoint = this.GetGroundContactPoint();
			Vector3 position2 = this.groundPointSpring.TrackDampingRatio(groundContactPoint, this.groundTrackingFrequency, this.groundTrackingDampingRatio, Time.deltaTime);
			Vector3 b = this.spinTransform.TransformPoint(position2);
			Quaternion rhs = Quaternion.AngleAxis(90f * this.tiltLeftRight, Vector3.forward) * Quaternion.AngleAxis(90f * this.tiltFrontBack, Vector3.right);
			this.spinAxis = base.transform.InverseTransformDirection(base.transform.up);
			Quaternion lhs = Quaternion.AngleAxis(this.spinAngle, this.spinAxis);
			this.spinTransform.localRotation = lhs * rhs;
			Vector3 a = base.transform.InverseTransformVector(Vector3.Dot(position - b, base.transform.up) * base.transform.up);
			this.spinTransform.localPosition = a + this.pathOffset;
			this.spinTransform.TransformPoint(this.bottomPointOffset);
		}

		// Token: 0x04008819 RID: 34841
		[Header("References")]
		[SerializeField]
		private Transform spinTransform;

		// Token: 0x0400881A RID: 34842
		[SerializeField]
		private MeshCollider dreidelCollider;

		// Token: 0x0400881B RID: 34843
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400881C RID: 34844
		[SerializeField]
		private AudioClip spinLoopAudio;

		// Token: 0x0400881D RID: 34845
		[SerializeField]
		private AudioClip fallSound;

		// Token: 0x0400881E RID: 34846
		[SerializeField]
		private AudioClip gimelConfettiSound;

		// Token: 0x0400881F RID: 34847
		[SerializeField]
		private ParticleSystem gimelConfetti;

		// Token: 0x04008820 RID: 34848
		[Header("Offsets")]
		[SerializeField]
		private Vector3 centerOfMassOffset = Vector3.zero;

		// Token: 0x04008821 RID: 34849
		[SerializeField]
		private Vector3 bottomPointOffset = Vector3.zero;

		// Token: 0x04008822 RID: 34850
		[SerializeField]
		private Vector2 bodyRect = Vector2.one;

		// Token: 0x04008823 RID: 34851
		[SerializeField]
		private float confettiHeight = 0.125f;

		// Token: 0x04008824 RID: 34852
		[Header("Surface Detection")]
		[SerializeField]
		private float surfaceCheckDistance = 0.15f;

		// Token: 0x04008825 RID: 34853
		[SerializeField]
		private float surfaceUprightThreshold = 0.5f;

		// Token: 0x04008826 RID: 34854
		[SerializeField]
		private float surfaceDreidelAngleThreshold = 0.9f;

		// Token: 0x04008827 RID: 34855
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04008828 RID: 34856
		[Header("Spin Paramss")]
		[SerializeField]
		private float spinSpeedStart = 2f;

		// Token: 0x04008829 RID: 34857
		[SerializeField]
		private float spinSpeedEnd = 1f;

		// Token: 0x0400882A RID: 34858
		[SerializeField]
		private float spinTime = 10f;

		// Token: 0x0400882B RID: 34859
		[SerializeField]
		private Vector2 spinTimeRange = new Vector2(7f, 12f);

		// Token: 0x0400882C RID: 34860
		[SerializeField]
		private float spinWobbleFrequency = 0.1f;

		// Token: 0x0400882D RID: 34861
		[SerializeField]
		private float spinWobbleAmplitude = 0.01f;

		// Token: 0x0400882E RID: 34862
		[SerializeField]
		private float spinWobbleAmplitudeEndMin = 0.01f;

		// Token: 0x0400882F RID: 34863
		[SerializeField]
		private float tiltFrontBack;

		// Token: 0x04008830 RID: 34864
		[SerializeField]
		private float tiltLeftRight;

		// Token: 0x04008831 RID: 34865
		[SerializeField]
		private float groundTrackingDampingRatio = 0.9f;

		// Token: 0x04008832 RID: 34866
		[SerializeField]
		private float groundTrackingFrequency = 1f;

		// Token: 0x04008833 RID: 34867
		[Header("Motion Path")]
		[SerializeField]
		private float pathMoveSpeed = 0.1f;

		// Token: 0x04008834 RID: 34868
		[SerializeField]
		private float pathStartTurnRate = 360f;

		// Token: 0x04008835 RID: 34869
		[SerializeField]
		private float pathEndTurnRate = 90f;

		// Token: 0x04008836 RID: 34870
		[SerializeField]
		private float pathTurnRateSinOffset = 180f;

		// Token: 0x04008837 RID: 34871
		[Header("Falling Params")]
		[SerializeField]
		private float spinSpeedStopRate = 1f;

		// Token: 0x04008838 RID: 34872
		[SerializeField]
		private float tumbleFallDampingRatio = 0.4f;

		// Token: 0x04008839 RID: 34873
		[SerializeField]
		private float tumbleFallFrequency = 6f;

		// Token: 0x0400883A RID: 34874
		[SerializeField]
		private float tumbleFallFrontBackDampingRatio = 0.4f;

		// Token: 0x0400883B RID: 34875
		[SerializeField]
		private float tumbleFallFrontBackFrequency = 6f;

		// Token: 0x0400883C RID: 34876
		[SerializeField]
		private float smoothFallDampingRatio = 0.9f;

		// Token: 0x0400883D RID: 34877
		[SerializeField]
		private float smoothFallFrequency = 2f;

		// Token: 0x0400883E RID: 34878
		[SerializeField]
		private float slowTurnDampingRatio = 0.9f;

		// Token: 0x0400883F RID: 34879
		[SerializeField]
		private float slowTurnFrequency = 2f;

		// Token: 0x04008840 RID: 34880
		[SerializeField]
		private float bounceFallSwitchTime = 0.5f;

		// Token: 0x04008841 RID: 34881
		[SerializeField]
		private float slowTurnSwitchTime = 0.5f;

		// Token: 0x04008842 RID: 34882
		[SerializeField]
		private float respawnTimeAfterLanding = 3f;

		// Token: 0x04008843 RID: 34883
		[SerializeField]
		private float fallTimeTumble = 3f;

		// Token: 0x04008844 RID: 34884
		[SerializeField]
		private float fallTimeSlowTurn = 5f;

		// Token: 0x04008845 RID: 34885
		private Dreidel.State state;

		// Token: 0x04008846 RID: 34886
		private double stateStartTime;

		// Token: 0x04008847 RID: 34887
		private float spinSpeed;

		// Token: 0x04008848 RID: 34888
		private float spinAngle;

		// Token: 0x04008849 RID: 34889
		private Vector3 spinAxis = Vector3.up;

		// Token: 0x0400884A RID: 34890
		private bool canStartSpin;

		// Token: 0x0400884B RID: 34891
		private double spinStartTime = -1.0;

		// Token: 0x0400884C RID: 34892
		private float tiltWobble;

		// Token: 0x0400884D RID: 34893
		private bool falseTargetReached;

		// Token: 0x0400884E RID: 34894
		private bool hasLanded;

		// Token: 0x0400884F RID: 34895
		private Vector3 pathOffset = Vector3.zero;

		// Token: 0x04008850 RID: 34896
		private Vector3 pathDir = Vector3.forward;

		// Token: 0x04008851 RID: 34897
		private Vector3 surfacePlanePoint;

		// Token: 0x04008852 RID: 34898
		private Vector3 surfacePlaneNormal;

		// Token: 0x04008853 RID: 34899
		private FloatSpring tiltFrontBackSpring;

		// Token: 0x04008854 RID: 34900
		private FloatSpring tiltLeftRightSpring;

		// Token: 0x04008855 RID: 34901
		private FloatSpring spinSpeedSpring;

		// Token: 0x04008856 RID: 34902
		private Vector3Spring groundPointSpring;

		// Token: 0x04008857 RID: 34903
		private Vector2[] landingTiltValues = new Vector2[]
		{
			new Vector2(1f, -1f),
			new Vector2(1f, 0f),
			new Vector2(-1f, 1f),
			new Vector2(-1f, 0f)
		};

		// Token: 0x04008858 RID: 34904
		private Vector2 landingTiltLeadingTarget = Vector2.zero;

		// Token: 0x04008859 RID: 34905
		private Vector2 landingTiltTarget = Vector2.zero;

		// Token: 0x0400885A RID: 34906
		[Header("Debug Params")]
		[SerializeField]
		private Dreidel.Side landingSide;

		// Token: 0x0400885B RID: 34907
		[SerializeField]
		private Dreidel.Variation landingVariation;

		// Token: 0x0400885C RID: 34908
		[SerializeField]
		private bool spinCounterClockwise;

		// Token: 0x0400885D RID: 34909
		[SerializeField]
		private bool debugDraw;

		// Token: 0x02001270 RID: 4720
		private enum State
		{
			// Token: 0x0400885F RID: 34911
			Idle,
			// Token: 0x04008860 RID: 34912
			FindingSurface,
			// Token: 0x04008861 RID: 34913
			Spinning,
			// Token: 0x04008862 RID: 34914
			Falling,
			// Token: 0x04008863 RID: 34915
			Fallen
		}

		// Token: 0x02001271 RID: 4721
		public enum Side
		{
			// Token: 0x04008865 RID: 34917
			Shin,
			// Token: 0x04008866 RID: 34918
			Hey,
			// Token: 0x04008867 RID: 34919
			Gimel,
			// Token: 0x04008868 RID: 34920
			Nun,
			// Token: 0x04008869 RID: 34921
			Count
		}

		// Token: 0x02001272 RID: 4722
		public enum Variation
		{
			// Token: 0x0400886B RID: 34923
			Tumble,
			// Token: 0x0400886C RID: 34924
			Smooth,
			// Token: 0x0400886D RID: 34925
			Bounce,
			// Token: 0x0400886E RID: 34926
			SlowTurn,
			// Token: 0x0400886F RID: 34927
			FalseSlowTurn,
			// Token: 0x04008870 RID: 34928
			Count
		}
	}
}
