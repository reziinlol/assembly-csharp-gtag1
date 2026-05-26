using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x020010CC RID: 4300
	public class JoustGame : ArcadeGame
	{
		// Token: 0x06006BAB RID: 27563 RVA: 0x0022E1B6 File Offset: 0x0022C3B6
		public override byte[] GetNetworkState()
		{
			return new byte[0];
		}

		// Token: 0x06006BAC RID: 27564 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void SetNetworkState(byte[] obj)
		{
		}

		// Token: 0x06006BAD RID: 27565 RVA: 0x0022E1BE File Offset: 0x0022C3BE
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button != ArcadeButtons.GRAB)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					this.joustPlayers[player].Flap();
					return;
				}
			}
			else
			{
				this.joustPlayers[player].gameObject.SetActive(true);
			}
		}

		// Token: 0x06006BAE RID: 27566 RVA: 0x0022E1ED File Offset: 0x0022C3ED
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.GRAB)
			{
				this.joustPlayers[player].gameObject.SetActive(false);
			}
		}

		// Token: 0x06006BAF RID: 27567 RVA: 0x0022E208 File Offset: 0x0022C408
		private void Start()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				this.joustPlayers[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x06006BB0 RID: 27568 RVA: 0x0022E23C File Offset: 0x0022C43C
		private void Update()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				if (this.joustPlayers[i].gameObject.activeInHierarchy)
				{
					int num = (base.getButtonState(i, ArcadeButtons.LEFT) ? -1 : 0) + (base.getButtonState(i, ArcadeButtons.RIGHT) ? 1 : 0);
					this.joustPlayers[i].HorizontalSpeed = Mathf.Clamp(this.joustPlayers[i].HorizontalSpeed + (float)num * Time.deltaTime, -1f, 1f);
				}
			}
		}

		// Token: 0x06006BB1 RID: 27569 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void OnTimeout()
		{
		}

		// Token: 0x04007BED RID: 31725
		[SerializeField]
		private JoustPlayer[] joustPlayers;
	}
}
