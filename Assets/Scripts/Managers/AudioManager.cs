using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class responsible for audio
public class AudioManager : Persistent
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if(_instance != null)
            {
                return _instance;
            }

            return null;
        }
    }
    AudioSource audioSource;
    [SerializeField] private AudioClip scoreSFX;
    [SerializeField] private AudioClip bombSFX;

    private void Init()
    {
        _instance = this;
    }

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Audio");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Init();
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayScoreSFX()
    {
        audioSource.PlayOneShot(scoreSFX);
    }

    public void PlayBoomSFX()
    {
        audioSource.PlayOneShot(bombSFX, 0.15f);
    }
}
