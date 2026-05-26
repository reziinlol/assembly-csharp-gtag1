using System;

// Token: 0x0200027B RID: 635
internal class PropHuntPools_Callbacks
{
	// Token: 0x0600112F RID: 4399 RVA: 0x0005C59E File Offset: 0x0005A79E
	internal void ListenForZoneChanged()
	{
		if (PropHuntPools_Callbacks._isListeningForZoneChanged)
		{
			return;
		}
		ZoneManagement.OnZoneChange += this._OnZoneChanged;
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0005C5BC File Offset: 0x0005A7BC
	private void _OnZoneChanged(ZoneData[] zoneDatas)
	{
		if (VRRigCache.Instance == null || VRRigCache.Instance.localRig == null || VRRigCache.Instance.localRig.Rig == null || VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone != GTZone.bayou)
		{
			return;
		}
		PropHuntPools_Callbacks._isListeningForZoneChanged = false;
		ZoneManagement.OnZoneChange -= this._OnZoneChanged;
		PropHuntPools.OnLocalPlayerEnteredBayou();
	}

	// Token: 0x0400147C RID: 5244
	private const string preLog = "PropHuntPools_Callbacks: ";

	// Token: 0x0400147D RID: 5245
	private const string preLogEd = "(editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x0400147E RID: 5246
	private const string preLogBeta = "(beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x0400147F RID: 5247
	private const string preErr = "ERROR!!!  PropHuntPools_Callbacks: ";

	// Token: 0x04001480 RID: 5248
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x04001481 RID: 5249
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x04001482 RID: 5250
	internal static readonly PropHuntPools_Callbacks instance = new PropHuntPools_Callbacks();

	// Token: 0x04001483 RID: 5251
	private static bool _isListeningForZoneChanged;
}
