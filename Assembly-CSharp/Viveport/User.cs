using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000E37 RID: 3639
	public class User
	{
		// Token: 0x06005889 RID: 22665 RVA: 0x001CAEA5 File Offset: 0x001C90A5
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			User.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x001CAEB4 File Offset: 0x001C90B4
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			User.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(User.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return User.IsReady_64(new StatusCallback(User.IsReadyIl2cppCallback));
			}
			return User.IsReady(new StatusCallback(User.IsReadyIl2cppCallback));
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x001CAF24 File Offset: 0x001C9124
		public static string GetUserId()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserID_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserID(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600588C RID: 22668 RVA: 0x001CAF64 File Offset: 0x001C9164
		public static string GetUserName()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserName_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserName(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x001CAFA4 File Offset: 0x001C91A4
		public static string GetUserAvatarUrl()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (IntPtr.Size == 8)
			{
				User.GetUserAvatarUrl_64(stringBuilder, 512);
			}
			else
			{
				User.GetUserAvatarUrl(stringBuilder, 512);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400690A RID: 26890
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400690B RID: 26891
		private const int MaxIdLength = 256;

		// Token: 0x0400690C RID: 26892
		private const int MaxNameLength = 256;

		// Token: 0x0400690D RID: 26893
		private const int MaxUrlLength = 512;
	}
}
