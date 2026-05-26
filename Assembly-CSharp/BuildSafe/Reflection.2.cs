using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x0200100A RID: 4106
	public static class Reflection
	{
		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06006689 RID: 26249 RVA: 0x002102C4 File Offset: 0x0020E4C4
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x0600668A RID: 26250 RVA: 0x002102CB File Offset: 0x0020E4CB
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x0600668B RID: 26251 RVA: 0x002102D2 File Offset: 0x0020E4D2
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x0600668C RID: 26252 RVA: 0x002102E0 File Offset: 0x0020E4E0
		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = (from a in AppDomain.CurrentDomain.GetAssemblies()
			where a != null
			select a).ToArray<Assembly>();
		}

		// Token: 0x0600668D RID: 26253 RVA: 0x00210334 File Offset: 0x0020E534
		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = (from t in Reflection.PreFetchAllAssemblies().SelectMany((Assembly a) => a.GetTypes())
			where t != null
			select t).ToArray<Type>();
		}

		// Token: 0x0600668E RID: 26254 RVA: 0x002103A8 File Offset: 0x0020E5A8
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
			where m.GetCustomAttributes(typeof(T), false).Length != 0
			select m).ToArray<MethodInfo>();
		}

		// Token: 0x040075F7 RID: 30199
		private static Assembly[] gAssemblyCache;

		// Token: 0x040075F8 RID: 30200
		private static Type[] gTypeCache;
	}
}
