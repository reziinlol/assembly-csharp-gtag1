using System;
using System.Collections.Generic;
using CjLib;
using Unity.Collections;
using UnityEngine;

// Token: 0x020007B3 RID: 1971
public class GRNoiseEventManager : MonoBehaviourTick
{
	// Token: 0x0600322C RID: 12844 RVA: 0x00113890 File Offset: 0x00111A90
	public void Awake()
	{
		GRNoiseEventManager.instance = this;
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x00113898 File Offset: 0x00111A98
	public override void Tick()
	{
		this.RemoveExpiredEvents();
		if (GhostReactorManager.noiseDebugEnabled)
		{
			this.RenderDebug();
		}
	}

	// Token: 0x0600322F RID: 12847 RVA: 0x001138AD File Offset: 0x00111AAD
	private int FindUnusedEventEntry()
	{
		return -1;
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x001138B0 File Offset: 0x00111AB0
	public void AddNoiseEvent(Vector3 position, float magnitude = 1f, float duration = 1f)
	{
		GameNoiseEvent gameNoiseEvent = new GameNoiseEvent
		{
			position = position,
			eventTime = Time.timeAsDouble,
			duration = duration,
			magnitude = magnitude
		};
		int num = this.FindUnusedEventEntry();
		if (num == -1)
		{
			this.noiseEvents.Add(gameNoiseEvent);
			return;
		}
		this.noiseEvents[num] = gameNoiseEvent;
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x00113910 File Offset: 0x00111B10
	public List<GameNoiseEvent> GetNoiseEventsInRadius(Vector3 origin, float radius)
	{
		List<GameNoiseEvent> list = new List<GameNoiseEvent>();
		float num = radius * radius;
		foreach (GameNoiseEvent gameNoiseEvent in this.noiseEvents)
		{
			if (gameNoiseEvent.IsValid())
			{
				float sqrMagnitude = (gameNoiseEvent.position - origin).sqrMagnitude;
				float num2 = gameNoiseEvent.magnitude * gameNoiseEvent.magnitude;
				if (sqrMagnitude < num * num2)
				{
					list.Add(gameNoiseEvent);
				}
			}
		}
		return list;
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x001139A4 File Offset: 0x00111BA4
	public bool GetMostRecentNoiseEventInRadius(Vector3 origin, float radius, out GameNoiseEvent outEvent)
	{
		double timeAsDouble = Time.timeAsDouble;
		float num = radius * radius;
		double num2 = -1.0;
		int num3 = -1;
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			GameNoiseEvent gameNoiseEvent = this.noiseEvents[i];
			if (gameNoiseEvent.IsValid())
			{
				float sqrMagnitude = (gameNoiseEvent.position - origin).sqrMagnitude;
				float num4 = gameNoiseEvent.magnitude * gameNoiseEvent.magnitude;
				if (sqrMagnitude < num * num4)
				{
					double num5 = timeAsDouble - gameNoiseEvent.eventTime;
					if (num3 < 0 || num5 < num2)
					{
						num3 = i;
						num2 = num5;
					}
				}
			}
		}
		if (num3 < 0)
		{
			outEvent = default(GameNoiseEvent);
			return false;
		}
		outEvent = this.noiseEvents[num3];
		return true;
	}

	// Token: 0x06003233 RID: 12851 RVA: 0x00113A60 File Offset: 0x00111C60
	public void RenderDebug()
	{
		int num = 0;
		float num2 = 5f;
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			GameNoiseEvent gameNoiseEvent = this.noiseEvents[i];
			if (gameNoiseEvent.IsValid())
			{
				float radius = this.debugMeshScale * gameNoiseEvent.magnitude * num2;
				DebugUtil.DrawSphere(gameNoiseEvent.position, radius, 8, 6, Color.green, true, DebugUtil.Style.Wireframe);
				num++;
			}
		}
	}

	// Token: 0x06003234 RID: 12852 RVA: 0x00113ACC File Offset: 0x00111CCC
	private void RemoveExpiredEvents()
	{
		for (int i = 0; i < this.noiseEvents.Count; i++)
		{
			if (!this.noiseEvents[i].IsValid())
			{
				this.noiseEvents.RemoveAtSwapBack(i);
				i--;
			}
		}
	}

	// Token: 0x0400412D RID: 16685
	private List<GameNoiseEvent> noiseEvents = new List<GameNoiseEvent>();

	// Token: 0x0400412E RID: 16686
	public static GRNoiseEventManager instance;

	// Token: 0x0400412F RID: 16687
	public float debugMeshScale = 1f;
}
