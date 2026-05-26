using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200122A RID: 4650
	public class AOESender : MonoBehaviour
	{
		// Token: 0x06007450 RID: 29776 RVA: 0x002612A3 File Offset: 0x0025F4A3
		private void Awake()
		{
			if (this.hits == null || this.hits.Length != this.maxColliders)
			{
				this.hits = new Collider[Mathf.Max(8, this.maxColliders)];
			}
		}

		// Token: 0x06007451 RID: 29777 RVA: 0x002612D4 File Offset: 0x0025F4D4
		private void OnEnable()
		{
			if (this.applyOnEnable)
			{
				this.ApplyAOE();
			}
			this.nextTime = Time.time + this.repeatInterval;
		}

		// Token: 0x06007452 RID: 29778 RVA: 0x002612F6 File Offset: 0x0025F4F6
		private void Update()
		{
			if (this.repeatInterval > 0f && Time.time >= this.nextTime)
			{
				this.ApplyAOE();
				this.nextTime = Time.time + this.repeatInterval;
			}
		}

		// Token: 0x06007453 RID: 29779 RVA: 0x0026132A File Offset: 0x0025F52A
		public void ApplyAOE()
		{
			this.ApplyAOE(base.transform.position);
		}

		// Token: 0x06007454 RID: 29780 RVA: 0x00261340 File Offset: 0x0025F540
		public void ApplyAOE(Vector3 worldOrigin)
		{
			this.visited.Clear();
			int num = Physics.OverlapSphereNonAlloc(worldOrigin, this.radius, this.hits, this.layerMask, this.triggerInteraction);
			float num2 = Mathf.Max(0.0001f, this.radius);
			for (int i = 0; i < num; i++)
			{
				Collider collider = this.hits[i];
				if (collider)
				{
					AOEReceiver componentInChildren = (collider.attachedRigidbody ? collider.attachedRigidbody.transform : collider.transform).GetComponentInChildren<AOEReceiver>(true);
					if (componentInChildren != null && this.TagValidation(componentInChildren.gameObject) && !this.visited.Contains(componentInChildren))
					{
						this.visited.Add(componentInChildren);
						float num3 = Vector3.Distance(worldOrigin, componentInChildren.transform.position);
						float num4 = Mathf.Clamp01(num3 / num2);
						float num5 = this.EvaluateFalloff(num4);
						float finalStrength = Mathf.Max(this.minStrength, this.strength * num5);
						AOEReceiver.AOEContext aoecontext = new AOEReceiver.AOEContext
						{
							origin = worldOrigin,
							radius = this.radius,
							instigator = base.gameObject,
							baseStrength = this.strength,
							finalStrength = finalStrength,
							distance = num3,
							normalizedDistance = num4
						};
						componentInChildren.ReceiveAOE(aoecontext);
					}
				}
			}
		}

		// Token: 0x06007455 RID: 29781 RVA: 0x002614B8 File Offset: 0x0025F6B8
		private float EvaluateFalloff(float t)
		{
			switch (this.falloffMode)
			{
			case AOESender.FalloffMode.None:
				return 1f;
			case AOESender.FalloffMode.Linear:
				return 1f - t;
			case AOESender.FalloffMode.AnimationCurve:
				return Mathf.Max(0f, this.falloffCurve.Evaluate(t));
			default:
				return 1f;
			}
		}

		// Token: 0x06007456 RID: 29782 RVA: 0x0026150C File Offset: 0x0025F70C
		private bool TagValidation(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.includeTags == null || this.includeTags.Length == 0)
			{
				return true;
			}
			string tag = go.tag;
			foreach (string text in this.includeTags)
			{
				if (!string.IsNullOrEmpty(text) && tag == text)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04008587 RID: 34183
		[Min(0f)]
		[SerializeField]
		private float radius = 3f;

		// Token: 0x04008588 RID: 34184
		[SerializeField]
		private LayerMask layerMask = -1;

		// Token: 0x04008589 RID: 34185
		[SerializeField]
		private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

		// Token: 0x0400858A RID: 34186
		[Tooltip("If empty, all AOEReceiver targets pass. If not empty, only receivers with these tags pass.")]
		[SerializeField]
		private string[] includeTags;

		// Token: 0x0400858B RID: 34187
		[SerializeField]
		private AOESender.FalloffMode falloffMode = AOESender.FalloffMode.Linear;

		// Token: 0x0400858C RID: 34188
		[SerializeField]
		private AnimationCurve falloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x0400858D RID: 34189
		[Tooltip("Base strength before distance falloff.")]
		[SerializeField]
		private float strength = 1f;

		// Token: 0x0400858E RID: 34190
		[Tooltip("Optional after falloff, applied as: max(minStrength, base*falloff).")]
		[SerializeField]
		private float minStrength;

		// Token: 0x0400858F RID: 34191
		[SerializeField]
		private bool applyOnEnable;

		// Token: 0x04008590 RID: 34192
		[Min(0f)]
		[SerializeField]
		private float repeatInterval;

		// Token: 0x04008591 RID: 34193
		[SerializeField]
		[Tooltip("Max colliders captured per trigger/apply.")]
		private int maxColliders = 16;

		// Token: 0x04008592 RID: 34194
		private Collider[] hits;

		// Token: 0x04008593 RID: 34195
		private readonly HashSet<AOEReceiver> visited = new HashSet<AOEReceiver>();

		// Token: 0x04008594 RID: 34196
		private float nextTime;

		// Token: 0x0200122B RID: 4651
		private enum FalloffMode
		{
			// Token: 0x04008596 RID: 34198
			None,
			// Token: 0x04008597 RID: 34199
			Linear,
			// Token: 0x04008598 RID: 34200
			AnimationCurve
		}
	}
}
