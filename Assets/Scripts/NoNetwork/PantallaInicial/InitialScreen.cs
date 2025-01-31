using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InitialScreen : MonoBehaviour
{
    SaveData saveData;
    [SerializeField] GameObject characterContainer;
    [SerializeField] List<CharacterItem> characters;
    CharacterItem actualCharacter;
    [SerializeField] List<ObjectItem> shoes;
    [SerializeField] GameObject treadmillPrefab;
    [SerializeField] TMP_InputField inputName;
    private void Start()
    {
        saveData = GetComponent<SaveData>();
        ReadUsername();
        ReadCharacter();
        ResetScenesPlayed();
        if (RaceManager.Instance != null) RaceManager.Instance.Reset();
    }

    void ReadUsername()
    {
        if (saveData.player.username == "Username") return;
        else
        {
            inputName.text = saveData.player.username;
        }
    }
    void ReadCharacter()
    {
        //Leer la skin
        string savedSkin = saveData.player.playerCharacterData.characterName;
        if (savedSkin == null)
        {
            print("Error: No hay personaje guardado");
            //instanciar a juan
            Instantiate(characters[0].prefab, Vector3.zero, Quaternion.identity, characterContainer.transform);
            return;
        }
        //Buscar personaje
        actualCharacter = characters.Find(character => character.characterName == savedSkin);

        //Instanciar la cinta de correr
        GameObject treadmill = Instantiate(treadmillPrefab, Vector3.zero, Quaternion.identity, characterContainer.transform);

        //Instanciar el personaje como hijo de la cinta
        DestroyImmediate(characterContainer.transform.GetChild(0).gameObject);
        GameObject characterInstance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, treadmill.transform);
        characterInstance.GetComponent<Animator>().SetBool("isRunning", true);
        characterInstance.GetComponent<Outline>().enabled = false;

        UpdateShoes();
        UpdateColors();

        //Colocar a personaje adecuadamente en la cinta
        characterInstance.transform.Rotate(transform.up, 180f);
        characterInstance.transform.position = new Vector3(0, 0.54f, 1.6f);

        //Para alejarlo un poco de la camara
        characterContainer.transform.position = new Vector3(0, 0, -2.91f);
        characterContainer.transform.Rotate(transform.up, 120f);
    }

    void UpdateShoes()
    {
        //Actualizar zapatillas
        GameObject zapatos = GameObject.FindGameObjectWithTag("Shoes");
        SkinnedMeshRenderer renderer = zapatos.GetComponent<SkinnedMeshRenderer>();
        int i = saveData.player.playerCharacterData.shoes;

        //  Debug.Log($"ANTES: Zapato GO: {zapatos.name}. Mesh rendered: {renderer.sharedMesh}. ActualShoe id: {i}");

        foreach (ObjectItem shoeItem in shoes)
        {
            if (shoeItem.id == i)
            {
                renderer.sharedMesh = shoeItem.mesh;
                renderer.materials = shoeItem.materials;
                break;
            }
        }

        //  Debug.Log($"DESPUES: Zapato GO: {zapatos.name}. Mesh rendered: {renderer.sharedMesh}. ActualShoe id: {i}");
    }

    void UpdateColors()
    {
        Color color = Color.black; //si falla saldr� negro
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.hairColor, out color))
        {
            actualCharacter.hair.color = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.skinColor, out color))
        {
            actualCharacter.skin.color = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.bottomColor, out color))
        {
            actualCharacter.bottom.color = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.topColor, out color))
        {
            actualCharacter.top.color = color;
        }
    }

    public void SaveUsername(string value)
    {
        saveData.player.username = value;
        saveData.SaveToJson();
    }

    public void ResetScenesPlayed()
    {
        saveData.player.scenesPlayed.Clear();
        saveData.SaveToJson();
    }
}
