using System;
using UnityEngine;

// Token: 0x02000D20 RID: 3360
[Serializable]
public struct AnimHashId
{
	// Token: 0x170007D8 RID: 2008
	// (get) Token: 0x060052EF RID: 21231 RVA: 0x001B2D2D File Offset: 0x001B0F2D
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x170007D9 RID: 2009
	// (get) Token: 0x060052F0 RID: 21232 RVA: 0x001B2D35 File Offset: 0x001B0F35
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x060052F1 RID: 21233 RVA: 0x001B2D3D File Offset: 0x001B0F3D
	public AnimHashId(string text)
	{
		this._text = text;
		this._hash = Animator.StringToHash(text);
	}

	// Token: 0x060052F2 RID: 21234 RVA: 0x001B2D2D File Offset: 0x001B0F2D
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x060052F3 RID: 21235 RVA: 0x001B2D35 File Offset: 0x001B0F35
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x060052F4 RID: 21236 RVA: 0x001B2D35 File Offset: 0x001B0F35
	public static implicit operator int(AnimHashId h)
	{
		return h._hash;
	}

	// Token: 0x060052F5 RID: 21237 RVA: 0x001B2D52 File Offset: 0x001B0F52
	public static implicit operator AnimHashId(string s)
	{
		return new AnimHashId(s);
	}

	// Token: 0x04006454 RID: 25684
	[SerializeField]
	private string _text;

	// Token: 0x04006455 RID: 25685
	[NonSerialized]
	private int _hash;
}
