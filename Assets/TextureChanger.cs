using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureChanger : MonoBehaviour
{
    Renderer _renderer;
    [SerializeField] int materialNum = 1;
    [SerializeField] Texture texture;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        CollectableManager.instance.collectedAll.AddListener(ChangeTexture);
    }

    public void ChangeTexture(){
        _renderer.materials[materialNum].SetTexture("_mainTexture", texture);
    }

}
