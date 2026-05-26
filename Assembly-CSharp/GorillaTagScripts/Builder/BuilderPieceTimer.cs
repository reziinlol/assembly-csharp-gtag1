using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FAD RID: 4013
	public class BuilderPieceTimer : MonoBehaviour, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06006432 RID: 25650 RVA: 0x00204B3D File Offset: 0x00202D3D
		private void Awake()
		{
			this.buttonTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnButtonPressed));
		}

		// Token: 0x06006433 RID: 25651 RVA: 0x00204B5B File Offset: 0x00202D5B
		private void OnDestroy()
		{
			if (this.buttonTrigger != null)
			{
				this.buttonTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06006434 RID: 25652 RVA: 0x00204B88 File Offset: 0x00202D88
		private void OnButtonPressed()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (Time.time > this.lastTriggeredTime + this.debounceTime)
			{
				this.lastTriggeredTime = Time.time;
				if (!this.isStart && this.stopSoundBank != null)
				{
					this.stopSoundBank.Play();
				}
				else if (this.activateSoundBank != null)
				{
					this.activateSoundBank.Play();
				}
				if (this.isBoth && this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: 00:00:0";
				}
				PlayerTimerManager.instance.RequestTimerToggle(this.isStart);
			}
		}

		// Token: 0x06006435 RID: 25653 RVA: 0x00204C40 File Offset: 0x00202E40
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isStart && !this.isBoth)
			{
				return;
			}
			double num = timeDelta;
			this.latestTime = num / 1000.0;
			if (this.latestTime > 3599.989990234375)
			{
				this.latestTime = 3599.989990234375;
			}
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds(this.latestTime).ToString("mm\\:ss\\:ff");
			if (this.isBoth && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStart = true;
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x00204CEF File Offset: 0x00202EEF
		private void OnLocalTimerStarted()
		{
			if (this.isBoth)
			{
				this.isStart = false;
			}
			if (this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x00204D1C File Offset: 0x00202F1C
		private void OnZoneChanged()
		{
			bool active = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.displayText != null)
			{
				this.displayText.gameObject.SetActive(active);
			}
		}

		// Token: 0x06006438 RID: 25656 RVA: 0x00204D64 File Offset: 0x00202F64
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.latestTime = double.MaxValue;
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
				this.OnZoneChanged();
				this.displayText.text = "TIME: __:__:_";
			}
		}

		// Token: 0x06006439 RID: 25657 RVA: 0x00204DCA File Offset: 0x00202FCA
		public void OnPieceDestroy()
		{
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x0600643A RID: 25658 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x0600643B RID: 25659 RVA: 0x00204E00 File Offset: 0x00203000
		public void OnPieceActivate()
		{
			this.lastTriggeredTime = 0f;
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBoth)
			{
				this.isStart = !PlayerTimerManager.instance.IsLocalTimerStarted();
				if (!this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: __:__:_";
				}
			}
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x0600643C RID: 25660 RVA: 0x00204EAC File Offset: 0x002030AC
		public void OnPieceDeactivate()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			if (this.displayText != null)
			{
				this.displayText.text = "TIME: --:--:-";
			}
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x0600643D RID: 25661 RVA: 0x00204F28 File Offset: 0x00203128
		// (set) Token: 0x0600643E RID: 25662 RVA: 0x00204F30 File Offset: 0x00203130
		public bool TickRunning { get; set; }

		// Token: 0x0600643F RID: 25663 RVA: 0x00204F3C File Offset: 0x0020313C
		public void Tick()
		{
			if (this.displayText != null)
			{
				float num = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
				num = Mathf.Clamp(num, 0f, 3599.99f);
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)num).ToString("mm\\:ss\\:f");
			}
		}

		// Token: 0x04007305 RID: 29445
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04007306 RID: 29446
		[SerializeField]
		private bool isStart;

		// Token: 0x04007307 RID: 29447
		[SerializeField]
		private bool isBoth;

		// Token: 0x04007308 RID: 29448
		[SerializeField]
		private BuilderSmallHandTrigger buttonTrigger;

		// Token: 0x04007309 RID: 29449
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x0400730A RID: 29450
		[SerializeField]
		private SoundBankPlayer stopSoundBank;

		// Token: 0x0400730B RID: 29451
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x0400730C RID: 29452
		private float lastTriggeredTime;

		// Token: 0x0400730D RID: 29453
		private double latestTime = 3.4028234663852886E+38;

		// Token: 0x0400730E RID: 29454
		[SerializeField]
		private TMP_Text displayText;
	}
}
