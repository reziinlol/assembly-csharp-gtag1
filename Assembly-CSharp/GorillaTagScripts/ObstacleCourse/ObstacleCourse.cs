using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000F7B RID: 3963
	public class ObstacleCourse : MonoBehaviour
	{
		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x060062EE RID: 25326 RVA: 0x001FDF98 File Offset: 0x001FC198
		// (set) Token: 0x060062EF RID: 25327 RVA: 0x001FDFA0 File Offset: 0x001FC1A0
		public int winnerActorNumber { get; private set; }

		// Token: 0x060062F0 RID: 25328 RVA: 0x001FDFAC File Offset: 0x001FC1AC
		private void Awake()
		{
			this.numPlayersOnCourse = 0;
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter += this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit += this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped += this.OnEndLineTrigger;
		}

		// Token: 0x060062F1 RID: 25329 RVA: 0x001FE020 File Offset: 0x001FC220
		private void OnDestroy()
		{
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter -= this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit -= this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped -= this.OnEndLineTrigger;
		}

		// Token: 0x060062F2 RID: 25330 RVA: 0x001FE08D File Offset: 0x001FC28D
		private void Start()
		{
			this.RestartTimer(false);
		}

		// Token: 0x060062F3 RID: 25331 RVA: 0x001FE096 File Offset: 0x001FC296
		public void InvokeUpdate()
		{
			if (NetworkSystem.Instance.InRoom && ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Finished && Time.time - this.startTime >= this.cooldownTime)
			{
				this.RestartTimer(true);
			}
		}

		// Token: 0x060062F4 RID: 25332 RVA: 0x001FE0D4 File Offset: 0x001FC2D4
		public void OnPlayerEnterZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse++;
			}
		}

		// Token: 0x060062F5 RID: 25333 RVA: 0x001FE0F0 File Offset: 0x001FC2F0
		public void OnPlayerExitZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse--;
			}
		}

		// Token: 0x060062F6 RID: 25334 RVA: 0x001FE10C File Offset: 0x001FC30C
		private void RestartTimer(bool playFx = true)
		{
			this.UpdateState(ObstacleCourse.RaceState.Started, playFx);
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x001FE116 File Offset: 0x001FC316
		private void EndRace()
		{
			this.UpdateState(ObstacleCourse.RaceState.Finished, true);
			this.startTime = Time.time;
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x001FE12C File Offset: 0x001FC32C
		public void PlayWinningEffects()
		{
			if (this.confettiParticle)
			{
				this.confettiParticle.Play();
			}
			if (this.bannerRenderer)
			{
				UberShaderProperty baseColor = UberShader.BaseColor;
				Material material = this.bannerRenderer.material;
				RigContainer rigContainer = this.winnerRig;
				baseColor.SetValue<Color?>(material, (rigContainer != null) ? new Color?(rigContainer.Rig.playerColor) : null);
			}
			this.audioSource.GTPlay();
		}

		// Token: 0x060062F9 RID: 25337 RVA: 0x001FE1A2 File Offset: 0x001FC3A2
		public void OnEndLineTrigger(VRRig rig)
		{
			if (ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.winnerActorNumber = rig.creator.ActorNumber;
				this.winnerRig = rig.rigContainer;
				this.EndRace();
			}
		}

		// Token: 0x060062FA RID: 25338 RVA: 0x001FE1DB File Offset: 0x001FC3DB
		public void Deserialize(int _winnerActorNumber, ObstacleCourse.RaceState _currentState)
		{
			if (!ObstacleCourseManager.Instance.IsMine)
			{
				this.winnerActorNumber = _winnerActorNumber;
				VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(this.winnerActorNumber), out this.winnerRig);
				this.UpdateState(_currentState, true);
			}
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x001FE21C File Offset: 0x001FC41C
		private void UpdateState(ObstacleCourse.RaceState state, bool playFX = true)
		{
			this.currentState = state;
			WinnerScoreboard winnerScoreboard = this.scoreboard;
			RigContainer rigContainer = this.winnerRig;
			winnerScoreboard.UpdateBoard((rigContainer != null) ? rigContainer.Rig.playerNameVisible : null, this.currentState);
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.PlayWinningEffects();
			}
			else if (this.currentState == ObstacleCourse.RaceState.Started && this.bannerRenderer)
			{
				UberShader.BaseColor.SetValue<Color>(this.bannerRenderer.material, Color.white);
			}
			this.UpdateStartingGate();
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x001FE2A0 File Offset: 0x001FC4A0
		private void UpdateStartingGate()
		{
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, 90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, -90f);
				return;
			}
			if (this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, -90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, 90f);
			}
		}

		// Token: 0x040071AA RID: 29098
		public WinnerScoreboard scoreboard;

		// Token: 0x040071AC RID: 29100
		private RigContainer winnerRig;

		// Token: 0x040071AD RID: 29101
		public ObstacleCourseZoneTrigger[] zoneTriggers;

		// Token: 0x040071AE RID: 29102
		[HideInInspector]
		public ObstacleCourse.RaceState currentState;

		// Token: 0x040071AF RID: 29103
		[SerializeField]
		private ParticleSystem confettiParticle;

		// Token: 0x040071B0 RID: 29104
		[SerializeField]
		private Renderer bannerRenderer;

		// Token: 0x040071B1 RID: 29105
		[SerializeField]
		private TappableBell TappableBell;

		// Token: 0x040071B2 RID: 29106
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040071B3 RID: 29107
		[SerializeField]
		private float cooldownTime = 20f;

		// Token: 0x040071B4 RID: 29108
		public GameObject leftGate;

		// Token: 0x040071B5 RID: 29109
		public GameObject rightGate;

		// Token: 0x040071B6 RID: 29110
		private int numPlayersOnCourse;

		// Token: 0x040071B7 RID: 29111
		private float startTime;

		// Token: 0x02000F7C RID: 3964
		public enum RaceState
		{
			// Token: 0x040071B9 RID: 29113
			Started,
			// Token: 0x040071BA RID: 29114
			Waiting,
			// Token: 0x040071BB RID: 29115
			Finished
		}
	}
}
