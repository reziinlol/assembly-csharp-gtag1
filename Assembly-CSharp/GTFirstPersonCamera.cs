using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000372 RID: 882
[DefaultExecutionOrder(-2147483648)]
public class GTFirstPersonCamera : MonoBehaviour
{
	// Token: 0x17000224 RID: 548
	// (get) Token: 0x060015A0 RID: 5536 RVA: 0x00072410 File Offset: 0x00070610
	// (set) Token: 0x060015A1 RID: 5537 RVA: 0x00072417 File Offset: 0x00070617
	public static Camera camera { get; private set; }

	// Token: 0x060015A2 RID: 5538 RVA: 0x0007241F File Offset: 0x0007061F
	public void Awake()
	{
		GTFirstPersonCamera.camera = base.GetComponent<Camera>();
		if (GTFirstPersonCamera.camera == null)
		{
			Debug.LogError("[GTFirstPersonCamera]  ERROR!!!  Could not find Camera on same GameObject!");
			return;
		}
		RenderPipelineManager.beginCameraRendering += this._OnPreRender;
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x00072455 File Offset: 0x00070655
	private void _OnPreRender(ScriptableRenderContext context, Camera cam)
	{
		if (cam == GTFirstPersonCamera.camera)
		{
			Action onPreRenderEvent = GTFirstPersonCamera.OnPreRenderEvent;
			if (onPreRenderEvent == null)
			{
				return;
			}
			onPreRenderEvent();
		}
	}

	// Token: 0x04001A68 RID: 6760
	private const string preLog = "[GTFirstPersonCamera]  ";

	// Token: 0x04001A69 RID: 6761
	private const string preErr = "[GTFirstPersonCamera]  ERROR!!!  ";

	// Token: 0x04001A6B RID: 6763
	public static Action OnPreRenderEvent;
}
