using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200137A RID: 4986
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x06007D84 RID: 32132 RVA: 0x00293928 File Offset: 0x00291B28
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007D85 RID: 32133 RVA: 0x00293930 File Offset: 0x00291B30
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007D86 RID: 32134 RVA: 0x00293938 File Offset: 0x00291B38
		public void Update()
		{
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			if (this.m_fieldResourceSetId != component.GpuResourceSetId)
			{
				if (this.m_matProps == null)
				{
					this.m_matProps = new MaterialPropertyBlock();
				}
				if (component.UpdateShaderConstants(this.m_matProps, this.PositionSampleMultiplier, this.RotationSampleMultiplier))
				{
					this.m_fieldResourceSetId = component.GpuResourceSetId;
					foreach (Renderer renderer in new Renderer[]
					{
						base.GetComponent<MeshRenderer>(),
						base.GetComponent<SkinnedMeshRenderer>()
					})
					{
						if (!(renderer == null))
						{
							renderer.SetPropertyBlock(this.m_matProps);
						}
					}
				}
			}
		}

		// Token: 0x04008EEB RID: 36587
		public BoingReactorField ReactorField;

		// Token: 0x04008EEC RID: 36588
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x04008EED RID: 36589
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x04008EEE RID: 36590
		private MaterialPropertyBlock m_matProps;

		// Token: 0x04008EEF RID: 36591
		private int m_fieldResourceSetId = -1;
	}
}
