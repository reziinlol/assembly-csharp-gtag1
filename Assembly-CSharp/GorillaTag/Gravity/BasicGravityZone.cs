using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001186 RID: 4486
	public class BasicGravityZone : MonoBehaviour, ICallbackUnique, ICallBack
	{
		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x0600717F RID: 29055 RVA: 0x00250180 File Offset: 0x0024E380
		private IReadOnlyList<MonkeGravityController> GravityTargets
		{
			get
			{
				return this.m_gravityTargets.GetReadonlyList();
			}
		}

		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x06007180 RID: 29056 RVA: 0x0025018D File Offset: 0x0024E38D
		// (set) Token: 0x06007181 RID: 29057 RVA: 0x00250195 File Offset: 0x0024E395
		bool ICallbackUnique.Registered { get; set; }

		// Token: 0x06007182 RID: 29058 RVA: 0x002501A0 File Offset: 0x0024E3A0
		protected virtual void Awake()
		{
			this.m_gravityDirection = base.gameObject.transform.up;
			this.invertRotationDirection = ((this.gravityStrength > 0f && !this.invertRotationDirection) || (this.gravityStrength <= 0f && this.invertRotationDirection));
		}

		// Token: 0x06007183 RID: 29059 RVA: 0x002501F7 File Offset: 0x0024E3F7
		protected virtual void OnEnable()
		{
			this.m_gravityTargets.ItemProcessor = new InAction<MonkeGravityController>(this.ProcessGravityTargets);
		}

		// Token: 0x06007184 RID: 29060 RVA: 0x00250210 File Offset: 0x0024E410
		protected virtual void OnDisable()
		{
			this.m_gravityTargets.ItemProcessor = new InAction<MonkeGravityController>(this.ProcessRemoveTargets);
			this.m_gravityTargets.ProcessList();
			this.m_gravityTargets.Clear();
			this.m_targetGravityInfos.Clear();
			MonkeGravityManager.RemoveGravityCallback(this);
		}

		// Token: 0x06007185 RID: 29061 RVA: 0x00250250 File Offset: 0x0024E450
		public virtual void CallBack()
		{
			this.m_gravityTargets.ProcessList();
		}

		// Token: 0x06007186 RID: 29062 RVA: 0x0025025D File Offset: 0x0024E45D
		private void ProcessRemoveTargets(in MonkeGravityController target)
		{
			target.OnLeftGravityZone(this);
		}

		// Token: 0x06007187 RID: 29063 RVA: 0x00250268 File Offset: 0x0024E468
		private void ProcessGravityTargets(in MonkeGravityController targetController)
		{
			if (!this.PassesScaleFilter(targetController))
			{
				this.OnTargetFilteredOut(targetController);
			}
			GravityInfo gravityInfo = default(GravityInfo);
			Vector3 worldPoint = targetController.GetWorldPoint();
			Vector3 gravityVectorAtPoint = this.GetGravityVectorAtPoint(worldPoint, targetController);
			Vector3 normalized = gravityVectorAtPoint.normalized;
			gravityInfo.gravityUpDirection = normalized;
			gravityInfo.rotationDirection = this.GetRotationDirection(normalized);
			gravityInfo.gravityStrength = this.GetGravityStrength(gravityVectorAtPoint);
			gravityInfo.rotationSpeed = this.GetRotationSpeed(gravityVectorAtPoint);
			gravityInfo.rotate = this.GetRotationIntent(gravityVectorAtPoint);
			this.m_targetGravityInfos[targetController] = gravityInfo;
			if (gravityInfo.gravityStrength != 0f)
			{
				MonkeGravityController monkeGravityController = targetController;
				Vector3 vector = normalized * gravityInfo.gravityStrength;
				monkeGravityController.ApplyGravityForce(vector, ForceMode.Acceleration);
			}
		}

		// Token: 0x06007188 RID: 29064 RVA: 0x00250320 File Offset: 0x0024E520
		protected virtual Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return this.m_gravityDirection;
		}

		// Token: 0x06007189 RID: 29065 RVA: 0x00250328 File Offset: 0x0024E528
		protected virtual float GetGravityStrength(in Vector3 offsetFromGravity)
		{
			return this.gravityStrength;
		}

		// Token: 0x0600718A RID: 29066 RVA: 0x00250330 File Offset: 0x0024E530
		protected virtual bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			return this.rotateTarget;
		}

		// Token: 0x0600718B RID: 29067 RVA: 0x00250338 File Offset: 0x0024E538
		protected virtual Vector3 GetRotationDirection(in Vector3 gravityDirection)
		{
			if (this.invertRotationDirection)
			{
				return -gravityDirection;
			}
			return gravityDirection;
		}

		// Token: 0x0600718C RID: 29068 RVA: 0x00250354 File Offset: 0x0024E554
		protected virtual float GetRotationSpeed(in Vector3 offsetFromGravity)
		{
			return this.rotationSpeed;
		}

		// Token: 0x0600718D RID: 29069 RVA: 0x0025035C File Offset: 0x0024E55C
		public bool GetGravityInfo(MonkeGravityController target, out GravityInfo info)
		{
			return this.m_targetGravityInfos.TryGetValue(target, out info);
		}

		// Token: 0x0600718E RID: 29070 RVA: 0x0025036C File Offset: 0x0024E56C
		public void RemoveTarget(MonkeGravityController target)
		{
			if (!target.Register || !this.m_gravityTargets.Remove(target))
			{
				return;
			}
			this.m_targetGravityInfos.Remove(target);
			target.OnLeftGravityZone(this);
			this.OnTargetExited(target);
			if (this.m_gravityTargets.Count < 1)
			{
				MonkeGravityManager.RemoveGravityCallback(this);
			}
		}

		// Token: 0x0600718F RID: 29071 RVA: 0x002503C0 File Offset: 0x0024E5C0
		public void AddTarget(MonkeGravityController target)
		{
			if (!target.Register || this.m_gravityTargets.Contains(target))
			{
				return;
			}
			this.m_gravityTargets.Add(target);
			target.OnEnteredGravityZone(this);
			MonkeGravityManager.AddGravityCallback(this);
		}

		// Token: 0x06007190 RID: 29072 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected virtual void OnTargetExited(MonkeGravityController target)
		{
		}

		// Token: 0x06007191 RID: 29073 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected virtual void OnTargetFilteredOut(MonkeGravityController target)
		{
		}

		// Token: 0x06007192 RID: 29074 RVA: 0x002503F4 File Offset: 0x0024E5F4
		private bool PassesScaleFilter(MonkeGravityController target)
		{
			if (this.scaleFilter == GravityZoneScaleFilter.Anyone)
			{
				return true;
			}
			bool flag = target.Scale < 1f;
			if (this.scaleFilter != GravityZoneScaleFilter.SmallOnly)
			{
				return !flag;
			}
			return flag;
		}

		// Token: 0x06007193 RID: 29075 RVA: 0x00250428 File Offset: 0x0024E628
		private void OnTriggerEnter(Collider other)
		{
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.AddTarget(monkeGravityController.Item2);
		}

		// Token: 0x06007194 RID: 29076 RVA: 0x00250454 File Offset: 0x0024E654
		private void OnTriggerExit(Collider other)
		{
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.RemoveTarget(monkeGravityController.Item2);
		}

		// Token: 0x04008148 RID: 33096
		[Header("Gravity Settings")]
		[Tooltip("negative number pulls, positive number expels")]
		public float gravityStrength = -9.81f;

		// Token: 0x04008149 RID: 33097
		[Tooltip("Filter which players are affected based on scale. Small = scale < 1")]
		[SerializeField]
		private GravityZoneScaleFilter scaleFilter;

		// Token: 0x0400814A RID: 33098
		[Header("Rotation Settings")]
		[Tooltip("If enabled, rotates the target away from gravity direction to be upside down")]
		[SerializeField]
		protected bool invertRotationDirection;

		// Token: 0x0400814B RID: 33099
		[SerializeField]
		protected bool rotateTarget = true;

		// Token: 0x0400814C RID: 33100
		[SerializeField]
		protected float rotationSpeed = 10f;

		// Token: 0x0400814D RID: 33101
		protected Vector3 m_gravityDirection;

		// Token: 0x0400814E RID: 33102
		protected ListProcessor<MonkeGravityController> m_gravityTargets = new ListProcessor<MonkeGravityController>(5, null);

		// Token: 0x0400814F RID: 33103
		private Dictionary<MonkeGravityController, GravityInfo> m_targetGravityInfos = new Dictionary<MonkeGravityController, GravityInfo>(5);
	}
}
