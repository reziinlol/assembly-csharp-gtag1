using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;

// Token: 0x02000DD2 RID: 3538
public class TextureFromURL : MonoBehaviour
{
	// Token: 0x060056A9 RID: 22185 RVA: 0x001C16DA File Offset: 0x001BF8DA
	private void OnEnable()
	{
		if (this.data.Length == 0)
		{
			return;
		}
		if (this.source == TextureFromURL.Source.TitleData)
		{
			this.LoadFromTitleData();
			return;
		}
		this.applyRemoteTexture(this.data);
	}

	// Token: 0x060056AA RID: 22186 RVA: 0x001C1708 File Offset: 0x001BF908
	private void LoadFromTitleData()
	{
		TextureFromURL.<LoadFromTitleData>d__7 <LoadFromTitleData>d__;
		<LoadFromTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadFromTitleData>d__.<>4__this = this;
		<LoadFromTitleData>d__.<>1__state = -1;
		<LoadFromTitleData>d__.<>t__builder.Start<TextureFromURL.<LoadFromTitleData>d__7>(ref <LoadFromTitleData>d__);
	}

	// Token: 0x060056AB RID: 22187 RVA: 0x001C173F File Offset: 0x001BF93F
	private void OnDisable()
	{
		if (this.texture != null)
		{
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x060056AC RID: 22188 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnPlayFabError(PlayFabError error)
	{
	}

	// Token: 0x060056AD RID: 22189 RVA: 0x001C1764 File Offset: 0x001BF964
	private void OnTitleDataRequestComplete(string imageUrl)
	{
		imageUrl = imageUrl.Replace("\\r", "\r").Replace("\\n", "\n");
		if (imageUrl[0] == '"' && imageUrl[imageUrl.Length - 1] == '"')
		{
			imageUrl = imageUrl.Substring(1, imageUrl.Length - 2);
		}
		this.applyRemoteTexture(imageUrl);
	}

	// Token: 0x060056AE RID: 22190 RVA: 0x001C17C8 File Offset: 0x001BF9C8
	private void applyRemoteTexture(string imageUrl)
	{
		TextureFromURL.<applyRemoteTexture>d__11 <applyRemoteTexture>d__;
		<applyRemoteTexture>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<applyRemoteTexture>d__.<>4__this = this;
		<applyRemoteTexture>d__.imageUrl = imageUrl;
		<applyRemoteTexture>d__.<>1__state = -1;
		<applyRemoteTexture>d__.<>t__builder.Start<TextureFromURL.<applyRemoteTexture>d__11>(ref <applyRemoteTexture>d__);
	}

	// Token: 0x060056AF RID: 22191 RVA: 0x001C1808 File Offset: 0x001BFA08
	private Task<Texture2D> GetRemoteTexture(string url)
	{
		TextureFromURL.<GetRemoteTexture>d__12 <GetRemoteTexture>d__;
		<GetRemoteTexture>d__.<>t__builder = AsyncTaskMethodBuilder<Texture2D>.Create();
		<GetRemoteTexture>d__.url = url;
		<GetRemoteTexture>d__.<>1__state = -1;
		<GetRemoteTexture>d__.<>t__builder.Start<TextureFromURL.<GetRemoteTexture>d__12>(ref <GetRemoteTexture>d__);
		return <GetRemoteTexture>d__.<>t__builder.Task;
	}

	// Token: 0x0400668E RID: 26254
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x0400668F RID: 26255
	[SerializeField]
	private TextureFromURL.Source source;

	// Token: 0x04006690 RID: 26256
	[Tooltip("If Source is set to 'TitleData' Data should be the id of the title data entry that defines an image URL. If Source is set to 'URL' Data should be a URL that points to an image.")]
	[SerializeField]
	private string data;

	// Token: 0x04006691 RID: 26257
	private Texture2D texture;

	// Token: 0x04006692 RID: 26258
	private int maxTitleDataAttempts = 10;

	// Token: 0x02000DD3 RID: 3539
	private enum Source
	{
		// Token: 0x04006694 RID: 26260
		TitleData,
		// Token: 0x04006695 RID: 26261
		URL
	}
}
