using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011B6 RID: 4534
	public class GuidedRefTargetMonoComponent : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06007282 RID: 29314 RVA: 0x00254C00 File Offset: 0x00252E00
		// (set) Token: 0x06007283 RID: 29315 RVA: 0x00254C08 File Offset: 0x00252E08
		GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
		{
			get
			{
				return this.guidedRefTargetInfo;
			}
			set
			{
				this.guidedRefTargetInfo = value;
			}
		}

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06007284 RID: 29316 RVA: 0x00254C11 File Offset: 0x00252E11
		public Object GuidedRefTargetObject
		{
			get
			{
				return this.targetComponent;
			}
		}

		// Token: 0x06007285 RID: 29317 RVA: 0x0012FBB9 File Offset: 0x0012DDB9
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06007286 RID: 29318 RVA: 0x00254C19 File Offset: 0x00252E19
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoComponent>(this, true);
		}

		// Token: 0x06007287 RID: 29319 RVA: 0x00254C22 File Offset: 0x00252E22
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoComponent>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06007289 RID: 29321 RVA: 0x00086271 File Offset: 0x00084471
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600728A RID: 29322 RVA: 0x00018FAD File Offset: 0x000171AD
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400824D RID: 33357
		[SerializeField]
		private Component targetComponent;

		// Token: 0x0400824E RID: 33358
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
