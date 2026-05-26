using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001232 RID: 4658
	public class MedusaEyeLantern : MonoBehaviour
	{
		// Token: 0x0600748B RID: 29835 RVA: 0x002626FC File Offset: 0x002608FC
		private void Awake()
		{
			foreach (MedusaEyeLantern.EyeState eyeState in this.allStates)
			{
				this.allStatesDict.Add(eyeState.eyeState, eyeState);
			}
		}

		// Token: 0x0600748C RID: 29836 RVA: 0x00262734 File Offset: 0x00260934
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x0600748D RID: 29837 RVA: 0x00262741 File Offset: 0x00260941
		private void Start()
		{
			if (this.rotatingObjectTransform == null)
			{
				this.rotatingObjectTransform = base.transform;
			}
			this.initialRotation = this.rotatingObjectTransform.localRotation;
			this.SwitchState(MedusaEyeLantern.State.DORMANT);
		}

		// Token: 0x0600748E RID: 29838 RVA: 0x00262778 File Offset: 0x00260978
		private void Update()
		{
			if (!this.transferableParent.InHand() && this.currentState != MedusaEyeLantern.State.DORMANT)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
			if (!this.transferableParent.InHand())
			{
				return;
			}
			this.UpdateState();
			if (this.velocityTracker == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			float x = Mathf.Clamp(-normalized.z, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			float z = Mathf.Clamp(normalized.x, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			this.targetRotation = this.initialRotation * Quaternion.Euler(x, 0f, z);
			if (magnitude > this.sloshVelocityThreshold)
			{
				this.SwitchState(MedusaEyeLantern.State.SLOSHING);
			}
			if ((double)magnitude < 0.01)
			{
				this.targetRotation = this.initialRotation;
			}
			if (!this.EyeIsLockedOn())
			{
				this.rotatingObjectTransform.localRotation = Quaternion.Slerp(this.rotatingObjectTransform.localRotation, this.targetRotation, Time.deltaTime * this.rotationSmoothing);
			}
		}

		// Token: 0x0600748F RID: 29839 RVA: 0x002628D6 File Offset: 0x00260AD6
		public void HandleOnNoOneInRange()
		{
			this.SwitchState(MedusaEyeLantern.State.RESET);
			this.resetTargetTime = Time.time;
			this.rotatingObjectTransform.localRotation = this.initialRotation;
		}

		// Token: 0x06007490 RID: 29840 RVA: 0x002628FB File Offset: 0x00260AFB
		public void HandleOnNewPlayerDetected(VRRig target, float distance)
		{
			this.targetRig = target;
			if (this.currentState != MedusaEyeLantern.State.SLOSHING)
			{
				this.SwitchState(MedusaEyeLantern.State.TRACKING);
			}
		}

		// Token: 0x06007491 RID: 29841 RVA: 0x00262914 File Offset: 0x00260B14
		private void Sloshing()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			if ((double)vector.magnitude < 0.01)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
		}

		// Token: 0x06007492 RID: 29842 RVA: 0x00262968 File Offset: 0x00260B68
		private void FaceTarget()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 normalized = (this.targetRig.tagSound.transform.position - this.rotatingObjectTransform.position).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.z).normalized;
			Debug.DrawRay(this.rotatingObjectTransform.position, this.rotatingObjectTransform.forward * 0.3f, Color.blue);
			Debug.DrawRay(this.rotatingObjectTransform.position, normalized2 * 0.3f, Color.green);
			if (normalized2.sqrMagnitude > 0.001f)
			{
				float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(this.rotatingObjectTransform.forward.normalized, normalized2), -1f, 1f)) * 57.29578f;
				if (180f - num < this.targetHeadAngleThreshold && this.currentState == MedusaEyeLantern.State.TRACKING)
				{
					this.SwitchState(MedusaEyeLantern.State.WARMUP);
					return;
				}
				Quaternion to = Quaternion.LookRotation(-normalized2, Vector3.up);
				this.rotatingObjectTransform.rotation = Quaternion.RotateTowards(this.rotatingObjectTransform.rotation, to, this.lookAtTargetSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06007493 RID: 29843 RVA: 0x00262ACC File Offset: 0x00260CCC
		private bool IsTargetLookingAtEye()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return false;
			}
			Transform transform = this.targetRig.tagSound.transform;
			Vector3 normalized = (this.rotatingObjectTransform.position - this.rotatingObjectTransform.forward * this.faceDistanceOffset - transform.position).normalized;
			float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.up.normalized, normalized), -1f, 1f)) * 57.29578f;
			Debug.DrawRay(transform.position, transform.up * 0.3f, Color.magenta);
			Debug.DrawRay(transform.position, normalized * 0.3f, Color.yellow);
			return num < this.lookAtEyeAngleThreshold;
		}

		// Token: 0x06007494 RID: 29844 RVA: 0x00262BB4 File Offset: 0x00260DB4
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case MedusaEyeLantern.State.SLOSHING:
				this.Sloshing();
				break;
			case MedusaEyeLantern.State.DORMANT:
				this.warmupCounter = 0f;
				this.petrificationStarted = float.PositiveInfinity;
				if (this.targetRig != null && (this.targetRig.transform.position - base.transform.position).IsShorterThan(this.distanceChecker.distanceThreshold))
				{
					this.SwitchState(MedusaEyeLantern.State.TRACKING);
				}
				break;
			case MedusaEyeLantern.State.TRACKING:
				this.FaceTarget();
				break;
			case MedusaEyeLantern.State.WARMUP:
				this.warmupCounter += Time.deltaTime;
				this.FaceTarget();
				if (this.warmupCounter > this.warmUpProgressTime)
				{
					this.SwitchState(MedusaEyeLantern.State.PRIMING);
					this.warmupCounter = 0f;
				}
				break;
			case MedusaEyeLantern.State.PRIMING:
				this.FaceTarget();
				if (this.IsTargetLookingAtEye())
				{
					UnityEvent<VRRig> onPetrification = this.OnPetrification;
					if (onPetrification != null)
					{
						onPetrification.Invoke(this.targetRig);
					}
					this.SwitchState(MedusaEyeLantern.State.PETRIFICATION);
					this.petrificationStarted = Time.time;
				}
				break;
			case MedusaEyeLantern.State.PETRIFICATION:
				if (Time.time - this.petrificationStarted > this.petrificationDuration)
				{
					this.SwitchState(MedusaEyeLantern.State.COOLDOWN);
				}
				break;
			case MedusaEyeLantern.State.COOLDOWN:
				if (Time.time - this.petrificationStarted > this.resetCooldown)
				{
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
					this.petrificationStarted = float.PositiveInfinity;
				}
				break;
			case MedusaEyeLantern.State.RESET:
				if (Time.time - this.resetTargetTime > this.resetTargetTimer)
				{
					this.resetTargetTime = float.PositiveInfinity;
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
				}
				break;
			}
			this.PlayHaptic(this.currentState);
		}

		// Token: 0x06007495 RID: 29845 RVA: 0x00262D64 File Offset: 0x00260F64
		private void SwitchState(MedusaEyeLantern.State newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			MedusaEyeLantern.EyeState eyeState;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(newState, out eyeState))
			{
				UnityEvent onEnterState = eyeState.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
			}
			MedusaEyeLantern.EyeState eyeState2;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(this.lastState, out eyeState2))
			{
				UnityEvent onExitState = eyeState2.onExitState;
				if (onExitState == null)
				{
					return;
				}
				onExitState.Invoke();
			}
		}

		// Token: 0x06007496 RID: 29846 RVA: 0x00262DE8 File Offset: 0x00260FE8
		private void PlayHaptic(MedusaEyeLantern.State state)
		{
			if (!this.transferableParent.IsMyItem())
			{
				return;
			}
			MedusaEyeLantern.EyeState eyeState;
			this.allStatesDict.TryGetValue(state, out eyeState);
			if (this.currentState == MedusaEyeLantern.State.WARMUP)
			{
				float time = Mathf.Clamp01(this.warmupCounter / this.warmUpProgressTime);
				if (eyeState != null && eyeState.hapticStrength != null)
				{
					float amplitude = eyeState.hapticStrength.Evaluate(time);
					bool forLeftController = this.transferableParent.InLeftHand();
					GorillaTagger.Instance.StartVibration(forLeftController, amplitude, Time.deltaTime);
					return;
				}
			}
			else if (eyeState != null && eyeState.hapticStrength != null)
			{
				float amplitude2 = eyeState.hapticStrength.Evaluate(0.5f);
				bool forLeftController2 = this.transferableParent.InLeftHand();
				GorillaTagger.Instance.StartVibration(forLeftController2, amplitude2, Time.deltaTime);
			}
		}

		// Token: 0x06007497 RID: 29847 RVA: 0x00262EA1 File Offset: 0x002610A1
		private bool EyeIsLockedOn()
		{
			return this.currentState == MedusaEyeLantern.State.TRACKING || this.currentState == MedusaEyeLantern.State.WARMUP || this.currentState == MedusaEyeLantern.State.PRIMING;
		}

		// Token: 0x040085C3 RID: 34243
		[SerializeField]
		private DistanceCheckerCosmetic distanceChecker;

		// Token: 0x040085C4 RID: 34244
		[SerializeField]
		private TransferrableObject transferableParent;

		// Token: 0x040085C5 RID: 34245
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x040085C6 RID: 34246
		[SerializeField]
		private Transform rotatingObjectTransform;

		// Token: 0x040085C7 RID: 34247
		[Space]
		[Header("Rotation Settings")]
		[SerializeField]
		private float maxRotationAngle = 50f;

		// Token: 0x040085C8 RID: 34248
		[SerializeField]
		private float sloshVelocityThreshold = 1f;

		// Token: 0x040085C9 RID: 34249
		[SerializeField]
		private float rotationSmoothing = 10f;

		// Token: 0x040085CA RID: 34250
		[SerializeField]
		private float rotationSpeedMultiplier = 5f;

		// Token: 0x040085CB RID: 34251
		[Space]
		[Header("Target Tracking Settings")]
		[SerializeField]
		private float lookAtEyeAngleThreshold = 90f;

		// Token: 0x040085CC RID: 34252
		[SerializeField]
		private float targetHeadAngleThreshold = 5f;

		// Token: 0x040085CD RID: 34253
		[SerializeField]
		private float lookAtTargetSpeed = 5f;

		// Token: 0x040085CE RID: 34254
		[SerializeField]
		private float warmUpProgressTime = 3f;

		// Token: 0x040085CF RID: 34255
		[SerializeField]
		private float resetCooldown = 5f;

		// Token: 0x040085D0 RID: 34256
		[SerializeField]
		private float faceDistanceOffset = 0.2f;

		// Token: 0x040085D1 RID: 34257
		[SerializeField]
		private float petrificationDuration = 0.2f;

		// Token: 0x040085D2 RID: 34258
		[Space]
		[Header("Eye State Settings")]
		public MedusaEyeLantern.EyeState[] allStates = new MedusaEyeLantern.EyeState[0];

		// Token: 0x040085D3 RID: 34259
		public UnityEvent<VRRig> OnPetrification;

		// Token: 0x040085D4 RID: 34260
		private Quaternion initialRotation;

		// Token: 0x040085D5 RID: 34261
		private Quaternion targetRotation;

		// Token: 0x040085D6 RID: 34262
		private MedusaEyeLantern.State currentState;

		// Token: 0x040085D7 RID: 34263
		private MedusaEyeLantern.State lastState;

		// Token: 0x040085D8 RID: 34264
		private float petrificationStarted = float.PositiveInfinity;

		// Token: 0x040085D9 RID: 34265
		private float warmupCounter;

		// Token: 0x040085DA RID: 34266
		private Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState> allStatesDict = new Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState>();

		// Token: 0x040085DB RID: 34267
		private VRRig targetRig;

		// Token: 0x040085DC RID: 34268
		private float resetTargetTimer = 1f;

		// Token: 0x040085DD RID: 34269
		private float resetTargetTime = float.PositiveInfinity;

		// Token: 0x02001233 RID: 4659
		[Serializable]
		public class EyeState
		{
			// Token: 0x040085DE RID: 34270
			public MedusaEyeLantern.State eyeState;

			// Token: 0x040085DF RID: 34271
			public AnimationCurve hapticStrength;

			// Token: 0x040085E0 RID: 34272
			public UnityEvent onEnterState;

			// Token: 0x040085E1 RID: 34273
			public UnityEvent onExitState;
		}

		// Token: 0x02001234 RID: 4660
		public enum State
		{
			// Token: 0x040085E3 RID: 34275
			SLOSHING,
			// Token: 0x040085E4 RID: 34276
			DORMANT,
			// Token: 0x040085E5 RID: 34277
			TRACKING,
			// Token: 0x040085E6 RID: 34278
			WARMUP,
			// Token: 0x040085E7 RID: 34279
			PRIMING,
			// Token: 0x040085E8 RID: 34280
			PETRIFICATION,
			// Token: 0x040085E9 RID: 34281
			COOLDOWN,
			// Token: 0x040085EA RID: 34282
			RESET
		}
	}
}
