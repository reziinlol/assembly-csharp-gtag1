using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x020010E9 RID: 4329
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSwimmingParameters", order = 1)]
	public class PlayerSwimmingParameters : ScriptableObject
	{
		// Token: 0x04007DAA RID: 32170
		[Header("Base Settings")]
		public float floatingWaterLevelBelowHead = 0.6f;

		// Token: 0x04007DAB RID: 32171
		public float buoyancyFadeDist = 0.3f;

		// Token: 0x04007DAC RID: 32172
		public bool extendBouyancyFromSpeed;

		// Token: 0x04007DAD RID: 32173
		public float buoyancyExtensionDecayHalflife = 0.2f;

		// Token: 0x04007DAE RID: 32174
		public float baseUnderWaterDampingHalfLife = 0.25f;

		// Token: 0x04007DAF RID: 32175
		public float swimUnderWaterDampingHalfLife = 1.1f;

		// Token: 0x04007DB0 RID: 32176
		public AnimationCurve speedToBouyancyExtension = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DB1 RID: 32177
		public Vector2 speedToBouyancyExtensionMinMax = Vector2.zero;

		// Token: 0x04007DB2 RID: 32178
		public float swimmingVelocityOutOfWaterDrainRate = 3f;

		// Token: 0x04007DB3 RID: 32179
		[Range(0f, 1f)]
		public float underwaterJumpsAsSwimVelocityFactor = 1f;

		// Token: 0x04007DB4 RID: 32180
		[Range(0f, 1f)]
		public float swimmingHapticsStrength = 0.5f;

		// Token: 0x04007DB5 RID: 32181
		[Header("Surface Jumping")]
		public bool allowWaterSurfaceJumps;

		// Token: 0x04007DB6 RID: 32182
		public float waterSurfaceJumpHandSpeedThreshold = 1f;

		// Token: 0x04007DB7 RID: 32183
		public float waterSurfaceJumpAmount;

		// Token: 0x04007DB8 RID: 32184
		public float waterSurfaceJumpMaxSpeed = 1f;

		// Token: 0x04007DB9 RID: 32185
		public AnimationCurve waterSurfaceJumpPalmFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DBA RID: 32186
		public AnimationCurve waterSurfaceJumpHandVelocityFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DBB RID: 32187
		[Header("Diving")]
		public bool applyDiveSteering;

		// Token: 0x04007DBC RID: 32188
		public bool applyDiveDampingMultiplier;

		// Token: 0x04007DBD RID: 32189
		public float diveDampingMultiplier = 1f;

		// Token: 0x04007DBE RID: 32190
		[Tooltip("In degrees")]
		public float maxDiveSteerAnglePerStep = 1f;

		// Token: 0x04007DBF RID: 32191
		public float diveVelocityAveragingWindow = 0.1f;

		// Token: 0x04007DC0 RID: 32192
		public bool applyDiveSwimVelocityConversion;

		// Token: 0x04007DC1 RID: 32193
		[Tooltip("In meters per second")]
		public float diveSwimVelocityConversionRate = 3f;

		// Token: 0x04007DC2 RID: 32194
		public float diveMaxSwimVelocityConversion = 3f;

		// Token: 0x04007DC3 RID: 32195
		public bool reduceDiveSteeringBelowVelocityPlane;

		// Token: 0x04007DC4 RID: 32196
		public float reduceDiveSteeringBelowPlaneFadeStartDist = 0.4f;

		// Token: 0x04007DC5 RID: 32197
		public float reduceDiveSteeringBelowPlaneFadeEndDist = 0.55f;

		// Token: 0x04007DC6 RID: 32198
		public AnimationCurve palmFacingToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DC7 RID: 32199
		public Vector2 palmFacingToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04007DC8 RID: 32200
		public AnimationCurve swimSpeedToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DC9 RID: 32201
		public Vector2 swimSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04007DCA RID: 32202
		public AnimationCurve swimSpeedToMaxRedirectAngle = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DCB RID: 32203
		public Vector2 swimSpeedToMaxRedirectAngleMinMax = Vector2.zero;

		// Token: 0x04007DCC RID: 32204
		public AnimationCurve handSpeedToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04007DCD RID: 32205
		public Vector2 handSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04007DCE RID: 32206
		public AnimationCurve handAccelToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04007DCF RID: 32207
		public Vector2 handAccelToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04007DD0 RID: 32208
		public AnimationCurve nonDiveDampingHapticsAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007DD1 RID: 32209
		public Vector2 nonDiveDampingHapticsAmountMinMax = Vector2.zero;
	}
}
