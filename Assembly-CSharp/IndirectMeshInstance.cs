using System;
using UnityEngine;

// Token: 0x02000375 RID: 885
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public sealed class IndirectMeshInstance : MonoBehaviour
{
	// Token: 0x060015AB RID: 5547 RVA: 0x00072538 File Offset: 0x00070738
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshFilter = base.GetComponent<MeshFilter>();
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00072554 File Offset: 0x00070754
	private void OnEnable()
	{
		if (this._registered)
		{
			return;
		}
		this._registered = true;
		IndirectMeshGroup componentInParent = base.GetComponentInParent<IndirectMeshGroup>();
		IndirectMeshRenderer.Register(this, (componentInParent != null) ? componentInParent.GetInstanceID() : 0);
		if (this.dynamic)
		{
			this.meshRenderer.enabled = false;
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001A6F RID: 6767
	[Tooltip("When true, the transform is tracked and updated each frame instead of baked at registration time.")]
	[SerializeField]
	internal bool dynamic;

	// Token: 0x04001A70 RID: 6768
	internal MeshRenderer meshRenderer;

	// Token: 0x04001A71 RID: 6769
	internal MeshFilter meshFilter;

	// Token: 0x04001A72 RID: 6770
	private bool _registered;
}
