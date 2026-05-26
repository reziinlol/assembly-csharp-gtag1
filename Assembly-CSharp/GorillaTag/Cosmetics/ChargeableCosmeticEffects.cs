using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001248 RID: 4680
	public class ChargeableCosmeticEffects : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007542 RID: 30018 RVA: 0x00266DBE File Offset: 0x00264FBE
		private bool HasFractionals()
		{
			return this.continuousProperties.Count > 0 || this.whileCharging.GetPersistentEventCount() > 0;
		}

		// Token: 0x06007543 RID: 30019 RVA: 0x00266DDE File Offset: 0x00264FDE
		private void Awake()
		{
			this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
			this.hasFractionalsCached = this.HasFractionals();
		}

		// Token: 0x06007544 RID: 30020 RVA: 0x00266DFE File Offset: 0x00264FFE
		public void SetMaxChargeSeconds(float s)
		{
			this.maxChargeSeconds = s;
			this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
			this.SetChargeTime(this.chargeTime);
		}

		// Token: 0x06007545 RID: 30021 RVA: 0x00266E25 File Offset: 0x00265025
		public void SetChargeState(bool state)
		{
			if (this.isCharging != state)
			{
				TickSystem<object>.AddTickCallback(this);
				this.isCharging = state;
			}
		}

		// Token: 0x06007546 RID: 30022 RVA: 0x00266E3D File Offset: 0x0026503D
		public void StartCharging()
		{
			this.SetChargeState(true);
		}

		// Token: 0x06007547 RID: 30023 RVA: 0x00266E46 File Offset: 0x00265046
		public void StopCharging()
		{
			this.SetChargeState(false);
		}

		// Token: 0x06007548 RID: 30024 RVA: 0x00266E4F File Offset: 0x0026504F
		public void ToggleCharging()
		{
			this.SetChargeState(!this.isCharging);
		}

		// Token: 0x06007549 RID: 30025 RVA: 0x00266E60 File Offset: 0x00265060
		public void SetChargeTime(float t)
		{
			if (t >= this.maxChargeSeconds)
			{
				if (this.chargeTime < this.maxChargeSeconds)
				{
					this.RunMaxCharge();
					return;
				}
			}
			else if (t <= 0f)
			{
				if (this.chargeTime > 0f)
				{
					this.RunNoCharge();
					return;
				}
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
				this.chargeTime = t;
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
				}
			}
		}

		// Token: 0x0600754A RID: 30026 RVA: 0x00266EC2 File Offset: 0x002650C2
		public void SetChargeFrac(float f)
		{
			this.SetChargeTime(f * this.maxChargeSeconds);
		}

		// Token: 0x0600754B RID: 30027 RVA: 0x00266ED2 File Offset: 0x002650D2
		public void EmptyCharge()
		{
			this.SetChargeTime(0f);
		}

		// Token: 0x0600754C RID: 30028 RVA: 0x00266EDF File Offset: 0x002650DF
		public void FillCharge()
		{
			this.SetChargeTime(this.maxChargeSeconds);
		}

		// Token: 0x0600754D RID: 30029 RVA: 0x00266EED File Offset: 0x002650ED
		public void EmptyAndStop()
		{
			this.isCharging = false;
			this.EmptyCharge();
		}

		// Token: 0x0600754E RID: 30030 RVA: 0x00266EFC File Offset: 0x002650FC
		public void FillAndStop()
		{
			this.StopCharging();
			this.FillCharge();
		}

		// Token: 0x0600754F RID: 30031 RVA: 0x00266F0A File Offset: 0x0026510A
		public void EmptyAndStart()
		{
			this.StartCharging();
			this.EmptyCharge();
		}

		// Token: 0x06007550 RID: 30032 RVA: 0x00266F18 File Offset: 0x00265118
		public void FillAndStart()
		{
			this.isCharging = true;
			this.FillCharge();
		}

		// Token: 0x06007551 RID: 30033 RVA: 0x00266F28 File Offset: 0x00265128
		private void OnEnable()
		{
			if ((this.chargeTime <= 0f && this.isCharging) || (this.chargeTime >= this.maxChargeSeconds && !this.isCharging) || (this.chargeTime > 0f && this.chargeTime < this.maxChargeSeconds))
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06007552 RID: 30034 RVA: 0x00019E47 File Offset: 0x00018047
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007553 RID: 30035 RVA: 0x00266F84 File Offset: 0x00265184
		private void RunMaxCharge()
		{
			if (this.isCharging)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.chargeTime = this.maxChargeSeconds;
			UnityEvent unityEvent = this.onMaxCharge;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(1f);
			}
			this.continuousProperties.ApplyAll(1f);
		}

		// Token: 0x06007554 RID: 30036 RVA: 0x00266FEC File Offset: 0x002651EC
		private void RunNoCharge()
		{
			if (!this.isCharging)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.chargeTime = 0f;
			UnityEvent unityEvent = this.onNoCharge;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(0f);
			}
			this.continuousProperties.ApplyAll(0f);
		}

		// Token: 0x06007555 RID: 30037 RVA: 0x00267054 File Offset: 0x00265254
		private void RunChargeFrac()
		{
			float num = this.masterChargeRemapCurve.Evaluate(this.chargeTime * this.inverseMaxChargeSeconds);
			UnityEvent<float> unityEvent = this.whileCharging;
			if (unityEvent != null)
			{
				unityEvent.Invoke(num);
			}
			this.continuousProperties.ApplyAll(num);
		}

		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x06007556 RID: 30038 RVA: 0x00267098 File Offset: 0x00265298
		// (set) Token: 0x06007557 RID: 30039 RVA: 0x002670A0 File Offset: 0x002652A0
		public bool TickRunning { get; set; }

		// Token: 0x06007558 RID: 30040 RVA: 0x002670AC File Offset: 0x002652AC
		public void Tick()
		{
			if (this.isCharging && this.chargeTime < this.maxChargeSeconds)
			{
				this.chargeTime += Time.deltaTime * this.chargeGainSpeed;
				if (this.chargeTime >= this.maxChargeSeconds)
				{
					this.RunMaxCharge();
					return;
				}
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
					return;
				}
			}
			else if (!this.isCharging && this.chargeTime > 0f)
			{
				this.chargeTime -= Time.deltaTime * this.chargeLossSpeed;
				if (this.chargeTime <= 0f)
				{
					this.RunNoCharge();
					return;
				}
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
				}
			}
		}

		// Token: 0x040086E0 RID: 34528
		[SerializeField]
		private float maxChargeSeconds = 1f;

		// Token: 0x040086E1 RID: 34529
		[SerializeField]
		private float chargeGainSpeed = 1f;

		// Token: 0x040086E2 RID: 34530
		[SerializeField]
		private float chargeLossSpeed = 1f;

		// Token: 0x040086E3 RID: 34531
		[Tooltip("This will remap the internal charge output to whatever you set. The remapped value will be output by 'whileCharging' and the 'continuousProperties' (keep in mind that the remapped value will then be used as an INPUT for the curves on each ContinuousProperty).\n\nIt should start at (0,0) and end at (1,1).\n\nDisabled if there are no ContinuousProperties and no whileCharging event callbacks.")]
		[SerializeField]
		private AnimationCurve masterChargeRemapCurve = AnimationCurves.Linear;

		// Token: 0x040086E4 RID: 34532
		[SerializeField]
		private bool isCharging;

		// Token: 0x040086E5 RID: 34533
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x040086E6 RID: 34534
		[SerializeField]
		private UnityEvent<float> whileCharging;

		// Token: 0x040086E7 RID: 34535
		[SerializeField]
		private UnityEvent onMaxCharge;

		// Token: 0x040086E8 RID: 34536
		[SerializeField]
		private UnityEvent onNoCharge;

		// Token: 0x040086E9 RID: 34537
		private float chargeTime;

		// Token: 0x040086EA RID: 34538
		private float inverseMaxChargeSeconds;

		// Token: 0x040086EB RID: 34539
		private bool hasFractionalsCached;
	}
}
