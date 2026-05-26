using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011AA RID: 4522
	public abstract class BaseGuidedRefTargetMono : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06007253 RID: 29267 RVA: 0x0012FBB9 File Offset: 0x0012DDB9
		protected virtual void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06007254 RID: 29268 RVA: 0x00253A06 File Offset: 0x00251C06
		protected virtual void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this, true);
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06007255 RID: 29269 RVA: 0x00253A0F File Offset: 0x00251C0F
		// (set) Token: 0x06007256 RID: 29270 RVA: 0x00253A17 File Offset: 0x00251C17
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

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06007257 RID: 29271 RVA: 0x00082EEE File Offset: 0x000810EE
		Object IGuidedRefTargetMono.GuidedRefTargetObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06007258 RID: 29272 RVA: 0x00253A20 File Offset: 0x00251C20
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x0600725A RID: 29274 RVA: 0x00086271 File Offset: 0x00084471
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600725B RID: 29275 RVA: 0x00018FAD File Offset: 0x000171AD
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x04008233 RID: 33331
		public GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
