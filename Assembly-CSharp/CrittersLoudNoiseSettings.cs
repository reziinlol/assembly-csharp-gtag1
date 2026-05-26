using System;

// Token: 0x02000064 RID: 100
public class CrittersLoudNoiseSettings : CrittersActorSettings
{
	// Token: 0x060001FA RID: 506 RVA: 0x0000B8DC File Offset: 0x00009ADC
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)this.parentActor;
		crittersLoudNoise.soundVolume = this._soundVolume;
		crittersLoudNoise.soundDuration = this._soundDuration;
		crittersLoudNoise.soundEnabled = this._soundEnabled;
		crittersLoudNoise.disableWhenSoundDisabled = this._disableWhenSoundDisabled;
		crittersLoudNoise.volumeFearAttractionMultiplier = this._volumeFearAttractionMultiplier;
	}

	// Token: 0x04000235 RID: 565
	public float _soundVolume;

	// Token: 0x04000236 RID: 566
	public float _soundDuration;

	// Token: 0x04000237 RID: 567
	public bool _soundEnabled;

	// Token: 0x04000238 RID: 568
	public bool _disableWhenSoundDisabled;

	// Token: 0x04000239 RID: 569
	public float _volumeFearAttractionMultiplier = 1f;
}
