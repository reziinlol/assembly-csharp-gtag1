using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001195 RID: 4501
	[RequireComponent(typeof(AudioSource))]
	[NetworkBehaviourWeaved(2)]
	public class SportScoreboard : NetworkComponent
	{
		// Token: 0x060071ED RID: 29165 RVA: 0x002515DC File Offset: 0x0024F7DC
		protected override void Awake()
		{
			base.Awake();
			SportScoreboard.Instance = this;
			this.audioSource = base.GetComponent<AudioSource>();
			this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x060071EE RID: 29166 RVA: 0x00251645 File Offset: 0x0024F845
		public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
		{
			this.scoreVisuals[TeamIndex] = visuals;
			this.UpdateScoreboard();
		}

		// Token: 0x060071EF RID: 29167 RVA: 0x00251658 File Offset: 0x0024F858
		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				if (!(this.scoreVisuals[i] == null))
				{
					int num = this.teamScores[i];
					if (this.scoreVisuals[i].score1s != null)
					{
						this.scoreVisuals[i].score1s.SetUVOffset(num % 10);
					}
					if (this.scoreVisuals[i].score10s != null)
					{
						this.scoreVisuals[i].score10s.SetUVOffset(num / 10 % 10);
					}
				}
			}
		}

		// Token: 0x060071F0 RID: 29168 RVA: 0x002516F4 File Offset: 0x0024F8F4
		private void OnScoreUpdated()
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				if (this.teamScores[i] > this.teamScoresPrev[i] && this.teamParameters[i].goalScoredAudio != null && this.teamScores[i] < this.matchEndScore)
				{
					this.audioSource.GTPlayOneShot(this.teamParameters[i].goalScoredAudio, 1f);
				}
				this.teamScoresPrev[i] = this.teamScores[i];
			}
			if (!this.runningMatchEndCoroutine)
			{
				for (int j = 0; j < this.teamScores.Count; j++)
				{
					if (this.teamScores[j] >= this.matchEndScore)
					{
						base.StartCoroutine(this.MatchEndCoroutine(j));
						break;
					}
				}
			}
			this.UpdateScoreboard();
		}

		// Token: 0x060071F1 RID: 29169 RVA: 0x002517E8 File Offset: 0x0024F9E8
		public void TeamScored(int team)
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				if (team >= 0 && team < this.teamScores.Count)
				{
					this.teamScores[team] = this.teamScores[team] + 1;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x060071F2 RID: 29170 RVA: 0x00251838 File Offset: 0x0024FA38
		public void ResetScores()
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					this.teamScores[i] = 0;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x060071F3 RID: 29171 RVA: 0x0025187E File Offset: 0x0024FA7E
		private IEnumerator MatchEndCoroutine(int winningTeam)
		{
			this.runningMatchEndCoroutine = true;
			if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && this.teamParameters[winningTeam].matchWonAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.teamParameters[winningTeam].matchWonAudio, 1f);
			}
			yield return new WaitForSeconds(this.matchEndScoreResetDelayTime);
			this.runningMatchEndCoroutine = false;
			this.ResetScores();
			yield break;
		}

		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x060071F4 RID: 29172 RVA: 0x00251894 File Offset: 0x0024FA94
		[Networked]
		[Capacity(2)]
		[NetworkedWeaved(0, 2)]
		[NetworkedWeavedArray(2, 1, typeof(ElementReaderWriterInt32))]
		public unsafe NetworkArray<int> Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing SportScoreboard.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkArray<int>((byte*)(this.Ptr + 0), 2, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x060071F5 RID: 29173 RVA: 0x002518D0 File Offset: 0x0024FAD0
		public override void WriteDataFusion()
		{
			this.Data.CopyFrom(this.teamScores, 0, this.teamScores.Count);
		}

		// Token: 0x060071F6 RID: 29174 RVA: 0x00251900 File Offset: 0x0024FB00
		public override void ReadDataFusion()
		{
			this.teamScores.Clear();
			this.Data.CopyTo(this.teamScores);
			this.OnScoreUpdated();
		}

		// Token: 0x060071F7 RID: 29175 RVA: 0x00251934 File Offset: 0x0024FB34
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				stream.SendNext(this.teamScores[i]);
			}
		}

		// Token: 0x060071F8 RID: 29176 RVA: 0x00251970 File Offset: 0x0024FB70
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				this.teamScores[i] = (int)stream.ReceiveNext();
			}
			this.OnScoreUpdated();
		}

		// Token: 0x060071FA RID: 29178 RVA: 0x002519EB File Offset: 0x0024FBEB
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			NetworkBehaviourUtils.InitializeNetworkArray<int>(this.Data, this._Data, "Data");
		}

		// Token: 0x060071FB RID: 29179 RVA: 0x00251A0D File Offset: 0x0024FC0D
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			NetworkBehaviourUtils.CopyFromNetworkArray<int>(this.Data, ref this._Data);
		}

		// Token: 0x0400819B RID: 33179
		[OnEnterPlay_SetNull]
		public static SportScoreboard Instance;

		// Token: 0x0400819C RID: 33180
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x0400819D RID: 33181
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x0400819E RID: 33182
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x0400819F RID: 33183
		private List<int> teamScores = new List<int>();

		// Token: 0x040081A0 RID: 33184
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x040081A1 RID: 33185
		private bool runningMatchEndCoroutine;

		// Token: 0x040081A2 RID: 33186
		private AudioSource audioSource;

		// Token: 0x040081A3 RID: 33187
		private SportScoreboardVisuals[] scoreVisuals;

		// Token: 0x040081A4 RID: 33188
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 2)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int[] _Data;

		// Token: 0x02001196 RID: 4502
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x040081A5 RID: 33189
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x040081A6 RID: 33190
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
