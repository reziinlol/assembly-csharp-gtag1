using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02001315 RID: 4885
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06007B0B RID: 31499 RVA: 0x002832FA File Offset: 0x002814FA
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x06007B0C RID: 31500 RVA: 0x00283308 File Offset: 0x00281508
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x06007B0D RID: 31501 RVA: 0x00283310 File Offset: 0x00281510
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06007B0E RID: 31502 RVA: 0x0028334C File Offset: 0x0028154C
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06007B0F RID: 31503 RVA: 0x00283380 File Offset: 0x00281580
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06007B10 RID: 31504 RVA: 0x0028339C File Offset: 0x0028159C
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06007B11 RID: 31505 RVA: 0x002833A9 File Offset: 0x002815A9
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06007B12 RID: 31506 RVA: 0x002833B6 File Offset: 0x002815B6
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06007B13 RID: 31507 RVA: 0x002833C3 File Offset: 0x002815C3
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06007B14 RID: 31508 RVA: 0x002833D5 File Offset: 0x002815D5
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06007B15 RID: 31509 RVA: 0x002833E7 File Offset: 0x002815E7
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06007B16 RID: 31510 RVA: 0x002833FC File Offset: 0x002815FC
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06007B17 RID: 31511 RVA: 0x00283411 File Offset: 0x00281611
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06007B18 RID: 31512 RVA: 0x0028341B File Offset: 0x0028161B
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06007B19 RID: 31513 RVA: 0x00283428 File Offset: 0x00281628
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06007B1A RID: 31514 RVA: 0x00283430 File Offset: 0x00281630
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06007B1B RID: 31515 RVA: 0x00283440 File Offset: 0x00281640
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06007B1C RID: 31516 RVA: 0x00283470 File Offset: 0x00281670
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x06007B1D RID: 31517 RVA: 0x002834B8 File Offset: 0x002816B8
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06007B1E RID: 31518 RVA: 0x002834E4 File Offset: 0x002816E4
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06007B1F RID: 31519 RVA: 0x0028351D File Offset: 0x0028171D
		public override int GetHashCode()
		{
			return (int)(this.value * 3 / 2 ^ this.value);
		}

		// Token: 0x06007B20 RID: 31520 RVA: 0x00283530 File Offset: 0x00281730
		public int CompareTo(Half value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			if (this != value)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(value))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06007B21 RID: 31521 RVA: 0x00283588 File Offset: 0x00281788
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Half))
			{
				throw new ArgumentException("The argument value must be a SlimMath.Half.");
			}
			Half half = (Half)value;
			if (this < half)
			{
				return -1;
			}
			if (this > half)
			{
				return 1;
			}
			if (this != half)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(half))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06007B22 RID: 31522 RVA: 0x002835FC File Offset: 0x002817FC
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06007B23 RID: 31523 RVA: 0x0028360C File Offset: 0x0028180C
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06007B24 RID: 31524 RVA: 0x0028361C File Offset: 0x0028181C
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06007B25 RID: 31525 RVA: 0x0028364E File Offset: 0x0028184E
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06007B26 RID: 31526 RVA: 0x0028365F File Offset: 0x0028185F
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06007B27 RID: 31527 RVA: 0x00283671 File Offset: 0x00281871
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06007B28 RID: 31528 RVA: 0x00283683 File Offset: 0x00281883
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x06007B29 RID: 31529 RVA: 0x0028368F File Offset: 0x0028188F
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x06007B2A RID: 31530 RVA: 0x0028369B File Offset: 0x0028189B
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06007B2B RID: 31531 RVA: 0x002836AD File Offset: 0x002818AD
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06007B2C RID: 31532 RVA: 0x002836BF File Offset: 0x002818BF
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06007B2D RID: 31533 RVA: 0x002836D1 File Offset: 0x002818D1
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06007B2E RID: 31534 RVA: 0x002836E3 File Offset: 0x002818E3
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06007B2F RID: 31535 RVA: 0x002836F5 File Offset: 0x002818F5
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06007B30 RID: 31536 RVA: 0x00283707 File Offset: 0x00281907
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06007B31 RID: 31537 RVA: 0x00283714 File Offset: 0x00281914
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06007B32 RID: 31538 RVA: 0x0028372E File Offset: 0x0028192E
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06007B33 RID: 31539 RVA: 0x00283740 File Offset: 0x00281940
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06007B34 RID: 31540 RVA: 0x00283752 File Offset: 0x00281952
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04008C95 RID: 35989
		private ushort value;

		// Token: 0x04008C96 RID: 35990
		public const int PrecisionDigits = 3;

		// Token: 0x04008C97 RID: 35991
		public const int MantissaBits = 11;

		// Token: 0x04008C98 RID: 35992
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04008C99 RID: 35993
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04008C9A RID: 35994
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04008C9B RID: 35995
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04008C9C RID: 35996
		public const int ExponentRadix = 2;

		// Token: 0x04008C9D RID: 35997
		public const int AdditionRounding = 1;

		// Token: 0x04008C9E RID: 35998
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x04008C9F RID: 35999
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x04008CA0 RID: 36000
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x04008CA1 RID: 36001
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x04008CA2 RID: 36002
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x04008CA3 RID: 36003
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
