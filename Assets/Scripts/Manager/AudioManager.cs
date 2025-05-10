using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource background;
    private List<AudioSource> effectAudioList = new List<AudioSource>();
    private Dictionary<string, AudioClip> clipDic = new Dictionary<string, AudioClip>();
    // 오브젝트 풀링의 제한 개수
    private int limitCount = 10;
    // 제한 개수 이상이 되었을 경우 파괴시킬 시간 간격
    private float intervalTime = 1.0f;
    private float prevTime = 0;

    private ClipController bgmClips;
    private ClipController sfxClips;

    private Dictionary<string, int> soundActiveDic = new Dictionary<string, int>();
    private Dictionary<string, bool> soundActiveFrameDic = new Dictionary<string, bool>();

    private void OnApplicationFocus(bool focus)
    {
        if (!SettingManager.Instance.muteOnBackground)
            return;

        SetMute(!focus);
    }

    private void SetMute(bool value)
    {
        background.mute = value;
        foreach (AudioSource fx in effectAudioList)
            fx.mute = value;
    }

    private void AddClipDic(List<AudioClip> clipList)
    {
        foreach (AudioClip clip in clipList)
        {
            clipDic.Add(clip.name, clip);
        }
    }

    public void Init()
    {
        background = gameObject.AddComponent<AudioSource>();

        background.spatialBlend = 0;
        background.volume = 1.0f;

        background.playOnAwake = false;

        if (bgmClips == null)
        {
            ClipController prefab = Resources.Load<ClipController>("Prefab/Audio/ClipController");
            bgmClips = Instantiate(prefab);
            AddClipDic(bgmClips.audioClips);
            DontDestroyOnLoad(bgmClips);
        }
    }

    public void ChangeVolume(string target, float value)
    {
        try
        {
            Bus targetBus = RuntimeManager.GetBus("bus:/" + target);
            targetBus.setVolume(value);
        }
        catch (Exception e)
        {

        }
    }


    public void UpdateMusicVolume(float volume)
    {
        background.volume = volume;
    }

    public void UpdateFxVolume(float volume)
    {
        foreach (AudioSource effect in effectAudioList)
        {
            effect.volume = volume;
        }
    }

    private string curBGMEventID = null;

    private EventInstance currentBGM;

    public void StopBGM(int fadeTileMilliSec = 500)
    {
        if (currentBGM.isValid())
        {
            currentBGM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentBGM.release();
        }
    }

    public void PlayBackground(FMODUnity.EventReference soundClip, int fadeTileMilliSec = 500)
    {
        StopBGM(fadeTileMilliSec);
        currentBGM = RuntimeManager.CreateInstance(soundClip);
        currentBGM.start();
    }

    public void PlayBackground(string name, float voulme = 1.0f)
    {
        if (clipDic.ContainsKey(name))
        {
            background.clip = clipDic[name];
            background.volume = voulme;
            background.Play();
        }
    }

    public void PlayBackground(AudioClip clip, float voulme = 1.0f)
    {
        if (clip != null)
        {
            background.clip = clip;
            background.volume = voulme;
            background.loop = true;
            background.Play();
        }
    }

    // 오브젝트 풀링으로 오디오소스 관리
    AudioSource Pooling()
    {
        AudioSource audioSource = null;
        for (int i = 0; i < effectAudioList.Count; ++i)
        {
            if (effectAudioList[i].gameObject.activeSelf == false)
            {
                audioSource = effectAudioList[i];
                audioSource.gameObject.SetActive(true);
                break;
            }
        }
        if (audioSource == null)
        {
            audioSource = UtilHelper.CreateObject<AudioSource>(transform);
            effectAudioList.Add(audioSource);
        }
        return audioSource;
    }

    IEnumerator IDeactiveAudio(AudioSource audio)
    {
        yield return new WaitForSeconds(audio.clip.length);
        soundActiveDic[audio.clip.name]--;
        audio.gameObject.SetActive(false);
    }

    public void Play(string name, float spatialBelnd, float volume, Vector3 position)
    {
        if (clipDic.ContainsKey(name) == false)
            return;

        Play(clipDic[name], spatialBelnd, volume, position);
    }

    public void Play(AudioClip clip, float spatialBelnd, float volume, Vector3 position)
    {
        if (clip == null)
            return;

        if (soundActiveDic.ContainsKey(clip.name) && soundActiveDic[clip.name] > 5)
            return;

        AudioSource audioSource = Pooling();
        audioSource.clip = clip;
        audioSource.spatialBlend = spatialBelnd;
        audioSource.volume = volume;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 2;
        audioSource.maxDistance = 10;
        audioSource.Play();
        if (!soundActiveDic.ContainsKey(clip.name))
        {
            soundActiveDic.Add(clip.name, 0);
            soundActiveFrameDic.Add(clip.name, false);
        }
        soundActiveDic[clip.name]++;
        soundActiveFrameDic[clip.name] = true;
        StartCoroutine(IDeactiveAudio(audioSource));
    }

    public void Play2DSound(string name, float volume = 1.0f)
    {
        Play(name, 0, volume, Vector3.zero);
    }

    public void Play2DSound(AudioClip clip, float volume = 1.0f)
    {
        Play(clip, 0, volume, Vector3.zero);
    }

    public void Play3DSound(string name, Vector3 position, float volume = 1.0f)
    {
        Play(name, 1, volume, position);
    }

    public void Play3DSound(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        Play(clip, 1, volume, position);
    }


    void Update()
    {
        if (effectAudioList.Count > limitCount)
        {
            float elapsedTime = Time.time - prevTime;
            // 경과 시간이 기준시간을 지났다면
            if (elapsedTime > intervalTime)
            {
                for (int i = 0; i < effectAudioList.Count; ++i)
                {
                    if (effectAudioList[i].gameObject.activeSelf == false)
                    {
                        AudioSource audioSource = effectAudioList[i];
                        effectAudioList.RemoveAt(i);
                        prevTime = Time.time;
                        Destroy(audioSource.gameObject);
                        return;
                    }
                }
            }
        }
    }
}
