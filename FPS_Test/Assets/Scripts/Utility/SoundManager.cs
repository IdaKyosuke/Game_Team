using UnityEngine;

public class SoundManager
{
    static public void Play3D(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1)
    {
        PlaySe(clip, position, 1, volume, pitch, false);
    }

    static public void Play2D(AudioClip clip, float volume = 1, float pitch = 1)
    {
        PlaySe(clip, Vector3.zero, 0, volume, pitch, false);
    }

    static public AudioSource PlayLoop3D(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1)
    {
        return PlaySe(clip, position, 1, volume, pitch, true);
    }

    static public AudioSource PlayLoop2D(AudioClip clip, float volume = 1, float pitch = 1)
    {
        return PlaySe(clip, Vector3.zero, 0, volume, pitch, true);
    }

    static AudioSource PlaySe(AudioClip clip, Vector3 position, float spatialBlend, float volume, float pitch, bool isLoop)
    {
        GameObject obj = new GameObject(clip.name);

        AudioSource audio = obj.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.transform.position = position;
        audio.spatialBlend = spatialBlend;
        audio.loop = isLoop;
        audio.volume = volume;
        audio.pitch = pitch;

        audio.Play();

        //再生終了後にオブジェクトを破棄
        if (!isLoop) MonoBehaviour.Destroy(obj, clip.length * (1.0f / pitch));

        //再生停止用にオブジェクトを返す
        return audio;
    }
}