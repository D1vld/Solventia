using UnityEngine;
using UnityEngine.Android;
using System;

public class PermissionRequest : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private void Start()
    {
        Debug.Log("?? Verificando permisos de almacenamiento...");

        // 1?? Pedir permisos de lectura y escritura en Android 10 o menor
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Debug.Log("?? Solicitando permiso READ_EXTERNAL_STORAGE...");
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("?? Solicitando permiso WRITE_EXTERNAL_STORAGE...");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        // 2?? Si es Android 11+ (API 30+), verificar "Manage External Storage"
        if (IsAndroid11OrHigher() && !HasManageExternalStoragePermission())
        {
            Debug.Log("?? Permiso MANAGE_EXTERNAL_STORAGE no concedido. Solicitándolo...");
            RequestManageStoragePermission();
        }
        else
        {
            Debug.Log("? Permiso MANAGE_EXTERNAL_STORAGE concedido.");
        }
    }
#endif

    private bool IsAndroid11OrHigher()
    {
        return new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT") >= 30;
    }

    private bool HasManageExternalStoragePermission()
    {
        try
        {
            AndroidJavaClass environmentClass = new AndroidJavaClass("android.os.Environment");
            return environmentClass.CallStatic<bool>("isExternalStorageManager");
        }
        catch (Exception ex)
        {
            Debug.LogError($"? Error verificando permisos de almacenamiento: {ex.Message}");
            return false;
        }
    }

    private void RequestManageStoragePermission()
    {
        try
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent",
                "android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION");

            string packageName = activity.Call<string>("getPackageName");
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "package:" + packageName);

            intent.Call<AndroidJavaObject>("setData", uri);
            activity.Call("startActivity", intent);

            Debug.Log("?? Se abrió la pantalla de permisos. El usuario debe habilitar el acceso a archivos.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"? Error al abrir la pantalla de permisos: {ex.Message}");
        }
    }
}
