using System;
using System.Collections;
using GoogleTextToSpeech.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//sing UnityEngine.XR.Interaction.Toolkit.Transformers;
using System.Collections.Generic;

public class TextToSpeechExample : MonoBehaviour
{
    // Referencias serializadas configurables desde el Inspector de Unity
    [SerializeField] private VoiceScriptableObject voice; // Objeto scriptable que contiene la configuración de la voz
    [SerializeField] private TextToSpeech textToSpeech; // Componente que maneja la conversión de texto a voz
    [SerializeField] private AudioSource audioSource; // Componente que reproduce el audio
    [SerializeField] private TextMeshProUGUI inputField; // Campo de texto que contiene el texto a convertir a voz
    [SerializeField] private Toggle ToggleNarrar; // Toggle para activar o desactivar la narración
    [SerializeField] private Sprite playImage; // Imagen para el estado de "play"
    [SerializeField] private Sprite pauseImage; // Imagen para el estado de "pause"
    [SerializeField] private string contentName = "Content"; // Nombre del objeto hijo "Content"
    [SerializeField] private string backgroundName = "Background"; // Nombre del objeto hijo "Background"

    // Referencias privadas
    private Image toggleImage; // Referencia al componente Image del objeto "Background"
    private static TextToSpeechExample activeToggle; // Referencia al toggle activo para asegurar que solo uno esté activo a la vez
    private bool _isPlaying; // Indica si el audio está reproduciéndose
    private bool _repeat; // Indica si se debe repetir la reproducción

    void Start()
    {
        // Obtener las referencias a los componentes de audio y texto a voz
        audioSource = GameObject.Find("TEXT_TO_SPEECH_KEY").GetComponent<AudioSource>();
        textToSpeech = GameObject.Find("TEXT_TO_SPEECH_KEY").GetComponent<TextToSpeech>();

        // Buscar el componente "Image" del objeto "Background" dentro del objeto "Content" del toggle
        Transform contentTransform = ToggleNarrar.transform.Find(contentName);
        if (contentTransform != null)
        {
            Transform backgroundTransform = contentTransform.Find(backgroundName);
            if (backgroundTransform != null)
            {
                toggleImage = backgroundTransform.GetComponent<Image>();
            }
        }

        // Comprobar que se ha encontrado el componente "Image"
        if (toggleImage == null)
        {
            Debug.LogError("No se encontró un componente Image en el objeto Background.");
            return;
        }

        // Suscribir el método "OnToggleChanged" al evento de cambio de valor del toggle
        ToggleNarrar.onValueChanged.AddListener(OnToggleChanged);
    }

    // Método que se llama cuando el valor del toggle cambia
    public void OnToggleChanged(bool isOn)
    {
        Debug.Log($"OnToggleChanged called with isOn: {isOn}");
        if (isOn)
        {
            // Si hay un toggle activo diferente, desactivarlo
            if (activeToggle != null && activeToggle != this)
            {
                activeToggle.SetToggleToPlay();
            }

            // Si el mismo toggle se presiona nuevamente mientras está reproduciendo, pausar o reanudar el audio
            if (activeToggle == this && _isPlaying)
            {
                if (_repeat)
                {
                    PauseAudio();
                    _repeat = false;
                    Debug.Log("PAUSADO ");
                }
                else
                {
                    ResumeAudio();
                    _repeat = true;
                    Debug.Log("REANUDADO ");
                }
            }
            else
            {
                // Activar este toggle y empezar la reproducción
                activeToggle = this;
                SetToggleToPause();
                StartCoroutine(RequestAndPlayAudio());
            }
        }
        else
        {
            PauseAudio();
        }
    }

    // Cambia la imagen del toggle a "play" y detiene el audio
    private void SetToggleToPlay()
    {
        if (toggleImage != null)
        {
            toggleImage.sprite = playImage;
            toggleImage.color = Color.white; // Cambiar el color a blanco
            Debug.Log("SetToggleToPlay: Imagen cambiada a play");
        }
        ToggleNarrar.isOn = false;
        _isPlaying = false;
        Debug.Log("SetToggleToPlay: Estado de reproducción desactivado");
    }

    // Cambia la imagen del toggle a "pause"
    private void SetToggleToPause()
    {
        if (toggleImage != null)
        {
            toggleImage.sprite = pauseImage;
            toggleImage.color = Color.blue; // Cambiar el color a azul
            Debug.Log("SetToggleToPause: Imagen cambiada a pause");
        }
        _isPlaying = true;
        Debug.Log("SetToggleToPause: Estado de reproducción activado");
    }

    // Coroutine para solicitar el audio de Google TTS y reproducirlo
    private IEnumerator RequestAndPlayAudio()
    {
        _isPlaying = false;
        _repeat = false;

        bool audioReceived = false;
        // Solicitar el audio a Google TTS
        textToSpeech.GetSpeechAudioFromGoogle(inputField.text, voice, clip =>
        {
            if (clip == null)
            {
                Debug.LogError("AudioClipReceived: Error al cargar el clip de audio");
                SetToggleToPlay();
            }
            else
            {
                audioSource.clip = clip;
                audioSource.Play();
                StartCoroutine(WaitForAudioToEnd(clip.length));
                Debug.Log("AudioClipReceived: Audio clip recibido y reproducción iniciada");
            }
            audioReceived = true;
        }, badRequestData =>
        {
            Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
            SetToggleToPlay();
            audioReceived = true;
        });

        // Esperar a que se reciba el audio
        while (!audioReceived)
        {
            yield return null;
        }

        _isPlaying = true;


    }

    // Pausa el audio y cambia la imagen del toggle a "play"
    private void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            SetToggleToPlay();
            Debug.Log("PauseAudio: Audio pausado");
        }
    }

    // Reanuda el audio desde donde se pausó y cambia la imagen del toggle a "pause"
    private void ResumeAudio()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
            SetToggleToPause();
            StartCoroutine(WaitForAudioToEnd(audioSource.clip.length - audioSource.time));
            Debug.Log("ResumeAudio: Audio reanudado");
        }
    }

    // Coroutine que espera el tiempo restante del clip de audio y cambia la imagen del toggle a "play" cuando el audio termina
    private IEnumerator WaitForAudioToEnd(float remainingTime)
    {
        Debug.Log("WaitForAudioToEnd: Coroutine iniciada");
        yield return new WaitForSeconds(remainingTime);
        if (_isPlaying && !audioSource.isPlaying)
        {
            SetToggleToPlay();
            Debug.LogWarning("WaitForAudioToEnd: AUDIO TERMINADO: CAMBIO A PLAY");
        }
    }
}
