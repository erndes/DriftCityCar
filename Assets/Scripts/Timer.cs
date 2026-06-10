using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    public float countDownTimer = 5f;

    [Header("Things to stop")]
    // Cambiato in array per gestire più auto del giocatore
    public PlayerCarController[] playerCarControllers; 
    public OpponentCar[] opponentCars;

    [Header("Animations")]
    public Animator[] wheelAnimators; 

    public TextMeshProUGUI countDownText;

    void Start()
    {
        // Recuperiamo l'indice salvato nel garage (default a 0 se non esiste)
    int selectedIndex = PlayerPrefs.GetInt("CarSelected", 0);

    // Cicliamo l'array delle auto che hai nell'Inspector
    for (int i = 0; i < playerCarControllers.Length; i++)
    {
        if (playerCarControllers[i] != null)
        {
            // Attiva l'auto se l'indice corrisponde, altrimenti disattivala
            playerCarControllers[i].gameObject.SetActive(i == selectedIndex);
        }
    }

    SetAnimationsSpeed(0f);
    StartCoroutine(TimeCount());

    }

    void Update()
    {
        if(countDownTimer > 0)
        {
            // Blocca il movimento di TUTTE le auto del giocatore
            foreach(PlayerCarController pCar in playerCarControllers) {
                if(pCar != null) pCar.accelerationForce = 0f;
            }

            // Blocca il movimento di TUTTE le auto avversarie
            foreach(OpponentCar car in opponentCars) {
                if(car != null) car.movingSpeed = 0f;
            }

            SetAnimationsSpeed(0f); 
        }
        else
        {
            // Sblocca il movimento di TUTTE le auto del giocatore
            foreach(PlayerCarController pCar in playerCarControllers) {
                if(pCar != null) pCar.accelerationForce = 300f; // Assicurati che 300 sia il valore giusto per te
            }
            
            // Impostiamo velocità diverse per gli avversari
            if (opponentCars.Length >= 5) {
                opponentCars[0].movingSpeed = 13f;
                opponentCars[1].movingSpeed = 14.5f;
                opponentCars[2].movingSpeed = 16f;
                opponentCars[3].movingSpeed = 10f;
                opponentCars[4].movingSpeed = 9f;
            }

            SetAnimationsSpeed(1f); 
        }
    }

    void SetAnimationsSpeed(float speed)
    {
        foreach (Animator anim in wheelAnimators)
        {
            if (anim != null) anim.speed = speed;
        }
    }

    IEnumerator TimeCount()
    {
        while(countDownTimer > 0)
        {
            countDownText.text = countDownTimer.ToString("0");
            yield return new WaitForSeconds(1f);
            countDownTimer--;
        }

        countDownText.text = "GO";
        yield return new WaitForSeconds(1f);
        countDownText.gameObject.SetActive(false);
    }
}
