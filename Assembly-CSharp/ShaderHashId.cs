using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000D79 RID: 3449
[Serializable]
public struct ShaderHashId : IEquatable<ShaderHashId>
{
	// Token: 0x170007F8 RID: 2040
	// (get) Token: 0x0600548E RID: 21646 RVA: 0x001B90A3 File Offset: 0x001B72A3
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x170007F9 RID: 2041
	// (get) Token: 0x0600548F RID: 21647 RVA: 0x001B90AB File Offset: 0x001B72AB
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x06005490 RID: 21648 RVA: 0x001B90B3 File Offset: 0x001B72B3
	public ShaderHashId(string text)
	{
		this._text = text;
		this._hash = Shader.PropertyToID(text);
	}

	// Token: 0x06005491 RID: 21649 RVA: 0x001B90A3 File Offset: 0x001B72A3
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x06005492 RID: 21650 RVA: 0x001B90AB File Offset: 0x001B72AB
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x06005493 RID: 21651 RVA: 0x001B90AB File Offset: 0x001B72AB
	public static implicit operator int(ShaderHashId h)
	{
		return h._hash;
	}

	// Token: 0x06005494 RID: 21652 RVA: 0x001B90C8 File Offset: 0x001B72C8
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x06005495 RID: 21653 RVA: 0x001B90D0 File Offset: 0x001B72D0
	public bool Equals(ShaderHashId other)
	{
		return this._hash == other._hash;
	}

	// Token: 0x06005496 RID: 21654 RVA: 0x001B90E0 File Offset: 0x001B72E0
	public override bool Equals(object obj)
	{
		if (obj is ShaderHashId)
		{
			ShaderHashId other = (ShaderHashId)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06005497 RID: 21655 RVA: 0x001B9105 File Offset: 0x001B7305
	public static bool operator ==(ShaderHashId x, ShaderHashId y)
	{
		return x.Equals(y);
	}

	// Token: 0x06005498 RID: 21656 RVA: 0x001B910F File Offset: 0x001B730F
	public static bool operator !=(ShaderHashId x, ShaderHashId y)
	{
		return !x.Equals(y);
	}

	// Token: 0x0400652D RID: 25901
	[FormerlySerializedAs("_hashText")]
	[SerializeField]
	private string _text;

	// Token: 0x0400652E RID: 25902
	[NonSerialized]
	private int _hash;
}
