using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x020003EC RID: 1004
public class LckCameraEvents : MonoBehaviour
{
	// Token: 0x060017E4 RID: 6116 RVA: 0x00088B81 File Offset: 0x00086D81
	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering += this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x00088BA5 File Offset: 0x00086DA5
	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering -= this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x00088BC9 File Offset: 0x00086DC9
	private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPreRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x00088BEA File Offset: 0x00086DEA
	private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPostRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04002318 RID: 8984
	[SerializeField]
	private Camera _camera;

	// Token: 0x04002319 RID: 8985
	public UnityEvent onPreRender;

	// Token: 0x0400231A RID: 8986
	public UnityEvent onPostRender;
}
