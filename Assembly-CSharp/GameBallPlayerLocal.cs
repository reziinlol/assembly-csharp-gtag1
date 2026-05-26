using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005EF RID: 1519
public class GameBallPlayerLocal : MonoBehaviour
{
	// Token: 0x060025D5 RID: 9685 RVA: 0x000C85C8 File Offset: 0x000C67C8
	private void Awake()
	{
		GameBallPlayerLocal.instance = this;
		this.hands = new GameBallPlayerLocal.HandData[2];
		this.inputData = new GameBallPlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GameBallPlayerLocal.InputData(32);
		}
		Application.quitting += GameBallPlayerLocal._OnApplicationQuit;
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000C8628 File Offset: 0x000C6828
	private static void _OnApplicationQuit()
	{
		if (MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000C8641 File Offset: 0x000C6841
	private void OnApplicationPause(bool pause)
	{
		if (pause && MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000C865D File Offset: 0x000C685D
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x000C8680 File Offset: 0x000C6880
	public void OnUpdateInteract()
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(j);
		}
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000C86CC File Offset: 0x000C68CC
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GameBallPlayerLocal.InputDataMotion data = default(GameBallPlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out data.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out data.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out data.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out data.angVelocity);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x000C8758 File Offset: 0x000C6958
	private void UpdateHand(int handIndex)
	{
		if (GameBallManager.Instance == null)
		{
			return;
		}
		if (!this.gamePlayer.GetGameBallId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000C879C File Offset: 0x000C699C
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = 0.0;
		this.hands[handIndex] = handData;
		this.UpdateStuckState();
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000C87D9 File Offset: 0x000C69D9
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000C87E8 File Offset: 0x000C69E8
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000C8810 File Offset: 0x000C6A10
	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameBallId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000C8858 File Offset: 0x000C6A58
	private void UpdateHandEmpty(int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = ControllerInputPoller.GripFloat(this.GetXRNode(handIndex)) > 0.7f;
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Vector3 position = this.GetHandTransform(handIndex).position;
			GameBallId gameBallId = GameBallManager.Instance.TryGrabLocal(position, this.gamePlayer.teamId);
			float num2 = 0.15f;
			if (gameBallId.IsValid())
			{
				bool flag2 = GameBallPlayerLocal.IsLeftHand(handIndex);
				BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
				object obj = flag2 ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
				Vector3 position2 = gameBall.transform.position;
				Vector3 vector = gameBall.transform.position - position;
				if (vector.sqrMagnitude > num2 * num2)
				{
					position2 = position + vector.normalized * num2;
				}
				object obj2 = obj;
				Vector3 localPosition = obj2.InverseTransformPoint(position2);
				Quaternion localRotation = Quaternion.Inverse(obj2.rotation) * gameBall.transform.rotation;
				obj2.InverseTransformPoint(gameBall.transform.position);
				GameBallManager.Instance.RequestGrabBall(gameBallId, flag2, localPosition, localRotation);
			}
		}
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x000C89E4 File Offset: 0x000C6BE4
	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion rotation;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation2 = GTPlayer.Instance.turnParent.transform.rotation;
			GameBallPlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation2 * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation2 * -(Quaternion.Inverse(rotation) * vector);
			GameBallId gameBallId = this.gamePlayer.GetGameBallId(handIndex);
			GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
			if (gameBall == null)
			{
				return;
			}
			if (gameBall.IsLaunched)
			{
				return;
			}
			if (gameBall.disc)
			{
				Vector3 vector3 = gameBall.transform.rotation * gameBall.localDiscUp;
				vector3.Normalize();
				float d = Vector3.Dot(vector3, vector);
				vector = vector3 * d;
				vector *= 1.25f;
				vector2 *= 1.25f;
			}
			else
			{
				vector2 *= 1.5f;
			}
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameBallManager.Instance.RequestThrowBall(gameBallId, GameBallPlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x000C8B9C File Offset: 0x000C6D9C
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x000C8BA4 File Offset: 0x000C6DA4
	private Transform GetHandTransform(int handIndex)
	{
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		return ((handIndex == 0) ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform).parent;
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x000C8513 File Offset: 0x000C6713
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x060025E5 RID: 9701 RVA: 0x000C8519 File Offset: 0x000C6719
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x000C8BD7 File Offset: 0x000C6DD7
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000C8BF3 File Offset: 0x000C6DF3
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x0400313E RID: 12606
	public GameBallPlayer gamePlayer;

	// Token: 0x0400313F RID: 12607
	private const int MAX_INPUT_HISTORY = 32;

	// Token: 0x04003140 RID: 12608
	private GameBallPlayerLocal.HandData[] hands;

	// Token: 0x04003141 RID: 12609
	private GameBallPlayerLocal.InputData[] inputData;

	// Token: 0x04003142 RID: 12610
	[OnEnterPlay_SetNull]
	public static volatile GameBallPlayerLocal instance;

	// Token: 0x020005F0 RID: 1520
	private enum HandGrabState
	{
		// Token: 0x04003144 RID: 12612
		Empty,
		// Token: 0x04003145 RID: 12613
		Holding
	}

	// Token: 0x020005F1 RID: 1521
	private struct HandData
	{
		// Token: 0x04003146 RID: 12614
		public GameBallPlayerLocal.HandGrabState grabState;

		// Token: 0x04003147 RID: 12615
		public bool gripWasHeld;

		// Token: 0x04003148 RID: 12616
		public double gripPressedTime;

		// Token: 0x04003149 RID: 12617
		public GameBallId grabbedGameBallId;
	}

	// Token: 0x020005F2 RID: 1522
	public struct InputDataMotion
	{
		// Token: 0x0400314A RID: 12618
		public double time;

		// Token: 0x0400314B RID: 12619
		public Vector3 position;

		// Token: 0x0400314C RID: 12620
		public Quaternion rotation;

		// Token: 0x0400314D RID: 12621
		public Vector3 velocity;

		// Token: 0x0400314E RID: 12622
		public Vector3 angVelocity;
	}

	// Token: 0x020005F3 RID: 1523
	public class InputData
	{
		// Token: 0x060025E9 RID: 9705 RVA: 0x000C8C15 File Offset: 0x000C6E15
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GameBallPlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x000C8C30 File Offset: 0x000C6E30
		public void AddInput(GameBallPlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x000C8C60 File Offset: 0x000C6E60
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000C8CDC File Offset: 0x000C6EDC
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 a = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					a += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return a / (float)num3;
		}

		// Token: 0x0400314F RID: 12623
		public int maxInputs;

		// Token: 0x04003150 RID: 12624
		public List<GameBallPlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
