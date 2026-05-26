using System;
using GorillaNetworking.Store;

// Token: 0x02000567 RID: 1383
public class TryOnBundleButton : GorillaPressableButton
{
	// Token: 0x06002322 RID: 8994 RVA: 0x000BD1F9 File Offset: 0x000BB3F9
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		BundleManager.instance.PressTryOnBundleButton(this, isLeftHand);
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000BD210 File Offset: 0x000BB410
	public override void UpdateColor()
	{
		if (this.playfabBundleID == "NULL")
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = "";
			}
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x04002E43 RID: 11843
	public int buttonIndex;

	// Token: 0x04002E44 RID: 11844
	public string playfabBundleID = "NULL";
}
