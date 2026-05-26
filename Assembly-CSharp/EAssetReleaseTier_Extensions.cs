using System;

// Token: 0x020000F5 RID: 245
public static class EAssetReleaseTier_Extensions
{
	// Token: 0x060005E4 RID: 1508 RVA: 0x00022145 File Offset: 0x00020345
	public static bool ShouldIncludeInBuild(this EAssetReleaseTier assetTier, EBuildReleaseTier buildTier)
	{
		return assetTier != EAssetReleaseTier.Disabled && assetTier <= (EAssetReleaseTier)buildTier;
	}
}
