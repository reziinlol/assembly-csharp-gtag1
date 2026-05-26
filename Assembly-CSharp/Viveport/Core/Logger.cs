using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x02000E6F RID: 3695
	public class Logger
	{
		// Token: 0x06005A3B RID: 23099 RVA: 0x001CDC34 File Offset: 0x001CBE34
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x06005A3C RID: 23100 RVA: 0x001CDC51 File Offset: 0x001CBE51
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x001CDC60 File Offset: 0x001CBE60
		private static void UnityLog(string message)
		{
			try
			{
				if (Logger._unityLogType == null)
				{
					Logger._unityLogType = Logger.GetType("UnityEngine.Debug");
				}
				Logger._unityLogType.GetMethod("Log", new Type[]
				{
					typeof(string)
				}).Invoke(null, new object[]
				{
					message
				});
				Logger._usingUnityLog = true;
			}
			catch (Exception)
			{
				Logger.ConsoleLog(message);
				Logger._usingUnityLog = false;
			}
			Logger._hasDetected = true;
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x001CDCEC File Offset: 0x001CBEEC
		private static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x040069A7 RID: 27047
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x040069A8 RID: 27048
		private static bool _hasDetected;

		// Token: 0x040069A9 RID: 27049
		private static bool _usingUnityLog = true;

		// Token: 0x040069AA RID: 27050
		private static Type _unityLogType;
	}
}
