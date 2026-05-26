using System;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class DJDeckEqualizer : MonoBehaviour
{
	// Token: 0x0600120F RID: 4623 RVA: 0x00060C1D File Offset: 0x0005EE1D
	private void Start()
	{
		this.inputColorHash = this.inputColorProperty;
		this.material = this.display.material;
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00060C44 File Offset: 0x0005EE44
	private void Update()
	{
		Color value = default(Color);
		value.r = 0.25f;
		value.g = 0.25f;
		value.b = 0.5f;
		for (int i = 0; i < this.redTracks.Length; i++)
		{
			AudioSource audioSource = this.redTracks[i];
			if (audioSource.isPlaying)
			{
				value.r = Mathf.Lerp(0.25f, 1f, this.redTrackCurves[i].Evaluate(audioSource.time));
				break;
			}
		}
		for (int j = 0; j < this.greenTracks.Length; j++)
		{
			AudioSource audioSource2 = this.greenTracks[j];
			if (audioSource2.isPlaying)
			{
				value.g = Mathf.Lerp(0.25f, 1f, this.greenTrackCurves[j].Evaluate(audioSource2.time));
				break;
			}
		}
		this.material.SetColor(this.inputColorHash, value);
	}

	// Token: 0x040015D3 RID: 5587
	[SerializeField]
	private MeshRenderer display;

	// Token: 0x040015D4 RID: 5588
	[SerializeField]
	private AnimationCurve[] redTrackCurves;

	// Token: 0x040015D5 RID: 5589
	[SerializeField]
	private AnimationCurve[] greenTrackCurves;

	// Token: 0x040015D6 RID: 5590
	[SerializeField]
	private AudioSource[] redTracks;

	// Token: 0x040015D7 RID: 5591
	[SerializeField]
	private AudioSource[] greenTracks;

	// Token: 0x040015D8 RID: 5592
	private Material material;

	// Token: 0x040015D9 RID: 5593
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x040015DA RID: 5594
	private ShaderHashId inputColorHash;
}
