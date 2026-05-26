using System;

namespace GorillaTag
{
	// Token: 0x02001177 RID: 4471
	public abstract class DelegateListProcessorPlusMinus<T1, T2> : ListProcessorAbstract<T2> where T1 : DelegateListProcessorPlusMinus<T1, T2>, new() where T2 : Delegate
	{
		// Token: 0x06007134 RID: 28980 RVA: 0x0024FBD2 File Offset: 0x0024DDD2
		protected DelegateListProcessorPlusMinus()
		{
		}

		// Token: 0x06007135 RID: 28981 RVA: 0x0024FBDA File Offset: 0x0024DDDA
		protected DelegateListProcessorPlusMinus(int capacity) : base(capacity)
		{
		}

		// Token: 0x06007136 RID: 28982 RVA: 0x0024FBE3 File Offset: 0x0024DDE3
		public static T1 operator +(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				left = Activator.CreateInstance<T1>();
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Add(right);
			return (T1)((object)left);
		}

		// Token: 0x06007137 RID: 28983 RVA: 0x0024FC14 File Offset: 0x0024DE14
		public static T1 operator -(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				return default(T1);
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Remove(right);
			return (T1)((object)left);
		}
	}
}
