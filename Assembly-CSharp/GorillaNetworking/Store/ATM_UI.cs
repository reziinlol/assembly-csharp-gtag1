using System;
using System.Runtime.CompilerServices;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaNetworking.Store
{
	// Token: 0x02001092 RID: 4242
	public class ATM_UI : MonoBehaviour
	{
		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06006A5A RID: 27226 RVA: 0x002261B5 File Offset: 0x002243B5
		public string PurchaseLocation
		{
			get
			{
				return this.purchaseLocation;
			}
		}

		// Token: 0x06006A5B RID: 27227 RVA: 0x002261C0 File Offset: 0x002243C0
		private void Start()
		{
			if (ATM_Manager.instance == null || ATM_Manager.instance.atmUIs.Contains(this))
			{
				return;
			}
			if (!this.memberCodeTitleDataKey.IsNullOrEmpty())
			{
				this.loadMemberCodeFromTitleDate(this.memberCodeTitleDataKey);
				return;
			}
			if (!this.memberCode.IsNullOrEmpty() && this.groupId != null)
			{
				ATM_Manager.instance.AddATM(this, new Tuple<string, string>(this.memberCode, this.groupId.Code));
				return;
			}
			ATM_Manager.instance.AddATM(this, null);
		}

		// Token: 0x06006A5C RID: 27228 RVA: 0x00226258 File Offset: 0x00224458
		private void loadMemberCodeFromTitleDate(string memberCodeTitleDataKey)
		{
			ATM_UI.<loadMemberCodeFromTitleDate>d__14 <loadMemberCodeFromTitleDate>d__;
			<loadMemberCodeFromTitleDate>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<loadMemberCodeFromTitleDate>d__.<>4__this = this;
			<loadMemberCodeFromTitleDate>d__.memberCodeTitleDataKey = memberCodeTitleDataKey;
			<loadMemberCodeFromTitleDate>d__.<>1__state = -1;
			<loadMemberCodeFromTitleDate>d__.<>t__builder.Start<ATM_UI.<loadMemberCodeFromTitleDate>d__14>(ref <loadMemberCodeFromTitleDate>d__);
		}

		// Token: 0x06006A5D RID: 27229 RVA: 0x00226298 File Offset: 0x00224498
		private void onTD(string result)
		{
			if (result.Contains("$"))
			{
				string[] array = result.Split('$', StringSplitOptions.None);
				ATM_Manager.instance.AddATM(this, new Tuple<string, string>(array[0], array[1]));
				return;
			}
			if (this.groupId != null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"ATM_UI(",
					AssetUtils.GetGameObjectPath(base.gameObject),
					") :: Title Data missing group code. Using \"",
					result,
					"$",
					this.groupId.Code,
					"\". Expected format: \"<MemberCode>$<GroupCode>\" Got: \"",
					result,
					"\""
				}));
				ATM_Manager.instance.AddATM(this, new Tuple<string, string>(result, this.groupId.Code));
				return;
			}
			Debug.LogError(string.Concat(new string[]
			{
				"ATM_UI(",
				AssetUtils.GetGameObjectPath(base.gameObject),
				") :: Title Data missing group code. No code is set. Expected format: \"<MemberCode>$<GroupCode>\" Got: \"",
				result,
				"\""
			}));
			ATM_Manager.instance.AddATM(this, null);
		}

		// Token: 0x06006A5E RID: 27230 RVA: 0x002263A7 File Offset: 0x002245A7
		private void onTDError(PlayFabError error)
		{
			Debug.LogError(string.Format("ATM_UI({0}) :: PlayFabError :: {1}", AssetUtils.GetGameObjectPath(base.gameObject), error));
			ATM_Manager.instance.AddATM(this, null);
		}

		// Token: 0x06006A5F RID: 27231 RVA: 0x002263D2 File Offset: 0x002245D2
		public void SetCustomMapScene(Scene scene)
		{
			this.customMapScene = scene;
		}

		// Token: 0x06006A60 RID: 27232 RVA: 0x002263DB File Offset: 0x002245DB
		public bool IsFromCustomMapScene(Scene scene)
		{
			return this.customMapScene == scene;
		}

		// Token: 0x06006A61 RID: 27233 RVA: 0x002263E9 File Offset: 0x002245E9
		internal void SetCreatorCodeTitle(string result)
		{
			if (this.creatorCodeTitle != null)
			{
				this.creatorCodeTitle.text = result;
			}
		}

		// Token: 0x06006A62 RID: 27234 RVA: 0x00226405 File Offset: 0x00224605
		internal void SetCreatorCodeField(string v)
		{
			if (this.creatorCodeField != null)
			{
				this.creatorCodeField.text = v;
			}
		}

		// Token: 0x06006A63 RID: 27235 RVA: 0x00226421 File Offset: 0x00224621
		internal void HideCreatorCode()
		{
			if (this.creatorCodeObject != null)
			{
				this.creatorCodeObject.SetActive(false);
			}
		}

		// Token: 0x06006A64 RID: 27236 RVA: 0x0022643D File Offset: 0x0022463D
		internal void ShowCreatorCode()
		{
			if (this.creatorCodeObject != null)
			{
				this.creatorCodeObject.SetActive(true);
			}
		}

		// Token: 0x04007A9B RID: 31387
		public TMP_Text atmText;

		// Token: 0x04007A9C RID: 31388
		public TMP_Text[] ATM_RightColumnButtonText;

		// Token: 0x04007A9D RID: 31389
		public TMP_Text[] ATM_RightColumnArrowText;

		// Token: 0x04007A9E RID: 31390
		[SerializeField]
		private string purchaseLocation;

		// Token: 0x04007A9F RID: 31391
		[SerializeField]
		private GameObject creatorCodeObject;

		// Token: 0x04007AA0 RID: 31392
		[SerializeField]
		private TMP_Text creatorCodeTitle;

		// Token: 0x04007AA1 RID: 31393
		[SerializeField]
		private TMP_Text creatorCodeField;

		// Token: 0x04007AA2 RID: 31394
		[SerializeField]
		private string memberCode;

		// Token: 0x04007AA3 RID: 31395
		[SerializeField]
		private NexusGroupId groupId;

		// Token: 0x04007AA4 RID: 31396
		[SerializeField]
		private string memberCodeTitleDataKey;

		// Token: 0x04007AA5 RID: 31397
		private Scene customMapScene;
	}
}
