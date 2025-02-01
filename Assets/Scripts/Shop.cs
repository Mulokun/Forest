using UnityEngine;

namespace Forest
{
    public class Shop : MonoBehaviour
    {
        private GameController controller;

        [SerializeField] private ShopContent content;
        [SerializeField] private ShopSlot slotPrefab;
        [SerializeField] private Transform slotContainer;

        private Pooler<ShopSlot> slotPooler;
        private UnlockManager<ShopItem, GameContext> shopUnlock;

        protected void Awake()
        {
            slotPooler = new(slotPrefab, 3);
        }

        public void Initialize(GameController game)
        {
            controller = game;

            shopUnlock = new(content.Items, controller.Game);
            shopUnlock.OnUnlock += UnlockItem;

            for (int i = 0; i < controller.Game.Variables.Count; i++)
            {
                controller.Game.Variables[i].OnUpdate += CheckUnlocks;
            }
        }

        private void CheckUnlocks(double _, double __)
        {
            shopUnlock.CheckUnlocks();
        }

        private void UnlockItem(ShopItem item)
        {
            ShopSlot s = slotPooler.Borrow();
            s.SetItem(item, controller);
            s.transform.SetParent(slotContainer, false);
            // s.transform.localScale = Vector3.one;
            s.gameObject.SetActive(true);
        }
    }
}
