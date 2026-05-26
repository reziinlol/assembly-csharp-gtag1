using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000352 RID: 850
public class JoinTriggerUI : MonoBehaviour
{
	// Token: 0x060014D6 RID: 5334 RVA: 0x0006EEFF File Offset: 0x0006D0FF
	private void Awake()
	{
		this.joinTrigger_isRefResolved = (this.joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.joinTrigger) && this.joinTrigger != null);
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x0006EF29 File Offset: 0x0006D129
	private void Start()
	{
		this.didStart = true;
		this.OnEnable();
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x0006EF38 File Offset: 0x0006D138
	private void OnEnable()
	{
		if (this.didStart && this._IsValid())
		{
			this.joinTrigger.RegisterUI(this);
		}
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x0006EF56 File Offset: 0x0006D156
	private void OnDisable()
	{
		if (this._IsValid())
		{
			this.joinTrigger.UnregisterUI(this);
		}
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x0006EF6C File Offset: 0x0006D16C
	public void SetState(JoinTriggerVisualState state, Func<string> oldZone, Func<string> newZone, Func<string> oldGameMode, Func<string> newGameMode)
	{
		switch (state)
		{
		case JoinTriggerVisualState.ConnectionError:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_Error;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_Error;
			this.screenText.text = (this.template.showFullErrorMessages ? GorillaScoreboardTotalUpdater.instance.offlineTextErrorString : this.template.ScreenText_Error);
			return;
		case JoinTriggerVisualState.AlreadyInRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AlreadyInRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AlreadyInRoom;
			this.screenText.text = this.template.ScreenText_AlreadyInRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.InPrivateRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_InPrivateRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_InPrivateRoom;
			this.screenText.text = this.template.ScreenText_InPrivateRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.NotConnectedSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_NotConnectedSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_NotConnectedSoloJoin;
			this.screenText.text = this.template.ScreenText_NotConnectedSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndSoloJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndPartyJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndGroupJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndGroupJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndGroupJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.AbandonPartyAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AbandonPartyAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AbandonPartyAndSoloJoin;
			this.screenText.text = this.template.ScreenText_AbandonPartyAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.ChangingGameModeSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_ChangingGameModeSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_ChangingGameModeSoloJoin;
			this.screenText.text = this.template.ScreenText_ChangingGameModeSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		default:
			return;
		}
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x0006F220 File Offset: 0x0006D420
	private bool _IsValid()
	{
		if (!this.joinTrigger_isRefResolved)
		{
			if (this.joinTriggerRef.TargetID == 0)
			{
				Debug.LogError("ERROR!!!  JoinTriggerUI: XSceneRef `joinTriggerRef` is not assigned so could not resolve. Path=" + base.transform.GetPathQ(), this);
			}
			else
			{
				Debug.LogError("ERROR!!!  JoinTriggerUI: XSceneRef `joinTriggerRef` could not be resolved. Path=" + base.transform.GetPathQ(), this);
			}
		}
		return this.joinTrigger_isRefResolved;
	}

	// Token: 0x0400198E RID: 6542
	[SerializeField]
	private XSceneRef joinTriggerRef;

	// Token: 0x0400198F RID: 6543
	private GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04001990 RID: 6544
	private bool joinTrigger_isRefResolved;

	// Token: 0x04001991 RID: 6545
	[SerializeField]
	private MeshRenderer milestoneRenderer;

	// Token: 0x04001992 RID: 6546
	[SerializeField]
	private MeshRenderer screenBGRenderer;

	// Token: 0x04001993 RID: 6547
	[SerializeField]
	private TextMeshPro screenText;

	// Token: 0x04001994 RID: 6548
	[SerializeField]
	private JoinTriggerUITemplate template;

	// Token: 0x04001995 RID: 6549
	private new bool didStart;
}
