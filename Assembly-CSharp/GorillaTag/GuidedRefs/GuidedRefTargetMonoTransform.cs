using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011B8 RID: 4536
	public class GuidedRefTargetMonoTransform : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06007294 RID: 29332 RVA: 0x00254C64 File Offset: 0x00252E64
		// (set) Token: 0x06007295 RID: 29333 RVA: 0x00254C6C File Offset: 0x00252E6C
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

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06007296 RID: 29334 RVA: 0x00086271 File Offset: 0x00084471
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x06007297 RID: 29335 RVA: 0x0012FBB9 File Offset: 0x0012DDB9
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06007298 RID: 29336 RVA: 0x00254C75 File Offset: 0x00252E75
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoTransform>(this, true);
		}

		// Token: 0x06007299 RID: 29337 RVA: 0x00254C7E File Offset: 0x00252E7E
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoTransform>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x0600729B RID: 29339 RVA: 0x00086271 File Offset: 0x00084471
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600729C RID: 29340 RVA: 0x00018FAD File Offset: 0x000171AD
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x04008250 RID: 33360
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
