using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001379 RID: 4985
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x06007D7F RID: 32127 RVA: 0x0029380E File Offset: 0x00291A0E
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007D80 RID: 32128 RVA: 0x00293816 File Offset: 0x00291A16
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007D81 RID: 32129 RVA: 0x00293820 File Offset: 0x00291A20
		public void SampleFromField()
		{
			this.m_objPosition = base.transform.position;
			this.m_objRotation = base.transform.rotation;
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				return;
			}
			Vector3 a;
			Vector4 v;
			if (!component.SampleCpuGrid(base.transform.position, out a, out v))
			{
				return;
			}
			base.transform.position = this.m_objPosition + a * this.PositionSampleMultiplier;
			base.transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(v, true), this.RotationSampleMultiplier) * this.m_objRotation;
		}

		// Token: 0x06007D82 RID: 32130 RVA: 0x002938DF File Offset: 0x00291ADF
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x04008EE5 RID: 36581
		public BoingReactorField ReactorField;

		// Token: 0x04008EE6 RID: 36582
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x04008EE7 RID: 36583
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04008EE8 RID: 36584
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04008EE9 RID: 36585
		private Vector3 m_objPosition;

		// Token: 0x04008EEA RID: 36586
		private Quaternion m_objRotation;
	}
}
