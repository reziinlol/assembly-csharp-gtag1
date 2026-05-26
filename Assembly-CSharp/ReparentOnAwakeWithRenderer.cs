using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200038C RID: 908
public class ReparentOnAwakeWithRenderer : MonoBehaviour, IBuildValidation
{
	// Token: 0x060015EB RID: 5611 RVA: 0x0007FFAD File Offset: 0x0007E1AD
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() != null && this.myRenderer == null)
		{
			Debug.Log(base.name + " needs a reference to its renderer since it has one - ");
			return false;
		}
		return true;
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x0007FFE4 File Offset: 0x0007E1E4
	private void OnEnable()
	{
		base.transform.SetParent(this.newParent, true);
		if (this.sortLast)
		{
			base.transform.SetAsLastSibling();
		}
		else
		{
			base.transform.SetAsFirstSibling();
		}
		if (this.myRenderer != null)
		{
			this.myRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			this.myRenderer.lightProbeUsage = LightProbeUsage.Off;
			this.myRenderer.probeAnchor = this.newParent;
		}
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x0008005A File Offset: 0x0007E25A
	[ContextMenu("Set Renderer")]
	public void SetMyRenderer()
	{
		this.myRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x0400203D RID: 8253
	public Transform newParent;

	// Token: 0x0400203E RID: 8254
	public MeshRenderer myRenderer;

	// Token: 0x0400203F RID: 8255
	[Tooltip("We're mostly using this for UI elements like text and images, so this will help you separate these in whatever target parent object.Keep images and texts together, otherwise you'll get extra draw calls. Put images above text or they'll overlap weird tho lol")]
	public bool sortLast;
}
