using System;
using System.Collections.Generic;
using GorillaExtensions;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001171 RID: 4465
	public class RigidbodyHighlighter : MonoBehaviour
	{
		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x06007112 RID: 28946 RVA: 0x0024F538 File Offset: 0x0024D738
		private string ButtonText
		{
			get
			{
				if (!this.Active)
				{
					return "Highlight Rigidbodies";
				}
				return "Unhighlight Rigidbodies";
			}
		}

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x06007113 RID: 28947 RVA: 0x0024F54D File Offset: 0x0024D74D
		// (set) Token: 0x06007114 RID: 28948 RVA: 0x0024F555 File Offset: 0x0024D755
		public bool Active { get; set; }

		// Token: 0x06007115 RID: 28949 RVA: 0x0024F560 File Offset: 0x0024D760
		private void Awake()
		{
			Object.Destroy(base.gameObject);
			if (RigidbodyHighlighter.Instance != null && RigidbodyHighlighter.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			RigidbodyHighlighter.Instance = this;
			this._lineRenderer.startWidth = this._lineWidth;
			this._lineRenderer.endWidth = this._lineWidth;
		}

		// Token: 0x06007116 RID: 28950 RVA: 0x0024F5C8 File Offset: 0x0024D7C8
		private void Update()
		{
			if (!this.Active)
			{
				this._lineRenderer.positionCount = 0;
				return;
			}
			this._rigidbodies.Clear();
			this._rigidbodies.AddAll(RigidbodyHighlighter.GetAwakeRigidbodies());
			this.DrawTracers();
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				RigidbodyHighlighter.DrawBox(rigidbody.transform, Color.red, 0.1f);
			}
		}

		// Token: 0x06007117 RID: 28951 RVA: 0x0024F660 File Offset: 0x0024D860
		private static List<Rigidbody> GetAwakeRigidbodies()
		{
			List<Rigidbody> list = new List<Rigidbody>();
			Object[] array = Object.FindObjectsByType(typeof(Rigidbody), FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody rigidbody = array[i] as Rigidbody;
				if (rigidbody == null)
				{
					throw new Exception("Non-rigidbody found by FindObjectsByType.");
				}
				if (!rigidbody.IsSleeping())
				{
					list.Add(rigidbody);
				}
			}
			return list;
		}

		// Token: 0x06007118 RID: 28952 RVA: 0x0024F6B9 File Offset: 0x0024D8B9
		private void HighlightActiveRigidbodies()
		{
			this.Active = !this.Active;
		}

		// Token: 0x06007119 RID: 28953 RVA: 0x0024F6CC File Offset: 0x0024D8CC
		private void GetRigidbodyNames()
		{
			List<Rigidbody> list = (this._rigidbodies.Count > 0) ? this._rigidbodies : RigidbodyHighlighter.GetAwakeRigidbodies();
			for (int i = 0; i < list.Count; i++)
			{
				Debug.Log(string.Format("Rigidbody {0} of {1}: {2}", i, list.Count, list[i].name));
			}
		}

		// Token: 0x0600711A RID: 28954 RVA: 0x0024F734 File Offset: 0x0024D934
		private void OnDrawGizmos()
		{
			if (!this.Active)
			{
				return;
			}
			Gizmos.color = Color.red;
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				Gizmos.DrawWireCube(rigidbody.transform.position, Vector3.one);
			}
		}

		// Token: 0x0600711B RID: 28955 RVA: 0x0024F7A8 File Offset: 0x0024D9A8
		private static void DrawBox(Transform tx, Color color, float duration)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.SetTRS(tx.position, tx.rotation, tx.lossyScale);
			Vector3 vector = matrix4x.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
			Vector3 vector2 = matrix4x.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
			Vector3 vector3 = matrix4x.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
			Vector3 vector4 = matrix4x.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
			Vector3 vector5 = matrix4x.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
			Vector3 vector6 = matrix4x.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
			Vector3 vector7 = matrix4x.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
			Vector3 vector8 = matrix4x.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
			Debug.DrawLine(vector, vector2, color, duration, false);
			Debug.DrawLine(vector2, vector4, color, duration, false);
			Debug.DrawLine(vector4, vector3, color, duration, false);
			Debug.DrawLine(vector3, vector, color, duration, false);
			Debug.DrawLine(vector8, vector7, color, duration, false);
			Debug.DrawLine(vector7, vector5, color, duration, false);
			Debug.DrawLine(vector5, vector6, color, duration, false);
			Debug.DrawLine(vector6, vector8, color, duration, false);
			Debug.DrawLine(vector, vector5, color, duration, false);
			Debug.DrawLine(vector2, vector6, color, duration, false);
			Debug.DrawLine(vector3, vector7, color, duration, false);
			Debug.DrawLine(vector4, vector8, color, duration, false);
		}

		// Token: 0x0600711C RID: 28956 RVA: 0x0024F944 File Offset: 0x0024DB44
		private void DrawTracers()
		{
			Vector3[] array = new Vector3[this._rigidbodies.Count * 2 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((i % 2 == 0) ? (Camera.main.transform.position + this._tracerOffset) : this._rigidbodies[i / 2].transform.position);
			}
			this._lineRenderer.positionCount = array.Length;
			this._lineRenderer.SetPositions(array);
		}

		// Token: 0x0400812A RID: 33066
		[CanBeNull]
		public static RigidbodyHighlighter Instance;

		// Token: 0x0400812B RID: 33067
		[SerializeField]
		private float _inGameDuration = 10f;

		// Token: 0x0400812C RID: 33068
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x0400812D RID: 33069
		[SerializeField]
		private float _lineWidth = 0.01f;

		// Token: 0x0400812E RID: 33070
		[SerializeField]
		private Vector3 _tracerOffset = 0.5f * Vector3.down;

		// Token: 0x04008130 RID: 33072
		private readonly List<Rigidbody> _rigidbodies = new List<Rigidbody>();
	}
}
