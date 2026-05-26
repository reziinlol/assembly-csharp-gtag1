using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200073E RID: 1854
public class GRArmorEnemy : MonoBehaviour
{
	// Token: 0x06002F11 RID: 12049 RVA: 0x00100BE1 File Offset: 0x000FEDE1
	private void Awake()
	{
		this.SetHp(0);
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x00100BF6 File Offset: 0x000FEDF6
	public void SetHp(int hp)
	{
		this.hp = hp;
		this.RefreshArmor();
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x00100C08 File Offset: 0x000FEE08
	private void RefreshArmor()
	{
		bool flag = this.hp > 0;
		GREnemy.HideRenderers(this.renderers, !flag);
		GREnemy.HideObjects(this.visibleObjects, !flag);
		if (this.armorStateData.Count > 0)
		{
			int num = -1;
			Material mainRendererMaterial = this.armorStateData[0].mainRendererMaterial;
			for (int i = 0; i < this.armorStateData.Count; i++)
			{
				num = i;
				mainRendererMaterial = this.armorStateData[i].mainRendererMaterial;
				if (this.hp >= this.armorStateData[i].healthThreshold)
				{
					break;
				}
			}
			if (flag && this.materialSwapRenderer != null && mainRendererMaterial != this.materialSwapRenderer.material)
			{
				this.materialSwapRenderer.material = mainRendererMaterial;
				this.SetArmorColor(this.GetArmorColor());
			}
			if (num != -1)
			{
				GREnemy.HideObjects(this.armorStateData[num].visibleObjects, !flag);
				for (int j = 0; j < this.armorStateData[num].hiddenObjects.Count; j++)
				{
					GameObject gameObject = this.armorStateData[num].hiddenObjects[j];
					if (gameObject.activeInHierarchy)
					{
						this.PlayDestroyFx(gameObject.transform.position);
					}
				}
				GREnemy.HideObjects(this.armorStateData[num].hiddenObjects, true);
			}
		}
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x00100D73 File Offset: 0x000FEF73
	public void SetArmorColor(Color newColor)
	{
		if (this.renderers != null && this.renderers.Count > 0)
		{
			this.materialSwapRenderer.material.SetColor("_BaseColor", newColor);
		}
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x00100DA4 File Offset: 0x000FEFA4
	public Color GetArmorColor()
	{
		Color result = Color.white;
		if (this.materialSwapRenderer != null)
		{
			result = this.materialSwapRenderer.material.GetColor("_BaseColor");
		}
		return result;
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x00100DDC File Offset: 0x000FEFDC
	public void PlayHitFx(Vector3 position)
	{
		this.PlayFx(this.fxHit, position);
		this.PlaySound(this.hitSound, this.hitSoundVolume, position);
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x00100DFE File Offset: 0x000FEFFE
	public void PlayBlockFx(Vector3 position)
	{
		this.PlayFx(this.fxBlock, position);
		this.PlaySound(this.blockSound, this.blockSoundVolume, position);
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x00100E20 File Offset: 0x000FF020
	public void PlayDestroyFx(Vector3 position)
	{
		this.PlayFx(this.fxDestroy, position);
		this.PlaySound(this.destroySound, this.destroySoundVolume, position);
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x00100E42 File Offset: 0x000FF042
	private void PlayFx(GameObject fx, Vector3 position)
	{
		if (fx == null)
		{
			return;
		}
		fx.SetActive(false);
		fx.SetActive(true);
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x00100E5C File Offset: 0x000FF05C
	private void PlaySound(AudioClip clip, float volume, Vector3 position)
	{
		this.audioSource.clip = clip;
		this.audioSource.volume = volume;
		this.audioSource.Play();
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x00100E84 File Offset: 0x000FF084
	public void FragmentArmor()
	{
		if (this.entity.IsAuthority())
		{
			float num = 0f;
			for (int i = 0; i < this.numFragmentsWhenShattered; i++)
			{
				num += 360f / (float)this.numFragmentsWhenShattered;
				Quaternion rotation = Quaternion.Euler(0f, num, this.fragmentLaunchPitch);
				Vector3 b = rotation * this.fragmentSpawnOffset;
				this.entity.manager.RequestCreateItem(this.armorFragmentPrefab.name.GetStaticHash(), base.transform.position + b, rotation, (long)this.entity.GetNetId());
			}
		}
	}

	// Token: 0x04003C4E RID: 15438
	[SerializeField]
	private List<Renderer> renderers;

	// Token: 0x04003C4F RID: 15439
	[SerializeField]
	private List<GameObject> visibleObjects;

	// Token: 0x04003C50 RID: 15440
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003C51 RID: 15441
	[SerializeField]
	private GameObject fxHit;

	// Token: 0x04003C52 RID: 15442
	[SerializeField]
	private AudioClip hitSound;

	// Token: 0x04003C53 RID: 15443
	[SerializeField]
	private float hitSoundVolume;

	// Token: 0x04003C54 RID: 15444
	[SerializeField]
	private GameObject fxBlock;

	// Token: 0x04003C55 RID: 15445
	[SerializeField]
	private AudioClip blockSound;

	// Token: 0x04003C56 RID: 15446
	[SerializeField]
	private float blockSoundVolume;

	// Token: 0x04003C57 RID: 15447
	[SerializeField]
	private GameObject fxDestroy;

	// Token: 0x04003C58 RID: 15448
	[SerializeField]
	private AudioClip destroySound;

	// Token: 0x04003C59 RID: 15449
	[SerializeField]
	private float destroySoundVolume;

	// Token: 0x04003C5A RID: 15450
	[SerializeField]
	public List<GRArmorEnemy.GREnemyArmorLevel> armorStateData;

	// Token: 0x04003C5B RID: 15451
	[SerializeField]
	public Renderer materialSwapRenderer;

	// Token: 0x04003C5C RID: 15452
	private GameEntity entity;

	// Token: 0x04003C5D RID: 15453
	public GameObject armorFragmentPrefab;

	// Token: 0x04003C5E RID: 15454
	public Vector3 fragmentSpawnOffset = new Vector3(0f, 0.5f, 0.5f);

	// Token: 0x04003C5F RID: 15455
	public int numFragmentsWhenShattered = 3;

	// Token: 0x04003C60 RID: 15456
	public float fragmentLaunchPitch = 30f;

	// Token: 0x04003C61 RID: 15457
	private int hp;

	// Token: 0x0200073F RID: 1855
	[Serializable]
	public struct GREnemyArmorLevel
	{
		// Token: 0x04003C62 RID: 15458
		public int healthThreshold;

		// Token: 0x04003C63 RID: 15459
		public Material mainRendererMaterial;

		// Token: 0x04003C64 RID: 15460
		public List<GameObject> visibleObjects;

		// Token: 0x04003C65 RID: 15461
		public List<GameObject> hiddenObjects;
	}
}
