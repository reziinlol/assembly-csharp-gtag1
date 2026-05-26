using System;
using UnityEngine;

// Token: 0x02000AEE RID: 2798
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170006A8 RID: 1704
	// (get) Token: 0x06004796 RID: 18326 RVA: 0x00180EE4 File Offset: 0x0017F0E4
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x06004797 RID: 18327 RVA: 0x00180EEC File Offset: 0x0017F0EC
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x06004798 RID: 18328 RVA: 0x00180EE4 File Offset: 0x0017F0E4
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x06004799 RID: 18329 RVA: 0x00180F0A File Offset: 0x0017F10A
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x0600479A RID: 18330 RVA: 0x00180F29 File Offset: 0x0017F129
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x0600479B RID: 18331 RVA: 0x00180F38 File Offset: 0x0017F138
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x0600479C RID: 18332 RVA: 0x00180F72 File Offset: 0x0017F172
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x0600479D RID: 18333 RVA: 0x00180F88 File Offset: 0x0017F188
	public override string ToString()
	{
		TEnum value = this.Value;
		return value.ToString();
	}

	// Token: 0x040059CA RID: 22986
	[SerializeField]
	private TEnum m_EnumValue;
}
