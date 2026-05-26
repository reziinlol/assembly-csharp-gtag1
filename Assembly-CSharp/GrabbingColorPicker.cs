using System;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000574 RID: 1396
public class GrabbingColorPicker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002377 RID: 9079 RVA: 0x000BF284 File Offset: 0x000BD484
	private void Start()
	{
		if (!this.setPlayerColor)
		{
			return;
		}
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		this.LoadPlayerColor(@float, float2, float3);
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000BF2D4 File Offset: 0x000BD4D4
	private void LoadPlayerColor(float r, float g, float b)
	{
		this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
		this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
		this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
		this.R_PushSlider.SetProgress(r);
		this.G_PushSlider.SetProgress(g);
		this.B_PushSlider.SetProgress(b);
		this.UpdateDisplay();
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000BF35C File Offset: 0x000BD55C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.setPlayerColor)
		{
			return;
		}
		CosmeticsController.OnPlayerColorSet = (Action<float, float, float>)Delegate.Combine(CosmeticsController.OnPlayerColorSet, new Action<float, float, float>(this.LoadPlayerColor));
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += this.HandleLocalColorChanged;
		}
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000BF3D4 File Offset: 0x000BD5D4
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.setPlayerColor)
		{
			return;
		}
		CosmeticsController.OnPlayerColorSet = (Action<float, float, float>)Delegate.Remove(CosmeticsController.OnPlayerColorSet, new Action<float, float, float>(this.LoadPlayerColor));
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= this.HandleLocalColorChanged;
		}
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000BF44C File Offset: 0x000BD64C
	public void SliceUpdate()
	{
		float num = Vector3.Distance(base.transform.position, GTPlayer.Instance.transform.position);
		this.hasUpdated = false;
		if (num < 5f)
		{
			int segment = this.Segment1;
			int segment2 = this.Segment2;
			int segment3 = this.Segment3;
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.R_PushSlider.GetProgress()));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.G_PushSlider.GetProgress()));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.B_PushSlider.GetProgress()));
			if (segment != this.Segment1 || segment2 != this.Segment2 || segment3 != this.Segment3)
			{
				this.hasUpdated = true;
				if (this.setPlayerColor)
				{
					this.SetPlayerColor();
				}
				this.UpdateDisplay();
				this.UpdateColor.Invoke(new Vector3((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f));
				if (segment != this.Segment1)
				{
					this.R_SliderAudio.transform.position = this.R_PushSlider.transform.position;
					this.R_SliderAudio.GTPlay();
				}
				if (segment2 != this.Segment2)
				{
					this.G_SliderAudio.transform.position = this.G_PushSlider.transform.position;
					this.G_SliderAudio.GTPlay();
				}
				if (segment3 != this.Segment3)
				{
					this.B_SliderAudio.transform.position = this.B_PushSlider.transform.position;
					this.B_SliderAudio.GTPlay();
				}
			}
		}
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x000BF61C File Offset: 0x000BD81C
	private void SetPlayerColor()
	{
		PlayerPrefs.SetFloat("redValue", (float)this.Segment1 / 9f);
		PlayerPrefs.SetFloat("greenValue", (float)this.Segment2 / 9f);
		PlayerPrefs.SetFloat("blueValue", (float)this.Segment3 / 9f);
		GorillaTagger.Instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		GorillaComputer.instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		PlayerPrefs.Save();
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				(float)this.Segment1 / 9f,
				(float)this.Segment2 / 9f,
				(float)this.Segment3 / 9f
			});
		}
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000BF740 File Offset: 0x000BD940
	private void SetSliderColors(float r, float g, float b)
	{
		if (!this.hasUpdated)
		{
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
			this.R_PushSlider.SetProgress(r);
			this.G_PushSlider.SetProgress(g);
			this.B_PushSlider.SetProgress(b);
			this.UpdateDisplay();
		}
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000BF7D0 File Offset: 0x000BD9D0
	private void HandleLocalColorChanged(Color newColor)
	{
		this.SetSliderColors(newColor.r, newColor.g, newColor.b);
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000BF7EC File Offset: 0x000BD9EC
	private void UpdateDisplay()
	{
		this.textR.text = this.Segment1.ToString();
		this.textG.text = this.Segment2.ToString();
		this.textB.text = this.Segment3.ToString();
		Color color = new Color((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		Renderer[] componentsInChildren = this.ColorSwatch.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].color = color;
			}
		}
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000BF8A8 File Offset: 0x000BDAA8
	public void ResetSliders(Vector3 v)
	{
		this.SetSliderColors(v.x, v.y, v.z);
	}

	// Token: 0x04002E9B RID: 11931
	[SerializeField]
	private bool setPlayerColor = true;

	// Token: 0x04002E9C RID: 11932
	[SerializeField]
	private PushableSlider R_PushSlider;

	// Token: 0x04002E9D RID: 11933
	[SerializeField]
	private PushableSlider G_PushSlider;

	// Token: 0x04002E9E RID: 11934
	[SerializeField]
	private PushableSlider B_PushSlider;

	// Token: 0x04002E9F RID: 11935
	[SerializeField]
	private AudioSource R_SliderAudio;

	// Token: 0x04002EA0 RID: 11936
	[SerializeField]
	private AudioSource G_SliderAudio;

	// Token: 0x04002EA1 RID: 11937
	[SerializeField]
	private AudioSource B_SliderAudio;

	// Token: 0x04002EA2 RID: 11938
	[SerializeField]
	private TextMeshPro textR;

	// Token: 0x04002EA3 RID: 11939
	[SerializeField]
	private TextMeshPro textG;

	// Token: 0x04002EA4 RID: 11940
	[SerializeField]
	private TextMeshPro textB;

	// Token: 0x04002EA5 RID: 11941
	[SerializeField]
	private GameObject ColorSwatch;

	// Token: 0x04002EA6 RID: 11942
	[SerializeField]
	private UnityEvent<Vector3> UpdateColor;

	// Token: 0x04002EA7 RID: 11943
	private int Segment1;

	// Token: 0x04002EA8 RID: 11944
	private int Segment2;

	// Token: 0x04002EA9 RID: 11945
	private int Segment3;

	// Token: 0x04002EAA RID: 11946
	private bool hasUpdated;
}
