using UnityEngine;

[CreateAssetMenu(menuName = nameof(MenuLayoutContent), fileName = nameof(MenuLayoutContent))]
public class MenuLayoutContent : ScriptableObject
{
    [SerializeField] MenuLayoutItem[] _Items;
    public MenuLayoutItem[] Items => _Items;
}

[System.Serializable]
public class MenuLayoutItem
{
    public string Name;
    public Sprite Icon;
}