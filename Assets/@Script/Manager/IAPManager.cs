using System;
using MewVivor.Enum;
using UnityEngine;
using UnityEngine.Purchasing;

namespace MewVivor
{
    [Serializable]
    public struct ReceiptData
    {
        public string os;
        public string bundleId; // "com.gomblegames.mew"
        public string productId;
        public string transactionId;
    }

    public class IAPManager : IStoreListener
    {
        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;

        private Action<ReceiptData?> _onPurchasedSucceededCallback;
        private Action _onPurchasedFailedCallback;
        
        public void Initialize()
        {
            if (storeController == null)
                InitializePurchasing();
        }

        private void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct(ShopIAPIdType.shop_huntpass.ToString(), ProductType.Consumable);
            
            builder.AddProduct(ShopIAPIdType.shop_jewel_100.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_jewel_500.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_jewel_3000.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_jewel_5000.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_jewel_10000.ToString(), ProductType.Consumable);
            
            builder.AddProduct(ShopIAPIdType.shop_pack_1.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_pack_2.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_pack_3.ToString(), ProductType.Consumable);
            
            builder.AddProduct(ShopIAPIdType.shop_vip.ToString(), ProductType.Consumable);
            builder.AddProduct(ShopIAPIdType.shop_vvip.ToString(), ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public string GetProductPriceString(ShopIAPIdType shopIAPIdType)
        {
            foreach (var product in storeController.products.all)
            {
                if (product.definition.id == shopIAPIdType.ToString())
                {
                    return $"{product.metadata.localizedPriceString}";
                }
            }
            
            Debug.LogError($"Error {shopIAPIdType}");
            return "Error";
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    break;
            }

            Debug.LogError($"{nameof(OnInitializeFailed)} / {error} / message {message}");
        }

        public void BuyProduct(ShopIAPIdType shopIdType, Action<ReceiptData?> onPurchasedSucceededCallback, Action onPurchasedFailedCallback, bool isTest)
        {
            string productId = shopIdType.ToString();
            if (productId == null)
            {
                return;
            }
            
            if (storeController.products.WithID(productId) == null)
            {
                return;
            }

            if (isTest)
            {
                string purchasedProductId = null;
                string os = "";
#if  UNITY_EDITOR
                os = OS.EDITOR.ToString();
#elif UNITY_ANDROID
                os = OS.AOS.ToString();
#elif UNITY_IOS || UNITY_IPHONE
                os = OS.IOS.ToString();
#endif
                string bundleId = Application.identifier; // "com.gomblegames.mew"
                string transactionId = "";

                ReceiptData receiptData = new ReceiptData
                {
                    os = os,
                    bundleId = bundleId,
                    productId = "",
                    transactionId = transactionId
                };

                onPurchasedSucceededCallback.Invoke(receiptData);
                return;
            }

            _onPurchasedSucceededCallback = onPurchasedSucceededCallback;
            _onPurchasedFailedCallback = onPurchasedFailedCallback;
            BuyProductID(productId);
        }

        private void BuyProductID(string productId)
        {
            if (storeController != null && storeController.products != null)
            {
                Product product = storeController.products.WithID(productId);
                Debug.Log($"product :" + product != null);
                Debug.Log("available :" + product.availableToPurchase);
                if (product != null && product.availableToPurchase)
                {
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("Product not found or not available");
                }
            }
        }
        
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string purchasedProductId = args.purchasedProduct.definition.id;
            Product product = args.purchasedProduct;
            string os = Application.platform == RuntimePlatform.Android ? OS.AOS.ToString() : OS.IOS.ToString();
#if  UNITY_EDITOR || UNITY_ANDROID
            os = OS.EDITOR.ToString();
#endif
            string bundleId = Application.identifier; // "com.gomblegames.mew"
            string productId = product.definition.id;
            string transactionId = product.transactionID;

            ReceiptData receiptData = new ReceiptData
            {
                os = os,
                bundleId = bundleId,
                productId = productId,
                transactionId = transactionId
            };

            Debug.Log($"[IAP] 구매 성공: {purchasedProductId}");
            _onPurchasedSucceededCallback?.Invoke(receiptData);
            return PurchaseProcessingResult.Complete;
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            storeExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error) =>
            Debug.Log($"IAP 초기화 실패: {error}");

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"구매 실패: {failureReason}");
            _onPurchasedFailedCallback?.Invoke();
        }
    }

}