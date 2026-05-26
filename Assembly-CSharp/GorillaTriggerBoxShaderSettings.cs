using System;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020005D5 RID: 1493
public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox
{
	// Token: 0x06002544 RID: 9540 RVA: 0x000C5F20 File Offset: 0x000C4120
	private void Awake()
	{
		if (this.sameSceneSettingsRef != null)
		{
			this.settings = this.sameSceneSettingsRef;
			return;
		}
		this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
	}

	// Token: 0x06002545 RID: 9541 RVA: 0x000C5F50 File Offset: 0x000C4150
	public override void OnBoxTriggered()
	{
		if (this.settings == null)
		{
			if (this.sameSceneSettingsRef != null)
			{
				this.settings = this.sameSceneSettingsRef;
			}
			else
			{
				this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
			}
		}
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance(false);
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	// Token: 0x040030C4 RID: 12484
	[SerializeField]
	private XSceneRef settingsRef;

	// Token: 0x040030C5 RID: 12485
	[SerializeField]
	private ZoneShaderSettings sameSceneSettingsRef;

	// Token: 0x040030C6 RID: 12486
	private ZoneShaderSettings settings;
}
