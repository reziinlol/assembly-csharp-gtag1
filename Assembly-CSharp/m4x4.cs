using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000ADD RID: 2781
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct m4x4
{
	// Token: 0x060046F8 RID: 18168 RVA: 0x0017F0EC File Offset: 0x0017D2EC
	public m4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
	{
		this = default(m4x4);
		this.m00 = m00;
		this.m01 = m01;
		this.m02 = m02;
		this.m03 = m03;
		this.m10 = m10;
		this.m11 = m11;
		this.m12 = m12;
		this.m13 = m13;
		this.m20 = m20;
		this.m21 = m21;
		this.m22 = m22;
		this.m23 = m23;
		this.m30 = m30;
		this.m31 = m31;
		this.m32 = m32;
		this.m33 = m33;
	}

	// Token: 0x060046F9 RID: 18169 RVA: 0x0017F17D File Offset: 0x0017D37D
	public m4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
	{
		this = default(m4x4);
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x060046FA RID: 18170 RVA: 0x0017F1A4 File Offset: 0x0017D3A4
	public void Clear()
	{
		this.m00 = 0f;
		this.m01 = 0f;
		this.m02 = 0f;
		this.m03 = 0f;
		this.m10 = 0f;
		this.m11 = 0f;
		this.m12 = 0f;
		this.m13 = 0f;
		this.m20 = 0f;
		this.m21 = 0f;
		this.m22 = 0f;
		this.m23 = 0f;
		this.m30 = 0f;
		this.m31 = 0f;
		this.m32 = 0f;
		this.m33 = 0f;
	}

	// Token: 0x060046FB RID: 18171 RVA: 0x0017F261 File Offset: 0x0017D461
	public void SetRow0(ref Vector4 v)
	{
		this.m00 = v.x;
		this.m01 = v.y;
		this.m02 = v.z;
		this.m03 = v.w;
	}

	// Token: 0x060046FC RID: 18172 RVA: 0x0017F293 File Offset: 0x0017D493
	public void SetRow1(ref Vector4 v)
	{
		this.m10 = v.x;
		this.m11 = v.y;
		this.m12 = v.z;
		this.m13 = v.w;
	}

	// Token: 0x060046FD RID: 18173 RVA: 0x0017F2C5 File Offset: 0x0017D4C5
	public void SetRow2(ref Vector4 v)
	{
		this.m20 = v.x;
		this.m21 = v.y;
		this.m22 = v.z;
		this.m23 = v.w;
	}

	// Token: 0x060046FE RID: 18174 RVA: 0x0017F2F7 File Offset: 0x0017D4F7
	public void SetRow3(ref Vector4 v)
	{
		this.m30 = v.x;
		this.m31 = v.y;
		this.m32 = v.z;
		this.m33 = v.w;
	}

	// Token: 0x060046FF RID: 18175 RVA: 0x0017F32C File Offset: 0x0017D52C
	public void Transpose()
	{
		float num = this.m01;
		float num2 = this.m02;
		float num3 = this.m03;
		float num4 = this.m10;
		float num5 = this.m12;
		float num6 = this.m13;
		float num7 = this.m20;
		float num8 = this.m21;
		float num9 = this.m23;
		float num10 = this.m30;
		float num11 = this.m31;
		float num12 = this.m32;
		this.m01 = num4;
		this.m02 = num7;
		this.m03 = num10;
		this.m10 = num;
		this.m12 = num8;
		this.m13 = num11;
		this.m20 = num2;
		this.m21 = num5;
		this.m23 = num12;
		this.m30 = num3;
		this.m31 = num6;
		this.m32 = num9;
	}

	// Token: 0x06004700 RID: 18176 RVA: 0x0017F3F1 File Offset: 0x0017D5F1
	public void Set(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06004701 RID: 18177 RVA: 0x0017F424 File Offset: 0x0017D624
	public void SetTransposed(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.m00 = row0.x;
		this.m01 = row1.x;
		this.m02 = row2.x;
		this.m03 = row3.x;
		this.m10 = row0.y;
		this.m11 = row1.y;
		this.m12 = row2.y;
		this.m13 = row3.y;
		this.m20 = row0.z;
		this.m21 = row1.z;
		this.m22 = row2.z;
		this.m23 = row3.z;
		this.m30 = row0.w;
		this.m31 = row1.w;
		this.m32 = row2.w;
		this.m33 = row3.w;
	}

	// Token: 0x06004702 RID: 18178 RVA: 0x0017F4F8 File Offset: 0x0017D6F8
	public void Set(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m01;
		this.m02 = x.m02;
		this.m03 = x.m03;
		this.m10 = x.m10;
		this.m11 = x.m11;
		this.m12 = x.m12;
		this.m13 = x.m13;
		this.m20 = x.m20;
		this.m21 = x.m21;
		this.m22 = x.m22;
		this.m23 = x.m23;
		this.m30 = x.m30;
		this.m31 = x.m31;
		this.m32 = x.m32;
		this.m33 = x.m33;
	}

	// Token: 0x06004703 RID: 18179 RVA: 0x0017F5C8 File Offset: 0x0017D7C8
	public void SetTransposed(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m10;
		this.m02 = x.m20;
		this.m03 = x.m30;
		this.m10 = x.m01;
		this.m11 = x.m11;
		this.m12 = x.m21;
		this.m13 = x.m31;
		this.m20 = x.m02;
		this.m21 = x.m12;
		this.m22 = x.m22;
		this.m23 = x.m32;
		this.m30 = x.m03;
		this.m31 = x.m13;
		this.m32 = x.m23;
		this.m33 = x.m33;
	}

	// Token: 0x06004704 RID: 18180 RVA: 0x0017F698 File Offset: 0x0017D898
	public void Push(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m01;
		x.m02 = this.m02;
		x.m03 = this.m03;
		x.m10 = this.m10;
		x.m11 = this.m11;
		x.m12 = this.m12;
		x.m13 = this.m13;
		x.m20 = this.m20;
		x.m21 = this.m21;
		x.m22 = this.m22;
		x.m23 = this.m23;
		x.m30 = this.m30;
		x.m31 = this.m31;
		x.m32 = this.m32;
		x.m33 = this.m33;
	}

	// Token: 0x06004705 RID: 18181 RVA: 0x0017F768 File Offset: 0x0017D968
	public void PushTransposed(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m10;
		x.m02 = this.m20;
		x.m03 = this.m30;
		x.m10 = this.m01;
		x.m11 = this.m11;
		x.m12 = this.m21;
		x.m13 = this.m31;
		x.m20 = this.m02;
		x.m21 = this.m12;
		x.m22 = this.m22;
		x.m23 = this.m32;
		x.m30 = this.m03;
		x.m31 = this.m13;
		x.m32 = this.m23;
		x.m33 = this.m33;
	}

	// Token: 0x06004706 RID: 18182 RVA: 0x0017F835 File Offset: 0x0017DA35
	public static ref m4x4 From(ref Matrix4x4 src)
	{
		return Unsafe.As<Matrix4x4, m4x4>(ref src);
	}

	// Token: 0x0400594D RID: 22861
	[FixedBuffer(typeof(float), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_f>e__FixedBuffer data_f;

	// Token: 0x0400594E RID: 22862
	[FixedBuffer(typeof(int), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_i>e__FixedBuffer data_i;

	// Token: 0x0400594F RID: 22863
	[FixedBuffer(typeof(ushort), 32)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public m4x4.<data_h>e__FixedBuffer data_h;

	// Token: 0x04005950 RID: 22864
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 r0;

	// Token: 0x04005951 RID: 22865
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 r1;

	// Token: 0x04005952 RID: 22866
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 r2;

	// Token: 0x04005953 RID: 22867
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 r3;

	// Token: 0x04005954 RID: 22868
	[NonSerialized]
	[FieldOffset(0)]
	public float m00;

	// Token: 0x04005955 RID: 22869
	[NonSerialized]
	[FieldOffset(4)]
	public float m01;

	// Token: 0x04005956 RID: 22870
	[NonSerialized]
	[FieldOffset(8)]
	public float m02;

	// Token: 0x04005957 RID: 22871
	[NonSerialized]
	[FieldOffset(12)]
	public float m03;

	// Token: 0x04005958 RID: 22872
	[NonSerialized]
	[FieldOffset(16)]
	public float m10;

	// Token: 0x04005959 RID: 22873
	[NonSerialized]
	[FieldOffset(20)]
	public float m11;

	// Token: 0x0400595A RID: 22874
	[NonSerialized]
	[FieldOffset(24)]
	public float m12;

	// Token: 0x0400595B RID: 22875
	[NonSerialized]
	[FieldOffset(28)]
	public float m13;

	// Token: 0x0400595C RID: 22876
	[NonSerialized]
	[FieldOffset(32)]
	public float m20;

	// Token: 0x0400595D RID: 22877
	[NonSerialized]
	[FieldOffset(36)]
	public float m21;

	// Token: 0x0400595E RID: 22878
	[NonSerialized]
	[FieldOffset(40)]
	public float m22;

	// Token: 0x0400595F RID: 22879
	[NonSerialized]
	[FieldOffset(44)]
	public float m23;

	// Token: 0x04005960 RID: 22880
	[NonSerialized]
	[FieldOffset(48)]
	public float m30;

	// Token: 0x04005961 RID: 22881
	[NonSerialized]
	[FieldOffset(52)]
	public float m31;

	// Token: 0x04005962 RID: 22882
	[NonSerialized]
	[FieldOffset(56)]
	public float m32;

	// Token: 0x04005963 RID: 22883
	[NonSerialized]
	[FieldOffset(60)]
	public float m33;

	// Token: 0x04005964 RID: 22884
	[HideInInspector]
	[FieldOffset(0)]
	public int i00;

	// Token: 0x04005965 RID: 22885
	[HideInInspector]
	[FieldOffset(4)]
	public int i01;

	// Token: 0x04005966 RID: 22886
	[HideInInspector]
	[FieldOffset(8)]
	public int i02;

	// Token: 0x04005967 RID: 22887
	[HideInInspector]
	[FieldOffset(12)]
	public int i03;

	// Token: 0x04005968 RID: 22888
	[HideInInspector]
	[FieldOffset(16)]
	public int i10;

	// Token: 0x04005969 RID: 22889
	[HideInInspector]
	[FieldOffset(20)]
	public int i11;

	// Token: 0x0400596A RID: 22890
	[HideInInspector]
	[FieldOffset(24)]
	public int i12;

	// Token: 0x0400596B RID: 22891
	[HideInInspector]
	[FieldOffset(28)]
	public int i13;

	// Token: 0x0400596C RID: 22892
	[HideInInspector]
	[FieldOffset(32)]
	public int i20;

	// Token: 0x0400596D RID: 22893
	[HideInInspector]
	[FieldOffset(36)]
	public int i21;

	// Token: 0x0400596E RID: 22894
	[HideInInspector]
	[FieldOffset(40)]
	public int i22;

	// Token: 0x0400596F RID: 22895
	[HideInInspector]
	[FieldOffset(44)]
	public int i23;

	// Token: 0x04005970 RID: 22896
	[HideInInspector]
	[FieldOffset(48)]
	public int i30;

	// Token: 0x04005971 RID: 22897
	[HideInInspector]
	[FieldOffset(52)]
	public int i31;

	// Token: 0x04005972 RID: 22898
	[HideInInspector]
	[FieldOffset(56)]
	public int i32;

	// Token: 0x04005973 RID: 22899
	[HideInInspector]
	[FieldOffset(60)]
	public int i33;

	// Token: 0x04005974 RID: 22900
	[NonSerialized]
	[FieldOffset(0)]
	public ushort h00_a;

	// Token: 0x04005975 RID: 22901
	[NonSerialized]
	[FieldOffset(2)]
	public ushort h00_b;

	// Token: 0x04005976 RID: 22902
	[NonSerialized]
	[FieldOffset(4)]
	public ushort h01_a;

	// Token: 0x04005977 RID: 22903
	[NonSerialized]
	[FieldOffset(6)]
	public ushort h01_b;

	// Token: 0x04005978 RID: 22904
	[NonSerialized]
	[FieldOffset(8)]
	public ushort h02_a;

	// Token: 0x04005979 RID: 22905
	[NonSerialized]
	[FieldOffset(10)]
	public ushort h02_b;

	// Token: 0x0400597A RID: 22906
	[NonSerialized]
	[FieldOffset(12)]
	public ushort h03_a;

	// Token: 0x0400597B RID: 22907
	[NonSerialized]
	[FieldOffset(14)]
	public ushort h03_b;

	// Token: 0x0400597C RID: 22908
	[NonSerialized]
	[FieldOffset(16)]
	public ushort h10_a;

	// Token: 0x0400597D RID: 22909
	[NonSerialized]
	[FieldOffset(18)]
	public ushort h10_b;

	// Token: 0x0400597E RID: 22910
	[NonSerialized]
	[FieldOffset(20)]
	public ushort h11_a;

	// Token: 0x0400597F RID: 22911
	[NonSerialized]
	[FieldOffset(22)]
	public ushort h11_b;

	// Token: 0x04005980 RID: 22912
	[NonSerialized]
	[FieldOffset(24)]
	public ushort h12_a;

	// Token: 0x04005981 RID: 22913
	[NonSerialized]
	[FieldOffset(26)]
	public ushort h12_b;

	// Token: 0x04005982 RID: 22914
	[NonSerialized]
	[FieldOffset(28)]
	public ushort h13_a;

	// Token: 0x04005983 RID: 22915
	[NonSerialized]
	[FieldOffset(30)]
	public ushort h13_b;

	// Token: 0x04005984 RID: 22916
	[NonSerialized]
	[FieldOffset(32)]
	public ushort h20_a;

	// Token: 0x04005985 RID: 22917
	[NonSerialized]
	[FieldOffset(34)]
	public ushort h20_b;

	// Token: 0x04005986 RID: 22918
	[NonSerialized]
	[FieldOffset(36)]
	public ushort h21_a;

	// Token: 0x04005987 RID: 22919
	[NonSerialized]
	[FieldOffset(38)]
	public ushort h21_b;

	// Token: 0x04005988 RID: 22920
	[NonSerialized]
	[FieldOffset(40)]
	public ushort h22_a;

	// Token: 0x04005989 RID: 22921
	[NonSerialized]
	[FieldOffset(42)]
	public ushort h22_b;

	// Token: 0x0400598A RID: 22922
	[NonSerialized]
	[FieldOffset(44)]
	public ushort h23_a;

	// Token: 0x0400598B RID: 22923
	[NonSerialized]
	[FieldOffset(46)]
	public ushort h23_b;

	// Token: 0x0400598C RID: 22924
	[NonSerialized]
	[FieldOffset(48)]
	public ushort h30_a;

	// Token: 0x0400598D RID: 22925
	[NonSerialized]
	[FieldOffset(50)]
	public ushort h30_b;

	// Token: 0x0400598E RID: 22926
	[NonSerialized]
	[FieldOffset(52)]
	public ushort h31_a;

	// Token: 0x0400598F RID: 22927
	[NonSerialized]
	[FieldOffset(54)]
	public ushort h31_b;

	// Token: 0x04005990 RID: 22928
	[NonSerialized]
	[FieldOffset(56)]
	public ushort h32_a;

	// Token: 0x04005991 RID: 22929
	[NonSerialized]
	[FieldOffset(58)]
	public ushort h32_b;

	// Token: 0x04005992 RID: 22930
	[NonSerialized]
	[FieldOffset(60)]
	public ushort h33_a;

	// Token: 0x04005993 RID: 22931
	[NonSerialized]
	[FieldOffset(62)]
	public ushort h33_b;

	// Token: 0x02000ADE RID: 2782
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_f>e__FixedBuffer
	{
		// Token: 0x04005994 RID: 22932
		public float FixedElementField;
	}

	// Token: 0x02000ADF RID: 2783
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_h>e__FixedBuffer
	{
		// Token: 0x04005995 RID: 22933
		public ushort FixedElementField;
	}

	// Token: 0x02000AE0 RID: 2784
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_i>e__FixedBuffer
	{
		// Token: 0x04005996 RID: 22934
		public int FixedElementField;
	}
}
