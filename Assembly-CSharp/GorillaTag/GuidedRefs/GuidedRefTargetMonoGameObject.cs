using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011B7 RID: 4535
	public class GuidedRefTargetMonoGameObject : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x0600728B RID: 29323 RVA: 0x00254C36 File Offset: 0x00252E36
		// (set) Token: 0x0600728C RID: 29324 RVA: 0x00254C3E File Offset: 0x00252E3E
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

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x0600728D RID: 29325 RVA: 0x0000636B File Offset: 0x0000456B
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x0600728E RID: 29326 RVA: 0x0012FBB9 File Offset: 0x0012DDB9
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x0600728F RID: 29327 RVA: 0x00254C47 File Offset: 0x00252E47
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoGameObject>(this, true);
		}

		// Token: 0x06007290 RID: 29328 RVA: 0x00254C50 File Offset: 0x00252E50
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoGameObject>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06007292 RID: 29330 RVA: 0x00086271 File Offset: 0x00084471
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06007293 RID: 29331 RVA: 0x00018FAD File Offset: 0x000171AD
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400824F RID: 33359
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
