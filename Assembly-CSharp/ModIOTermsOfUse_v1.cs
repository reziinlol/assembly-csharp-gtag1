using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000A7E RID: 2686
public class ModIOTermsOfUse_v1 : MonoBehaviour
{
	// Token: 0x06004482 RID: 17538 RVA: 0x00170B49 File Offset: 0x0016ED49
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004483 RID: 17539 RVA: 0x00170B6D File Offset: 0x0016ED6D
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004484 RID: 17540 RVA: 0x00170B91 File Offset: 0x0016ED91
	private void PostUpdate()
	{
		if (ControllerBehaviour.Instance.IsLeftStick)
		{
			this.TurnPage(-1);
		}
		if (ControllerBehaviour.Instance.IsRightStick)
		{
			this.TurnPage(1);
		}
		if (this.waitingForAcknowledge)
		{
			this.acceptButtonDown = ControllerBehaviour.Instance.ButtonDown;
		}
	}

	// Token: 0x06004485 RID: 17541 RVA: 0x00170BD4 File Offset: 0x0016EDD4
	private void Start()
	{
		ModIOTermsOfUse_v1.<Start>d__19 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<Start>d__19>(ref <Start>d__);
	}

	// Token: 0x06004486 RID: 17542 RVA: 0x00170C0C File Offset: 0x0016EE0C
	private Task<bool> UpdateTextFromTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__20 <UpdateTextFromTerms>d__;
		<UpdateTextFromTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromTerms>d__.<>4__this = this;
		<UpdateTextFromTerms>d__.<>1__state = -1;
		<UpdateTextFromTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__20>(ref <UpdateTextFromTerms>d__);
		return <UpdateTextFromTerms>d__.<>t__builder.Task;
	}

	// Token: 0x06004487 RID: 17543 RVA: 0x00170C50 File Offset: 0x0016EE50
	public Task<bool> UpdateTextWithFullTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__21 <UpdateTextWithFullTerms>d__;
		<UpdateTextWithFullTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextWithFullTerms>d__.<>1__state = -1;
		<UpdateTextWithFullTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__21>(ref <UpdateTextWithFullTerms>d__);
		return <UpdateTextWithFullTerms>d__.<>t__builder.Task;
	}

	// Token: 0x06004488 RID: 17544 RVA: 0x00170C8C File Offset: 0x0016EE8C
	private string GetStringForListItemIdx_LowerAlpha(int idx)
	{
		switch (idx)
		{
		case 0:
			return "  a. <indent=5%>";
		case 1:
			return "  b. <indent=5%>";
		case 2:
			return "  c. <indent=5%>";
		case 3:
			return "  d. <indent=5%>";
		case 4:
			return "  e. <indent=5%>";
		case 5:
			return "  f. <indent=5%>";
		case 6:
			return "  g. <indent=5%>";
		case 7:
			return "  h. <indent=5%>";
		case 8:
			return "  i. <indent=5%>";
		case 9:
			return "  j. <indent=5%>";
		case 10:
			return "  k. <indent=5%>";
		case 11:
			return "  l. <indent=5%>";
		case 12:
			return "  m. <indent=5%>";
		case 13:
			return "  n. <indent=5%>";
		case 14:
			return "  o. <indent=5%>";
		case 15:
			return "  p. <indent=5%>";
		case 16:
			return "  q. <indent=5%>";
		case 17:
			return "  r. <indent=5%>";
		case 18:
			return "  s. <indent=5%>";
		case 19:
			return "  t. <indent=5%>";
		case 20:
			return "  u. <indent=5%>";
		case 21:
			return "  v. <indent=5%>";
		case 22:
			return "  w. <indent=5%>";
		case 23:
			return "  x. <indent=5%>";
		case 24:
			return "  y. <indent=5%>";
		case 25:
			return "  z. <indent=5%>";
		default:
			return "";
		}
	}

	// Token: 0x06004489 RID: 17545 RVA: 0x00170DB0 File Offset: 0x0016EFB0
	private Task WaitForAcknowledgement()
	{
		ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__23 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__23>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x0600448A RID: 17546 RVA: 0x00170DF4 File Offset: 0x0016EFF4
	public void TurnPage(int i)
	{
		this.tmpBody.pageToDisplay = Mathf.Clamp(this.tmpBody.pageToDisplay + i, 1, this.tmpBody.textInfo.pageCount);
		this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
		this.nextButton.SetActive(this.tmpBody.pageToDisplay < this.tmpBody.textInfo.pageCount);
		this.prevButton.SetActive(this.tmpBody.pageToDisplay > 1);
		this.ActivateAcceptButtonGroup();
	}

	// Token: 0x0600448B RID: 17547 RVA: 0x00170EB0 File Offset: 0x0016F0B0
	private void ActivateAcceptButtonGroup()
	{
		bool active = this.tmpBody.pageToDisplay == this.tmpBody.textInfo.pageCount;
		this.yesNoButtons.SetActive(active);
		this.waitingForAcknowledge = active;
	}

	// Token: 0x0600448C RID: 17548 RVA: 0x00170EEE File Offset: 0x0016F0EE
	public void Acknowledge(bool didAccept)
	{
		this.accepted = didAccept;
	}

	// Token: 0x04005695 RID: 22165
	[SerializeField]
	private Transform uiParent;

	// Token: 0x04005696 RID: 22166
	[SerializeField]
	private string title;

	// Token: 0x04005697 RID: 22167
	[SerializeField]
	private TMP_Text tmpBody;

	// Token: 0x04005698 RID: 22168
	[SerializeField]
	private TMP_Text tmpTitle;

	// Token: 0x04005699 RID: 22169
	[SerializeField]
	private TMP_Text tmpPage;

	// Token: 0x0400569A RID: 22170
	[SerializeField]
	public GameObject yesNoButtons;

	// Token: 0x0400569B RID: 22171
	[SerializeField]
	public GameObject nextButton;

	// Token: 0x0400569C RID: 22172
	[SerializeField]
	public GameObject prevButton;

	// Token: 0x0400569D RID: 22173
	private bool hasTermsOfUse;

	// Token: 0x0400569E RID: 22174
	private Action<bool> termsAcknowledgedCallback;

	// Token: 0x0400569F RID: 22175
	private string cachedTermsText;

	// Token: 0x040056A0 RID: 22176
	private bool waitingForAcknowledge;

	// Token: 0x040056A1 RID: 22177
	private bool accepted;

	// Token: 0x040056A2 RID: 22178
	private bool acceptButtonDown;

	// Token: 0x040056A3 RID: 22179
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x040056A4 RID: 22180
	[SerializeField]
	private LineRenderer progressBar;
}
