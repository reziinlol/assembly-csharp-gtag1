using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000DBF RID: 3519
public class FireworksController : MonoBehaviour
{
	// Token: 0x0600564C RID: 22092 RVA: 0x001C0096 File Offset: 0x001BE296
	private void Awake()
	{
		this._launchOrder = this.fireworks.ToArray<Firework>();
		this._rnd = new SRand(this.seed);
	}

	// Token: 0x0600564D RID: 22093 RVA: 0x001C00BC File Offset: 0x001BE2BC
	public void LaunchVolley()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this._rnd.Shuffle<Firework>(this._launchOrder);
		for (int i = 0; i < this._launchOrder.Length; i++)
		{
			MonoBehaviour monoBehaviour = this._launchOrder[i];
			float time = this._rnd.NextFloat() * this.roundLength;
			monoBehaviour.Invoke("Launch", time);
		}
	}

	// Token: 0x0600564E RID: 22094 RVA: 0x001C0120 File Offset: 0x001BE320
	public void LaunchVolleyRound()
	{
		int num = 0;
		while ((long)num < (long)((ulong)this.roundNumVolleys))
		{
			float time = this._rnd.NextFloat() * this.roundLength;
			base.Invoke("LaunchVolley", time);
			num++;
		}
	}

	// Token: 0x0600564F RID: 22095 RVA: 0x001C0164 File Offset: 0x001BE364
	public void Launch(Firework fw)
	{
		if (!fw)
		{
			return;
		}
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		AudioSource sourceOrigin = fw.sourceOrigin;
		int num = this._rnd.NextInt(this.bursts.Length);
		AudioClip audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		AudioClip audioClip2 = this.bursts[num];
		while (this._lastWhistle == audioClip)
		{
			audioClip = this.whistles[this._rnd.NextInt(this.whistles.Length)];
		}
		while (this._lastBurst == audioClip2)
		{
			num = this._rnd.NextInt(this.bursts.Length);
			audioClip2 = this.bursts[num];
		}
		this._lastWhistle = audioClip;
		this._lastBurst = audioClip2;
		int num2 = this._rnd.NextInt(fw.explosions.Length);
		ParticleSystem particleSystem = fw.explosions[num2];
		if (fw.doTrail)
		{
			ParticleSystem trail = fw.trail;
			trail.startColor = fw.colorOrigin;
			trail.subEmitters.GetSubEmitterSystem(0).colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fw.colorOrigin, fw.colorTarget);
			trail.Stop();
			trail.Play();
		}
		sourceOrigin.pitch = this._rnd.NextFloat(0.92f, 1f);
		fw.doTrailAudio = this._rnd.NextBool();
		FireworksController.ExplosionEvent ev = new FireworksController.ExplosionEvent
		{
			firework = fw,
			timeSince = TimeSince.Now(),
			burstIndex = num,
			explosionIndex = num2,
			delay = (double)(fw.doTrail ? audioClip.length : 0f),
			active = true
		};
		if (fw.doExplosion)
		{
			this.PostExplosionEvent(ev);
		}
		if (fw.doTrailAudio && this._timeSinceLastWhistle > this.minWhistleDelay)
		{
			this._timeSinceLastWhistle = TimeSince.Now();
			sourceOrigin.PlayOneShot(audioClip, this._rnd.NextFloat(this.whistleVolumeMin, this.whistleVolumeMax));
		}
		particleSystem.Stop();
		particleSystem.transform.position = position2;
	}

	// Token: 0x06005650 RID: 22096 RVA: 0x001C0394 File Offset: 0x001BE594
	private void PostExplosionEvent(FireworksController.ExplosionEvent ev)
	{
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			if (!this._explosionQueue[i].active)
			{
				this._explosionQueue[i] = ev;
				return;
			}
		}
	}

	// Token: 0x06005651 RID: 22097 RVA: 0x001C03D5 File Offset: 0x001BE5D5
	private void Update()
	{
		this.ProcessEvents();
	}

	// Token: 0x06005652 RID: 22098 RVA: 0x001C03E0 File Offset: 0x001BE5E0
	private void ProcessEvents()
	{
		if (this._explosionQueue == null || this._explosionQueue.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this._explosionQueue.Length; i++)
		{
			FireworksController.ExplosionEvent explosionEvent = this._explosionQueue[i];
			if (explosionEvent.active && explosionEvent.timeSince >= explosionEvent.delay)
			{
				this.DoExplosion(explosionEvent);
				this._explosionQueue[i] = default(FireworksController.ExplosionEvent);
			}
		}
	}

	// Token: 0x06005653 RID: 22099 RVA: 0x001C0454 File Offset: 0x001BE654
	private void DoExplosion(FireworksController.ExplosionEvent ev)
	{
		Firework firework = ev.firework;
		ParticleSystem particleSystem = firework.explosions[ev.explosionIndex];
		ParticleSystem.MinMaxGradient color = new ParticleSystem.MinMaxGradient(firework.colorOrigin, firework.colorTarget);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = particleSystem.subEmitters.GetSubEmitterSystem(0).colorOverLifetime;
		colorOverLifetime.color = color;
		colorOverLifetime2.color = color;
		ParticleSystem particleSystem2 = firework.explosions[ev.explosionIndex];
		particleSystem2.Stop();
		particleSystem2.Play();
		firework.sourceTarget.PlayOneShot(this.bursts[ev.burstIndex]);
	}

	// Token: 0x06005654 RID: 22100 RVA: 0x001C04E4 File Offset: 0x001BE6E4
	public void RenderGizmo(Firework fw, Color c)
	{
		if (!fw)
		{
			return;
		}
		if (!fw.origin || !fw.target)
		{
			return;
		}
		Gizmos.color = c;
		Vector3 position = fw.origin.position;
		Vector3 position2 = fw.target.position;
		Gizmos.DrawLine(position, position2);
		Gizmos.DrawWireCube(position, Vector3.one * 0.5f);
		Gizmos.DrawWireCube(position2, Vector3.one * 0.5f);
	}

	// Token: 0x04006624 RID: 26148
	public Firework[] fireworks;

	// Token: 0x04006625 RID: 26149
	public AudioClip[] whistles;

	// Token: 0x04006626 RID: 26150
	public AudioClip[] bursts;

	// Token: 0x04006627 RID: 26151
	[Space]
	[Range(0f, 1f)]
	public float whistleVolumeMin = 0.1f;

	// Token: 0x04006628 RID: 26152
	[Range(0f, 1f)]
	public float whistleVolumeMax = 0.15f;

	// Token: 0x04006629 RID: 26153
	public float minWhistleDelay = 1f;

	// Token: 0x0400662A RID: 26154
	[Space]
	[NonSerialized]
	private AudioClip _lastWhistle;

	// Token: 0x0400662B RID: 26155
	[NonSerialized]
	private AudioClip _lastBurst;

	// Token: 0x0400662C RID: 26156
	[NonSerialized]
	private Firework[] _launchOrder;

	// Token: 0x0400662D RID: 26157
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x0400662E RID: 26158
	[NonSerialized]
	private FireworksController.ExplosionEvent[] _explosionQueue = new FireworksController.ExplosionEvent[8];

	// Token: 0x0400662F RID: 26159
	[NonSerialized]
	private TimeSince _timeSinceLastWhistle = 10f;

	// Token: 0x04006630 RID: 26160
	[Space]
	public string seed = "Fireworks.Summer23";

	// Token: 0x04006631 RID: 26161
	[Space]
	public uint roundNumVolleys = 6U;

	// Token: 0x04006632 RID: 26162
	public uint roundLength = 6U;

	// Token: 0x04006633 RID: 26163
	[FormerlySerializedAs("_timeOfDayEvent")]
	[FormerlySerializedAs("_timeOfDay")]
	[Space]
	[SerializeField]
	private TimeEvent _fireworksEvent;

	// Token: 0x02000DC0 RID: 3520
	[Serializable]
	public struct ExplosionEvent
	{
		// Token: 0x04006634 RID: 26164
		public TimeSince timeSince;

		// Token: 0x04006635 RID: 26165
		public double delay;

		// Token: 0x04006636 RID: 26166
		public int explosionIndex;

		// Token: 0x04006637 RID: 26167
		public int burstIndex;

		// Token: 0x04006638 RID: 26168
		public bool active;

		// Token: 0x04006639 RID: 26169
		public Firework firework;
	}
}
