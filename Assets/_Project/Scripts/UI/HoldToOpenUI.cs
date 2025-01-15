using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoldToOpenUI : MonoBehaviour
{
    public GameObject UIPrefab; // Prefab de la UI que queremos spawnear
    public float holdTime = 2f; // Tiempo que tenemos que mantener para spawnear la UI
    public Transform spawnPosition; // Posición donde se spawneará la UI
    public Vector3 rotationAngle = new Vector3(0, 30, 0); // Ángulo de rotación para la UI
    public Vector3 offset = new Vector3(1, 0, 0); // Offset para spawnnear al lado

    private bool isHolding = false;
    private bool uiSpawned = false; // Variable para verificar si ya se ha spawneado una UI
    private float timer = 0f;

    void Update()
    {
        if (isHolding && !uiSpawned)
        {
            timer += Time.deltaTime;
            if (timer >= holdTime)
            {
                SpawnUI();
                uiSpawned = true; // Marcar como UI spawneada
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void StartHold()
    {
        isHolding = true;
    }

    public void EndHold()
    {
        isHolding = false;
    }

    void SpawnUI()
    {
        if (UIPrefab != null && spawnPosition != null && !uiSpawned)
        {
            Vector3 spawnPos = spawnPosition.position + offset;
            GameObject spawnedUI = Instantiate(UIPrefab, spawnPos, spawnPosition.rotation);
            spawnedUI.transform.Rotate(rotationAngle); // Aplicar rotación adicional
        }
    }
}