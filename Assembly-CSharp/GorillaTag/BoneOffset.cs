using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001136 RID: 4406
	[Serializable]
	public struct BoneOffset
	{
		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x06006FE7 RID: 28647 RVA: 0x00248B9C File Offset: 0x00246D9C
		public Vector3 pos
		{
			get
			{
				return this.offset.pos;
			}
		}

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x06006FE8 RID: 28648 RVA: 0x00248BA9 File Offset: 0x00246DA9
		public Quaternion rot
		{
			get
			{
				return this.offset.rot;
			}
		}

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06006FE9 RID: 28649 RVA: 0x00248BB6 File Offset: 0x00246DB6
		public Vector3 scale
		{
			get
			{
				return this.offset.scale;
			}
		}

		// Token: 0x06006FEA RID: 28650 RVA: 0x00248BC3 File Offset: 0x00246DC3
		public BoneOffset(GTHardCodedBones.EBone bone)
		{
			this.bone = bone;
			this.offset = XformOffset.Identity;
		}

		// Token: 0x06006FEB RID: 28651 RVA: 0x00248BDC File Offset: 0x00246DDC
		public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
		{
			this.bone = bone;
			this.offset = offset;
		}

		// Token: 0x06006FEC RID: 28652 RVA: 0x00248BF1 File Offset: 0x00246DF1
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot);
		}

		// Token: 0x06006FED RID: 28653 RVA: 0x00248C0C File Offset: 0x00246E0C
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles);
		}

		// Token: 0x06006FEE RID: 28654 RVA: 0x00248C27 File Offset: 0x00246E27
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot, scale);
		}

		// Token: 0x06006FEF RID: 28655 RVA: 0x00248C44 File Offset: 0x00246E44
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles, scale);
		}

		// Token: 0x04007FF8 RID: 32760
		public GTHardCodedBones.SturdyEBone bone;

		// Token: 0x04007FF9 RID: 32761
		public XformOffset offset;

		// Token: 0x04007FFA RID: 32762
		public static readonly BoneOffset Identity = new BoneOffset
		{
			bone = GTHardCodedBones.EBone.None,
			offset = XformOffset.Identity
		};
	}
}
