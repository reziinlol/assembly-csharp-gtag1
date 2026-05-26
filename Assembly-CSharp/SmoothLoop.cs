using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class SmoothLoop : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x060022F0 RID: 8944 RVA: 0x000BB6F1 File Offset: 0x000B98F1
	public bool BuildValidationCheck()
	{
		if (this.source == null)
		{
			Debug.LogError("missing audio source, this will fail", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000BB714 File Offset: 0x000B9914
	private void Start()
	{
		if (this.delay != 0f && !this.randomStart)
		{
			this.source.GTStop();
			base.StartCoroutine(this.DelayedStart());
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000BB794 File Offset: 0x000B9994
	public void SliceUpdate()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.source.time > this.source.clip.length * this.loopEnd)
		{
			this.source.time = this.loopStart;
		}
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000BB7D4 File Offset: 0x000B99D4
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (!this.sourceCheck())
		{
			return;
		}
		if (this.randomStart)
		{
			if (this.source.isActiveAndEnabled)
			{
				this.source.GTPlay();
			}
			this.source.time = Random.Range(0f, this.source.clip.length);
		}
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000BB838 File Offset: 0x000B9A38
	private bool sourceCheck()
	{
		if (!this.source || !this.source.clip)
		{
			Debug.LogError("SmoothLoop: Disabling because AudioSource is null or has no clip assigned. Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			base.StopAllCoroutines();
			return false;
		}
		return true;
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000BB88F File Offset: 0x000B9A8F
	public IEnumerator DelayedStart()
	{
		if (!this.sourceCheck())
		{
			yield break;
		}
		yield return new WaitForSeconds(this.delay);
		this.source.GTPlay();
		yield break;
	}

	// Token: 0x04002DF9 RID: 11769
	public AudioSource source;

	// Token: 0x04002DFA RID: 11770
	public float delay;

	// Token: 0x04002DFB RID: 11771
	public bool randomStart;

	// Token: 0x04002DFC RID: 11772
	[SerializeField]
	[Range(0f, 1f)]
	private float loopStart = 0.1f;

	// Token: 0x04002DFD RID: 11773
	[SerializeField]
	[Range(0f, 1f)]
	private float loopEnd = 0.95f;
}
