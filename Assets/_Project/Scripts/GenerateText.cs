using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateText : MonoBehaviour
{
    #region Variables 
    // Referencias a los componentes necesarios
    [Tooltip("Donde quieres que se se lea lo que hay escrito")]
    public InputField inputField;  
    [Tooltip("Donde quieres que pongan los objetos que se creen")]
    public GameObject parentObject;
    public RectTransform parentRectTransform;  // Referencia al RectTransform del contenedor de mensajes
    public ScrollRect scrollRect;  // Referencia al componente ScrollRect
    
    // Prefabs para cada tipo de mensaje
    [Header("Mensajes")]
    public GameObject mensajeUsuarioPrefab;
    public GameObject mensajeIAPrefab;
    public GameObject imgIAPrefab;
    public GameObject imgIAPrefabbuttom; // Asignar en el Inspector

    // Arreglo de Sprites
    [Header("Imagenes, Titulos, Descripciones")]
    public Sprite[] hotelImages; // Asignar en el Inspector

    // Variables para el título y la descripción
    public string[] hotelTitles;  // Array para los nombres de los hoteles
    public string[] hotelDescriptions;  // Array para las descripciones de los hoteles

    private string storedText;
    private int messageCount = 0;  // Contador de mensajes
    #endregion

    void Update()
    {
        // Comprueba si se ha pulsado la tecla Enter para mensaje de usuario
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PrintInputFieldText();
        }
        // Comprueba si se ha pulsado la tecla U para mensaje de IA
        else if (Input.GetKeyDown(KeyCode.U))
        {
            CreateMensajeIA(inputField.text);
        }
        // Comprueba si se ha pulsado la tecla I para imagen de IA
        else if (Input.GetKeyDown(KeyCode.I))
        {
            CreateImgIA(0); // Usa un índice por defecto
        }
    }
    #region  Funciones
    public void PrintInputFieldText()
    {
        // Obtener el texto del InputField
        storedText = inputField.text;

        // Crear mensaje de usuario
        CreateMensajeUsuario(storedText);

        // Limpiar el texto del InputField
        inputField.text = string.Empty;

        // Desglosar el texto almacenado y responder en consecuencia
        RespondToStoredText();
    }

    void CreateMensajeUsuario(string message)
    {
        // Genera un nuevo mensaje de usuario solo si el campo de entrada no está vacío
        if (!string.IsNullOrEmpty(message))
        {
            GameObject newTextObject = Instantiate(mensajeUsuarioPrefab, parentObject.transform);
            TMP_Text newText = newTextObject.GetComponentInChildren<TMP_Text>();  // Usar TMP_Text
            if (newText != null)
            {
                newText.text = message;
            }
            else
            {
                Debug.LogError("El componente TMP_Text no se encontró en el prefab de mensaje de usuario.");
            }
            IncrementMessageCount(1, 1);
        }
    }

    void CreateMensajeIA(string message)
    {
        // Genera un nuevo mensaje de IA
        if (!string.IsNullOrEmpty(message))
        {
            GameObject newTextObject = Instantiate(mensajeIAPrefab, parentObject.transform);
            TMP_Text newText = newTextObject.GetComponentInChildren<TMP_Text>();  // Usar TMP_Text
            if (newText != null)
            {
                newText.text = message;
            }
            else
            {
                Debug.LogError("El componente TMP_Text no se encontró en el prefab de mensaje IA.");
            }
            IncrementMessageCount(1, 1);
        }
    }

    void CreateImgIA(int index)
    {
        // Genera una nueva imagen de IA, asigna el sprite especificado y actualiza el texto del hotel
        GameObject newImageObject = Instantiate(imgIAPrefabbuttom, parentObject.transform);

        // Obtener todos los componentes de tipo Image
        Image[] images = newImageObject.GetComponentsInChildren<Image>();

        // Verificar que hay al menos cuatro componentes de tipo Image
        if (images.Length >= 4)
        {
            if (index >= 0 && index < hotelImages.Length)
            {
                images[3].sprite = hotelImages[index]; // Asignar el sprite al cuarto componente la imagen al lado de la descripcion
            }
            else
            {
                Debug.LogError("Índice de imagen fuera de rango.");
            }
        }
        else
        {
            Debug.LogError("No se encontraron suficientes componentes Image en el prefab.");
        }

        TMP_Text[] textComponents = newImageObject.GetComponentsInChildren<TMP_Text>();
        if (textComponents.Length >= 2)
        {
            textComponents[0].text = hotelTitles.Length > index ? hotelTitles[index] : "Nombre del Hotel";
            textComponents[1].text = hotelDescriptions.Length > index ? hotelDescriptions[index] : "Descripción del Hotel";
        }
        else
        {
            Debug.LogError("No se encontraron los componentes TMP_Text necesarios en el prefab.");
        }

        // Accede al componente Toggle y modifica el evento OnValueChanged
        Toggle toggle = newImageObject.GetComponentInChildren<Toggle>();
        if (toggle != null)
        {
            // Elimina todos los listeners existentes para evitar duplicaciones
            toggle.onValueChanged.RemoveAllListeners();

            // Añade un nuevo listener que llame a la función con el índice correcto
            toggle.onValueChanged.AddListener(delegate {
                SetHotelIndex(index);
            });
        }
        else
        {
            Debug.LogError("El componente Toggle no se encontró en el prefab.");
        }

        // Accede al componente WindowInteractor y modifica el valor de prefabIndex
        WindowInteractor windowInteractor = newImageObject.GetComponentInChildren<WindowInteractor>();
        if (windowInteractor != null)
        {
            windowInteractor.prefabIndex = index;
        }
        else
        {
            Debug.LogError("El componente WindowInteractor no se encontró en el prefab.");
        }

        IncrementMessageCount(2, 2);
    }

    // Función que se llama cuando el Toggle cambia de valor
    public void SetHotelIndex(int index)
    {
        Debug.Log("Toggle cambiado al índice: " + index);
        // Aquí puedes añadir la lógica que necesites para manejar el cambio de índice
    }
    void IncrementMessageCount(int caseNumber, int incrementAmount)
    {
        // Incrementar el contador de mensajes por la cantidad especificada
        messageCount += incrementAmount;
        if (messageCount == 5 && incrementAmount == 2 )
        {
            caseNumber = 1;
        }
        
        

        // Verificar si el contador de mensajes supera el umbral
        if (messageCount > 6)
        {
            MoveContainerUpwards(caseNumber);
        }
    }

    void MoveContainerUpwards(int caseNumber)
    {
        // Verificar el caso recibido
        switch (caseNumber)
        {
            case 1:
                // Caso 1: Mover el contenedor de mensajes hacia arriba con y += 255
                Vector2 newPosition1 = parentRectTransform.anchoredPosition;
                newPosition1.y += 255; // Ajusta el valor según sea necesario
                parentRectTransform.anchoredPosition = newPosition1;
                break;

            case 2:
                // Caso 2: Mover el contenedor de mensajes hacia arriba con y += 510
                Vector2 newPosition2 = parentRectTransform.anchoredPosition;
                newPosition2.y += 510; // Ajusta el valor según sea necesario
                parentRectTransform.anchoredPosition = newPosition2;
                break;
        }
    }
    #endregion

    #region  Chat
    void RespondToStoredText()
    {
        string[] words = storedText.ToLower().Split(' ');

        // Palabras clave para hoteles
        string[] hotelWords = { "baobab", "melonera", "villa", "conde", "faro",  "dunamar", "buenaventura", "catarina", "continental", "interclub", "beach", "corallium", "abora"};

        // Palabras clave para otras categorías
        string[] otherWords = { "maspalomas", "canaria", "norte", "familia", "descuento", "valorado",
                                "relajante", "relajacion", "lopesan", "atraccion", "turistica", "actividades", "triana", "niños" , /*"sur",*/ "oferta", "mejor", "spa", "masaje", "golf",
                                "superior", "hijos","playa", "costa", "vegueta", "palmas", "infantil", "asequible", "economico", "cumbre","rebajado", "modico", "barato",
                                "valoracion", "opinion", "opiniones", "gimnasio", "vegetariano", "lujo", "todo", "incluido", "desayuno"/*, "hotel"*/};
                                
        // Si no se encuentra una frase específica, procesar palabra por palabra
        foreach (string word in words)
        {
            foreach (string hotelWord in hotelWords)
            {
                if (StringUtils.IsSimilar(word, hotelWord, 1))
                {
                    switch (hotelWord)
                    {
                        case "baobab":
                            CreateImgIA(0); // baobab
                            return;
                        case "melonera":
                            CreateImgIA(1); // melonera
                            return;
                        case "villa":
                        case "conde":
                            CreateImgIA(2); // villa del conde
                            return;
                        case "faro":
                            CreateImgIA(3); // faro
                            return;
                        case "dunamar":
                            CreateImgIA(4); // corallium dunamar
                            return;
                        case "buenaventura":
                            CreateImgIA(7); // buenaventura
                            return;
                        case "catarina":
                            CreateImgIA(6); // catarina
                            return;
                        case "continental":
                            CreateImgIA(5); // continental
                            return;
                        case "interclub":
                            CreateImgIA(9); // interclub
                            return;
                        case "beach":
                            CreateImgIA(8); // beach
                            return;
                        case "corallium":
                            CreateMensajeIA("Podemos mostrar los siguientes hoteles relacionado a la marca Corallium, el hotel Corallium dunamar y el hotel Corallium beach");
                            return;
                        case "abora":
                            CreateMensajeIA("Podemos mostrar los siguientes hoteles relacionados a la marca Abora, el hotel Abora buenaventura, el hotel Abora Catarina, el hotel Abora continental y el hotel Abora interclub");
                            return;    
                    }
                }
            }
        }

        foreach (string word in words)
        {
            foreach (string otherWord in otherWords)
            {
                if (StringUtils.IsSimilar(word, otherWord, 2))
                {
                    switch (otherWord)
                    {
                        case "maspalomas":
                            Debug.LogError("Maspalomas");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "meloneras":
                            Debug.LogError("Meloneras");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "lopesan":

                            Debug.LogError("lopesan");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "sur":
                            Debug.LogError("sur");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "superior":
                            Debug.LogError("superior");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "canaria":
                            Debug.LogError("canaria");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "atraccion":
                            Debug.LogError("atraccion");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "turistica":
                            Debug.LogError("turisticas");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "actividades":
                            Debug.LogError("actividades");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "playa":
                            Debug.LogError("playa");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;
                        case "costa":
                            Debug.LogError("costa");
                            CreateMensajeIA("Algunos de los mejores hoteles de tu categoría de búsqueda serían los que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            CreateImgIA(1); // melonera
                            CreateImgIA(0); // baobab
                            return;

                        case "norte":
                        case "triana":
                        case "vegueta":
                        case "palmas":
                        case "rural":
                        case "campo":
                        case "cumbre":
                        case "rustico":
                            CreateMensajeIA("Lo siento, ahora mismo me encuentro en fase de desarrollo y no dispongo de hoteles para mostrar en esta zona.");
                            return;

                        case "niños":
                        case "hijos":
                        case "familia":
                        case "infantil":
                            CreateMensajeIA("Te presento uno de los mejores hoteles de Gran Canaria para disfrutar de unas vacaciones en familia.");
                            CreateImgIA(0); // baobab
                            return;

                        case "descuento":
                        case "oferta":
                        case "economico":
                        case "asequible":
                        case "rebajado":
                        case "modico":
                        case "barato":
                            CreateMensajeIA("El hotel con mejor precio y que más se asemeja a lo que estás buscando es el que te muestro a continuación.");
                            CreateImgIA(4); // corallium dunamar
                            return;

                        case "mejor":
                        case "valorado":
                        case "valoracion":
                        case "opinion":
                            CreateMensajeIA("El hotel con mejor precio y que más se asemeja a lo que estás buscando es el hotel Corallium Dunamar, que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            return;

                        case "relajacion":
                        case "relajante":
                        case "spa":
                        case "masaje":
                        case "golf":
                        case "gimnasio":
                        case "vegetariano":
                        case "lujo":
                        case "todo":
                        case "incluido":
                        case "desayuno":
                            CreateMensajeIA("El hotel que más se asemeja a lo que estás buscando es el que te muestro a continuación.");
                            CreateImgIA(2); // villa del conde
                            return;

                        case "hotel":
                            CreateMensajeIA("Podrías decirme dónde quieres que esté ubicado el hotel.");
                            return;
                    }
                }
            }
        }

    // Si no se encuentra nada relevante
    CreateMensajeIA("Actualmente estoy en fase de desarrollo y solo puedo mostrar hoteles.");
    }

    

    public void Prompt(string value)
    {
        switch (value.ToLower())
        {
            case "1":
                //CreateMensajeIA("Podrías decirme dónde quieres que esté ubicado el hotel.");

                CreateImgIA(0); // baobab 
                CreateImgIA(1); // melonera
                CreateImgIA(2); // villa del conde
                CreateImgIA(3); // faro               
                CreateImgIA(4); // corallium dunamar  
                CreateImgIA(5); // continental
                CreateImgIA(6); // catarina
                CreateImgIA(7); // buenaventura
                CreateImgIA(8); // beach 
                CreateImgIA(9); // interclub
                return;

            case "2":
                CreateImgIA(0); // baobab
                return;

            case "3":
                CreateImgIA(1); // melonera
                return;

            default:
                CreateMensajeIA("No se ha escrito bien el prompt");
                return;
        }
    }
    #endregion
}