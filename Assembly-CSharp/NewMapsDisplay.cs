using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio;
using Modio.Images;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x02000A83 RID: 2691
public class NewMapsDisplay : MonoBehaviour
{
	// Token: 0x06004496 RID: 17558 RVA: 0x0017147C File Offset: 0x0016F67C
	public void OnEnable()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
		if (!UGCPermissionManager.IsUGCDisabled)
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
			}
			else
			{
				if (this.newMapsModId == ModId.Null)
				{
					return;
				}
				this.Initialize();
			}
		}
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x06004497 RID: 17559 RVA: 0x00171538 File Offset: 0x0016F738
	public void OnDisable()
	{
		if (this.initCoroutine != null)
		{
			base.StopCoroutine(this.initCoroutine);
			this.initCoroutine = null;
		}
		this.newMapsModProfile = null;
		this.newMapDatas.Clear();
		this.slideshowActive = false;
		this.slideshowIndex = 0;
		this.lastSlideshowUpdate = 0f;
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.loadingString;
		this.loadingText.gameObject.SetActive(false);
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
	}

	// Token: 0x06004498 RID: 17560 RVA: 0x00171604 File Offset: 0x0016F804
	private void OnUGCEnabled()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
				return;
			}
			if (this.newMapsModId == ModId.Null)
			{
				return;
			}
			this.Initialize();
		}
	}

	// Token: 0x06004499 RID: 17561 RVA: 0x00171660 File Offset: 0x0016F860
	private void OnUGCDisabled()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.ugcDisabledString;
		this.loadingText.gameObject.SetActive(true);
	}

	// Token: 0x0600449A RID: 17562 RVA: 0x001716C1 File Offset: 0x0016F8C1
	private IEnumerator DelayedInitialize()
	{
		while (!ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		this.initCoroutine = null;
		if (this.newMapsModId == ModId.Null)
		{
			yield break;
		}
		this.Initialize();
		yield break;
	}

	// Token: 0x0600449B RID: 17563 RVA: 0x001716D0 File Offset: 0x0016F8D0
	private Task<Error> Initialize()
	{
		NewMapsDisplay.<Initialize>d__28 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<NewMapsDisplay.<Initialize>d__28>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	// Token: 0x0600449C RID: 17564 RVA: 0x00171713 File Offset: 0x0016F913
	private void StartSlideshow()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			return;
		}
		this.slideshowIndex = 0;
		this.slideshowActive = true;
		this.UpdateSlideshow();
	}

	// Token: 0x0600449D RID: 17565 RVA: 0x00171737 File Offset: 0x0016F937
	public void Update()
	{
		if (!this.slideshowActive || Time.time - this.lastSlideshowUpdate < this.slideshowUpdateInterval)
		{
			return;
		}
		this.UpdateSlideshow();
	}

	// Token: 0x0600449E RID: 17566 RVA: 0x0017175C File Offset: 0x0016F95C
	private void UpdateSlideshow()
	{
		this.loadingText.gameObject.SetActive(false);
		this.lastSlideshowUpdate = Time.time;
		Texture2D image = this.newMapDatas[this.slideshowIndex].image;
		if (image != null)
		{
			Sprite sprite;
			if (!this.cachedTextures.TryGetValue(image, out sprite))
			{
				sprite = Sprite.Create(image, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
				this.cachedTextures.Add(image, sprite);
			}
			this.mapImage.sprite = sprite;
			this.mapImage.gameObject.SetActive(true);
		}
		else
		{
			this.mapImage.gameObject.SetActive(false);
		}
		this.mapInfoTMP.text = this.newMapDatas[this.slideshowIndex].info;
		this.mapInfoTMP.gameObject.SetActive(true);
		this.slideshowIndex++;
		if (this.slideshowIndex >= this.newMapDatas.Count)
		{
			this.slideshowIndex = 0;
		}
	}

	// Token: 0x040056B6 RID: 22198
	[SerializeField]
	private SpriteRenderer mapImage;

	// Token: 0x040056B7 RID: 22199
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040056B8 RID: 22200
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x040056B9 RID: 22201
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x040056BA RID: 22202
	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x040056BB RID: 22203
	[SerializeField]
	private TMP_Text mapInfoTMP;

	// Token: 0x040056BC RID: 22204
	[SerializeField]
	private float slideshowUpdateInterval = 1f;

	// Token: 0x040056BD RID: 22205
	[SerializeField]
	private string loadingString = "LOADING...";

	// Token: 0x040056BE RID: 22206
	[SerializeField]
	private string ugcDisabledString = "UGC DISABLED BY K-ID SETTINGS";

	// Token: 0x040056BF RID: 22207
	private ModId newMapsModId = ModId.Null;

	// Token: 0x040056C0 RID: 22208
	private Mod newMapsModProfile;

	// Token: 0x040056C1 RID: 22209
	private List<NewMapsDisplay.NewMapData> newMapDatas = new List<NewMapsDisplay.NewMapData>();

	// Token: 0x040056C2 RID: 22210
	private bool slideshowActive;

	// Token: 0x040056C3 RID: 22211
	private int slideshowIndex;

	// Token: 0x040056C4 RID: 22212
	private float lastSlideshowUpdate;

	// Token: 0x040056C5 RID: 22213
	private bool requestingNewMapsModProfile;

	// Token: 0x040056C6 RID: 22214
	private LazyImage<Texture2D> lazyImage;

	// Token: 0x040056C7 RID: 22215
	private bool downloadingImages;

	// Token: 0x040056C8 RID: 22216
	private bool downloadingImage;

	// Token: 0x040056C9 RID: 22217
	private Texture2D lastDownloadedImage;

	// Token: 0x040056CA RID: 22218
	private Coroutine initCoroutine;

	// Token: 0x040056CB RID: 22219
	private Dictionary<Texture2D, Sprite> cachedTextures = new Dictionary<Texture2D, Sprite>();

	// Token: 0x02000A84 RID: 2692
	private struct NewMapData
	{
		// Token: 0x040056CC RID: 22220
		public Texture2D image;

		// Token: 0x040056CD RID: 22221
		public string info;
	}
}
