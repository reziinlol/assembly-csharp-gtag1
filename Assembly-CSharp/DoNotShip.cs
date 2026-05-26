using System;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class DoNotShip : MonoBehaviour, IBuildValidation
{
	// Token: 0x060003DC RID: 988 RVA: 0x000172BC File Offset: 0x000154BC
	bool IBuildValidation.BuildValidationCheck()
	{
		Debug.LogError("This build has a an object '" + base.gameObject.name + "' in it that was marked as 'Do Not Ship'");
		return false;
	}
}
