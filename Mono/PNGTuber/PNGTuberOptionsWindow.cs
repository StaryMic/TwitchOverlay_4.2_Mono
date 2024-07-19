using Godot;

public partial class PNGTuberOptionsWindow : Window
{
	// PNGTuber Ref
	private PNGTuber _pngTuber;
	
	// Labels
	private Label _sampleCountLabel;
	private Label _talkThresholdLabel;
	private Label _screamThresholdLabel;
	
	// Sliders
	private Slider _sampleCountSlider;
	private Slider _talkThresholdSlider;
	private Slider _screamThresholdSlider;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get Node References
		_pngTuber = this.GetParent<PNGTuber>();
		
		_sampleCountLabel = GetNode<Label>("VBoxContainer/SampleCountLabel");
		_talkThresholdLabel = GetNode<Label>("VBoxContainer/TalkLabel");
		_screamThresholdLabel = GetNode<Label>("VBoxContainer/ScreamLabel");

		_sampleCountSlider = GetNode<Slider>("VBoxContainer/SampleCount");
		_talkThresholdSlider = GetNode<Slider>("VBoxContainer/Talk");
		_screamThresholdSlider = GetNode<Slider>("VBoxContainer/Scream");
		
		// Bind Actions
		_sampleCountSlider.DragEnded += SampleCountSliderOnDragEnded;
		_talkThresholdSlider.DragEnded += TalkThresholdSliderOnDragEnded;
		_screamThresholdSlider.DragEnded += ScreamThresholdSliderOnDragEnded;
	}

	private void ScreamThresholdSliderOnDragEnded(bool valuechanged)
	{
		_screamThresholdLabel.Text = "Scream Threshold: " + _screamThresholdSlider.Value;
		_pngTuber.ScreamThreshold = (float)_screamThresholdSlider.Value;
	}

	private void TalkThresholdSliderOnDragEnded(bool valuechanged)
	{
		_talkThresholdLabel.Text = "Talk Threshold: " + _talkThresholdSlider.Value;
		_pngTuber.TalkThreshold = (float)_talkThresholdSlider.Value;
	}

	private void SampleCountSliderOnDragEnded(bool valuechanged)
	{
		_sampleCountLabel.Text = "Samples: " + _sampleCountSlider.Value;
		_pngTuber.MicrophoneSmoothingSamples = (int)_sampleCountSlider.Value;
	}
}
