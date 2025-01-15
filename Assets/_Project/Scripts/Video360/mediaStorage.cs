using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video; // Asegúrate de incluir este espacio de nombres

public class MediaStorage : MonoBehaviour
{
    // Clase que representa una imagen con su nombre
    [System.Serializable]
    public class NamedImage
    {
        public string name; // Nombre de la imagen
        public Sprite image; // Imagen (Sprite)
    }

    // Lista de imágenes con nombres
    [SerializeField] private List<NamedImage> images = new List<NamedImage>();

    // Lista de videos (VideoClips)
    [SerializeField] private List<UnityEngine.Video.VideoClip> videos = new List<UnityEngine.Video.VideoClip>();

    // Diccionario para acceso rápido a los videos
    private Dictionary<string, UnityEngine.Video.VideoClip> videoDictionary = new Dictionary<string, UnityEngine.Video.VideoClip>();

    private void Awake()
    {
        // Cargar videos en el diccionario
        foreach (var video in videos)
        {
            if (video != null && !videoDictionary.ContainsKey(video.name))
            {
                videoDictionary.Add(video.name, video);
            }
        }
    }

    // Método para agregar una imagen con nombre a la lista
    public void AddImage(string imageName, Sprite image)
    {
        var namedImage = new NamedImage { name = imageName, image = image };
        images.Add(namedImage);
    }

    // Método para obtener todas las imágenes con nombres
    public List<NamedImage> GetAllImages()
    {
        return images;
    }

    // Método para obtener un video por nombre
    public UnityEngine.Video.VideoClip GetVideoByName(string name)
    {
        if (videoDictionary.ContainsKey(name))
        {
            return videoDictionary[name];
        }
        Debug.LogWarning("Video no encontrado con el nombre especificado");
        return null;
    }

    // Método para obtener todos los videos
    public List<UnityEngine.Video.VideoClip> GetAllVideos()
    {
        return videos;
    }
}
