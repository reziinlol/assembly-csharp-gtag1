using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005FE RID: 1534
public class MonkeBallResetGame : MonoBehaviourTick
{
	// Token: 0x0600264F RID: 9807 RVA: 0x000CB20C File Offset: 0x000C940C
	private void Awake()
	{
		this._resetButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
		if (this._resetButton == null)
		{
			this._buttonOrigin = this._resetButton.transform.position;
		}
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000CB259 File Offset: 0x000C9459
	public override void Tick()
	{
		if (this._cooldown)
		{
			this._cooldownTimer -= Time.deltaTime;
			if (this._cooldownTimer <= 0f)
			{
				this.ToggleButton(false, -1);
				this._cooldown = false;
			}
		}
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000CB294 File Offset: 0x000C9494
	public void ToggleReset(bool toggle, int teamId, bool force = false)
	{
		if (teamId < -1 || teamId >= this.teamMaterials.Length)
		{
			return;
		}
		if (toggle)
		{
			this.ToggleButton(true, teamId);
			this._cooldown = false;
			return;
		}
		if (force)
		{
			this.ToggleButton(false, -1);
			return;
		}
		this._cooldown = true;
		this._cooldownTimer = 3f;
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000CB2E4 File Offset: 0x000C94E4
	private void ToggleButton(bool toggle, int teamId)
	{
		this._resetButton.enabled = toggle;
		this.allowedTeamId = teamId;
		if (!toggle || teamId == -1)
		{
			this.button.sharedMaterial = this.neutralMaterial;
			return;
		}
		this.button.sharedMaterial = this.teamMaterials[teamId];
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000CB330 File Offset: 0x000C9530
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestResetGame();
	}

	// Token: 0x040031A6 RID: 12710
	[SerializeField]
	private GorillaPressableButton _resetButton;

	// Token: 0x040031A7 RID: 12711
	public Renderer button;

	// Token: 0x040031A8 RID: 12712
	public Vector3 buttonPressOffset;

	// Token: 0x040031A9 RID: 12713
	private Vector3 _buttonOrigin = Vector3.zero;

	// Token: 0x040031AA RID: 12714
	[Space]
	public Material[] teamMaterials;

	// Token: 0x040031AB RID: 12715
	public Material neutralMaterial;

	// Token: 0x040031AC RID: 12716
	public int allowedTeamId = -1;

	// Token: 0x040031AD RID: 12717
	[SerializeField]
	private TextMeshPro _resetLabel;

	// Token: 0x040031AE RID: 12718
	private bool _cooldown;

	// Token: 0x040031AF RID: 12719
	private float _cooldownTimer;
}
