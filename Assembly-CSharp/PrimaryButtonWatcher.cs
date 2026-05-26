using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005DC RID: 1500
public class PrimaryButtonWatcher : MonoBehaviour
{
	// Token: 0x06002556 RID: 9558 RVA: 0x000C62DF File Offset: 0x000C44DF
	private void Awake()
	{
		if (this.primaryButtonPress == null)
		{
			this.primaryButtonPress = new PrimaryButtonEvent();
		}
		this.devicesWithPrimaryButton = new List<InputDevice>();
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x000C6300 File Offset: 0x000C4500
	private void OnEnable()
	{
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevices(list);
		foreach (InputDevice device in list)
		{
			this.InputDevices_deviceConnected(device);
		}
		InputDevices.deviceConnected += this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected += this.InputDevices_deviceDisconnected;
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x000C637C File Offset: 0x000C457C
	private void OnDisable()
	{
		InputDevices.deviceConnected -= this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected -= this.InputDevices_deviceDisconnected;
		this.devicesWithPrimaryButton.Clear();
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x000C63AC File Offset: 0x000C45AC
	private void InputDevices_deviceConnected(InputDevice device)
	{
		bool flag;
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, out flag))
		{
			this.devicesWithPrimaryButton.Add(device);
		}
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000C63D5 File Offset: 0x000C45D5
	private void InputDevices_deviceDisconnected(InputDevice device)
	{
		if (this.devicesWithPrimaryButton.Contains(device))
		{
			this.devicesWithPrimaryButton.Remove(device);
		}
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x000C63F4 File Offset: 0x000C45F4
	private void Update()
	{
		bool flag = false;
		foreach (InputDevice inputDevice in this.devicesWithPrimaryButton)
		{
			bool flag2 = false;
			flag = ((inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out flag2) && flag2) || flag);
		}
		if (flag != this.lastButtonState)
		{
			this.primaryButtonPress.Invoke(flag);
			this.lastButtonState = flag;
		}
	}

	// Token: 0x040030DA RID: 12506
	public PrimaryButtonEvent primaryButtonPress;

	// Token: 0x040030DB RID: 12507
	private bool lastButtonState;

	// Token: 0x040030DC RID: 12508
	private List<InputDevice> devicesWithPrimaryButton;
}
