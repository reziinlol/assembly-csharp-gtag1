using System;

// Token: 0x02000D8D RID: 3469
public static class StaticHashExt
{
	// Token: 0x06005525 RID: 21797 RVA: 0x001BCEB1 File Offset: 0x001BB0B1
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Compute(i);
	}

	// Token: 0x06005526 RID: 21798 RVA: 0x001BCEB9 File Offset: 0x001BB0B9
	public static int GetStaticHash(this uint u)
	{
		return StaticHash.Compute(u);
	}

	// Token: 0x06005527 RID: 21799 RVA: 0x001BCEC1 File Offset: 0x001BB0C1
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Compute(f);
	}

	// Token: 0x06005528 RID: 21800 RVA: 0x001BCEC9 File Offset: 0x001BB0C9
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Compute(l);
	}

	// Token: 0x06005529 RID: 21801 RVA: 0x001BCED1 File Offset: 0x001BB0D1
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Compute(d);
	}

	// Token: 0x0600552A RID: 21802 RVA: 0x001BCED9 File Offset: 0x001BB0D9
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Compute(b);
	}

	// Token: 0x0600552B RID: 21803 RVA: 0x001BCEE1 File Offset: 0x001BB0E1
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Compute(dt);
	}

	// Token: 0x0600552C RID: 21804 RVA: 0x001BCEE9 File Offset: 0x001BB0E9
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Compute(s);
	}

	// Token: 0x0600552D RID: 21805 RVA: 0x001BCEF1 File Offset: 0x001BB0F1
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Compute(bytes);
	}
}
