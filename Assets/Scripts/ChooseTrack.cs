using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseTrack : MonoBehaviour
{
    public void SelectMap(string mapName)
    {
        // Carica la scena passata come nome dal bottone
        SceneManager.LoadScene(mapName);
    }
}