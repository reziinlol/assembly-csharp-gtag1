using System;
using UnityEngine;

// Token: 0x02000BA5 RID: 2981
public class KIDUI_DebugScreen : MonoBehaviour
{
	// Token: 0x06004ADC RID: 19164 RVA: 0x00190226 File Offset: 0x0018E426
	private void Awake()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06004ADD RID: 19165 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnResetUserAndQuit()
	{
	}

	// Token: 0x06004ADE RID: 19166 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnClose()
	{
	}

	// Token: 0x06004ADF RID: 19167 RVA: 0x00035D0D File Offset: 0x00033F0D
	public static string GetOrCreateUsername()
	{
		return null;
	}

	// Token: 0x06004AE0 RID: 19168 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ResetAll()
	{
	}

	// Token: 0x04005DCA RID: 24010
	public const string KID_ENABLED_KEY = "dbg-kid-enabled";
}
