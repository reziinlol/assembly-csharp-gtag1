using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000356 RID: 854
public class LocalChestController : MonoBehaviour
{
	// Token: 0x060014E5 RID: 5349 RVA: 0x0006F478 File Offset: 0x0006D678
	private void OnTriggerEnter(Collider other)
	{
		if (this.isOpen)
		{
			return;
		}
		TransformFollow component = other.GetComponent<TransformFollow>();
		if (component == null)
		{
			return;
		}
		Transform transformToFollow = component.transformToFollow;
		if (transformToFollow == null)
		{
			return;
		}
		VRRig componentInParent = transformToFollow.GetComponentInParent<VRRig>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.playerCollectionVolume != null && !this.playerCollectionVolume.containedRigs.Contains(componentInParent))
		{
			return;
		}
		this.isOpen = true;
		this.director.Play();
	}

	// Token: 0x040019C3 RID: 6595
	public PlayableDirector director;

	// Token: 0x040019C4 RID: 6596
	public MazePlayerCollection playerCollectionVolume;

	// Token: 0x040019C5 RID: 6597
	private bool isOpen;
}
