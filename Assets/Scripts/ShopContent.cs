using System.Collections.Generic;
using UnityEngine;

namespace Forest
{
    [CreateAssetMenu(fileName = "Shop", menuName = "Forest/New Shop")]
    public class ShopContent : ScriptableObject
    {
        public List<ShopItem> Items;
    }
}
