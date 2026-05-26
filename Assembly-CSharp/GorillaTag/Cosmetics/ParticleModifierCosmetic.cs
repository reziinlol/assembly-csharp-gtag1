using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200129C RID: 4764
	public class ParticleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x0600772B RID: 30507 RVA: 0x00271798 File Offset: 0x0026F998
		private void Awake()
		{
			this.StoreOriginalValues();
			this.currentIndex = -1;
		}

		// Token: 0x0600772C RID: 30508 RVA: 0x002717A7 File Offset: 0x0026F9A7
		private void OnValidate()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x0600772D RID: 30509 RVA: 0x002717A7 File Offset: 0x0026F9A7
		private void OnEnable()
		{
			this.StoreOriginalValues();
		}

		// Token: 0x0600772E RID: 30510 RVA: 0x002717AF File Offset: 0x0026F9AF
		private void OnDisable()
		{
			this.ResetToOriginal();
		}

		// Token: 0x0600772F RID: 30511 RVA: 0x002717B8 File Offset: 0x0026F9B8
		private void StoreOriginalValues()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			this.originalStartSize = main.startSize.constant;
			this.originalStartColor = main.startColor.color;
		}

		// Token: 0x06007730 RID: 30512 RVA: 0x0027180A File Offset: 0x0026FA0A
		public void ApplySetting(ParticleSettingsSO setting)
		{
			this.SetStartSize(setting.startSize);
			this.SetStartColor(setting.startColor);
		}

		// Token: 0x06007731 RID: 30513 RVA: 0x00271824 File Offset: 0x0026FA24
		public void ApplySettingLerp(ParticleSettingsSO setting)
		{
			this.LerpStartSize(setting.startSize);
			this.LerpStartColor(setting.startColor);
		}

		// Token: 0x06007732 RID: 30514 RVA: 0x00271840 File Offset: 0x0026FA40
		public void MoveToNextSetting()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[this.currentIndex];
				this.ApplySetting(setting);
			}
		}

		// Token: 0x06007733 RID: 30515 RVA: 0x0027188C File Offset: 0x0026FA8C
		public void MoveToNextSettingLerp()
		{
			this.currentIndex++;
			if (this.currentIndex > -1 && this.currentIndex < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[this.currentIndex];
				this.ApplySettingLerp(setting);
			}
		}

		// Token: 0x06007734 RID: 30516 RVA: 0x002718D5 File Offset: 0x0026FAD5
		public void ResetSettings()
		{
			this.currentIndex = -1;
			this.ResetToOriginal();
		}

		// Token: 0x06007735 RID: 30517 RVA: 0x002718E4 File Offset: 0x0026FAE4
		public void MoveToSettingIndex(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[index];
				this.ApplySetting(setting);
			}
		}

		// Token: 0x06007736 RID: 30518 RVA: 0x00271910 File Offset: 0x0026FB10
		public void MoveToSettingIndexLerp(int index)
		{
			if (index > -1 && index < this.particleSettings.Length)
			{
				ParticleSettingsSO setting = this.particleSettings[index];
				this.ApplySettingLerp(setting);
			}
		}

		// Token: 0x06007737 RID: 30519 RVA: 0x0027193C File Offset: 0x0026FB3C
		public void SetStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startSize = size;
			this.targetSize = null;
		}

		// Token: 0x06007738 RID: 30520 RVA: 0x00271980 File Offset: 0x0026FB80
		public void IncreaseStartSize(float delta)
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			float constant = main.startSize.constant;
			main.startSize = constant + delta;
			this.targetSize = null;
		}

		// Token: 0x06007739 RID: 30521 RVA: 0x002719D4 File Offset: 0x0026FBD4
		public void LerpStartSize(float size)
		{
			if (this.ps == null)
			{
				return;
			}
			if (Mathf.Abs(this.ps.main.startSize.constant - size) < 0.01f)
			{
				return;
			}
			this.targetSize = new float?(size);
		}

		// Token: 0x0600773A RID: 30522 RVA: 0x00271A28 File Offset: 0x0026FC28
		public void SetStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			this.ps.main.startColor = color;
			this.targetColor = null;
		}

		// Token: 0x0600773B RID: 30523 RVA: 0x00271A6C File Offset: 0x0026FC6C
		public void LerpStartColor(Color color)
		{
			if (this.ps == null)
			{
				return;
			}
			Color color2 = this.ps.main.startColor.color;
			if (this.IsColorApproximatelyEqual(color2, color, 0.0001f))
			{
				return;
			}
			this.targetColor = new Color?(color);
		}

		// Token: 0x0600773C RID: 30524 RVA: 0x00271AC0 File Offset: 0x0026FCC0
		public void SetStartValues(float size, Color color)
		{
			this.SetStartSize(size);
			this.SetStartColor(color);
		}

		// Token: 0x0600773D RID: 30525 RVA: 0x00271AD0 File Offset: 0x0026FCD0
		public void LerpStartValues(float size, Color color)
		{
			this.LerpStartSize(size);
			this.LerpStartColor(color);
		}

		// Token: 0x0600773E RID: 30526 RVA: 0x00271AE0 File Offset: 0x0026FCE0
		private void Update()
		{
			if (this.ps == null)
			{
				return;
			}
			ParticleSystem.MainModule main = this.ps.main;
			if (this.targetSize != null)
			{
				float num = Mathf.Lerp(main.startSize.constant, this.targetSize.Value, Time.deltaTime * this.transitionSpeed);
				main.startSize = num;
				if (Mathf.Abs(num - this.targetSize.Value) < 0.01f)
				{
					main.startSize = this.targetSize.Value;
					this.targetSize = null;
				}
			}
			if (this.targetColor != null)
			{
				Color color = Color.Lerp(main.startColor.color, this.targetColor.Value, Time.deltaTime * this.transitionSpeed);
				main.startColor = color;
				if (this.IsColorApproximatelyEqual(color, this.targetColor.Value, 0.0001f))
				{
					main.startColor = this.targetColor.Value;
					this.targetColor = null;
				}
			}
		}

		// Token: 0x0600773F RID: 30527 RVA: 0x00271C10 File Offset: 0x0026FE10
		[ContextMenu("Reset To Original")]
		public void ResetToOriginal()
		{
			if (this.ps == null)
			{
				return;
			}
			this.targetSize = null;
			this.targetColor = null;
			ParticleSystem.MainModule main = this.ps.main;
			main.startSize = this.originalStartSize;
			main.startColor = this.originalStartColor;
		}

		// Token: 0x06007740 RID: 30528 RVA: 0x00271C74 File Offset: 0x0026FE74
		private bool IsColorApproximatelyEqual(Color a, Color b, float threshold = 0.0001f)
		{
			float num = a.r - b.r;
			float num2 = a.g - b.g;
			float num3 = a.b - b.b;
			float num4 = a.a - b.a;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4 < threshold;
		}

		// Token: 0x04008980 RID: 35200
		[SerializeField]
		private ParticleSystem ps;

		// Token: 0x04008981 RID: 35201
		[Tooltip("For calling gradual functions only")]
		[SerializeField]
		private float transitionSpeed = 5f;

		// Token: 0x04008982 RID: 35202
		public ParticleSettingsSO[] particleSettings = new ParticleSettingsSO[0];

		// Token: 0x04008983 RID: 35203
		private float originalStartSize;

		// Token: 0x04008984 RID: 35204
		private Color originalStartColor;

		// Token: 0x04008985 RID: 35205
		private float? targetSize;

		// Token: 0x04008986 RID: 35206
		private Color? targetColor;

		// Token: 0x04008987 RID: 35207
		private int currentIndex;
	}
}
