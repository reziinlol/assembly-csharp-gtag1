using System;
using UnityEngine;

// Token: 0x02000421 RID: 1057
public class TestScreen : ArcadeGame
{
	// Token: 0x0600192A RID: 6442 RVA: 0x00035D0D File Offset: 0x00033F0D
	public override byte[] GetNetworkState()
	{
		return null;
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void SetNetworkState(byte[] b)
	{
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x0008DB48 File Offset: 0x0008BD48
	private int buttonToLightIndex(int player, ArcadeButtons button)
	{
		int num = 0;
		if (button <= ArcadeButtons.RIGHT)
		{
			switch (button)
			{
			case ArcadeButtons.GRAB:
				num = 0;
				break;
			case ArcadeButtons.UP:
				num = 1;
				break;
			case ArcadeButtons.GRAB | ArcadeButtons.UP:
				break;
			case ArcadeButtons.DOWN:
				num = 2;
				break;
			default:
				if (button != ArcadeButtons.LEFT)
				{
					if (button == ArcadeButtons.RIGHT)
					{
						num = 4;
					}
				}
				else
				{
					num = 3;
				}
				break;
			}
		}
		else if (button != ArcadeButtons.B0)
		{
			if (button != ArcadeButtons.B1)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					num = 7;
				}
			}
			else
			{
				num = 6;
			}
		}
		else
		{
			num = 5;
		}
		return (player * 8 + num) % this.lights.Length;
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x0008DBBF File Offset: 0x0008BDBF
	protected override void ButtonUp(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.red;
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x0008DBDA File Offset: 0x0008BDDA
	protected override void ButtonDown(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.green;
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnTimeout()
	{
	}

	// Token: 0x04002432 RID: 9266
	[SerializeField]
	private SpriteRenderer[] lights;

	// Token: 0x04002433 RID: 9267
	[SerializeField]
	private Transform dot;
}
