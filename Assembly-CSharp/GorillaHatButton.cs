using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000866 RID: 2150
[Obsolete("This class is obsolete and will be removed in a future version. (MattO 2024-02-26) It doesn't appear to be used anywhere.")]
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x060037FE RID: 14334 RVA: 0x00131930 File Offset: 0x0012FB30
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			if (this.touchTime + this.debounceTime < Time.time)
			{
				this.touchTime = Time.time;
				this.isOn = !this.isOn;
				this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			}
		}
	}

	// Token: 0x060037FF RID: 14335 RVA: 0x00131998 File Offset: 0x0012FB98
	private void OnTriggerEnter(Collider collider)
	{
		if (this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.isOn = !this.isOn;
			this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06003800 RID: 14336 RVA: 0x00131A38 File Offset: 0x0012FC38
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x040047E0 RID: 18400
	public GorillaHatButtonParent buttonParent;

	// Token: 0x040047E1 RID: 18401
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x040047E2 RID: 18402
	public bool isOn;

	// Token: 0x040047E3 RID: 18403
	public Material offMaterial;

	// Token: 0x040047E4 RID: 18404
	public Material onMaterial;

	// Token: 0x040047E5 RID: 18405
	public string offText;

	// Token: 0x040047E6 RID: 18406
	public string onText;

	// Token: 0x040047E7 RID: 18407
	public Text myText;

	// Token: 0x040047E8 RID: 18408
	public float debounceTime = 0.25f;

	// Token: 0x040047E9 RID: 18409
	public float touchTime;

	// Token: 0x040047EA RID: 18410
	public string cosmeticName;

	// Token: 0x040047EB RID: 18411
	public bool testPress;

	// Token: 0x02000867 RID: 2151
	public enum HatButtonType
	{
		// Token: 0x040047ED RID: 18413
		Hat,
		// Token: 0x040047EE RID: 18414
		Face,
		// Token: 0x040047EF RID: 18415
		Badge
	}
}
