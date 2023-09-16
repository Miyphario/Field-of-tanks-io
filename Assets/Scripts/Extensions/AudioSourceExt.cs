using UnityEngine;

public static class AudioSourceExt
{
    public static void SetRandomPitch(this AudioSource source, float minPitch, float maxPitch)
    {
        source.pitch = Random.Range(minPitch, maxPitch);
    }

    public static void SetRandomVolume(this AudioSource source, float minVolume, float maxVolume)
    {
        source.volume = Random.Range(minVolume, maxVolume);
    }

    public static void SetRandomPitchAndVolume(this AudioSource source, float minPitch, float maxPitch, float minVolume, float maxVolume)
    {
        SetRandomPitch(source, minPitch, maxPitch);
        SetRandomVolume(source, minVolume, maxVolume);
    }

    public static bool IsReversePitch(this AudioSource source)
    {
        return source.pitch < 0f;
    }

    public static float GetClipRemainingTime(this AudioSource source)
    {
        if (source.clip == null) return 0f;
        
        float remainingTime = (source.clip.length - source.time) / source.pitch;
        return source.IsReversePitch() ?
               (source.clip.length + remainingTime) :
               remainingTime;
    }
}
