using System;
using TMPro;
using UnityEngine;

// Token: 0x02000601 RID: 1537
public class MonkeBallShotclock : MonoBehaviourTick
{
	// Token: 0x06002661 RID: 9825 RVA: 0x000CB484 File Offset: 0x000C9684
	public override void Tick()
	{
		if (this._time >= 0f)
		{
			this._time -= Time.deltaTime;
			this.UpdateTimeText(this._time);
			if (this._time < 0f)
			{
				this.SetBackboard(this.neutralMaterial);
			}
		}
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000CB4D8 File Offset: 0x000C96D8
	public void SetTime(int teamId, float time)
	{
		this._time = time;
		if (teamId == -1)
		{
			this._time = 0f;
			this.SetBackboard(this.neutralMaterial);
		}
		else if (teamId >= 0 && teamId < this.teamMaterials.Length)
		{
			this.SetBackboard(this.teamMaterials[teamId]);
		}
		this.UpdateTimeText(time);
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000CB52D File Offset: 0x000C972D
	private void SetBackboard(Material teamMaterial)
	{
		if (this.backboard != null)
		{
			this.backboard.material = teamMaterial;
		}
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000CB54C File Offset: 0x000C974C
	private void UpdateTimeText(float time)
	{
		int num = Mathf.CeilToInt(time);
		if (this._timeInt != num)
		{
			this._timeInt = num;
			this.timeRemainingLabel.text = this._timeInt.ToString("#00");
		}
	}

	// Token: 0x040031BF RID: 12735
	public Renderer backboard;

	// Token: 0x040031C0 RID: 12736
	public Material[] teamMaterials;

	// Token: 0x040031C1 RID: 12737
	public Material neutralMaterial;

	// Token: 0x040031C2 RID: 12738
	public TextMeshPro timeRemainingLabel;

	// Token: 0x040031C3 RID: 12739
	private float _time;

	// Token: 0x040031C4 RID: 12740
	private int _timeInt = -1;
}
