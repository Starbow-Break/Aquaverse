using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private List<AudioClip> BGMClips; 
    
    private AudioSource BGMAudioSource;
    public Coroutine StopBGMCoroutine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        BGMAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayBGM(BGMType.StartBGM);
    }
    
    public async void PlayBGM(BGMType bgm)
    {
        await new WaitUntil(() => StopBGMCoroutine == null);
        
        BGMAudioSource.clip = GetBGMClip(bgm);
        BGMAudioSource.volume = 1f;
        BGMAudioSource.Play();
    }

    public void StopBGM()
    {
        if (StopBGMCoroutine != null)
            return;
        StopBGMCoroutine = StartCoroutine(StopBGMProcess());
    }
    
    private IEnumerator StopBGMProcess()
    {
        while (BGMAudioSource.volume > 0f)
        {
            BGMAudioSource.volume -= Time.unscaledDeltaTime;
            yield return null;
        }
        BGMAudioSource.Stop();
        StopBGMCoroutine = null;
    }

    private AudioClip GetBGMClip(BGMType type) => BGMClips[(int)type];
}