using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class TextureSlideshow : MonoBehaviour
{
	// Token: 0x060000B8 RID: 184 RVA: 0x000054DC File Offset: 0x000036DC
	private void Awake()
	{
		this._renderer = base.GetComponent<Renderer>();
		this._renderer.material.mainTexture = this.textures[0];
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005502 File Offset: 0x00003702
	private void OnEnable()
	{
		base.StartCoroutine(this.runSlideshow());
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00005511 File Offset: 0x00003711
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00005519 File Offset: 0x00003719
	private IEnumerator runSlideshow()
	{
		yield return new WaitForSecondsRealtime(this.prePause);
		int i = 0;
		for (;;)
		{
			yield return new WaitForSecondsRealtime(Random.Range(this.minMaxPause.x, this.minMaxPause.y));
			this._renderer.material.mainTexture = this.textures[i];
			i = (i + 1) % this.textures.Length;
		}
		yield break;
	}

	// Token: 0x040000D1 RID: 209
	private Renderer _renderer;

	// Token: 0x040000D2 RID: 210
	[SerializeField]
	private Texture[] textures;

	// Token: 0x040000D3 RID: 211
	[SerializeField]
	private Vector2 minMaxPause;

	// Token: 0x040000D4 RID: 212
	[SerializeField]
	private float prePause = 1f;
}
