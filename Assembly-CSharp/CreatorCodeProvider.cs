using System;
using Cosmetics;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class CreatorCodeProvider : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
{
	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000109 RID: 265 RVA: 0x000062F7 File Offset: 0x000044F7
	string ICreatorCodeProvider.TerminalId
	{
		get
		{
			return this.nexusCreatorCode.GroupId.Code + this.nexusCreatorCode.Code;
		}
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00006319 File Offset: 0x00004519
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusCreatorCode == null)
		{
			Debug.LogError("The CreatorCodeProvider component on " + base.name + " must be assigned a nexusCreatorCode.");
			return false;
		}
		return true;
	}

	// Token: 0x0600010B RID: 267 RVA: 0x00006346 File Offset: 0x00004546
	void ICreatorCodeProvider.GetCreatorCode(out string code, out NexusGroupId[] groups)
	{
		code = this.nexusCreatorCode.Code;
		groups = new NexusGroupId[]
		{
			this.nexusCreatorCode.GroupId
		};
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600010C RID: 268 RVA: 0x0000636B File Offset: 0x0000456B
	GameObject ICreatorCodeProvider.GameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	// Token: 0x04000115 RID: 277
	[SerializeField]
	private NexusCreatorCode nexusCreatorCode;
}
