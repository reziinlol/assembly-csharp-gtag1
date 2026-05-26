using System;
using UnityEngine;

// Token: 0x02000353 RID: 851
[CreateAssetMenu(fileName = "JoinTriggerUITemplate", menuName = "ScriptableObjects/JoinTriggerUITemplate")]
public class JoinTriggerUITemplate : ScriptableObject
{
	// Token: 0x04001996 RID: 6550
	public Material Milestone_Error;

	// Token: 0x04001997 RID: 6551
	public Material Milestone_AlreadyInRoom;

	// Token: 0x04001998 RID: 6552
	public Material Milestone_InPrivateRoom;

	// Token: 0x04001999 RID: 6553
	public Material Milestone_NotConnectedSoloJoin;

	// Token: 0x0400199A RID: 6554
	public Material Milestone_LeaveRoomAndSoloJoin;

	// Token: 0x0400199B RID: 6555
	public Material Milestone_LeaveRoomAndGroupJoin;

	// Token: 0x0400199C RID: 6556
	public Material Milestone_AbandonPartyAndSoloJoin;

	// Token: 0x0400199D RID: 6557
	public Material Milestone_ChangingGameModeSoloJoin;

	// Token: 0x0400199E RID: 6558
	public Material ScreenBG_Error;

	// Token: 0x0400199F RID: 6559
	public Material ScreenBG_AlreadyInRoom;

	// Token: 0x040019A0 RID: 6560
	public Material ScreenBG_InPrivateRoom;

	// Token: 0x040019A1 RID: 6561
	public Material ScreenBG_NotConnectedSoloJoin;

	// Token: 0x040019A2 RID: 6562
	public Material ScreenBG_LeaveRoomAndSoloJoin;

	// Token: 0x040019A3 RID: 6563
	public Material ScreenBG_LeaveRoomAndGroupJoin;

	// Token: 0x040019A4 RID: 6564
	public Material ScreenBG_AbandonPartyAndSoloJoin;

	// Token: 0x040019A5 RID: 6565
	public Material ScreenBG_ChangingGameModeSoloJoin;

	// Token: 0x040019A6 RID: 6566
	public string ScreenText_Error;

	// Token: 0x040019A7 RID: 6567
	public bool showFullErrorMessages;

	// Token: 0x040019A8 RID: 6568
	public JoinTriggerUITemplate.FormattedString ScreenText_AlreadyInRoom;

	// Token: 0x040019A9 RID: 6569
	public JoinTriggerUITemplate.FormattedString ScreenText_InPrivateRoom;

	// Token: 0x040019AA RID: 6570
	public JoinTriggerUITemplate.FormattedString ScreenText_NotConnectedSoloJoin;

	// Token: 0x040019AB RID: 6571
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndSoloJoin;

	// Token: 0x040019AC RID: 6572
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndGroupJoin;

	// Token: 0x040019AD RID: 6573
	public JoinTriggerUITemplate.FormattedString ScreenText_AbandonPartyAndSoloJoin;

	// Token: 0x040019AE RID: 6574
	public JoinTriggerUITemplate.FormattedString ScreenText_ChangingGameModeSoloJoin;

	// Token: 0x02000354 RID: 852
	[Serializable]
	public struct FormattedString
	{
		// Token: 0x060014DE RID: 5342 RVA: 0x0006F280 File Offset: 0x0006D480
		public string GetText(string oldZone, string newZone, string oldGameType, string newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(new string[]
			{
				oldZone,
				newZone,
				oldGameType,
				newGameType
			});
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x0006F2BD File Offset: 0x0006D4BD
		public string GetText(Func<string> oldZone, Func<string> newZone, Func<string> oldGameType, Func<string> newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldZone, newZone, oldGameType, newGameType);
		}

		// Token: 0x040019AF RID: 6575
		[TextArea]
		[SerializeField]
		private string formatText;

		// Token: 0x040019B0 RID: 6576
		[NonSerialized]
		private StringFormatter formatter;
	}
}
