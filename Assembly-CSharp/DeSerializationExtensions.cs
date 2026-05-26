using System;

// Token: 0x020001DB RID: 475
public static class DeSerializationExtensions
{
	// Token: 0x06000C9A RID: 3226 RVA: 0x00044E3C File Offset: 0x0004303C
	public static bool TryDeserializeTo<T1>(this object[] eventData, out T1 v1)
	{
		v1 = default(T1);
		if (eventData == null || eventData.Length != 1)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			v1 = t;
			return true;
		}
		return false;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x00044E7C File Offset: 0x0004307C
	public static bool TryDeserializeTo<T1, T2>(this object[] eventData, out T1 v1, out T2 v2)
	{
		v1 = default(T1);
		v2 = default(T2);
		if (eventData == null || eventData.Length != 2)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				v1 = t;
				v2 = t2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x00044EDC File Offset: 0x000430DC
	public static bool TryDeserializeTo<T1, T2, T3>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		if (eventData == null || eventData.Length != 3)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					v1 = t;
					v2 = t2;
					v3 = t3;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x00044F5C File Offset: 0x0004315C
	public static bool TryDeserializeTo<T1, T2, T3, T4>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3, out T4 v4)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		v4 = default(T4);
		if (eventData == null || eventData.Length != 4)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						v1 = t;
						v2 = t2;
						v3 = t3;
						v4 = t4;
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x0004500C File Offset: 0x0004320C
	public static bool TryDeserializeTo<T1, T2, T3, T4, T5>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3, out T4 v4, out T5 v5)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		v4 = default(T4);
		v5 = default(T5);
		if (eventData == null || eventData.Length != 5)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							v1 = t;
							v2 = t2;
							v3 = t3;
							v4 = t4;
							v5 = t5;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x000450E4 File Offset: 0x000432E4
	public static bool TryDeserializeTo<T1, T2, T3, T4, T5, T6>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3, out T4 v4, out T5 v5, out T6 v6)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		v4 = default(T4);
		v5 = default(T5);
		v6 = default(T6);
		if (eventData == null || eventData.Length != 6)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							obj = eventData[5];
							if (obj is T6)
							{
								T6 t6 = (T6)((object)obj);
								v1 = t;
								v2 = t2;
								v3 = t3;
								v4 = t4;
								v5 = t5;
								v6 = t6;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x000451E4 File Offset: 0x000433E4
	public static bool TryDeserializeToRef<T1>(this object[] eventData, ref T1 v1)
	{
		if (eventData == null || eventData.Length != 1)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			v1 = t;
			return true;
		}
		return false;
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x0004521C File Offset: 0x0004341C
	public static bool TryDeserializeToRef<T1, T2>(this object[] eventData, ref T1 v1, ref T2 v2)
	{
		if (eventData == null || eventData.Length != 2)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				v1 = t;
				v2 = t2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x00045270 File Offset: 0x00043470
	public static bool TryDeserializeToRef<T1, T2, T3>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3)
	{
		if (eventData == null || eventData.Length != 3)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					v1 = t;
					v2 = t2;
					v3 = t3;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x000452DC File Offset: 0x000434DC
	public static bool TryDeserializeToRef<T1, T2, T3, T4>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3, ref T4 v4)
	{
		if (eventData == null || eventData.Length != 4)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						v1 = t;
						v2 = t2;
						v3 = t3;
						v4 = t4;
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00045370 File Offset: 0x00043570
	public static bool TryDeserializeToRef<T1, T2, T3, T4, T5>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3, ref T4 v4, ref T5 v5)
	{
		if (eventData == null || eventData.Length != 5)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							v1 = t;
							v2 = t2;
							v3 = t3;
							v4 = t4;
							v5 = t5;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00045424 File Offset: 0x00043624
	public static bool TryDeserializeToRef<T1, T2, T3, T4, T5, T6>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3, ref T4 v4, ref T5 v5, ref T6 v6)
	{
		if (eventData == null || eventData.Length != 6)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							obj = eventData[5];
							if (obj is T6)
							{
								T6 t6 = (T6)((object)obj);
								v1 = t;
								v2 = t2;
								v3 = t3;
								v4 = t4;
								v5 = t5;
								v6 = t6;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}
}
