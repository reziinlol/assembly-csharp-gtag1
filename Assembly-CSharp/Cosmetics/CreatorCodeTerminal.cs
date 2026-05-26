using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace Cosmetics
{
	// Token: 0x0200112D RID: 4397
	public class CreatorCodeTerminal : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
	{
		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x06006FA9 RID: 28585 RVA: 0x00247B53 File Offset: 0x00245D53
		public NexusGroupId[] NexusGroups
		{
			get
			{
				return this.nexusGroups;
			}
		}

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x06006FAA RID: 28586 RVA: 0x00247B5B File Offset: 0x00245D5B
		public string TerminalId
		{
			get
			{
				return this.termId;
			}
		}

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x06006FAB RID: 28587 RVA: 0x0000636B File Offset: 0x0000456B
		GameObject ICreatorCodeProvider.GameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06006FAC RID: 28588 RVA: 0x00247B64 File Offset: 0x00245D64
		public void Awake()
		{
			this.termId = string.Empty;
			for (int i = 0; i < this.nexusGroups.Length; i++)
			{
				this.termId += this.nexusGroups[i].Code;
			}
			this.HookupToCreatorCodes();
		}

		// Token: 0x06006FAD RID: 28589 RVA: 0x00247BB3 File Offset: 0x00245DB3
		private void OnDestroy()
		{
			this.UnhookFromCreatorCodes();
		}

		// Token: 0x06006FAE RID: 28590 RVA: 0x00247BBC File Offset: 0x00245DBC
		public void HookupToCreatorCodes()
		{
			CreatorCodes.InitializedEvent += this.OnCreatorCodesInitialized;
			CreatorCodes.OnCreatorCodeChangedEvent += this.OnCreatorCodeChanged;
			CreatorCodes.OnCreatorCodeFailureEvent += this.OnCreatorCodeFailure;
			if (CreatorCodes.Intialized)
			{
				this.OnCreatorCodesInitialized();
			}
			CosmeticsController.PushTerminalMessage = (Action<string, string>)Delegate.Combine(CosmeticsController.PushTerminalMessage, new Action<string, string>(this.OnTerminalMessage));
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x00247C2C File Offset: 0x00245E2C
		private void OnTerminalMessage(string termId, string msg)
		{
			CreatorCodeTerminal.<OnTerminalMessage>d__13 <OnTerminalMessage>d__;
			<OnTerminalMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnTerminalMessage>d__.<>4__this = this;
			<OnTerminalMessage>d__.termId = termId;
			<OnTerminalMessage>d__.msg = msg;
			<OnTerminalMessage>d__.<>1__state = -1;
			<OnTerminalMessage>d__.<>t__builder.Start<CreatorCodeTerminal.<OnTerminalMessage>d__13>(ref <OnTerminalMessage>d__);
		}

		// Token: 0x06006FB0 RID: 28592 RVA: 0x00247C74 File Offset: 0x00245E74
		public void UnhookFromCreatorCodes()
		{
			CreatorCodes.InitializedEvent -= this.OnCreatorCodesInitialized;
			CreatorCodes.OnCreatorCodeChangedEvent -= this.OnCreatorCodeChanged;
			CreatorCodes.OnCreatorCodeFailureEvent -= this.OnCreatorCodeFailure;
			CosmeticsController.PushTerminalMessage = (Action<string, string>)Delegate.Remove(CosmeticsController.PushTerminalMessage, new Action<string, string>(this.OnTerminalMessage));
		}

		// Token: 0x06006FB1 RID: 28593 RVA: 0x00247CD4 File Offset: 0x00245ED4
		private void OnCreatorCodesInitialized()
		{
			this.OnCreatorCodeChanged(this.termId);
		}

		// Token: 0x06006FB2 RID: 28594 RVA: 0x00247CE4 File Offset: 0x00245EE4
		public void OnCreatorCodeChanged(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode(this.termId);
			string text = "CREATOR CODE:";
			CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus(this.termId);
			if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
			{
				if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
				{
					text += " VALID";
				}
			}
			else
			{
				text += " VALIDATING";
			}
			this.creatorCodeTitle.text = text;
		}

		// Token: 0x06006FB3 RID: 28595 RVA: 0x00247D56 File Offset: 0x00245F56
		public void CreatorCodeInput(string character)
		{
			CreatorCodes.AppendKey(this.termId, character);
		}

		// Token: 0x06006FB4 RID: 28596 RVA: 0x00247D64 File Offset: 0x00245F64
		public void CreatorCodeDelete()
		{
			CreatorCodes.DeleteCharacter(this.termId);
		}

		// Token: 0x06006FB5 RID: 28597 RVA: 0x00247D71 File Offset: 0x00245F71
		public void OnCreatorCodeValid(string id, string s, NexusGroupId ngid)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALID";
		}

		// Token: 0x06006FB6 RID: 28598 RVA: 0x00247D92 File Offset: 0x00245F92
		public void OnCreatorCodeValidating(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}

		// Token: 0x06006FB7 RID: 28599 RVA: 0x00247DB3 File Offset: 0x00245FB3
		public void CreatorCodeInvalid(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		// Token: 0x06006FB8 RID: 28600 RVA: 0x00247DB3 File Offset: 0x00245FB3
		public void OnCreatorCodeFailure(string id)
		{
			if (id != this.termId)
			{
				return;
			}
			this.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}

		// Token: 0x06006FB9 RID: 28601 RVA: 0x00247DD4 File Offset: 0x00245FD4
		bool IBuildValidation.BuildValidationCheck()
		{
			if (this.nexusGroups.Length == 0)
			{
				Debug.LogError("You have to set at least one nexus group in " + base.name + " or things will not work!");
				return false;
			}
			return true;
		}

		// Token: 0x06006FBA RID: 28602 RVA: 0x00247DFC File Offset: 0x00245FFC
		public void GetCreatorCode(out string code, out NexusGroupId[] groups)
		{
			code = CreatorCodes.getCurrentCreatorCode(this.termId);
			groups = this.nexusGroups;
		}

		// Token: 0x04007FB2 RID: 32690
		private string termId;

		// Token: 0x04007FB3 RID: 32691
		[SerializeField]
		private TMP_Text creatorCodeField;

		// Token: 0x04007FB4 RID: 32692
		[SerializeField]
		private TMP_Text creatorCodeTitle;

		// Token: 0x04007FB5 RID: 32693
		[SerializeField]
		private NexusGroupId[] nexusGroups;
	}
}
