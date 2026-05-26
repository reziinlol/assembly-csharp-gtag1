using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class ParticleEffectsPool : MonoBehaviour
{
	// Token: 0x060012EA RID: 4842 RVA: 0x000647FD File Offset: 0x000629FD
	public void Awake()
	{
		this.OnPoolAwake();
		this.Setup();
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnPoolAwake()
	{
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x0006480C File Offset: 0x00062A0C
	private void Setup()
	{
		this.MoveToSceneWorldRoot();
		this._pools = new RingBuffer<ParticleEffect>[this.effects.Length];
		this._effectToPool = new Dictionary<long, int>(this.effects.Length);
		for (int i = 0; i < this.effects.Length; i++)
		{
			ParticleEffect particleEffect = this.effects[i];
			this._pools[i] = this.InitPoolForPrefab(i, this.effects[i]);
			this._effectToPool.TryAdd(particleEffect.effectID, i);
		}
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x0006488B File Offset: 0x00062A8B
	private void MoveToSceneWorldRoot()
	{
		Transform transform = base.transform;
		transform.parent = null;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000648BC File Offset: 0x00062ABC
	private RingBuffer<ParticleEffect> InitPoolForPrefab(int index, ParticleEffect prefab)
	{
		RingBuffer<ParticleEffect> ringBuffer = new RingBuffer<ParticleEffect>(this.poolSize);
		string arg = prefab.name.Trim();
		for (int i = 0; i < this.poolSize; i++)
		{
			ParticleEffect particleEffect = Object.Instantiate<ParticleEffect>(prefab, base.transform);
			particleEffect.gameObject.SetActive(false);
			particleEffect.pool = this;
			particleEffect.poolIndex = index;
			particleEffect.name = ZString.Concat<string, string, int>(arg, "*", i);
			ringBuffer.Push(particleEffect);
		}
		return ringBuffer;
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00064934 File Offset: 0x00062B34
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos)
	{
		this.PlayEffect(effect.effectID, worldPos);
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00064943 File Offset: 0x00062B43
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos, float delay)
	{
		this.PlayEffect(effect.effectID, worldPos, delay);
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00064953 File Offset: 0x00062B53
	public void PlayEffect(long effectID, Vector3 worldPos)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos);
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00064963 File Offset: 0x00062B63
	public void PlayEffect(long effectID, Vector3 worldPos, float delay)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos, delay);
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00064974 File Offset: 0x00062B74
	public void PlayEffect(int index, Vector3 worldPos)
	{
		if (index == -1)
		{
			return;
		}
		ParticleEffect particleEffect;
		if (!this._pools[index].TryPop(out particleEffect))
		{
			return;
		}
		particleEffect.transform.localPosition = worldPos;
		particleEffect.Play();
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x000649AA File Offset: 0x00062BAA
	public void PlayEffect(int index, Vector3 worldPos, float delay)
	{
		if (delay.Approx(0f, 1E-06f))
		{
			this.PlayEffect(index, worldPos);
			return;
		}
		base.StartCoroutine(this.PlayDelayed(index, worldPos, delay));
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x000649D7 File Offset: 0x00062BD7
	private IEnumerator PlayDelayed(int index, Vector3 worldPos, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlayEffect(index, worldPos);
		yield break;
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x000649FB File Offset: 0x00062BFB
	public void Return(ParticleEffect effect)
	{
		this._pools[effect.poolIndex].Push(effect);
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x00064A14 File Offset: 0x00062C14
	public int GetPoolIndex(long effectID)
	{
		int result;
		if (this._effectToPool.TryGetValue(effectID, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x04001728 RID: 5928
	public ParticleEffect[] effects = new ParticleEffect[0];

	// Token: 0x04001729 RID: 5929
	[Space]
	public int poolSize = 10;

	// Token: 0x0400172A RID: 5930
	[Space]
	private RingBuffer<ParticleEffect>[] _pools = new RingBuffer<ParticleEffect>[0];

	// Token: 0x0400172B RID: 5931
	private Dictionary<long, int> _effectToPool = new Dictionary<long, int>();
}
