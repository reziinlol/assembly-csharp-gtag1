using System;
using System.Globalization;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200075A RID: 1882
public class GRDistillery : MonoBehaviour
{
	// Token: 0x06002FB1 RID: 12209 RVA: 0x00102FE0 File Offset: 0x001011E0
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		this.sentientCoreDeposit.Init(reactor);
		this.cores = PlayerPrefs.GetInt("_grDistilleryCore", -1);
		if (this.cores == -1)
		{
			this.cores = 0;
		}
		this.RestoreStartTime();
		this.InitializeGauges();
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x00103030 File Offset: 0x00101230
	private void SaveStartTime(DateTime time)
	{
		string value = time.ToString("O");
		PlayerPrefs.SetString("_grDistilleryStartTime", value);
		PlayerPrefs.Save();
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x0010305C File Offset: 0x0010125C
	private void RestoreStartTime()
	{
		string @string = PlayerPrefs.GetString("_grDistilleryStartTime", string.Empty);
		if (@string != string.Empty)
		{
			this.startTime = DateTime.ParseExact(@string, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x001030A1 File Offset: 0x001012A1
	public void StartResearch()
	{
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime();
			this.SaveStartTime(this.startTime);
			this.bProcessing = true;
			this.InitializeGauges();
		}
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x001030D8 File Offset: 0x001012D8
	public double CalculateRemaining()
	{
		return (double)this.secondsToResearchACore - (GorillaComputer.instance.GetServerTime() - this.startTime).TotalSeconds;
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x0010310C File Offset: 0x0010130C
	private void FirstUpdate()
	{
		double num = this.CalculateRemaining();
		while (this.cores > 0 && num < (double)(-(double)this.secondsToResearchACore))
		{
			if (num < (double)(-(double)this.secondsToResearchACore))
			{
				this.CompleteResearchingCore();
				num += (double)this.secondsToResearchACore;
			}
		}
		if (this.cores > 0 && num < 0.0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(num);
			num = this.CalculateRemaining();
			this.SaveStartTime(this.startTime);
		}
		if (this.cores > 0)
		{
			this.bProcessing = true;
			this.currentGaugeCore = this.cores - 1;
		}
		else
		{
			this.currentGaugeCore = 0;
		}
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = this.depositClosePosition.position;
		}
		else
		{
			this.depositDoor.transform.position = this.depositOpenPosition.position;
		}
		this.UpdateGauges();
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x00103204 File Offset: 0x00101404
	public void Update()
	{
		if (!this.firstUpdate)
		{
			this.FirstUpdate();
			this.firstUpdate = true;
		}
		this.UpdateDoorPosition();
		this.UpdateGauges();
		if (!this.bProcessing)
		{
			return;
		}
		this.remaingTime = this.CalculateRemaining();
		if (this.remaingTime <= 0.0)
		{
			this.CompleteResearchingCore();
		}
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x00103260 File Offset: 0x00101460
	private void UpdateDoorPosition()
	{
		if (this.cores >= 4)
		{
			this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositClosePosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
			return;
		}
		this.depositDoor.transform.position = Vector3.MoveTowards(this.depositDoor.transform.position, this.depositOpenPosition.transform.position, this.depositDoorCloseSpeed * Time.deltaTime);
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x001032FC File Offset: 0x001014FC
	private void CompleteResearchingCore()
	{
		this.cores = Math.Max(this.cores - 1, 0);
		this.currentGaugeCore = Math.Max(this.cores - 1, 0);
		PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
		PlayerPrefs.Save();
		if (this.cores > 0)
		{
			this.startTime = GorillaComputer.instance.GetServerTime().AddSeconds(this.remaingTime);
			this.SaveStartTime(this.startTime);
			this.remaingTime = this.CalculateRemaining();
		}
		if (this.cores == 0)
		{
			this.bProcessing = false;
		}
		this.UpdateGauges();
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x0010339C File Offset: 0x0010159C
	public void DepositCore()
	{
		if (this.cores < this.maxCores)
		{
			this.cores++;
			if (!this.bFillingGauge)
			{
				this.bFillingGauge = true;
				this.fillTime = 0f;
			}
			PlayerPrefs.SetInt("_grDistilleryCore", this.cores);
			PlayerPrefs.Save();
			if (this.cores == 1)
			{
				this.StartResearch();
			}
		}
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DebugFinishDistill()
	{
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x00103404 File Offset: 0x00101604
	private void OnEnable()
	{
		if (this._applyMaterialgauge1)
		{
			this._applyMaterialgauge1.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge2)
		{
			this._applyMaterialgauge2.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge3)
		{
			this._applyMaterialgauge3.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		if (this._applyMaterialgauge4)
		{
			this._applyMaterialgauge4.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
		this.InitializeGauges();
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x0010347C File Offset: 0x0010167C
	private void InitializeGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length - 1; i++)
		{
			this.gaugesFill[i] = ((this.cores >= i + 1) ? this.gaugeFullFillAmount : this.gaugeEmptyFillAmount);
		}
		this.researchGaugeFill = this.gaugesFill[0];
		this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x001034DC File Offset: 0x001016DC
	private void UpdateGauges()
	{
		for (int i = 0; i < this.gaugesFill.Length; i++)
		{
			if (i + 1 > this.cores)
			{
				this.gaugesFill[i] = this.gaugeEmptyFillAmount;
			}
		}
		if (this.bFillingGauge)
		{
			this.fillTime += Time.deltaTime;
			float num = this.fillTime / this.gaugeDrainTime;
			if (this.currentGaugeCore == this.cores - 1)
			{
				if (num > 1f)
				{
					this.bFillingGauge = false;
				}
				else
				{
					this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore), num);
				}
			}
			else
			{
				this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.currentGaugeFillAmount, this.gaugeFullFillAmount, num);
			}
			if (this.bFillingGauge && num > 1f)
			{
				this.currentGaugeCore++;
				this.currentGaugeFillAmount = this.gaugeEmptyFillAmount;
				this.fillTime = 0f;
			}
		}
		else if (this.bProcessing)
		{
			this.gaugesFill[this.currentGaugeCore] = Mathf.Lerp(this.gaugeEmptyFillAmount, this.gaugeFullFillAmount, (float)this.remaingTime / (float)this.secondsToResearchACore);
			this.currentGaugeFillAmount = this.gaugesFill[this.currentGaugeCore];
		}
		this._applyMaterialgauge1.SetFloat("_LiquidFill", this.gaugesFill[0]);
		this._applyMaterialgauge1.Apply();
		this._applyMaterialgauge2.SetFloat("_LiquidFill", this.gaugesFill[1]);
		this._applyMaterialgauge2.Apply();
		this._applyMaterialgauge3.SetFloat("_LiquidFill", this.gaugesFill[2]);
		this._applyMaterialgauge3.Apply();
		this._applyMaterialgauge4.SetFloat("_LiquidFill", this.gaugesFill[3]);
		this._applyMaterialgauge4.Apply();
		this._applyMaterialCurrentResearch.SetFloat("_LiquidFill", this.researchGaugeFill);
		this._applyMaterialCurrentResearch.Apply();
	}

	// Token: 0x04003D0E RID: 15630
	[SerializeField]
	private GRCurrencyDepositor sentientCoreDeposit;

	// Token: 0x04003D0F RID: 15631
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge1;

	// Token: 0x04003D10 RID: 15632
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge2;

	// Token: 0x04003D11 RID: 15633
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge3;

	// Token: 0x04003D12 RID: 15634
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialgauge4;

	// Token: 0x04003D13 RID: 15635
	[SerializeField]
	private ApplyMaterialProperty _applyMaterialCurrentResearch;

	// Token: 0x04003D14 RID: 15636
	[FormerlySerializedAs("emptyFillAmount")]
	public float gaugeEmptyFillAmount = 0.44f;

	// Token: 0x04003D15 RID: 15637
	[FormerlySerializedAs("fullFillAmount")]
	public float gaugeFullFillAmount = 0.56f;

	// Token: 0x04003D16 RID: 15638
	[SerializeField]
	private Transform depositClosePosition;

	// Token: 0x04003D17 RID: 15639
	[SerializeField]
	private Transform depositOpenPosition;

	// Token: 0x04003D18 RID: 15640
	[SerializeField]
	private GameObject depositDoor;

	// Token: 0x04003D19 RID: 15641
	[SerializeField]
	private float depositDoorCloseSpeed = 0.5f;

	// Token: 0x04003D1A RID: 15642
	[SerializeField]
	private TextMeshPro currentResearchPoints;

	// Token: 0x04003D1B RID: 15643
	public float researchGaugeEmptyFillAmount = 0.44f;

	// Token: 0x04003D1C RID: 15644
	public float researchGaugeFullFillAmount = 0.56f;

	// Token: 0x04003D1D RID: 15645
	public int secondsToResearchACore;

	// Token: 0x04003D1E RID: 15646
	public float gaugeDrainTime = 2f;

	// Token: 0x04003D1F RID: 15647
	public int maxCores = 4;

	// Token: 0x04003D20 RID: 15648
	public AudioSource feedbackSound;

	// Token: 0x04003D21 RID: 15649
	private DateTime startTime;

	// Token: 0x04003D22 RID: 15650
	private bool bProcessing;

	// Token: 0x04003D23 RID: 15651
	private int cores;

	// Token: 0x04003D24 RID: 15652
	private bool bFillingGauge;

	// Token: 0x04003D25 RID: 15653
	private int currentGaugeCore;

	// Token: 0x04003D26 RID: 15654
	private float currentGaugeFillAmount;

	// Token: 0x04003D27 RID: 15655
	private double remaingTime;

	// Token: 0x04003D28 RID: 15656
	private float fillTime;

	// Token: 0x04003D29 RID: 15657
	private float[] gaugesFill = new float[4];

	// Token: 0x04003D2A RID: 15658
	private float researchGaugeFill;

	// Token: 0x04003D2B RID: 15659
	private bool firstUpdate;

	// Token: 0x04003D2C RID: 15660
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x04003D2D RID: 15661
	private const string grDistilleryCorePrefsKey = "_grDistilleryCore";

	// Token: 0x04003D2E RID: 15662
	private const string grDistilleryStartTimePrefsKey = "_grDistilleryStartTime";
}
