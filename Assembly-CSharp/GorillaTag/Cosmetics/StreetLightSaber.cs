using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012AC RID: 4780
	public class StreetLightSaber : MonoBehaviour
	{
		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x060077A1 RID: 30625 RVA: 0x00273E17 File Offset: 0x00272017
		private StreetLightSaber.State CurrentState
		{
			get
			{
				return StreetLightSaber.values[this.currentIndex];
			}
		}

		// Token: 0x060077A2 RID: 30626 RVA: 0x00273E28 File Offset: 0x00272028
		private void Awake()
		{
			foreach (StreetLightSaber.StaffStates staffStates in this.allStates)
			{
				this.allStatesDict[staffStates.state] = staffStates;
			}
			this.currentIndex = 0;
			this.autoSwitchEnabledTime = 0f;
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(out list))
			{
				this.meshRenderer.GetSharedMaterials(list);
				this.instancedMaterial = new Material(list[this.materialIndex]);
				list[this.materialIndex] = this.instancedMaterial;
				this.meshRenderer.SetSharedMaterials(list);
			}
		}

		// Token: 0x060077A3 RID: 30627 RVA: 0x00273EF0 File Offset: 0x002720F0
		private void Update()
		{
			if (this.autoSwitch && Time.time - this.autoSwitchEnabledTime > this.autoSwitchTimer)
			{
				this.UpdateStateAuto();
			}
		}

		// Token: 0x060077A4 RID: 30628 RVA: 0x00273F14 File Offset: 0x00272114
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x060077A5 RID: 30629 RVA: 0x00273F21 File Offset: 0x00272121
		private void OnEnable()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x060077A6 RID: 30630 RVA: 0x00273F2C File Offset: 0x0027212C
		public void UpdateStateManual()
		{
			int newIndex = (this.currentIndex + 1) % StreetLightSaber.values.Length;
			this.SwitchState(newIndex);
		}

		// Token: 0x060077A7 RID: 30631 RVA: 0x00273F54 File Offset: 0x00272154
		private void UpdateStateAuto()
		{
			StreetLightSaber.State value = (this.CurrentState == StreetLightSaber.State.Green) ? StreetLightSaber.State.Red : StreetLightSaber.State.Green;
			int newIndex = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, value);
			this.SwitchState(newIndex);
			this.autoSwitchEnabledTime = Time.time;
		}

		// Token: 0x060077A8 RID: 30632 RVA: 0x00273F8D File Offset: 0x0027218D
		public void EnableAutoSwitch(bool enable)
		{
			this.autoSwitch = enable;
		}

		// Token: 0x060077A9 RID: 30633 RVA: 0x00273F21 File Offset: 0x00272121
		public void ResetStaff()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x060077AA RID: 30634 RVA: 0x00273F98 File Offset: 0x00272198
		public void HitReceived(Vector3 contact)
		{
			if (this.velocityTracker != null && this.velocityTracker.GetLatestVelocity(true).magnitude >= this.minHitVelocityThreshold)
			{
				StreetLightSaber.StaffStates staffStates = this.allStatesDict[this.CurrentState];
				if (staffStates == null)
				{
					return;
				}
				staffStates.OnSuccessfulHit.Invoke(contact);
			}
		}

		// Token: 0x060077AB RID: 30635 RVA: 0x00273FF0 File Offset: 0x002721F0
		private void SwitchState(int newIndex)
		{
			if (newIndex == this.currentIndex)
			{
				return;
			}
			StreetLightSaber.State currentState = this.CurrentState;
			StreetLightSaber.State key = StreetLightSaber.values[newIndex];
			StreetLightSaber.StaffStates staffStates;
			if (this.allStatesDict.TryGetValue(currentState, out staffStates))
			{
				UnityEvent onExitState = staffStates.onExitState;
				if (onExitState != null)
				{
					onExitState.Invoke();
				}
			}
			this.currentIndex = newIndex;
			StreetLightSaber.StaffStates staffStates2;
			if (this.allStatesDict.TryGetValue(key, out staffStates2))
			{
				UnityEvent onEnterState = staffStates2.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
				if (this.trailRenderer != null)
				{
					this.trailRenderer.startColor = staffStates2.color;
				}
				if (this.meshRenderer != null)
				{
					this.instancedMaterial.SetColor(this.hashId, staffStates2.color);
				}
			}
		}

		// Token: 0x060077AC RID: 30636 RVA: 0x002740A4 File Offset: 0x002722A4
		private void ForceSwitchTo(StreetLightSaber.State targetState)
		{
			int num = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, targetState);
			if (num >= 0)
			{
				this.SwitchState(num);
			}
		}

		// Token: 0x04008A35 RID: 35381
		[SerializeField]
		private float autoSwitchTimer = 5f;

		// Token: 0x04008A36 RID: 35382
		[SerializeField]
		private TrailRenderer trailRenderer;

		// Token: 0x04008A37 RID: 35383
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04008A38 RID: 35384
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04008A39 RID: 35385
		[SerializeField]
		private int materialIndex;

		// Token: 0x04008A3A RID: 35386
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04008A3B RID: 35387
		[SerializeField]
		private float minHitVelocityThreshold;

		// Token: 0x04008A3C RID: 35388
		private static readonly StreetLightSaber.State[] values = (StreetLightSaber.State[])Enum.GetValues(typeof(StreetLightSaber.State));

		// Token: 0x04008A3D RID: 35389
		[Space]
		[Header("Staff State Settings")]
		public StreetLightSaber.StaffStates[] allStates = new StreetLightSaber.StaffStates[0];

		// Token: 0x04008A3E RID: 35390
		private int currentIndex;

		// Token: 0x04008A3F RID: 35391
		private Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates> allStatesDict = new Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates>();

		// Token: 0x04008A40 RID: 35392
		private bool autoSwitch;

		// Token: 0x04008A41 RID: 35393
		private float autoSwitchEnabledTime;

		// Token: 0x04008A42 RID: 35394
		private int hashId;

		// Token: 0x04008A43 RID: 35395
		private Material instancedMaterial;

		// Token: 0x020012AD RID: 4781
		[Serializable]
		public class StaffStates
		{
			// Token: 0x04008A44 RID: 35396
			public StreetLightSaber.State state;

			// Token: 0x04008A45 RID: 35397
			public Color color;

			// Token: 0x04008A46 RID: 35398
			public UnityEvent onEnterState;

			// Token: 0x04008A47 RID: 35399
			public UnityEvent onExitState;

			// Token: 0x04008A48 RID: 35400
			public UnityEvent<Vector3> OnSuccessfulHit;
		}

		// Token: 0x020012AE RID: 4782
		public enum State
		{
			// Token: 0x04008A4A RID: 35402
			Off,
			// Token: 0x04008A4B RID: 35403
			Green,
			// Token: 0x04008A4C RID: 35404
			Red
		}
	}
}
