using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FA9 RID: 4009
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06006416 RID: 25622 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06006417 RID: 25623 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006418 RID: 25624 RVA: 0x00204734 File Offset: 0x00202934
		public void OnPiecePlacementDeserialized()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		// Token: 0x06006419 RID: 25625 RVA: 0x0020477C File Offset: 0x0020297C
		public void OnPieceActivate()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion rotation;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out rotation);
				this.alwaysFaceUp.rotation = rotation;
			}
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDeactivate()
		{
		}

		// Token: 0x040072F7 RID: 29431
		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
