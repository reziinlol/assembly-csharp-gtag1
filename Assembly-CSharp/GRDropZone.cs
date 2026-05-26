using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200075E RID: 1886
public class GRDropZone : MonoBehaviour
{
	// Token: 0x06002FC6 RID: 12230 RVA: 0x001037AF File Offset: 0x001019AF
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x001037D0 File Offset: 0x001019D0
	private void OnTriggerEnter(Collider other)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.manager.ghostReactorManager != null)
		{
			GhostReactorManager.Get(component).EntityEnteredDropZone(component);
		}
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x00103819 File Offset: 0x00101A19
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x00103824 File Offset: 0x00101A24
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x0010389D File Offset: 0x00101A9D
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x04003D35 RID: 15669
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x04003D36 RID: 15670
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x04003D37 RID: 15671
	public float effectDuration = 1f;

	// Token: 0x04003D38 RID: 15672
	private bool playingEffect;

	// Token: 0x04003D39 RID: 15673
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x04003D3A RID: 15674
	private Vector3 repelDirectionWorld = Vector3.up;
}
