using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200128C RID: 4748
	public class HandControlledSettingsSO : ScriptableObject
	{
		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x060076F1 RID: 30449 RVA: 0x002705F4 File Offset: 0x0026E7F4
		private bool IsAngle
		{
			get
			{
				return this.rotationControl == HandControlledCosmetic.RotationControl.Angle;
			}
		}

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x060076F2 RID: 30450 RVA: 0x002705FF File Offset: 0x0026E7FF
		private bool IsTranslation
		{
			get
			{
				return this.rotationControl == HandControlledCosmetic.RotationControl.Translation;
			}
		}

		// Token: 0x0400892F RID: 35119
		private const string SENS_TT = "The difference between the current input and cached input is magnified by this number.";

		// Token: 0x04008930 RID: 35120
		public HandControlledCosmetic.RotationControl rotationControl;

		// Token: 0x04008931 RID: 35121
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public float inputSensitivity = 2f;

		// Token: 0x04008932 RID: 35122
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public AnimationCurve verticalSensitivity = AnimationCurve.Constant(0f, 1f, 2f);

		// Token: 0x04008933 RID: 35123
		[Tooltip("The difference between the current input and cached input is magnified by this number.")]
		public AnimationCurve horizontalSensitivity = AnimationCurve.Constant(0f, 1f, 2f);

		// Token: 0x04008934 RID: 35124
		[Tooltip("How quickly the cached input approaches the current input. A high value will function more like a mouse, while a low value will function more like a joystick.")]
		public float inputDecaySpeed = 1f;

		// Token: 0x04008935 RID: 35125
		[Tooltip("How quickly the cached input approaches the current input, as a function of distance. A high value will function more like a mouse, while a low value will function more like a joystick.")]
		public AnimationCurve inputDecayCurve = AnimationCurve.Constant(0f, 2f, 1f);

		// Token: 0x04008936 RID: 35126
		[Tooltip("How quickly the transform approaches the intended angle (smaller value = more lag).")]
		public float rotationSpeed = 20f;

		// Token: 0x04008937 RID: 35127
		[Tooltip("The transform's local rotation cannot exceed these euler angles.")]
		public Vector3 angleLimits = new Vector3(45f, 360f, 0f);
	}
}
