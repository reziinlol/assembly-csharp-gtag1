using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NexusSDK;
using UnityEngine;

// Token: 0x02000554 RID: 1364
public class NexusManager : MonoBehaviour
{
	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x060022C3 RID: 8899 RVA: 0x000BAFB3 File Offset: 0x000B91B3
	public NexusManager.Environment CurrentEnvironment
	{
		get
		{
			return this.environment;
		}
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000BAFBB File Offset: 0x000B91BB
	private void Awake()
	{
		if (NexusManager.instance == null)
		{
			this.environment = NexusManager.Environment.PRODUCTION;
			NexusManager.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x000BAFDE File Offset: 0x000B91DE
	private void Start()
	{
		SDKInitializer.Init((this.environment == NexusManager.Environment.SANDBOX) ? "nexus_pk_ba155a8c229740489d214f024e25f25c" : "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb", (this.environment == NexusManager.Environment.SANDBOX) ? "sandbox" : "production");
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000BB010 File Offset: 0x000B9210
	public Task<Member> VerifyCreatorCode(string terminalId, string code, NexusGroupId id)
	{
		NexusManager.<VerifyCreatorCode>d__14 <VerifyCreatorCode>d__;
		<VerifyCreatorCode>d__.<>t__builder = AsyncTaskMethodBuilder<Member>.Create();
		<VerifyCreatorCode>d__.terminalId = terminalId;
		<VerifyCreatorCode>d__.code = code;
		<VerifyCreatorCode>d__.id = id;
		<VerifyCreatorCode>d__.<>1__state = -1;
		<VerifyCreatorCode>d__.<>t__builder.Start<NexusManager.<VerifyCreatorCode>d__14>(ref <VerifyCreatorCode>d__);
		return <VerifyCreatorCode>d__.<>t__builder.Task;
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x000BB064 File Offset: 0x000B9264
	public Task<bool> VerifyCreatorCodeJIT(string memberCode, string groupCode)
	{
		NexusManager.<VerifyCreatorCodeJIT>d__15 <VerifyCreatorCodeJIT>d__;
		<VerifyCreatorCodeJIT>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<VerifyCreatorCodeJIT>d__.memberCode = memberCode;
		<VerifyCreatorCodeJIT>d__.groupCode = groupCode;
		<VerifyCreatorCodeJIT>d__.<>1__state = -1;
		<VerifyCreatorCodeJIT>d__.<>t__builder.Start<NexusManager.<VerifyCreatorCodeJIT>d__15>(ref <VerifyCreatorCodeJIT>d__);
		return <VerifyCreatorCodeJIT>d__.<>t__builder.Task;
	}

	// Token: 0x04002DD3 RID: 11731
	private const string ENV_PRODUCTION = "production";

	// Token: 0x04002DD4 RID: 11732
	private const string ENV_SANDBOX = "sandbox";

	// Token: 0x04002DD5 RID: 11733
	private const string ENV_PRODUCTION_API_KEY = "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb";

	// Token: 0x04002DD6 RID: 11734
	private const string ENV_SANDBOX_API_KEY = "nexus_pk_ba155a8c229740489d214f024e25f25c";

	// Token: 0x04002DD7 RID: 11735
	private NexusManager.Environment environment = NexusManager.Environment.SANDBOX;

	// Token: 0x04002DD8 RID: 11736
	public static NexusManager instance;

	// Token: 0x04002DD9 RID: 11737
	private Member[] validatedMembers;

	// Token: 0x02000555 RID: 1365
	public enum Environment
	{
		// Token: 0x04002DDB RID: 11739
		PRODUCTION,
		// Token: 0x04002DDC RID: 11740
		SANDBOX
	}

	// Token: 0x02000556 RID: 1366
	[Serializable]
	public class MemberCode
	{
		// Token: 0x170003AA RID: 938
		// (get) Token: 0x060022C9 RID: 8905 RVA: 0x000BB0BE File Offset: 0x000B92BE
		// (set) Token: 0x060022CA RID: 8906 RVA: 0x000BB0C6 File Offset: 0x000B92C6
		public string memberCode { get; set; }

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x060022CB RID: 8907 RVA: 0x000BB0CF File Offset: 0x000B92CF
		// (set) Token: 0x060022CC RID: 8908 RVA: 0x000BB0D7 File Offset: 0x000B92D7
		public NexusGroupId groupId { get; set; }
	}

	// Token: 0x02000557 RID: 1367
	[Serializable]
	public struct GetMembersRequest
	{
		// Token: 0x170003AC RID: 940
		// (get) Token: 0x060022CE RID: 8910 RVA: 0x000BB0E0 File Offset: 0x000B92E0
		// (set) Token: 0x060022CF RID: 8911 RVA: 0x000BB0E8 File Offset: 0x000B92E8
		public int page { readonly get; set; }

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x060022D0 RID: 8912 RVA: 0x000BB0F1 File Offset: 0x000B92F1
		// (set) Token: 0x060022D1 RID: 8913 RVA: 0x000BB0F9 File Offset: 0x000B92F9
		public int pageSize { readonly get; set; }
	}
}
