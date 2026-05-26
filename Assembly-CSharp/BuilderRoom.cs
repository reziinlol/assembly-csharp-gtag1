using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000649 RID: 1609
public class BuilderRoom : MonoBehaviour
{
	// Token: 0x04003446 RID: 13382
	public List<GameObject> disableColliderRoots;

	// Token: 0x04003447 RID: 13383
	public List<GameObject> disableRenderRoots;

	// Token: 0x04003448 RID: 13384
	public List<GameObject> disableGameObjectsForScene;

	// Token: 0x04003449 RID: 13385
	public List<GameObject> disableObjectsForPersistent;

	// Token: 0x0400344A RID: 13386
	public List<MeshRenderer> disabledRenderersForPersistent;

	// Token: 0x0400344B RID: 13387
	public List<Collider> disabledCollidersForScene;
}
