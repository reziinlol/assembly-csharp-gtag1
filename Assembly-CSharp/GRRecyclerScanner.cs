using System;
using GorillaTagScripts.GhostReactor;
using TMPro;
using UnityEngine;

// Token: 0x020007C9 RID: 1993
public class GRRecyclerScanner : MonoBehaviour
{
	// Token: 0x060032CC RID: 13004 RVA: 0x001167C8 File Offset: 0x001149C8
	private void Awake()
	{
		this.titleText.text = "";
		this.descriptionText.text = "";
		this.annotationText.text = "";
		this.recycleValueText.text = "";
	}

	// Token: 0x060032CD RID: 13005 RVA: 0x00116818 File Offset: 0x00114A18
	public void ScanItem(GameEntityId id)
	{
		if (this.recycler != null && this.recycler.reactor != null && this.recycler.reactor.grManager != null && this.recycler.reactor.grManager.gameEntityManager != null)
		{
			GameEntity gameEntity = this.recycler.reactor.grManager.gameEntityManager.GetGameEntity(id);
			if (gameEntity == null)
			{
				return;
			}
			GRScannable component = gameEntity.GetComponent<GRScannable>();
			if (component == null)
			{
				return;
			}
			this.titleText.text = component.GetTitleText(this.recycler.reactor);
			this.descriptionText.text = component.GetBodyText(this.recycler.reactor);
			this.annotationText.text = component.GetAnnotationText(this.recycler.reactor);
			this.recycleValueText.text = string.Format("Recycle value: {0}", this.recycler.GetRecycleValue(gameEntity.gameObject.GetToolType()));
			this.audioSource.volume = this.recyclerBarcodeAudioVolume;
			this.audioSource.PlayOneShot(this.recyclerBarcodeAudio);
		}
	}

	// Token: 0x060032CE RID: 13006 RVA: 0x00116964 File Offset: 0x00114B64
	private void OnTriggerEnter(Collider other)
	{
		if (this.recycler.reactor == null)
		{
			return;
		}
		if (!this.recycler.reactor.grManager.IsAuthority())
		{
			return;
		}
		GRScannable componentInParent = other.gameObject.GetComponentInParent<GRScannable>();
		if (componentInParent == null)
		{
			return;
		}
		this.recycler.reactor.grManager.RequestRecycleScanItem(componentInParent.gameEntity.id);
	}

	// Token: 0x04004206 RID: 16902
	public GRRecycler recycler;

	// Token: 0x04004207 RID: 16903
	[SerializeField]
	private TextMeshPro titleText;

	// Token: 0x04004208 RID: 16904
	[SerializeField]
	private TextMeshPro descriptionText;

	// Token: 0x04004209 RID: 16905
	[SerializeField]
	private TextMeshPro annotationText;

	// Token: 0x0400420A RID: 16906
	[SerializeField]
	private TextMeshPro recycleValueText;

	// Token: 0x0400420B RID: 16907
	public AudioSource audioSource;

	// Token: 0x0400420C RID: 16908
	public AudioClip recyclerBarcodeAudio;

	// Token: 0x0400420D RID: 16909
	public float recyclerBarcodeAudioVolume = 0.5f;
}
