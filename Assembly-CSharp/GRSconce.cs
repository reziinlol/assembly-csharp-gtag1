using System;
using UnityEngine;

// Token: 0x020007CF RID: 1999
public class GRSconce : MonoBehaviour
{
	// Token: 0x060032FB RID: 13051 RVA: 0x001173D4 File Offset: 0x001155D4
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange += this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged += this.OnStateChange;
		}
		this.state = GRSconce.State.Off;
		this.StopLight();
	}

	// Token: 0x060032FC RID: 13052 RVA: 0x00117438 File Offset: 0x00115638
	private bool IsAuthority()
	{
		return this.gameEntity.IsAuthority();
	}

	// Token: 0x060032FD RID: 13053 RVA: 0x00117448 File Offset: 0x00115648
	private void SetState(GRSconce.State newState)
	{
		this.state = newState;
		GRSconce.State state = this.state;
		if (state != GRSconce.State.Off)
		{
			if (state == GRSconce.State.On)
			{
				this.StartLight();
			}
		}
		else
		{
			this.StopLight();
		}
		if (this.IsAuthority())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x060032FE RID: 13054 RVA: 0x0011749C File Offset: 0x0011569C
	private void StartLight()
	{
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.volume = this.lightOnSoundVolume;
		this.audioSource.clip = this.lightOnSound;
		this.audioSource.Play();
		this.meshRenderer.material = this.onMaterial;
	}

	// Token: 0x060032FF RID: 13055 RVA: 0x001174F8 File Offset: 0x001156F8
	private void StopLight()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x06003300 RID: 13056 RVA: 0x0011751C File Offset: 0x0011571C
	private void OnEnergyChange(GRTool tool, int energy, GameEntityId chargingEntityId)
	{
		if (this.IsAuthority() && this.state == GRSconce.State.Off && tool.IsEnergyFull())
		{
			this.SetState(GRSconce.State.On);
		}
	}

	// Token: 0x06003301 RID: 13057 RVA: 0x00117540 File Offset: 0x00115740
	private void OnStateChange(long prevState, long nextState)
	{
		if (!this.IsAuthority())
		{
			GRSconce.State state = (GRSconce.State)nextState;
			this.SetState(state);
		}
	}

	// Token: 0x04004239 RID: 16953
	public GameEntity gameEntity;

	// Token: 0x0400423A RID: 16954
	public GameLight gameLight;

	// Token: 0x0400423B RID: 16955
	public GRTool tool;

	// Token: 0x0400423C RID: 16956
	public MeshRenderer meshRenderer;

	// Token: 0x0400423D RID: 16957
	public Material offMaterial;

	// Token: 0x0400423E RID: 16958
	public Material onMaterial;

	// Token: 0x0400423F RID: 16959
	public AudioSource audioSource;

	// Token: 0x04004240 RID: 16960
	public AudioClip lightOnSound;

	// Token: 0x04004241 RID: 16961
	public float lightOnSoundVolume;

	// Token: 0x04004242 RID: 16962
	private GRSconce.State state;

	// Token: 0x020007D0 RID: 2000
	private enum State
	{
		// Token: 0x04004244 RID: 16964
		Off,
		// Token: 0x04004245 RID: 16965
		On
	}
}
