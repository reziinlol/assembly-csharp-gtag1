using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200105C RID: 4188
	public class GhostReactorProgression : MonoBehaviour
	{
		// Token: 0x0600692C RID: 26924 RVA: 0x00220980 File Offset: 0x0021EB80
		public void Awake()
		{
			GhostReactorProgression.instance = this;
		}

		// Token: 0x0600692D RID: 26925 RVA: 0x00220988 File Offset: 0x0021EB88
		public void Start()
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnTrackRead += this.OnTrackRead;
				ProgressionManager.Instance.OnTrackSet += this.OnTrackSet;
				ProgressionManager.Instance.OnNodeUnlocked += delegate(string a, string b)
				{
					this.OnNodeUnlocked();
				};
				return;
			}
			Debug.Log("GRP: ProgressionManager is null!");
		}

		// Token: 0x0600692E RID: 26926 RVA: 0x002209F0 File Offset: 0x0021EBF0
		public void GetStartingProgression(GRPlayer grPlayer)
		{
			GhostReactorProgression.<GetStartingProgression>d__6 <GetStartingProgression>d__;
			<GetStartingProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GetStartingProgression>d__.<>4__this = this;
			<GetStartingProgression>d__.grPlayer = grPlayer;
			<GetStartingProgression>d__.<>1__state = -1;
			<GetStartingProgression>d__.<>t__builder.Start<GhostReactorProgression.<GetStartingProgression>d__6>(ref <GetStartingProgression>d__);
		}

		// Token: 0x0600692F RID: 26927 RVA: 0x00220A2F File Offset: 0x0021EC2F
		public void SetProgression(int progressionAmountToAdd, GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
			ProgressionManager.Instance.SetProgression(this.progressionTrackId, progressionAmountToAdd);
		}

		// Token: 0x06006930 RID: 26928 RVA: 0x00220A49 File Offset: 0x0021EC49
		public void UnlockProgressionTreeNode(string treeId, string nodeId, GhostReactor reactor)
		{
			this._reactor = reactor;
			ProgressionManager.Instance.UnlockNode(treeId, nodeId);
		}

		// Token: 0x06006931 RID: 26929 RVA: 0x00220A60 File Offset: 0x0021EC60
		private void OnTrackRead(string trackId, int progress)
		{
			if (this._grPlayer == null)
			{
				Debug.Log("GRP: OnTrackRead Failure: player is null");
				return;
			}
			if (trackId != this.progressionTrackId)
			{
				Debug.Log(string.Format("GRP: OnTrackRead Failure: track [{0}] progressionTrack [{1}] progress {2}", trackId, this.progressionTrackId, progress));
				return;
			}
			this._grPlayer.SetProgressionData(progress, progress, false);
		}

		// Token: 0x06006932 RID: 26930 RVA: 0x00220ABF File Offset: 0x0021ECBF
		private void OnTrackSet(string trackId, int progress)
		{
			if (this._grPlayer == null)
			{
				return;
			}
			if (trackId != this.progressionTrackId)
			{
				return;
			}
			this._grPlayer.SetProgressionData(progress, this._grPlayer.CurrentProgression.redeemedPoints, false);
		}

		// Token: 0x06006933 RID: 26931 RVA: 0x00220AFC File Offset: 0x0021ECFC
		private void OnNodeUnlocked()
		{
			if (this._reactor != null && this._reactor.toolProgression != null)
			{
				this._reactor.UpdateLocalPlayerFromProgression();
			}
		}

		// Token: 0x06006934 RID: 26932 RVA: 0x00220B2C File Offset: 0x0021ED2C
		[return: TupleElementNames(new string[]
		{
			"tier",
			"grade",
			"totalPointsToNextLevel",
			"partialPointsToNextLevel"
		})]
		public static ValueTuple<int, int, int, int> GetGradePointDetails(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num2 = num;
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					break;
				}
			}
			if (points > num)
			{
				return new ValueTuple<int, int, int, int>(i - 1, 0, 0, 0);
			}
			int pointsPerGrade = GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
			int item = (points - num2) / pointsPerGrade;
			int item2 = (points - num2) % pointsPerGrade;
			return new ValueTuple<int, int, int, int>(i, item, pointsPerGrade, item2);
		}

		// Token: 0x06006935 RID: 26933 RVA: 0x00220BD4 File Offset: 0x0021EDD4
		public static string GetTitleNameAndGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName + " " + (GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1).ToString();
				}
			}
			return "null";
		}

		// Token: 0x06006936 RID: 26934 RVA: 0x00220CA0 File Offset: 0x0021EEA0
		public static string GetTitleName(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x06006937 RID: 26935 RVA: 0x00220D1C File Offset: 0x0021EF1C
		public static string GetTitleNameFromLevel(int level)
		{
			GhostReactorProgression.LoadGRPSO();
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				if (GhostReactorProgression.grPSO.progressionData[i].tierId >= level)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x06006938 RID: 26936 RVA: 0x00220D7C File Offset: 0x0021EF7C
		public static int GetGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1;
				}
			}
			return -1;
		}

		// Token: 0x06006939 RID: 26937 RVA: 0x00220E18 File Offset: 0x0021F018
		public static int GetTitleLevel(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierId;
				}
			}
			return -1;
		}

		// Token: 0x0600693A RID: 26938 RVA: 0x00220E8F File Offset: 0x0021F08F
		public static void LoadGRPSO()
		{
			if (GhostReactorProgression.grPSO == null)
			{
				GhostReactorProgression.grPSO = Resources.Load<GRProgressionScriptableObject>("ProgressionTiersData");
			}
		}

		// Token: 0x0400795F RID: 31071
		public static GhostReactorProgression instance;

		// Token: 0x04007960 RID: 31072
		private string progressionTrackId = "a0208736-e696-489b-81cd-c0c772489cc5";

		// Token: 0x04007961 RID: 31073
		private GRPlayer _grPlayer;

		// Token: 0x04007962 RID: 31074
		private GhostReactor _reactor;

		// Token: 0x04007963 RID: 31075
		public static GRProgressionScriptableObject grPSO;

		// Token: 0x04007964 RID: 31076
		public const string grPSODirectory = "ProgressionTiersData";
	}
}
