using UnityEngine;
using UnityEngine.SceneManagement;
public class NewGameLoader : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
