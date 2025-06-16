using UnityEngine;
//
// [ExecuteInEditMode]
// public class AudioManager : MonoBehaviour
// {
//     [SerializeField] private AudioSource bgmAudioSource;
//
//     public static AudioManager Instance { get; private set; }
//     
//     private void Awake()
//     {
//         if (Application.isPlaying)
//         {
//             Debug.Assert(bgmAudioSource != null);
//             
//             if (Instance != null)
//             {
//                 Destroy(gameObject);
//                 return;
//             }
//             Instance = this;
//             
//             DontDestroyOnLoad(this);
//         }
//     }
// }