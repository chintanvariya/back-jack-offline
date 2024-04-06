using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace BlackJackOffline
{
    public class BlackjackProduct : MonoBehaviour
    {
        [SerializeField]
        private Image Icon;
        [SerializeField]
        private Text PriceText, discriptionText;
        [SerializeField]
        private Button PurchaseButton;

        public delegate void PurchaseEvent(Product Model, Action OnComplete);
        public event PurchaseEvent OnPurchase;

        private Product Model;

        public void Setup(Product Product)
        {
            Model = Product;
            PriceText.text = $"{Product.metadata.localizedPriceString} ";

            Debug.Log($"------------> {Product.metadata.localizedPriceString}");
            Debug.Log($"------------> {Product.metadata.localizedDescription}");
            // +$"{Product.metadata.isoCurrencyCode}");
            discriptionText.text = $"{Product.metadata.localizedDescription}";
            Texture2D texture = null;//= BlackjackStoreIconProvider.GetIcon(Product.definition.id);
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.one / 2f
                );

                Icon.sprite = sprite;
            }
            else
            {
                Debug.LogError($"No Sprite found for {Product.definition.id}!");
            }
        }

        public void Purchase()
        {
            PurchaseButton.enabled = false;
            OnPurchase?.Invoke(Model, HandlePurchaseComplete);
        }

        private void HandlePurchaseComplete()
        {
            PurchaseButton.enabled = true;
        }
    }
}