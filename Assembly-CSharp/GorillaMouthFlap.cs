using System;
using UnityEngine;

// Token: 0x0200087D RID: 2173
public class GorillaMouthFlap : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003892 RID: 14482 RVA: 0x001354EC File Offset: 0x001336EC
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
		this.targetFaceRenderer = this.targetFace.GetComponent<Renderer>();
		this.facePropBlock = new MaterialPropertyBlock();
		this.hasDefaultMouthAtlas = false;
		if (this.targetFaceRenderer != null)
		{
			this.SetDefaultMouthAtlas(this.targetFaceRenderer.material);
		}
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x00135547 File Offset: 0x00133747
	public void EnableLeafBlower()
	{
		this.leafBlowerActiveUntilTimestamp = Time.time + 0.1f;
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x0013555A File Offset: 0x0013375A
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTimeUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x0013557C File Offset: 0x0013377C
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.lastTimeUpdated;
		this.lastTimeUpdated = Time.time;
		if (this.speaker == null)
		{
			this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
			return;
		}
		float currentLoudness = 0f;
		if (this.speaker.IsSpeaking)
		{
			currentLoudness = this.speaker.Loudness;
		}
		this.CheckMouthflapChange(this.speaker.IsMicEnabled, currentLoudness);
		MouthFlapLevel mouthFlap = this.noMicFace;
		if (this.leafBlowerActiveUntilTimestamp > Time.time)
		{
			mouthFlap = this.leafBlowerFace;
		}
		else if (this.useMicEnabled)
		{
			mouthFlap = this.mouthFlapLevels[this.activeFlipbookIndex];
		}
		this.UpdateMouthFlapFlipbook(mouthFlap);
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x00135630 File Offset: 0x00133830
	private void CheckMouthflapChange(bool isMicEnabled, float currentLoudness)
	{
		if (isMicEnabled)
		{
			this.useMicEnabled = true;
			int i = this.mouthFlapLevels.Length - 1;
			while (i >= 0)
			{
				if (currentLoudness >= this.mouthFlapLevels[i].maxRequiredVolume)
				{
					return;
				}
				if (currentLoudness > this.mouthFlapLevels[i].minRequiredVolume)
				{
					if (this.activeFlipbookIndex != i)
					{
						this.activeFlipbookIndex = i;
						this.activeFlipbookPlayTime = 0f;
						return;
					}
					return;
				}
				else
				{
					i--;
				}
			}
			return;
		}
		if (this.useMicEnabled)
		{
			this.useMicEnabled = false;
			this.activeFlipbookPlayTime = 0f;
		}
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x001356BC File Offset: 0x001338BC
	private void UpdateMouthFlapFlipbook(MouthFlapLevel mouthFlap)
	{
		Material material = this.targetFaceRenderer.material;
		this.activeFlipbookPlayTime += this.deltaTime;
		this.activeFlipbookPlayTime %= mouthFlap.cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)mouthFlap.faces.Length / mouthFlap.cycleDuration);
		material.SetTextureOffset(this._MouthMap, mouthFlap.faces[num]);
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x00135734 File Offset: 0x00133934
	public void SetMouthTextureReplacement(Texture2D replacementMouthAtlas)
	{
		Material material = this.targetFaceRenderer.material;
		this.SetDefaultMouthAtlas(material);
		material.SetTexture(this._MouthMap, replacementMouthAtlas);
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x00135766 File Offset: 0x00133966
	public void ClearMouthTextureReplacement()
	{
		this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x0013578C File Offset: 0x0013398C
	public Material SetFaceMaterialReplacement(Material replacementFaceMaterial)
	{
		if (!this.hasDefaultFaceMaterial)
		{
			this.defaultFaceMaterial = this.targetFaceRenderer.material;
			this.hasDefaultFaceMaterial = true;
		}
		this.targetFaceRenderer.material = replacementFaceMaterial;
		if (this.hasDefaultMouthAtlas && this.defaultMouthAtlas != null)
		{
			this.targetFaceRenderer.material.SetTexture(this._MouthMap, this.defaultMouthAtlas);
		}
		return this.targetFaceRenderer.material;
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x00135807 File Offset: 0x00133A07
	public void ClearFaceMaterialReplacement()
	{
		if (this.hasDefaultFaceMaterial)
		{
			this.targetFaceRenderer.material = this.defaultFaceMaterial;
		}
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x00135822 File Offset: 0x00133A22
	private void SetDefaultMouthAtlas(Material face)
	{
		if (!this.hasDefaultMouthAtlas)
		{
			this.defaultMouthAtlas = face.GetTexture(this._MouthMap);
			this.hasDefaultMouthAtlas = true;
		}
	}

	// Token: 0x0400489C RID: 18588
	public GameObject targetFace;

	// Token: 0x0400489D RID: 18589
	public MouthFlapLevel[] mouthFlapLevels;

	// Token: 0x0400489E RID: 18590
	public MouthFlapLevel noMicFace;

	// Token: 0x0400489F RID: 18591
	public MouthFlapLevel leafBlowerFace;

	// Token: 0x040048A0 RID: 18592
	private bool useMicEnabled;

	// Token: 0x040048A1 RID: 18593
	private float leafBlowerActiveUntilTimestamp;

	// Token: 0x040048A2 RID: 18594
	private int activeFlipbookIndex;

	// Token: 0x040048A3 RID: 18595
	private float activeFlipbookPlayTime;

	// Token: 0x040048A4 RID: 18596
	private GorillaSpeakerLoudness speaker;

	// Token: 0x040048A5 RID: 18597
	private float lastTimeUpdated;

	// Token: 0x040048A6 RID: 18598
	private float deltaTime;

	// Token: 0x040048A7 RID: 18599
	private Renderer targetFaceRenderer;

	// Token: 0x040048A8 RID: 18600
	private MaterialPropertyBlock facePropBlock;

	// Token: 0x040048A9 RID: 18601
	private Texture defaultMouthAtlas;

	// Token: 0x040048AA RID: 18602
	private Material defaultFaceMaterial;

	// Token: 0x040048AB RID: 18603
	private bool hasDefaultMouthAtlas;

	// Token: 0x040048AC RID: 18604
	private bool hasDefaultFaceMaterial;

	// Token: 0x040048AD RID: 18605
	private ShaderHashId _MouthMap = "_MouthMap";

	// Token: 0x040048AE RID: 18606
	private ShaderHashId _BaseMap = "_BaseMap";
}
