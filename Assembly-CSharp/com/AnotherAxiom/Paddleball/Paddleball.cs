using System;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x020010C8 RID: 4296
	public class Paddleball : ArcadeGame
	{
		// Token: 0x06006B99 RID: 27545 RVA: 0x0022D3C6 File Offset: 0x0022B5C6
		protected override void Awake()
		{
			base.Awake();
			this.yPosToByteFactor = 255f / (2f * this.tableSizeBall.y);
			this.byteToYPosFactor = 1f / this.yPosToByteFactor;
		}

		// Token: 0x06006B9A RID: 27546 RVA: 0x0022D400 File Offset: 0x0022B600
		private void Start()
		{
			this.whiteWinScreen.SetActive(false);
			this.blackWinScreen.SetActive(false);
			this.titleScreen.SetActive(true);
			this.ball.gameObject.SetActive(false);
			this.currentScreenMode = Paddleball.ScreenMode.Title;
			this.paddleIdle = new float[this.p.Length];
			for (int i = 0; i < this.p.Length; i++)
			{
				this.p[i].gameObject.SetActive(false);
				this.paddleIdle[i] = 30f;
			}
			this.gameBallSpeed = this.initialBallSpeed;
			this.scoreR = (this.scoreL = 0);
			this.scoreFormat = this.scoreDisplay.text;
			this.UpdateScore();
		}

		// Token: 0x06006B9B RID: 27547 RVA: 0x0022D4C4 File Offset: 0x0022B6C4
		private void Update()
		{
			if (this.currentScreenMode == Paddleball.ScreenMode.Gameplay)
			{
				this.ball.Translate(this.ballTrajectory.normalized * Time.deltaTime * this.gameBallSpeed);
				if (this.ball.localPosition.y > this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.y < -this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, -this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.x > this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreL++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreL >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.WhiteWin);
					}
				}
				if (this.ball.localPosition.x < -this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(-this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreR++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreR >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.BlackWin);
					}
				}
			}
			if (this.returnToTitleAfterTimestamp != 0f && Time.time > this.returnToTitleAfterTimestamp)
			{
				this.ChangeScreen(Paddleball.ScreenMode.Title);
			}
			for (int i = 0; i < this.p.Length; i++)
			{
				if (base.IsPlayerLocallyControlled(i))
				{
					float num = this.requestedPos[i];
					if (base.getButtonState(i, ArcadeButtons.UP))
					{
						this.requestedPos[i] += Time.deltaTime * this.paddleSpeed;
					}
					else if (base.getButtonState(i, ArcadeButtons.DOWN))
					{
						this.requestedPos[i] -= Time.deltaTime * this.paddleSpeed;
					}
					this.requestedPos[i] = Mathf.Clamp(this.requestedPos[i], -this.tableSizePaddle.y, this.tableSizePaddle.y);
				}
				float value;
				if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
				{
					value = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.requestedPos[i], Time.deltaTime * this.paddleSpeed);
				}
				else
				{
					value = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.officialPos[i], Time.deltaTime * this.paddleSpeed);
				}
				this.p[i].transform.localPosition = this.p[i].transform.localPosition.WithY(Mathf.Clamp(value, -this.tableSizePaddle.y, this.tableSizePaddle.y));
				if (base.getButtonState(i, ArcadeButtons.GRAB))
				{
					this.paddleIdle[i] = 0f;
					Paddleball.ScreenMode screenMode = this.currentScreenMode;
					if (screenMode != Paddleball.ScreenMode.Title)
					{
						if (screenMode == Paddleball.ScreenMode.Gameplay)
						{
							this.returnToTitleAfterTimestamp = Time.time + 30f;
						}
					}
					else
					{
						this.ChangeScreen(Paddleball.ScreenMode.Gameplay);
					}
				}
				else
				{
					this.paddleIdle[i] += Time.deltaTime;
				}
				bool flag = this.paddleIdle[i] < 30f;
				if (this.p[i].gameObject.activeSelf != flag)
				{
					if (flag)
					{
						base.PlaySound(4, 3);
						Vector3 localPosition = this.p[i].transform.localPosition;
						localPosition.y = 0f;
						this.requestedPos[i] = localPosition.y;
						this.p[i].transform.localPosition = localPosition;
					}
					this.p[i].gameObject.SetActive(this.paddleIdle[i] < 30f);
				}
				if (this.p[i].gameObject.activeInHierarchy && Mathf.Abs(this.ball.localPosition.x - this.p[i].transform.localPosition.x) < 0.1f && Mathf.Abs(this.ball.localPosition.y - this.p[i].transform.localPosition.y) < 0.5f)
				{
					this.ballTrajectory.y = (this.ball.localPosition.y - this.p[i].transform.localPosition.y) * 1.25f;
					float x = this.ballTrajectory.x;
					if (this.p[i].Right)
					{
						this.ballTrajectory.x = Mathf.Abs(this.ballTrajectory.y) - 1f;
					}
					else
					{
						this.ballTrajectory.x = 1f - Mathf.Abs(this.ballTrajectory.y);
					}
					if (x > 0f != this.ballTrajectory.x > 0f)
					{
						base.PlaySound(1, 3);
					}
					this.ballTrajectory.Normalize();
					this.gameBallSpeed += this.ballSpeedBoost;
				}
			}
		}

		// Token: 0x06006B9C RID: 27548 RVA: 0x0022DB2C File Offset: 0x0022BD2C
		private void UpdateScore()
		{
			if (this.scoreFormat == null)
			{
				return;
			}
			this.scoreL = Mathf.Clamp(this.scoreL, 0, 10);
			this.scoreR = Mathf.Clamp(this.scoreR, 0, 10);
			this.scoreDisplay.text = string.Format(this.scoreFormat, this.scoreL, this.scoreR);
		}

		// Token: 0x06006B9D RID: 27549 RVA: 0x0022DB96 File Offset: 0x0022BD96
		private float ByteToYPos(byte Y)
		{
			return (float)Y / this.yPosToByteFactor - this.tableSizeBall.y;
		}

		// Token: 0x06006B9E RID: 27550 RVA: 0x0022DBAD File Offset: 0x0022BDAD
		private byte YPosToByte(float Y)
		{
			return (byte)Mathf.RoundToInt((Y + this.tableSizeBall.y) * this.yPosToByteFactor);
		}

		// Token: 0x06006B9F RID: 27551 RVA: 0x0022DBCC File Offset: 0x0022BDCC
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P0LocY = this.YPosToByte(this.p[0].transform.localPosition.y);
			this.netStateCur.P1LocY = this.YPosToByte(this.p[1].transform.localPosition.y);
			this.netStateCur.P2LocY = this.YPosToByte(this.p[2].transform.localPosition.y);
			this.netStateCur.P3LocY = this.YPosToByte(this.p[3].transform.localPosition.y);
			this.netStateCur.BallLocX = this.ball.localPosition.x;
			this.netStateCur.BallLocY = this.YPosToByte(this.ball.localPosition.y);
			this.netStateCur.BallTrajectoryX = (byte)((this.ballTrajectory.x + 1f) * 127.5f);
			this.netStateCur.BallTrajectoryY = (byte)((this.ballTrajectory.y + 1f) * 127.5f);
			this.netStateCur.BallSpeed = this.gameBallSpeed;
			this.netStateCur.ScoreLeft = this.scoreL;
			this.netStateCur.ScoreRight = this.scoreR;
			this.netStateCur.ScreenMode = (int)this.currentScreenMode;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06006BA0 RID: 27552 RVA: 0x0022DD80 File Offset: 0x0022BF80
		public override void SetNetworkState(byte[] b)
		{
			Paddleball.PaddleballNetState paddleballNetState = (Paddleball.PaddleballNetState)ArcadeGame.UnwrapNetState(b);
			this.officialPos[0] = this.ByteToYPos(paddleballNetState.P0LocY);
			this.officialPos[1] = this.ByteToYPos(paddleballNetState.P1LocY);
			this.officialPos[2] = this.ByteToYPos(paddleballNetState.P2LocY);
			this.officialPos[3] = this.ByteToYPos(paddleballNetState.P3LocY);
			Vector2 vector = new Vector2(paddleballNetState.BallLocX, this.ByteToYPos(paddleballNetState.BallLocY));
			Vector2 normalized = new Vector2((float)paddleballNetState.BallTrajectoryX * 0.007843138f - 1f, (float)paddleballNetState.BallTrajectoryY * 0.007843138f - 1f).normalized;
			Vector2 a = vector - normalized * Vector2.Dot(vector, normalized);
			Vector2 vector2 = this.ball.localPosition.xy();
			Vector2 b2 = vector2 - this.ballTrajectory * Vector2.Dot(vector2, this.ballTrajectory);
			if ((a - b2).IsLongerThan(0.1f))
			{
				this.ball.localPosition = vector;
				this.ballTrajectory = normalized.xy();
			}
			this.gameBallSpeed = paddleballNetState.BallSpeed;
			this.ChangeScreen((Paddleball.ScreenMode)paddleballNetState.ScreenMode);
			if (this.scoreL != paddleballNetState.ScoreLeft || this.scoreR != paddleballNetState.ScoreRight)
			{
				this.scoreL = paddleballNetState.ScoreLeft;
				this.scoreR = paddleballNetState.ScoreRight;
				this.UpdateScore();
			}
		}

		// Token: 0x06006BA1 RID: 27553 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06006BA2 RID: 27554 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06006BA3 RID: 27555 RVA: 0x0022DEFC File Offset: 0x0022C0FC
		private void ChangeScreen(Paddleball.ScreenMode mode)
		{
			if (this.currentScreenMode == mode)
			{
				return;
			}
			switch (this.currentScreenMode)
			{
			case Paddleball.ScreenMode.Title:
				this.titleScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(false);
				break;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(false);
				break;
			}
			this.currentScreenMode = mode;
			switch (mode)
			{
			case Paddleball.ScreenMode.Title:
				this.gameBallSpeed = this.initialBallSpeed;
				this.scoreL = 0;
				this.scoreR = 0;
				this.UpdateScore();
				this.returnToTitleAfterTimestamp = 0f;
				this.titleScreen.SetActive(true);
				return;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + 30f;
				return;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006BA4 RID: 27556 RVA: 0x0022E033 File Offset: 0x0022C233
		public override void OnTimeout()
		{
			this.ChangeScreen(Paddleball.ScreenMode.Title);
		}

		// Token: 0x06006BA5 RID: 27557 RVA: 0x0022E03C File Offset: 0x0022C23C
		public override void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			this.requestedPos[player] = this.ByteToYPos((byte)stream.ReceiveNext());
		}

		// Token: 0x06006BA6 RID: 27558 RVA: 0x0022E057 File Offset: 0x0022C257
		public override void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.YPosToByte(this.requestedPos[player]));
		}

		// Token: 0x04007BB8 RID: 31672
		[SerializeField]
		private PaddleballPaddle[] p;

		// Token: 0x04007BB9 RID: 31673
		private float[] requestedPos = new float[4];

		// Token: 0x04007BBA RID: 31674
		private float[] officialPos = new float[4];

		// Token: 0x04007BBB RID: 31675
		[SerializeField]
		private Transform ball;

		// Token: 0x04007BBC RID: 31676
		[SerializeField]
		private Vector2 ballTrajectory;

		// Token: 0x04007BBD RID: 31677
		[SerializeField]
		private float paddleSpeed = 1f;

		// Token: 0x04007BBE RID: 31678
		[SerializeField]
		private float initialBallSpeed = 1f;

		// Token: 0x04007BBF RID: 31679
		[SerializeField]
		private float ballSpeedBoost = 0.02f;

		// Token: 0x04007BC0 RID: 31680
		private float gameBallSpeed = 1f;

		// Token: 0x04007BC1 RID: 31681
		[SerializeField]
		private Vector2 tableSizeBall;

		// Token: 0x04007BC2 RID: 31682
		[SerializeField]
		private Vector2 tableSizePaddle;

		// Token: 0x04007BC3 RID: 31683
		[SerializeField]
		private GameObject blackWinScreen;

		// Token: 0x04007BC4 RID: 31684
		[SerializeField]
		private GameObject whiteWinScreen;

		// Token: 0x04007BC5 RID: 31685
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x04007BC6 RID: 31686
		[SerializeField]
		private float winScreenDuration;

		// Token: 0x04007BC7 RID: 31687
		private float returnToTitleAfterTimestamp;

		// Token: 0x04007BC8 RID: 31688
		private int scoreL;

		// Token: 0x04007BC9 RID: 31689
		private int scoreR;

		// Token: 0x04007BCA RID: 31690
		private string scoreFormat;

		// Token: 0x04007BCB RID: 31691
		[SerializeField]
		private TMP_Text scoreDisplay;

		// Token: 0x04007BCC RID: 31692
		private float[] paddleIdle;

		// Token: 0x04007BCD RID: 31693
		private Paddleball.ScreenMode currentScreenMode;

		// Token: 0x04007BCE RID: 31694
		private const int AUDIO_WALLBOUNCE = 0;

		// Token: 0x04007BCF RID: 31695
		private const int AUDIO_PADDLEBOUNCE = 1;

		// Token: 0x04007BD0 RID: 31696
		private const int AUDIO_SCORE = 2;

		// Token: 0x04007BD1 RID: 31697
		private const int AUDIO_WIN = 3;

		// Token: 0x04007BD2 RID: 31698
		private const int AUDIO_PLAYERJOIN = 4;

		// Token: 0x04007BD3 RID: 31699
		private const int VAR_REQUESTEDPOS = 0;

		// Token: 0x04007BD4 RID: 31700
		private const int MAXSCORE = 10;

		// Token: 0x04007BD5 RID: 31701
		private float yPosToByteFactor;

		// Token: 0x04007BD6 RID: 31702
		private float byteToYPosFactor;

		// Token: 0x04007BD7 RID: 31703
		private const float directionToByteFactor = 127.5f;

		// Token: 0x04007BD8 RID: 31704
		private const float byteToDirectionFactor = 0.007843138f;

		// Token: 0x04007BD9 RID: 31705
		private Paddleball.PaddleballNetState netStateLast;

		// Token: 0x04007BDA RID: 31706
		private Paddleball.PaddleballNetState netStateCur;

		// Token: 0x020010C9 RID: 4297
		private enum ScreenMode
		{
			// Token: 0x04007BDC RID: 31708
			Title,
			// Token: 0x04007BDD RID: 31709
			Gameplay,
			// Token: 0x04007BDE RID: 31710
			WhiteWin,
			// Token: 0x04007BDF RID: 31711
			BlackWin
		}

		// Token: 0x020010CA RID: 4298
		[Serializable]
		private struct PaddleballNetState : IEquatable<Paddleball.PaddleballNetState>
		{
			// Token: 0x06006BA8 RID: 27560 RVA: 0x0022E0CC File Offset: 0x0022C2CC
			public bool Equals(Paddleball.PaddleballNetState other)
			{
				return this.P0LocY == other.P0LocY && this.P1LocY == other.P1LocY && this.P2LocY == other.P2LocY && this.P3LocY == other.P3LocY && this.BallLocX.Approx(other.BallLocX, 1E-06f) && this.BallLocY == other.BallLocY && this.BallTrajectoryX == other.BallTrajectoryX && this.BallTrajectoryY == other.BallTrajectoryY && this.BallSpeed.Approx(other.BallSpeed, 1E-06f) && this.ScoreLeft == other.ScoreLeft && this.ScoreRight == other.ScoreRight && this.ScreenMode == other.ScreenMode;
			}

			// Token: 0x04007BE0 RID: 31712
			public byte P0LocY;

			// Token: 0x04007BE1 RID: 31713
			public byte P1LocY;

			// Token: 0x04007BE2 RID: 31714
			public byte P2LocY;

			// Token: 0x04007BE3 RID: 31715
			public byte P3LocY;

			// Token: 0x04007BE4 RID: 31716
			public float BallLocX;

			// Token: 0x04007BE5 RID: 31717
			public byte BallLocY;

			// Token: 0x04007BE6 RID: 31718
			public byte BallTrajectoryX;

			// Token: 0x04007BE7 RID: 31719
			public byte BallTrajectoryY;

			// Token: 0x04007BE8 RID: 31720
			public float BallSpeed;

			// Token: 0x04007BE9 RID: 31721
			public int ScoreLeft;

			// Token: 0x04007BEA RID: 31722
			public int ScoreRight;

			// Token: 0x04007BEB RID: 31723
			public int ScreenMode;
		}
	}
}
