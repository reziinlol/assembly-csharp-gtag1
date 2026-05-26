using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02001009 RID: 4105
	public static class Reflection<T>
	{
		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x0600667F RID: 26239 RVA: 0x00210200 File Offset: 0x0020E400
		public static Type Type { get; } = typeof(T);

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06006680 RID: 26240 RVA: 0x00210207 File Offset: 0x0020E407
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x06006681 RID: 26241 RVA: 0x0021020E File Offset: 0x0020E40E
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x06006682 RID: 26242 RVA: 0x00210215 File Offset: 0x0020E415
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06006683 RID: 26243 RVA: 0x0021021C File Offset: 0x0020E41C
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06006684 RID: 26244 RVA: 0x00210223 File Offset: 0x0020E423
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x06006685 RID: 26245 RVA: 0x00210247 File Offset: 0x0020E447
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x06006686 RID: 26246 RVA: 0x0021026B File Offset: 0x0020E46B
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x06006687 RID: 26247 RVA: 0x0021028F File Offset: 0x0020E48F
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x040075F1 RID: 30193
		private static Type gCachedType;

		// Token: 0x040075F2 RID: 30194
		private static MethodInfo[] gMethodsCache;

		// Token: 0x040075F3 RID: 30195
		private static FieldInfo[] gFieldsCache;

		// Token: 0x040075F4 RID: 30196
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x040075F5 RID: 30197
		private static EventInfo[] gEventsCache;
	}
}
