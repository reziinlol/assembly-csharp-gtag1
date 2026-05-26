using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000F9B RID: 3995
	public class BuilderAnimateOnTap : BuilderPieceTappable
	{
		// Token: 0x06006392 RID: 25490 RVA: 0x00200815 File Offset: 0x001FEA15
		public override void OnTapReplicated()
		{
			base.OnTapReplicated();
			this.anim.Rewind();
			this.anim.Play();
		}

		// Token: 0x04007239 RID: 29241
		[SerializeField]
		private Animation anim;
	}
}
