using System;
using Microsoft.Xna.Framework;

public class Timer
{
    private float _duration;
    private float _elapsed;
    private bool _isRunning;
    
    public Timer(float duration)
    {
        _duration = duration;
        _elapsed = 0f;
        _isRunning = false;
    }

    // Return true if the timer is completed
    public bool IsFinished()
    {
        return _elapsed >= _duration;
    }

    // Return true if the timer is not paused
    public bool IsRunning()
    {
        return _isRunning;
    }

    // Start/Play the timer
    public void Start()
    {
        _isRunning = true;
    }

    // Pause the timer
    public void Stop()
    {
        _isRunning = false;
    }

    // Reset timer
    public void Reset()
    {
        _elapsed = 0f;
    }

    // Return the timer duration
    public float GetDuration()
    {
        return _duration;
    }

    // Return the time elapsed
    public float GetTimeElapsed()
    {
        return _elapsed;
    }

    // Return the remaining time
    public float GetRemainingTime()
    {
        return MathF.Max(0f, GetDuration() - GetTimeElapsed());
    }

    // Update the timer for its progress
    public void Update(GameTime gameTime)
    {
        if (_isRunning)
        {
            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GetTimeElapsed() >= GetDuration())
            {
                Stop();
            }   
        }
    }
}