using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class SetAnimatorBoolCosmetic : MonoBehaviour
{
	// Token: 0x06000491 RID: 1169 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnAnimatorValueChanged()
	{
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00019B82 File Offset: 0x00017D82
	public void SetAnimatorBool(bool value)
	{
		if (this.bool1Hash == 0)
		{
			this.bool1Hash = Animator.StringToHash(this.boolParameterName);
		}
		this.animator.SetBool(this.bool1Hash, value);
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00019BAF File Offset: 0x00017DAF
	public void SetAnimatorBool2(bool value)
	{
		if (this.bool2Hash == 0)
		{
			this.bool2Hash = Animator.StringToHash(this.bool2ParameterName);
		}
		this.animator.SetBool(this.bool2Hash, value);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00019BDC File Offset: 0x00017DDC
	public void SetAnimatorBool3(bool value)
	{
		if (this.bool3Hash == 0)
		{
			this.bool3Hash = Animator.StringToHash(this.bool3ParameterName);
		}
		this.animator.SetBool(this.bool3Hash, value);
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00019C09 File Offset: 0x00017E09
	public void SetAnimatorBool4(bool value)
	{
		if (this.bool4Hash == 0)
		{
			this.bool4Hash = Animator.StringToHash(this.bool4ParameterName);
		}
		this.animator.SetBool(this.bool4Hash, value);
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00019C36 File Offset: 0x00017E36
	public void SetAnimatorBool5(bool value)
	{
		if (this.bool5Hash == 0)
		{
			this.bool5Hash = Animator.StringToHash(this.bool5ParameterName);
		}
		this.animator.SetBool(this.bool5Hash, value);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00019C63 File Offset: 0x00017E63
	public void SetAnimatorInteger1(int value)
	{
		if (this.int1Hash == 0)
		{
			this.int1Hash = Animator.StringToHash(this.int1ParameterName);
		}
		this.animator.SetInteger(this.int1Hash, value);
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00019C90 File Offset: 0x00017E90
	public void SetAnimatorInteger2(int value)
	{
		if (this.int2Hash == 0)
		{
			this.int2Hash = Animator.StringToHash(this.int2ParameterName);
		}
		this.animator.SetInteger(this.int2Hash, value);
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x00019CBD File Offset: 0x00017EBD
	public void SetAnimatorInteger3(int value)
	{
		if (this.int3Hash == 0)
		{
			this.int3Hash = Animator.StringToHash(this.int3ParameterName);
		}
		this.animator.SetInteger(this.int3Hash, value);
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00019CEA File Offset: 0x00017EEA
	public void SetAnimatorInteger4(int value)
	{
		if (this.int4Hash == 0)
		{
			this.int4Hash = Animator.StringToHash(this.int4ParameterName);
		}
		this.animator.SetInteger(this.int4Hash, value);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00019D17 File Offset: 0x00017F17
	public void SetAnimatorFloat1(float value)
	{
		if (this.float1Hash == 0)
		{
			this.float1Hash = Animator.StringToHash(this.float1ParameterName);
		}
		this.animator.SetFloat(this.float1Hash, value);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00019D44 File Offset: 0x00017F44
	public void SetAnimatorFloat2(float value)
	{
		if (this.float2Hash == 0)
		{
			this.float2Hash = Animator.StringToHash(this.float2ParameterName);
		}
		this.animator.SetFloat(this.float2Hash, value);
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00019D71 File Offset: 0x00017F71
	public void SetAnimatorFloat3(float value)
	{
		if (this.float3Hash == 0)
		{
			this.float3Hash = Animator.StringToHash(this.float3ParameterName);
		}
		this.animator.SetFloat(this.float3Hash, value);
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00019D9E File Offset: 0x00017F9E
	public void SetAnimatorFloat4(float value)
	{
		if (this.float4Hash == 0)
		{
			this.float4Hash = Animator.StringToHash(this.float4ParameterName);
		}
		this.animator.SetFloat(this.float4Hash, value);
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00019DCB File Offset: 0x00017FCB
	public void SetAnimatorTrigger(string triggerName)
	{
		this.animator.SetTrigger(triggerName);
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00019DD9 File Offset: 0x00017FD9
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x040004EB RID: 1259
	[SerializeField]
	private Animator animator;

	// Token: 0x040004EC RID: 1260
	[SerializeField]
	private string boolParameterName;

	// Token: 0x040004ED RID: 1261
	[SerializeField]
	private string bool2ParameterName;

	// Token: 0x040004EE RID: 1262
	[SerializeField]
	private string bool3ParameterName;

	// Token: 0x040004EF RID: 1263
	[SerializeField]
	private string bool4ParameterName;

	// Token: 0x040004F0 RID: 1264
	[SerializeField]
	private string bool5ParameterName;

	// Token: 0x040004F1 RID: 1265
	[SerializeField]
	private string int1ParameterName;

	// Token: 0x040004F2 RID: 1266
	[SerializeField]
	private string int2ParameterName;

	// Token: 0x040004F3 RID: 1267
	[SerializeField]
	private string int3ParameterName;

	// Token: 0x040004F4 RID: 1268
	[SerializeField]
	private string int4ParameterName;

	// Token: 0x040004F5 RID: 1269
	[SerializeField]
	private string float1ParameterName;

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private string float2ParameterName;

	// Token: 0x040004F7 RID: 1271
	[SerializeField]
	private string float3ParameterName;

	// Token: 0x040004F8 RID: 1272
	[SerializeField]
	private string float4ParameterName;

	// Token: 0x040004F9 RID: 1273
	private int bool1Hash;

	// Token: 0x040004FA RID: 1274
	private int bool2Hash;

	// Token: 0x040004FB RID: 1275
	private int bool3Hash;

	// Token: 0x040004FC RID: 1276
	private int bool4Hash;

	// Token: 0x040004FD RID: 1277
	private int bool5Hash;

	// Token: 0x040004FE RID: 1278
	private const int MAX_BOOLS = 5;

	// Token: 0x040004FF RID: 1279
	private int int1Hash;

	// Token: 0x04000500 RID: 1280
	private int int2Hash;

	// Token: 0x04000501 RID: 1281
	private int int3Hash;

	// Token: 0x04000502 RID: 1282
	private int int4Hash;

	// Token: 0x04000503 RID: 1283
	private const int MAX_INTS = 4;

	// Token: 0x04000504 RID: 1284
	private int float1Hash;

	// Token: 0x04000505 RID: 1285
	private int float2Hash;

	// Token: 0x04000506 RID: 1286
	private int float3Hash;

	// Token: 0x04000507 RID: 1287
	private int float4Hash;

	// Token: 0x04000508 RID: 1288
	private const int MAX_FLOATS = 4;
}
