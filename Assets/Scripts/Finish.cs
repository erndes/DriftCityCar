/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Finish : MonoBehaviour
{
    [Header("Finish UI Var")]
    public GameObject finishUI;
    public GameObject playerUI;
    public GameObject playerCar;

    [Header("Win/Lose Status")]
    public TextMeshProUGUI status;

    public void Start(){
        StartCoroutine(waitforthefinishUI());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine(finishZoneTimer());
            gameObject.GetComponent<BoxCollider>().enabled = false;

            status.text = "YOU WIN";
            status.color = Color.green;
        }
        else if(other.gameObject.tag == "OpponentCar")
        {
            StartCoroutine(finishZoneTimer());
            gameObject.GetComponent<BoxCollider>().enabled = false;

            status.text = "YOU LOSE";
            status.color = Color.red;
        }
    }

    IEnumerator waitforthefinishUI()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(25f);
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    IEnumerator finishZoneTimer()
    {
        finishUI.SetActive(true);
        playerUI.SetActive(false);
        playerCar.SetActive(false);

        yield return new WaitForSeconds(5f);
        Time.timeScale = 0f;
    }
}
*/





//VERSIONE NON CORRETTA PER L'AVVERSARIO, MA CORRETTA PER ME
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Finish : MonoBehaviour
{
    [Header("UI References")]
    public GameObject finishUI;
    public GameObject playerUI;
    public TextMeshProUGUI status;
    public TextMeshProUGUI lapText;

    [Header("Race Settings")]
    public int totalLaps = 3;
    private int lapsCompleted = 0;
    private bool canCountLap = true;
    private bool raceStarted = false; // Controlla se abbiamo già passato il via

    private void Start()
    {
        UpdateLapUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canCountLap)
        {
            // Protezione per evitare tocchi accidentali nel primo secondo di gioco
            if (Time.timeSinceLevelLoad < 1f) return;

            // PRIMO PASSAGGIO (Partenza)
            if (!raceStarted)
            {
                raceStarted = true;
                Debug.Log("Partenza confermata! Inizio Giro 1");
                StartCoroutine(LapCooldown());
                return; // Esci senza aumentare lapsCompleted
            }

            // PASSAGGI SUCCESSIVI (Giri completati)
            lapsCompleted++;
            UpdateLapUI();

            if (lapsCompleted >= totalLaps)
            {
                // VITTORIA
                status.text = "YOU WIN";
                status.color = Color.green;
                StartCoroutine(finishZoneTimer(other.gameObject));
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                StartCoroutine(LapCooldown());
            }
        }
    }

    void UpdateLapUI()
    {
        if (lapText != null)
        {
            // Se lapsCompleted è 0, la UI mostrerà "Lap: 1 / 3"
            lapText.text = "Lap: " + (lapsCompleted + 1) + " / " + totalLaps;
        }
    }

    IEnumerator LapCooldown()
    {
        canCountLap = false;
        yield return new WaitForSeconds(5f); // Impedisce di contare lo stesso giro per 5 secondi
        canCountLap = true;
    }

    IEnumerator finishZoneTimer(GameObject carToDisable)
    {
        finishUI.SetActive(true);
        playerUI.SetActive(false);
        if (carToDisable != null) carToDisable.SetActive(false);
        yield return new WaitForSeconds(5f);
        Time.timeScale = 0f;
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Finish : MonoBehaviour
{
    [Header("UI References")]
    public GameObject finishUI;
    public GameObject playerUI;
    public TextMeshProUGUI status;
    public TextMeshProUGUI lapText;

    [Header("Race Settings")]
    public int totalLaps = 3;
    public float lapCooldownTime = 5f; // Tempo minimo per fare un giro

    private void Start()
    {
        UpdateLapUI(0);
        // Protezione fisica iniziale per sicurezza
        StartCoroutine(InitialColliderSafety());
    }

    private void OnTriggerEnter(Collider other)
    {
        CarLapCounter lapCounter = other.GetComponentInParent<CarLapCounter>();

        // 1. Controlli di base
        if (lapCounter == null || lapCounter.isCoolingDown) return;
        if (Time.timeSinceLevelLoad < 0.5f) return;

        // 2. GESTIONE PRIMO PASSAGGIO (START LINE)
        // Se l'auto non ha mai passato il traguardo, questo tocco attiva la sua gara
        if (!lapCounter.raceStarted)
        {
            lapCounter.raceStarted = true;
            StartCoroutine(lapCounter.StartCooldown(lapCooldownTime));
            Debug.Log(other.gameObject.name + " ha attraversato la linea di PARTENZA.");
            return; // Usciamo senza contare il giro
        }

        // 3. GESTIONE PASSAGGI SUCCESSIVI (FINISH LINE)
        lapCounter.lapsDone++;
        StartCoroutine(lapCounter.StartCooldown(lapCooldownTime));
        Debug.Log(other.gameObject.name + " ha COMPLETATO il giro " + lapCounter.lapsDone);

        // 4. VERIFICA VITTORIA/SCONFITTA
        if (other.CompareTag("Player"))
        {
            UpdateLapUI(lapCounter.lapsDone);

            if (lapCounter.lapsDone >= totalLaps)
            {
                Win();
                if(other.gameObject.activeInHierarchy) other.gameObject.SetActive(false);
            }
        }
        else if (other.CompareTag("OpponentCar"))
        {
            if (lapCounter.lapsDone >= totalLaps)
            {
                Lose();
            }
        }
    }

    void UpdateLapUI(int currentLapsDone)
    {
        if (lapText != null)
        {
            // Mostriamo il giro in cui ci si trova (1, 2, 3...)
            int lapToShow = currentLapsDone + 1;
            if (lapToShow > totalLaps) lapToShow = totalLaps;
            lapText.text = "Lap: " + lapToShow + " / " + totalLaps;
        }
    }

    void Win()
    {
        status.text = "YOU WIN";
        status.color = Color.green;
        StartCoroutine(ShowFinishUI());
    }

    void Lose()
    {
        status.text = "YOU LOSE";
        status.color = Color.red;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) player.SetActive(false);
        StartCoroutine(ShowFinishUI());
    }
/*
    IEnumerator ShowFinishUI()
    {
        finishUI.SetActive(true);
        playerUI.SetActive(false);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(5f);
        Time.timeScale = 0f;
    }
*/



IEnumerator ShowFinishUI()
    {
        // 1. Attiviamo la schermata finale
        if (finishUI != null) finishUI.SetActive(true);

        // 2. Disattiviamo TUTTA la UI di gioco
        // Cerchiamo e spegniamo i Canvas principali per essere sicuri
        GameObject pCanvas = GameObject.Find("PlayerCanvas");
        if (pCanvas != null) pCanvas.SetActive(false);

        GameObject tCanvas = GameObject.Find("TimerCanvas");
        if (tCanvas != null) tCanvas.SetActive(false);

        // Se hai assegnato manualmente playerUI nell'inspector, spegniamo anche quello
        if (playerUI != null) playerUI.SetActive(false);

        gameObject.GetComponent<BoxCollider>().enabled = false;
        
        yield return new WaitForSeconds(5f);
        Time.timeScale = 0f;
    }
    IEnumerator InitialColliderSafety()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}