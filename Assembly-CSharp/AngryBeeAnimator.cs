using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class AngryBeeAnimator : MonoBehaviour
{
	// Token: 0x06000E05 RID: 3589 RVA: 0x0004C8D0 File Offset: 0x0004AAD0
	private void Awake()
	{
		this.bees = new GameObject[this.numBees];
		this.beeOrbits = new GameObject[this.numBees];
		this.beeOrbitalRadii = new float[this.numBees];
		this.beeOrbitalAxes = new Vector3[this.numBees];
		for (int i = 0; i < this.numBees; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			Vector2 vector = Random.insideUnitCircle * this.orbitMaxCenterDisplacement;
			gameObject.transform.localPosition = new Vector3(vector.x, Random.Range(-this.orbitMaxHeightDisplacement, this.orbitMaxHeightDisplacement), vector.y);
			gameObject.transform.localRotation = Quaternion.Euler(Random.Range(-this.orbitMaxTilt, this.orbitMaxTilt), (float)Random.Range(0, 360), 0f);
			this.beeOrbitalAxes[i] = gameObject.transform.up;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.beePrefab, gameObject.transform);
			float num = Random.Range(this.orbitMinRadius, this.orbitMaxRadius);
			this.beeOrbitalRadii[i] = num;
			gameObject2.transform.localPosition = Vector3.forward * num;
			gameObject2.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
			gameObject2.transform.localScale = Vector3.one * this.beeScale;
			this.bees[i] = gameObject2;
			this.beeOrbits[i] = gameObject;
		}
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0004CA6C File Offset: 0x0004AC6C
	private void Update()
	{
		float angle = this.orbitSpeed * Time.deltaTime;
		for (int i = 0; i < this.numBees; i++)
		{
			this.beeOrbits[i].transform.Rotate(this.beeOrbitalAxes[i], angle);
		}
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0004CAB8 File Offset: 0x0004ACB8
	public void SetEmergeFraction(float fraction)
	{
		for (int i = 0; i < this.numBees; i++)
		{
			this.bees[i].transform.localPosition = Vector3.forward * fraction * this.beeOrbitalRadii[i];
		}
	}

	// Token: 0x040010D3 RID: 4307
	[SerializeField]
	private GameObject beePrefab;

	// Token: 0x040010D4 RID: 4308
	[SerializeField]
	private int numBees;

	// Token: 0x040010D5 RID: 4309
	[SerializeField]
	private float orbitMinRadius;

	// Token: 0x040010D6 RID: 4310
	[SerializeField]
	private float orbitMaxRadius;

	// Token: 0x040010D7 RID: 4311
	[SerializeField]
	private float orbitMaxHeightDisplacement;

	// Token: 0x040010D8 RID: 4312
	[SerializeField]
	private float orbitMaxCenterDisplacement;

	// Token: 0x040010D9 RID: 4313
	[SerializeField]
	private float orbitMaxTilt;

	// Token: 0x040010DA RID: 4314
	[SerializeField]
	private float orbitSpeed;

	// Token: 0x040010DB RID: 4315
	[SerializeField]
	private float beeScale;

	// Token: 0x040010DC RID: 4316
	private GameObject[] beeOrbits;

	// Token: 0x040010DD RID: 4317
	private GameObject[] bees;

	// Token: 0x040010DE RID: 4318
	private Vector3[] beeOrbitalAxes;

	// Token: 0x040010DF RID: 4319
	private float[] beeOrbitalRadii;
}
