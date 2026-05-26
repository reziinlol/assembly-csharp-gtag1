using System;

// Token: 0x0200087B RID: 2171
public class GorillaModifyDayNightTrigger : GorillaTriggerBox
{
	// Token: 0x06003890 RID: 14480 RVA: 0x00135460 File Offset: 0x00133660
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (this.clearModifiedTime)
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}
		else
		{
			int num = this.timeOfDayIndex % BetterDayNightManager.instance.timeOfDayRange.Length;
			BetterDayNightManager.instance.SetTimeOfDay(this.timeOfDayIndex);
			BetterDayNightManager.instance.SetOverrideIndex(this.timeOfDayIndex);
		}
		if (this.setFixedWeather)
		{
			BetterDayNightManager.instance.SetFixedWeather(this.fixedWeather);
			return;
		}
		BetterDayNightManager.instance.ClearFixedWeather();
	}

	// Token: 0x04004894 RID: 18580
	public bool clearModifiedTime;

	// Token: 0x04004895 RID: 18581
	public int timeOfDayIndex;

	// Token: 0x04004896 RID: 18582
	public bool setFixedWeather;

	// Token: 0x04004897 RID: 18583
	public BetterDayNightManager.WeatherType fixedWeather;
}
