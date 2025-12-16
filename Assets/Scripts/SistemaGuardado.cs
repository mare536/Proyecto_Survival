using UnityEngine;
using System.IO;

public static class SistemaGuardado 
{
    public static bool ExistePartida(int slot){
        string ruta = Application.persistentDataPath + "/datosJuego_" + slot + ".json";
        return File.Exists(ruta);
    }
    public static void GuardarPartida(DatosJuego datos, int slot)
    {
        string ruta = Application.persistentDataPath + "/datosJuego_" + slot + ".json";
        string json = JsonUtility.ToJson(datos);
        File.WriteAllText(ruta, json);
    }

    public static DatosJuego CargarPartida(int slot)
    {
        string ruta = Application.persistentDataPath + "/datosJuego_" + slot + ".json";
        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            return JsonUtility.FromJson<DatosJuego>(json);
        }
        return null;
    }

    public static void BorrarPartida(int slot)
    {
        string ruta = Application.persistentDataPath + "/datosJuego_" + slot + ".json";
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
            Debug.Log("Datos borrados del slot " + slot);
        }
    }
}