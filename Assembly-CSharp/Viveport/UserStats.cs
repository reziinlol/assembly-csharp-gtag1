using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000E38 RID: 3640
	public class UserStats
	{
		// Token: 0x0600588F RID: 22671 RVA: 0x001CAFE4 File Offset: 0x001C91E4
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005890 RID: 22672 RVA: 0x001CAFF4 File Offset: 0x001C91F4
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.IsReady_64(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			}
			return UserStats.IsReady(new StatusCallback(UserStats.IsReadyIl2cppCallback));
		}

		// Token: 0x06005891 RID: 22673 RVA: 0x001CB061 File Offset: 0x001C9261
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06005892 RID: 22674 RVA: 0x001CB070 File Offset: 0x001C9270
		public static int DownloadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadStats_64(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			}
			return UserStats.DownloadStats(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x001CB0E0 File Offset: 0x001C92E0
		public static int GetStat(string name, int defaultValue)
		{
			int result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x001CB10C File Offset: 0x001C930C
		public static float GetStat(string name, float defaultValue)
		{
			float result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06005895 RID: 22677 RVA: 0x001CB138 File Offset: 0x001C9338
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06005896 RID: 22678 RVA: 0x001CB153 File Offset: 0x001C9353
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06005897 RID: 22679 RVA: 0x001CB16E File Offset: 0x001C936E
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x001CB17C File Offset: 0x001C937C
		public static int UploadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadStats_64(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			}
			return UserStats.UploadStats(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
		}

		// Token: 0x06005899 RID: 22681 RVA: 0x001CB1EC File Offset: 0x001C93EC
		public static bool GetAchievement(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievement_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievement(pchName, ref num);
			}
			return num == 1;
		}

		// Token: 0x0600589A RID: 22682 RVA: 0x001CB21C File Offset: 0x001C941C
		public static int GetAchievementUnlockTime(string pchName)
		{
			int result = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievementUnlockTime_64(pchName, ref result);
			}
			else
			{
				UserStats.GetAchievementUnlockTime(pchName, ref result);
			}
			return result;
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x0016D2F3 File Offset: 0x0016B4F3
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x0600589C RID: 22684 RVA: 0x0016D2F3 File Offset: 0x0016B4F3
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x0016D2F3 File Offset: 0x0016B4F3
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001CB248 File Offset: 0x001C9448
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x001CB25F File Offset: 0x001C945F
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x001CB276 File Offset: 0x001C9476
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x060058A1 RID: 22689 RVA: 0x001CB284 File Offset: 0x001C9484
		public static int DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, UserStats.LeaderBoardRequestType eLeaderboardDataRequest, UserStats.LeaderBoardTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadLeaderboardScores_64(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
			}
			return UserStats.DownloadLeaderboardScores(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
		}

		// Token: 0x060058A2 RID: 22690 RVA: 0x001CB2FF File Offset: 0x001C94FF
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x001CB30C File Offset: 0x001C950C
		public static int UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadLeaderboardScore_64(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
			}
			return UserStats.UploadLeaderboardScore(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
		}

		// Token: 0x060058A4 RID: 22692 RVA: 0x001CB380 File Offset: 0x001C9580
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				UserStats.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				UserStats.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x060058A5 RID: 22693 RVA: 0x001CB3EE File Offset: 0x001C95EE
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x060058A6 RID: 22694 RVA: 0x001CB403 File Offset: 0x001C9603
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x060058A7 RID: 22695 RVA: 0x001CB418 File Offset: 0x001C9618
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x0400690E RID: 26894
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400690F RID: 26895
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x04006910 RID: 26896
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x04006911 RID: 26897
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x04006912 RID: 26898
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000E39 RID: 3641
		public enum LeaderBoardRequestType
		{
			// Token: 0x04006914 RID: 26900
			GlobalData,
			// Token: 0x04006915 RID: 26901
			GlobalDataAroundUser,
			// Token: 0x04006916 RID: 26902
			LocalData,
			// Token: 0x04006917 RID: 26903
			LocalDataAroundUser
		}

		// Token: 0x02000E3A RID: 3642
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04006919 RID: 26905
			AllTime,
			// Token: 0x0400691A RID: 26906
			Daily,
			// Token: 0x0400691B RID: 26907
			Weekly,
			// Token: 0x0400691C RID: 26908
			Monthly
		}

		// Token: 0x02000E3B RID: 3643
		public enum LeaderBoardSortMethod
		{
			// Token: 0x0400691E RID: 26910
			None,
			// Token: 0x0400691F RID: 26911
			Ascending,
			// Token: 0x04006920 RID: 26912
			Descending
		}

		// Token: 0x02000E3C RID: 3644
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x04006922 RID: 26914
			None,
			// Token: 0x04006923 RID: 26915
			Numeric,
			// Token: 0x04006924 RID: 26916
			TimeSeconds,
			// Token: 0x04006925 RID: 26917
			TimeMilliSeconds
		}

		// Token: 0x02000E3D RID: 3645
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04006927 RID: 26919
			None,
			// Token: 0x04006928 RID: 26920
			KeepBest,
			// Token: 0x04006929 RID: 26921
			ForceUpdate
		}

		// Token: 0x02000E3E RID: 3646
		public enum AchievementDisplayAttribute
		{
			// Token: 0x0400692B RID: 26923
			Name,
			// Token: 0x0400692C RID: 26924
			Desc,
			// Token: 0x0400692D RID: 26925
			Hidden
		}
	}
}
