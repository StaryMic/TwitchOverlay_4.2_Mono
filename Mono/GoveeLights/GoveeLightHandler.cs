using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoveeCSharpConnector.Objects;
using GoveeCSharpConnector.Services;
[GlobalClass]
public partial class GoveeLightHandler : Node
{
	// Devices
	private GoveeUdpService _goveeUdpService = new GoveeUdpService();
	private List<GoveeUdpDevice> _udpDevices = new List<GoveeUdpDevice>();
	
	// Retry Counter
	private int _retryCount;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DiscoverAndConnectDevices();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void DiscoverAndConnectDevices()
	{
		_udpDevices = await _goveeUdpService.GetDevices();
		foreach (var device in _udpDevices)
		{
			GD.Print("Found ",device.ip);
		}

		if (_udpDevices.Count == 0)
		{
			if (_retryCount > 5)
			{
				GD.Print("GoveeLightHandler.cs: Failed to find any devices after 5 tries. Cancelling.");
				return;
			}
			GD.Print("Failed to find any devices. Trying again.");
			_retryCount++; // Increment retry counter.
			// Try again if you find nothing.
			DiscoverAndConnectDevices();
		}
	}

	public void ChangeColor(int R, int G, int B)
	{
		foreach (var device in _udpDevices)
		{
			_goveeUdpService.SetColor(device.ip, new RgbColor(R, G, B));
		}
	}
}
