using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F12 RID: 3858
	public class GorillaPlayerTimerCountDisplay : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006042 RID: 24642 RVA: 0x001F0864 File Offset: 0x001EEA64
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06006043 RID: 24643 RVA: 0x001F0864 File Offset: 0x001EEA64
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x06006044 RID: 24644 RVA: 0x001F086C File Offset: 0x001EEA6C
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			this.displayText.text = "TIME: --.--.-";
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.isInitialized = true;
		}

		// Token: 0x06006045 RID: 24645 RVA: 0x001F08F8 File Offset: 0x001EEAF8
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x001F095D File Offset: 0x001EEB5D
		private void OnLocalTimerStarted()
		{
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x001F0970 File Offset: 0x001EEB70
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				double value = timeDelta / 1000.0;
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds(value).ToString("mm\\:ss\\:f");
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06006048 RID: 24648 RVA: 0x001F09D4 File Offset: 0x001EEBD4
		private void UpdateLatestTime()
		{
			float timeForPlayer = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)timeForPlayer).ToString("mm\\:ss\\:f");
		}

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06006049 RID: 24649 RVA: 0x001F0A24 File Offset: 0x001EEC24
		// (set) Token: 0x0600604A RID: 24650 RVA: 0x001F0A2C File Offset: 0x001EEC2C
		public bool TickRunning { get; set; }

		// Token: 0x0600604B RID: 24651 RVA: 0x001F0A35 File Offset: 0x001EEC35
		public void Tick()
		{
			this.UpdateLatestTime();
		}

		// Token: 0x04006ED7 RID: 28375
		[SerializeField]
		private TMP_Text displayText;

		// Token: 0x04006ED8 RID: 28376
		private bool isInitialized;
	}
}
