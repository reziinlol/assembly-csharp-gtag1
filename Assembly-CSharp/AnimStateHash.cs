using System;
using UnityEngine;

// Token: 0x02000ABD RID: 2749
[Serializable]
public struct AnimStateHash
{
	// Token: 0x06004662 RID: 18018 RVA: 0x0017D7B8 File Offset: 0x0017B9B8
	public static implicit operator AnimStateHash(string s)
	{
		return new AnimStateHash
		{
			_hash = Animator.StringToHash(s)
		};
	}

	// Token: 0x06004663 RID: 18019 RVA: 0x0017D7DB File Offset: 0x0017B9DB
	public static implicit operator int(AnimStateHash ash)
	{
		return ash._hash;
	}

	// Token: 0x040058D6 RID: 22742
	[SerializeField]
	private int _hash;
}
