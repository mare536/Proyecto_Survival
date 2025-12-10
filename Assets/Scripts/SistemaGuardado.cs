using UnityEngine;
using System.IO;

public static class SistemaGuardado
{
    // Devuelve la ruta: .../PersistentData/save_slot_0.json
    private static string GetRuta(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"partida_slot_{slot}.json");
    }

    public static void GuardarPartida(DatosJuego datos, int slot)
    {
        string json = JsonUtility.ToJson(datos, true); // 'true' para que el texto sea legible
        File.WriteAllText(GetRuta(slot), json);
        Debug.Log($"Partida guardada en Slot {slot}");
    }

    public static DatosJuego CargarPartida(int slot)
    {
        string ruta = GetRuta(slot);
        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            DatosJuego datos = JsonUtility.FromJson<DatosJuego>(json);
            return datos;
        }
        else
        {
            Debug.LogWarning($"No existe partida en el Slot {slot}");
            return null;
        }
    }

    public static bool ExistePartida(int slot)
    {
        return File.Exists(GetRuta(slot));
    }
}