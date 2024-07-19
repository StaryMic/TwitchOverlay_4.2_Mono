using Godot;
using Godot.Collections;
using TwitchOverlay.Mono;
using AstroRaider2.Utility.Timers;

public partial class BitCup2D : Node2D
{
	public bool _processingQueue = true;
	
	private GlobalSceneSignals _globalSceneSignalsRef;
	private CooldownTimer _queueCooldown;
	private PackedScene _Bit2D;
	private Node2D _bitCollectionRef;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_globalSceneSignalsRef = GetTree().Root.GetChild(0).GetNode<GlobalSceneSignals>("GlobalSceneSignals");
		_globalSceneSignalsRef.Cheer += GlobalSceneSignalsRefOnCheer;
		_queueCooldown = new CooldownTimer(1);
		_Bit2D = ResourceLoader.Load<PackedScene>("res://Mono/Twitch/Events/Bits/Bit2D.tscn");
		_bitCollectionRef = GetNode<Node2D>("Bits");
		
		Dictionary<string, Variant> testDictionary = new Dictionary<string, Variant>()
		{
			{"Username", "pleebis"},
			{"SingleBits", 10},
			{"25Bits", 5},
			{"100Bits", 5},
			{"1000Bits", 5},
			{"10000Bits", 5},
			{"TotalBitsToSpawn", 30}
		};
		_cheerQueue.Add(testDictionary);
	}

	private Array<Dictionary<string, Variant>> _cheerQueue = new Array<Dictionary<string, Variant>>();
	
	private void GlobalSceneSignalsRefOnCheer(string username, string message, int bits)
	{
		int bitStorage = bits;
		Dictionary<string, Variant> templateDictionary = new Dictionary<string, Variant>()
		{
			{"Username", username},
			{"SingleBits", 0},
			{"25Bits", 0},
			{"100Bits", 0},
			{"1000Bits", 0},
			{"10000Bits", 0},
			{"TotalBitsToSpawn", 0}
		};

		if ((bitStorage / 10000) >= 1)
		{
			int result = bits / 10000;
			templateDictionary["10000Bits"] = result;

			bitStorage -= result * 10000; // Should be equivalent to bitstorage - (result * 10000)

		}
		
		if ((bitStorage / 1000) >= 1)
		{
			int result = bits / 1000;
			templateDictionary["1000Bits"] = result;

			bitStorage -= result * 1000; // Should be equivalent to bitstorage - (result * 1000)

		}
		
		if ((bitStorage / 100) >= 1)
		{
			int result = bits / 100;
			templateDictionary["100Bits"] = result;

			bitStorage -= result * 100; // Should be equivalent to bitstorage - (result * 100)

		}
		
		if ((bitStorage / 25) >= 1)
		{
			int result = bits / 25;
			templateDictionary["25Bits"] = result;

			bitStorage -= result * 25; // Should be equivalent to bitstorage - (result * 25)

		}

		if (bitStorage > 0)
		{
			templateDictionary["SingleBits"] = bitStorage;
			// Store the remainder in here.
		}

		templateDictionary["TotalBitsToSpawn"] = (int)templateDictionary["10000Bits"] +
		                                         (int)templateDictionary["1000Bits"] +
		                                         (int)templateDictionary["100Bits"] +
		                                         (int)templateDictionary["25Bits"] +
		                                         (int)templateDictionary["SingleBits"];
		
		_cheerQueue.Add(templateDictionary);

	}

	public override void _Process(double delta)
	{
		if (_cheerQueue.Count > 0 && _queueCooldown.HasCooldownElapsed() && _processingQueue)
		{
			Dictionary<string,Variant> current = _cheerQueue[0];
			
			for (int i = 1; i <= current["SingleBits"].AsInt32(); i++)
			{
				Bit2D _InstancedBit2D = _Bit2D.Instantiate() as Bit2D;
				_InstancedBit2D.Value = 1;
				_bitCollectionRef.AddChild(_InstancedBit2D,true);
			}
			for (int i = 1; i <= current["25Bits"].AsInt32(); i++)
			{
				Bit2D _InstancedBit2D = _Bit2D.Instantiate() as Bit2D;
				_InstancedBit2D.Value = 25;
				_bitCollectionRef.AddChild(_InstancedBit2D,true);
			}
			for (int i = 1; i <= current["100Bits"].AsInt32(); i++)
			{
				Bit2D _InstancedBit2D = _Bit2D.Instantiate() as Bit2D;
				_InstancedBit2D.Value = 100;
				_bitCollectionRef.AddChild(_InstancedBit2D,true);
			}
			for (int i = 1; i <= current["1000Bits"].AsInt32(); i++)
			{
				Bit2D _InstancedBit2D = _Bit2D.Instantiate() as Bit2D;
				_InstancedBit2D.Value = 1000;
				_bitCollectionRef.AddChild(_InstancedBit2D,true);
			}
			for (int i = 1; i <= current["10000Bits"].AsInt32(); i++)
			{
				Bit2D _InstancedBit2D = _Bit2D.Instantiate() as Bit2D;
				_InstancedBit2D.Value = 10000;
				_bitCollectionRef.AddChild(_InstancedBit2D,true);
			}
			_cheerQueue.RemoveAt(0); // Remove the entry we just processed.
		}
	}
}
