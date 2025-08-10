using UnityEngine;
using UnityEngine.UI;

public class CollectedItemsDisplay : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _container;
    [SerializeField] private ItemDisplay _itemDisplayPrefab;

    private PlayerController _player;

    public void Init(PlayerController player)
    {
        _player = player;

        _player.ItemCollected += OnItemCollected;
    }

    private void OnDestroy()
    {
        _player.ItemCollected -= OnItemCollected;
    }

    private void OnItemCollected(Item item)
    {
        ItemDisplay itemDisplay = Instantiate(_itemDisplayPrefab, _container.transform);
        itemDisplay.Init(item.Icon);
    }
}
