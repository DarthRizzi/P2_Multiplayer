using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    public AudioClip MenuThemeSong;
    public AudioClip GameThemeSong;
    
    [Header("Configurações do AudioSource")]
    [Range(0f, 1f)] public float volume = 0.3f;
    public bool loop = true;

    private AudioSource audioSource;
    private string currentSceneName;

    void Awake()
    {
        // Garante que o objeto persista entre cenas
        DontDestroyOnLoad(gameObject);

        // Configura o AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.loop = loop;
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        PlaySceneMusic();
    }

    void Update()
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        
        // Verifica se a cena mudou
        if (newSceneName != currentSceneName)
        {
            currentSceneName = newSceneName;
            PlaySceneMusic();
        }
    }

    void PlaySceneMusic()
    {
        AudioClip clipToPlay = null;

        switch (currentSceneName)
        {
            case "MenuScene":
                clipToPlay = MenuThemeSong;
                break;
            case "GameScene":
                clipToPlay = GameThemeSong;
                break;
        }

        if (clipToPlay != null && (audioSource.clip != clipToPlay || !audioSource.isPlaying))
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    // Método público para ajustar o volume durante o jogo
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}