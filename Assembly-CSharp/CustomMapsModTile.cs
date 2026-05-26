using System;
using System.Runtime.CompilerServices;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x02000A9A RID: 2714
public class CustomMapsModTile : CustomMapsScreenTouchPoint
{
	// Token: 0x17000661 RID: 1633
	// (get) Token: 0x0600454C RID: 17740 RVA: 0x00176F0A File Offset: 0x0017510A
	// (set) Token: 0x0600454D RID: 17741 RVA: 0x00176F17 File Offset: 0x00175117
	public string PlayerCountText
	{
		get
		{
			return this._playerCountText.text;
		}
		set
		{
			this._playerCountText.text = value;
		}
	}

	// Token: 0x17000662 RID: 1634
	// (get) Token: 0x0600454E RID: 17742 RVA: 0x00176F25 File Offset: 0x00175125
	public Mod CurrentMod
	{
		get
		{
			return this.currentMod;
		}
	}

	// Token: 0x0600454F RID: 17743 RVA: 0x00176F2D File Offset: 0x0017512D
	protected override void Awake()
	{
		base.Awake();
		this.defaultLogo = this.touchPointRenderer.sprite;
		this.highlight.SetActive(false);
	}

	// Token: 0x06004550 RID: 17744 RVA: 0x00176F54 File Offset: 0x00175154
	public void ShowTileText(bool show, bool useMapName)
	{
		if (!show)
		{
			this.ratingsText.gameObject.SetActive(false);
			this.mapNameText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
			this._playerCountText.gameObject.SetActive(false);
			return;
		}
		if (useMapName)
		{
			this.mapNameText.gameObject.SetActive(true);
			this.ratingsText.gameObject.SetActive(false);
			this.thumsbUp.SetActive(false);
		}
		else
		{
			this.ratingsText.gameObject.SetActive(true);
			this.thumsbUp.SetActive(true);
			this.mapNameText.gameObject.SetActive(false);
		}
		this._playerCountText.gameObject.SetActive(true);
	}

	// Token: 0x06004551 RID: 17745 RVA: 0x00177016 File Offset: 0x00175216
	public void ActivateTile(bool useMapName)
	{
		this.isActive = true;
		base.gameObject.SetActive(true);
		this.ShowTileText(true, useMapName);
		CustomMapsScreenTouchPoint.pressTime = Time.time;
	}

	// Token: 0x06004552 RID: 17746 RVA: 0x0017703D File Offset: 0x0017523D
	public void DeactivateTile()
	{
		this.isActive = false;
		base.gameObject.SetActive(false);
		this.highlight.SetActive(false);
		this.ShowTileText(false, false);
		this.ResetLogo();
	}

	// Token: 0x06004553 RID: 17747 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x06004554 RID: 17748 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnButtonPressedEvent()
	{
	}

	// Token: 0x06004555 RID: 17749 RVA: 0x0017706C File Offset: 0x0017526C
	public void SetMod(Mod mod, bool useMapName)
	{
		CustomMapsModTile.<SetMod>d__23 <SetMod>d__;
		<SetMod>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetMod>d__.<>4__this = this;
		<SetMod>d__.mod = mod;
		<SetMod>d__.useMapName = useMapName;
		<SetMod>d__.<>1__state = -1;
		<SetMod>d__.<>t__builder.Start<CustomMapsModTile.<SetMod>d__23>(ref <SetMod>d__);
	}

	// Token: 0x06004556 RID: 17750 RVA: 0x001770B3 File Offset: 0x001752B3
	public void ResetLogo()
	{
		this.touchPointRenderer.sprite = this.defaultLogo;
	}

	// Token: 0x06004557 RID: 17751 RVA: 0x001770C6 File Offset: 0x001752C6
	public void ShowDetails()
	{
		CustomMapsTerminal.ShowDetailsScreen(this.currentMod);
	}

	// Token: 0x06004558 RID: 17752 RVA: 0x001770D3 File Offset: 0x001752D3
	public void HighlightTile()
	{
		this.highlight.SetActive(true);
	}

	// Token: 0x06004559 RID: 17753 RVA: 0x001770E1 File Offset: 0x001752E1
	public bool IsCurrentModHidden()
	{
		return this.currentMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMod.IsHidden());
	}

	// Token: 0x040057B9 RID: 22457
	[SerializeField]
	private TMP_Text ratingsText;

	// Token: 0x040057BA RID: 22458
	[SerializeField]
	private TMP_Text mapNameText;

	// Token: 0x040057BB RID: 22459
	[SerializeField]
	private GameObject thumsbUp;

	// Token: 0x040057BC RID: 22460
	[SerializeField]
	private GameObject highlight;

	// Token: 0x040057BD RID: 22461
	[SerializeField]
	private TMP_Text _playerCountText;

	// Token: 0x040057BE RID: 22462
	private const float LOGO_WIDTH = 320f;

	// Token: 0x040057BF RID: 22463
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x040057C0 RID: 22464
	private Mod currentMod;

	// Token: 0x040057C1 RID: 22465
	private Sprite defaultLogo;

	// Token: 0x040057C2 RID: 22466
	private bool isDownloadingThumbnail;

	// Token: 0x040057C3 RID: 22467
	private bool newDownloadRequest;

	// Token: 0x040057C4 RID: 22468
	private bool isActive;
}
