using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Transition Objects")]
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;

    [Header("Settings")]
    [SerializeField] private float _transitionTime = 1f; // Waktu tunggu animasi selesai (detik)

    private void Start()
    {
        // 1. Saat scene baru mulai, nyalakan animasi "Buka Tirai" (Starting)
        if (_startingSceneTransition != null)
        {
            _startingSceneTransition.SetActive(true);

            // Opsional: Matikan objek transisi start setelah beberapa detik agar hemat performa
            Invoke("DisableStartingTransition", _transitionTime);
        }
    }

    private void DisableStartingTransition()
    {
        if (_startingSceneTransition != null)
            _startingSceneTransition.SetActive(false);
    }

    // Panggil fungsi ini dari tombol UI atau Script lain
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    IEnumerator LoadLevelRoutine(string sceneName)
    {
        // 2. Nyalakan animasi "Tutup Tirai" (Ending)
        if (_endingSceneTransition != null)
        {
            _endingSceneTransition.SetActive(true);
        }

        // 3. Tunggu sampai animasi selesai (sesuai waktu transitionTime)
        yield return new WaitForSeconds(_transitionTime);

        // 4. Baru pindah scene
        SceneManager.LoadScene(sceneName);
    }
}