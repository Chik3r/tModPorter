using System.Text;
using SysConsole = System.Console;

namespace tModPorter.Console;

public class ProgressBar : IDisposable, IProgress<double> {
	private readonly TimeSpan _animationInterval;
	private readonly byte _numberOfBlocks;
	private readonly Timer _timer;
	private double _progress;
	private bool _disposed;
	private bool _started;

	public ProgressBar(byte numberOfBlocks = 16, double animationInterval = 1.0 / 8) {
		_timer = new Timer(TimerCallback);
		_numberOfBlocks = numberOfBlocks;
		_animationInterval = TimeSpan.FromSeconds(animationInterval);
	}

	public void Dispose() {
		lock (_timer) {
			_disposed = true;
		}

		GC.SuppressFinalize(this);
	}

	public void Report(double v) {
		if (!_started) {
			Start();
		}

		_progress = v;
	}

	public void Start() {
		ResetTimer();
		_started = true;
	}

	public static ProgressBar StartNew(byte numberOfBlocks = 16, double animationInterval = 1.0 / 8) {
		ProgressBar bar = new(numberOfBlocks, animationInterval);
		bar.Start();
		return bar;
	}

	public void ForceUpdate() {
		lock (_timer) {
			ResetTimer();
			UpdateProgressText(CreateProgressText());
		}
	}

	private void TimerCallback(object state) {
		lock (_timer) {
			if (_disposed) return;

			ResetTimer();
			UpdateProgressText(CreateProgressText());
		}
	}

	private void ResetTimer() => _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));

	private string CreateProgressText() {
		// Calculate the number of blocks to draw with the "#" character
		int numFullBlocks = (int) Math.Round(_progress * _numberOfBlocks);

		// Append the characters to a string builder
		StringBuilder sb = new();
		for (int i = 0; i < _numberOfBlocks; i++)
			sb.Append(i <= numFullBlocks ? "#" : "-");

		// Format the string and return it
		return $"\r[{sb}] {_progress:P2}";
	}

	private void UpdateProgressText(string text) => SysConsole.Write(text);
}