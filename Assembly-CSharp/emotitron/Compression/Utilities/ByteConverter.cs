using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x02001314 RID: 4884
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteConverter
	{
		// Token: 0x17000BB0 RID: 2992
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.byte0;
				case 1:
					return this.byte1;
				case 2:
					return this.byte2;
				case 3:
					return this.byte3;
				case 4:
					return this.byte4;
				case 5:
					return this.byte5;
				case 6:
					return this.byte6;
				case 7:
					return this.byte7;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06007AF3 RID: 31475 RVA: 0x0028305C File Offset: 0x0028125C
		public static implicit operator ByteConverter(byte[] bytes)
		{
			ByteConverter result = default(ByteConverter);
			int num = bytes.Length;
			result.byte0 = bytes[0];
			if (num > 0)
			{
				result.byte1 = bytes[1];
			}
			if (num > 1)
			{
				result.byte2 = bytes[2];
			}
			if (num > 2)
			{
				result.byte3 = bytes[3];
			}
			if (num > 3)
			{
				result.byte4 = bytes[4];
			}
			if (num > 4)
			{
				result.byte5 = bytes[5];
			}
			if (num > 5)
			{
				result.byte6 = bytes[3];
			}
			if (num > 6)
			{
				result.byte7 = bytes[7];
			}
			return result;
		}

		// Token: 0x06007AF4 RID: 31476 RVA: 0x002830E0 File Offset: 0x002812E0
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x06007AF5 RID: 31477 RVA: 0x00283100 File Offset: 0x00281300
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x06007AF6 RID: 31478 RVA: 0x00283120 File Offset: 0x00281320
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x06007AF7 RID: 31479 RVA: 0x00283140 File Offset: 0x00281340
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x06007AF8 RID: 31480 RVA: 0x00283160 File Offset: 0x00281360
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x06007AF9 RID: 31481 RVA: 0x00283180 File Offset: 0x00281380
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x06007AFA RID: 31482 RVA: 0x002831A0 File Offset: 0x002813A0
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x06007AFB RID: 31483 RVA: 0x002831C0 File Offset: 0x002813C0
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x06007AFC RID: 31484 RVA: 0x002831E0 File Offset: 0x002813E0
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x06007AFD RID: 31485 RVA: 0x00283200 File Offset: 0x00281400
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x06007AFE RID: 31486 RVA: 0x00283224 File Offset: 0x00281424
		public void ExtractByteArray(byte[] targetArray)
		{
			int num = targetArray.Length;
			targetArray[0] = this.byte0;
			if (num > 0)
			{
				targetArray[1] = this.byte1;
			}
			if (num > 1)
			{
				targetArray[2] = this.byte2;
			}
			if (num > 2)
			{
				targetArray[3] = this.byte3;
			}
			if (num > 3)
			{
				targetArray[4] = this.byte4;
			}
			if (num > 4)
			{
				targetArray[5] = this.byte5;
			}
			if (num > 5)
			{
				targetArray[6] = this.byte6;
			}
			if (num > 6)
			{
				targetArray[7] = this.byte7;
			}
		}

		// Token: 0x06007AFF RID: 31487 RVA: 0x00283297 File Offset: 0x00281497
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x06007B00 RID: 31488 RVA: 0x0028329F File Offset: 0x0028149F
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x06007B01 RID: 31489 RVA: 0x002832A7 File Offset: 0x002814A7
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06007B02 RID: 31490 RVA: 0x002832AF File Offset: 0x002814AF
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06007B03 RID: 31491 RVA: 0x002832B7 File Offset: 0x002814B7
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06007B04 RID: 31492 RVA: 0x002832BF File Offset: 0x002814BF
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x06007B05 RID: 31493 RVA: 0x002832C7 File Offset: 0x002814C7
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x06007B06 RID: 31494 RVA: 0x002832CF File Offset: 0x002814CF
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x06007B07 RID: 31495 RVA: 0x002832D7 File Offset: 0x002814D7
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x06007B08 RID: 31496 RVA: 0x002832DF File Offset: 0x002814DF
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x06007B09 RID: 31497 RVA: 0x002832E7 File Offset: 0x002814E7
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x06007B0A RID: 31498 RVA: 0x002832EF File Offset: 0x002814EF
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x04008C82 RID: 35970
		[FieldOffset(0)]
		public float float32;

		// Token: 0x04008C83 RID: 35971
		[FieldOffset(0)]
		public double float64;

		// Token: 0x04008C84 RID: 35972
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x04008C85 RID: 35973
		[FieldOffset(0)]
		public short int16;

		// Token: 0x04008C86 RID: 35974
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04008C87 RID: 35975
		[FieldOffset(0)]
		public char character;

		// Token: 0x04008C88 RID: 35976
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04008C89 RID: 35977
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04008C8A RID: 35978
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04008C8B RID: 35979
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04008C8C RID: 35980
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04008C8D RID: 35981
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x04008C8E RID: 35982
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x04008C8F RID: 35983
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x04008C90 RID: 35984
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x04008C91 RID: 35985
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x04008C92 RID: 35986
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x04008C93 RID: 35987
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x04008C94 RID: 35988
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
