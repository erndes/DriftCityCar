/*using UnityEngine;

public class CarLapCounter : MonoBehaviour
{
    public int lapsDone = 0;
    public bool hasPassedCheckpoint = false; // Se vuoi usare i checkpoint
    public bool raceStarted = false;
}


using UnityEngine;

public class CarLapCounter : MonoBehaviour
{
    [HideInInspector] public int lapsDone = 0;
    [HideInInspector] public bool raceStarted = false;
}

*/

using UnityEngine;

public class CarLapCounter : MonoBehaviour
{
    public int lapsDone = 0;
    public bool raceStarted = false; // Indica se l'auto ha già passato il via
    public bool isCoolingDown = false; // Cooldown personale dell'auto

    // Funzione per gestire il cooldown della singola auto
    public System.Collections.IEnumerator StartCooldown(float seconds)
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(seconds);
        isCoolingDown = false;
    }
}