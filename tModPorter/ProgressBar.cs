using System;
using System.Text;
using System.Threading;

namespace tModPorter
{
    public class ProgressBar : IDisposable, IProgress<int>
    {
        private int _currentElements;
        private readonly int _maxElements;
        private readonly Timer _timer;
        private bool _disposed;
        private readonly byte _numberOfBlocks;
        private readonly TimeSpan _animationInterval;

        public ProgressBar(int maxElements, byte numberOfBlocks = 16, double animationInterval = 1.0 / 8)
        {
            _maxElements = maxElements;
            _timer = new Timer(TimerCallback);
            _numberOfBlocks = numberOfBlocks;
            _animationInterval = TimeSpan.FromSeconds(animationInterval);
        }

        public void Start() => ResetTimer();

        public static ProgressBar StartNew(int maxElements, byte numberOfBlocks = 16, double animationInterval = 1.0 / 8)
        {
            ProgressBar bar = new(maxElements, numberOfBlocks, animationInterval);
            bar.Start();
            return bar;
        }

        public void Dispose()
        {
            lock (_timer)
                _disposed = true;
            
            GC.SuppressFinalize(this);
        }

        public void Report(int v) => Interlocked.Add(ref _currentElements, v);
        
        private void TimerCallback(object state)
        {
            lock (_timer)
            {
                if (_disposed) return;

                ResetTimer();
                UpdateProgressText(CreateProgressText());
            }
        }

        private void ResetTimer() => _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));

        private string CreateProgressText()
        {
            // Calculate the number of blocks to draw with the "#" character
            double percent = (double) _currentElements / _maxElements;
            int numFullBlocks = (int) Math.Round(percent * _numberOfBlocks);
            
            // Append the characters to a string builder
            StringBuilder sb = new();
            for (int i = 0; i < _numberOfBlocks; i++) 
                sb.Append(i <= numFullBlocks ? "#" : "-");

            // Format the string and return it
            return $"\r[{sb}] {_currentElements}/{_maxElements,-5}";
        }

        private void UpdateProgressText(string text) => Console.Write(text);
    }
}