using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F1D RID: 3869
	public class MenorahCandle : MonoBehaviourPun
	{
		// Token: 0x06006096 RID: 24726 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void Awake()
		{
		}

		// Token: 0x06006097 RID: 24727 RVA: 0x001F1E70 File Offset: 0x001F0070
		private void Start()
		{
			this.EnableCandle(false);
			this.EnableFlame(false);
			this.litDate = new DateTime(this.year, this.month, this.day);
			this.currentDate = DateTime.Now;
			this.EnableCandle(this.CandleShouldBeVisible());
			this.EnableFlame(false);
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x001F1EEE File Offset: 0x001F00EE
		private void UpdateMenorah()
		{
			this.EnableCandle(this.CandleShouldBeVisible());
			if (this.ShouldLightCandle())
			{
				this.EnableFlame(true);
				return;
			}
			if (this.ShouldSnuffCandle())
			{
				this.EnableFlame(false);
			}
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x001F1F1B File Offset: 0x001F011B
		private void OnTimeChanged()
		{
			this.currentDate = GorillaComputer.instance.GetServerTime();
			this.UpdateMenorah();
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x001F1F35 File Offset: 0x001F0135
		public void OnTimeEventStart()
		{
			this.activeTimeEventDay = true;
			this.UpdateMenorah();
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x001F1F44 File Offset: 0x001F0144
		public void OnTimeEventEnd()
		{
			this.activeTimeEventDay = false;
			this.UpdateMenorah();
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x001F1F53 File Offset: 0x001F0153
		private void EnableCandle(bool enable)
		{
			if (this.candle)
			{
				this.candle.SetActive(enable);
			}
		}

		// Token: 0x0600609D RID: 24733 RVA: 0x001F1F6E File Offset: 0x001F016E
		private bool CandleShouldBeVisible()
		{
			return this.currentDate >= this.litDate;
		}

		// Token: 0x0600609E RID: 24734 RVA: 0x001F1F81 File Offset: 0x001F0181
		private void EnableFlame(bool enable)
		{
			if (this.flame)
			{
				this.flame.SetActive(enable);
			}
		}

		// Token: 0x0600609F RID: 24735 RVA: 0x001F1F9C File Offset: 0x001F019C
		private bool ShouldLightCandle()
		{
			return !this.activeTimeEventDay && this.CandleShouldBeVisible() && !this.flame.activeSelf;
		}

		// Token: 0x060060A0 RID: 24736 RVA: 0x001F1FBE File Offset: 0x001F01BE
		private bool ShouldSnuffCandle()
		{
			return this.activeTimeEventDay && this.flame.activeSelf;
		}

		// Token: 0x060060A1 RID: 24737 RVA: 0x001F1FD5 File Offset: 0x001F01D5
		private void OnDestroy()
		{
			if (GorillaComputer.instance)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x04006F32 RID: 28466
		public int day;

		// Token: 0x04006F33 RID: 28467
		public int month;

		// Token: 0x04006F34 RID: 28468
		public int year;

		// Token: 0x04006F35 RID: 28469
		public GameObject flame;

		// Token: 0x04006F36 RID: 28470
		public GameObject candle;

		// Token: 0x04006F37 RID: 28471
		private DateTime litDate;

		// Token: 0x04006F38 RID: 28472
		private bool activeTimeEventDay;

		// Token: 0x04006F39 RID: 28473
		private DateTime currentDate;
	}
}
