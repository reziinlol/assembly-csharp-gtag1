using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000602 RID: 1538
public class MonkeBallTeamSelector : MonoBehaviour
{
	// Token: 0x06002666 RID: 9830 RVA: 0x000CB59A File Offset: 0x000C979A
	public void Awake()
	{
		this._setTeamButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000CB5B8 File Offset: 0x000C97B8
	public void OnDestroy()
	{
		this._setTeamButton.onPressButton.RemoveListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000CB5D6 File Offset: 0x000C97D6
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestSetTeam(this.teamId);
	}

	// Token: 0x040031C5 RID: 12741
	public int teamId;

	// Token: 0x040031C6 RID: 12742
	[SerializeField]
	private GorillaPressableButton _setTeamButton;
}
