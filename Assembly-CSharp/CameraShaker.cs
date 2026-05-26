using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class CameraShaker : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060000C9 RID: 201 RVA: 0x000056E0 File Offset: 0x000038E0
	// (remove) Token: 0x060000CA RID: 202 RVA: 0x00005714 File Offset: 0x00003914
	private static event Action<float, float, Vector2, bool, Transform, float> ShakeRequested;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060000CB RID: 203 RVA: 0x00005748 File Offset: 0x00003948
	// (remove) Token: 0x060000CC RID: 204 RVA: 0x0000577C File Offset: 0x0000397C
	private static event Action HaltRequested;

	// Token: 0x060000CD RID: 205 RVA: 0x000057AF File Offset: 0x000039AF
	public static void Shake(float duration, float magnitude)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, new Vector2(0.02f, 0.1f), true, null, 0f);
		}
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000057DA File Offset: 0x000039DA
	public static void Shake(float duration, float magnitude, Vector2 freqRange)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, true, null, 0f);
		}
	}

	// Token: 0x060000CF RID: 207 RVA: 0x000057F7 File Offset: 0x000039F7
	public static void Shake(float duration, float magnitude, Vector2 freqRange, bool rollOffOverDuration)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, rollOffOverDuration, null, 0f);
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00005814 File Offset: 0x00003A14
	public static void ShakeInProximity(float duration, float magnitude, Vector2 freqRange, bool rollOffOverDuration, Transform source, float distance)
	{
		if (CameraShaker.ShakeRequested != null)
		{
			CameraShaker.ShakeRequested(duration, magnitude, freqRange, rollOffOverDuration, source, distance);
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x0000582F File Offset: 0x00003A2F
	public static void Halt()
	{
		if (CameraShaker.HaltRequested != null)
		{
			CameraShaker.HaltRequested();
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00005842 File Offset: 0x00003A42
	private void OnEnable()
	{
		CameraShaker.ShakeRequested += this._ShakeRequested;
		CameraShaker.HaltRequested += this._HaltRequested;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00005868 File Offset: 0x00003A68
	private void _ShakeRequested(float _duration, float _magnitude, Vector2 _freqRange, bool _rollOff, Transform source, float distance)
	{
		this.stopTime = Time.time + _duration;
		this.duration = _duration;
		this.magnitude = _magnitude;
		this.freqRange = _freqRange;
		this.rollOff = _rollOff;
		if (!this.rumbling && (source == null || (base.transform.position - source.transform.position).sqrMagnitude < distance * distance))
		{
			base.StartCoroutine(this.crRumble());
		}
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x000058E9 File Offset: 0x00003AE9
	private void _HaltRequested()
	{
		this.stopTime = Time.time;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x000058F6 File Offset: 0x00003AF6
	private void OnDisable()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
		CameraShaker.HaltRequested -= this._HaltRequested;
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x000058F6 File Offset: 0x00003AF6
	private void OnDestroy()
	{
		CameraShaker.ShakeRequested -= this._ShakeRequested;
		CameraShaker.HaltRequested -= this._HaltRequested;
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x0000591A File Offset: 0x00003B1A
	private IEnumerator crRumble()
	{
		this.rumbling = true;
		while (this.stopTime > Time.time)
		{
			Vector3 vector = Random.insideUnitSphere * this.magnitude;
			if (this.rollOff)
			{
				vector *= (this.stopTime - Time.time) / this.duration;
			}
			base.transform.localPosition += vector;
			yield return new WaitForSeconds(Random.Range(this.freqRange.x, this.freqRange.y));
		}
		this.rumbling = false;
		yield break;
	}

	// Token: 0x040000E0 RID: 224
	private bool rumbling;

	// Token: 0x040000E1 RID: 225
	private float stopTime;

	// Token: 0x040000E4 RID: 228
	private bool rollOff;

	// Token: 0x040000E5 RID: 229
	private float magnitude;

	// Token: 0x040000E6 RID: 230
	private float duration;

	// Token: 0x040000E7 RID: 231
	private Vector2 freqRange;
}
