using LitMotion;

public static class LitMotion_Extensions
{
    public static MotionBuilder<TValue, TOptions, TAdapter> IgnoreTimeScale<TValue, TOptions, TAdapter>
        (this MotionBuilder<TValue, TOptions, TAdapter> motion)
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        => motion.WithScheduler(MotionScheduler.UpdateIgnoreTimeScale);
        
    public static void Kill(this MotionHandle motion, bool complete = false)
    {
        if (!motion.IsActive()) return;
        if (!complete) motion.Cancel();
        else motion.Complete();
    }
}