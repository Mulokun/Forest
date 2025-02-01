using UnityEngine;
using TMPro;
using Argon.UI;

namespace Forest
{
    [RequireComponent(typeof(ButtonExtended))]
    public class ShopSlot : MonoBehaviour
    {
        private GameController controller;
        private bool isEventAdded = false;

        private ShopItem item = null;
        private int currentRank = 0;

        [SerializeField] private ButtonExtended button;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemPrice;

        private double price => item.GetPrice(currentRank);
        private bool isUnlocked => item.UnlockCondition.GetResult(controller.Game);
        private bool isAffordable => item != null && price <= controller.Game[GameVariables.Seeds].ModifiedValue;

        public void SetItem(ShopItem item, GameController game)
        {
            controller = game;

            this.item = item;
            currentRank = 0;

            UpdateInfo();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(TryBuy);
        }

        protected void OnEnable()
        {
            if (controller)
            {
                controller.Game[GameVariables.Seeds].OnUpdate += CheckIfAffordable;
                isEventAdded = true;
            }
        }

        protected void OnDisable()
        {
            if (controller && isEventAdded)
            {
                controller.Game[GameVariables.Seeds].OnUpdate -= CheckIfAffordable;
                isEventAdded = false;
            }
        }

        private void CheckIfAffordable(double seeds, double previousSeeds)
        {
            button.interactable = isAffordable;
        }

        public void UpdateInfo()
        {
            itemName.text = $"{item.Name} - Rank {currentRank + 1}";
            itemPrice.text = price + " Seeds";
            CheckIfAffordable(0, 0);
        }

        public void TryBuy()
        {
            if (controller.TryBuy(item, currentRank))
            {
                currentRank++;
                UpdateInfo();
                if (currentRank >= item.MaximumRank)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
