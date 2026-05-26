using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000A87 RID: 2695
public class CustomMapsAccessScreen : CustomMapsTerminalScreen
{
	// Token: 0x060044A9 RID: 17577 RVA: 0x00171D5C File Offset: 0x0016FF5C
	private void LateUpdate()
	{
		if (CustomMapsTerminal.GetDriverID() == -2)
		{
			return;
		}
		if (CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
		{
			return;
		}
		this.useNametags = GorillaComputer.instance.NametagsEnabled;
		this.SetDriverName();
	}

	// Token: 0x060044AA RID: 17578 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void Initialize()
	{
	}

	// Token: 0x060044AB RID: 17579 RVA: 0x00171DB8 File Offset: 0x0016FFB8
	public override void Show()
	{
		base.Show();
		if (this.displayedText == string.Empty)
		{
			this.displayedText = this.defaultText;
		}
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
		this.terminalControlPromptText.text = this.displayedText;
	}

	// Token: 0x060044AC RID: 17580 RVA: 0x00171E1C File Offset: 0x0017001C
	public override void Hide()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		base.Hide();
	}

	// Token: 0x060044AD RID: 17581 RVA: 0x00171E46 File Offset: 0x00170046
	public void Reset()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
		this.displayedText = this.defaultText;
	}

	// Token: 0x060044AE RID: 17582 RVA: 0x00171E76 File Offset: 0x00170076
	public void SetDetailsScreenForDriver()
	{
		this.displayedText = this.detailsScreenText;
	}

	// Token: 0x060044AF RID: 17583 RVA: 0x00171E84 File Offset: 0x00170084
	public void SetDriverName()
	{
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		string str;
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.GetDriverID());
			str = netPlayerByID.DefaultName;
			if (this.useNametags && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					str = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					str = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			str = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		this.displayedText = "TERMINAL CONTROLLED BY: " + str;
		if (!this.isControlScreen)
		{
			this.displayedText += this.detailsScreenText;
		}
		this.terminalControlPromptText.text = this.displayedText;
	}

	// Token: 0x060044B0 RID: 17584 RVA: 0x00171F5F File Offset: 0x0017015F
	public void DisplayError(string errorMessage)
	{
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x040056D9 RID: 22233
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040056DA RID: 22234
	[SerializeField]
	private TMP_Text terminalControlPromptText;

	// Token: 0x040056DB RID: 22235
	[SerializeField]
	private bool isControlScreen = true;

	// Token: 0x040056DC RID: 22236
	[SerializeField]
	private string defaultText = "PRESS THE 'TERMINAL AVAILABLE' BUTTON TO PROCEED.";

	// Token: 0x040056DD RID: 22237
	private string detailsScreenText = "\nMAP DETAILS WILL APPEAR HERE WHEN A MAP IS SELECTED.";

	// Token: 0x040056DE RID: 22238
	private string displayedText = string.Empty;

	// Token: 0x040056DF RID: 22239
	private bool useNametags;
}
