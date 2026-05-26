using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DA4 RID: 3492
public static class WaitUtils
{
	// Token: 0x060055AF RID: 21935 RVA: 0x001BE877 File Offset: 0x001BCA77
	public static WaitForSeconds WaitForSeconds(float seconds)
	{
		WaitUtils._waitForSecondsSetter(seconds);
		return WaitUtils._waitForSeconds;
	}

	// Token: 0x040065C3 RID: 26051
	private static WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);

	// Token: 0x040065C4 RID: 26052
	private static ParameterExpression _param = Expression.Parameter(typeof(float));

	// Token: 0x040065C5 RID: 26053
	private static Action<float> _waitForSecondsSetter = Expression.Lambda<Action<float>>(Expression.Assign(Expression.Field(Expression.Constant(WaitUtils._waitForSeconds, typeof(WaitForSeconds)), typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic)), WaitUtils._param), new ParameterExpression[]
	{
		WaitUtils._param
	}).Compile();
}
