using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if IAPENABLED
using UnityEngine.Purchasing;
#endif

/// <summary>
/// Handles IAP. 
/// Mandatory Requirements:
/// 1. In App Purchase must be enabled and imported from the Services Window.
/// 2. The preprocessor symbol "IAPENABLED" must be added to the project for the script to work.
/// 
/// The preprocessor is added to :
/// 1. make sure that there isn't a compile error if Unity IAP  is not imported
/// 2. To enable/disable IAP
/// The prepocessor can be enabled/disabled through the editor inspector of this script as well(Enable IAP checkbox).
/// </summary>
public class IAPManager : MonoBehaviour
{
    ///<summary>
    /// To enable/disable IAP.(from inspector)
    /// set this to true only if Unity IAP sdk is imported.
    /// </summary>
    [HideInInspector]
    public bool isIAPEnabled;

    ///<summary> listener for IAP events </summary>
#if IAPENABLED
    IAPManagerHandler IAPhander;
#endif

    ///<summary> Game identifier and product id from Google Play/App store console</summary>
    public string gameIdentifier = "com.PolyandCode.StackTheBlocks";
    public string NonConsumableProductID = "com.PolyandCode.StackTheBlocks.nonconsumable";

    ///<summary>
    /// Only required for App store
    /// Android will automatically configure IAP on reinstall of an app. For ios, user is required to manually do so.
    /// </summary>
    public Button restoreIAPBtn;

    ///<summary>
    /// Callbacks for successfula and failed purchase
    /// </summary>
    public UnityEvent OnPurchaseSuccessEvent, OnPurchaseFailedEvent;

    /// <summary>
    /// Initialize IAP and disable/enable the resotreIAP button accordign to platform
    /// </summary>
    private void Start()
    {
#if IAPENABLED
        IAPhander = new IAPManagerHandler();
        IAPhander.Init(gameIdentifier, NonConsumableProductID, OnPurchaseSuccessEvent, OnPurchaseFailedEvent);
#endif

        if (restoreIAPBtn != null)
        {
#if UNITY_IOS
        restoreIAPBtn.gameObject.SetActive(true);
        restoreIAPBtn.onClick.AddListener(RestorePurchases);
#else
            restoreIAPBtn.gameObject.SetActive(false);
#endif
        }

    }

    /// <summary>
    /// Buy the non consumable product.
    /// </summary>
    public void BuyNonConsumable()
    {
#if IAPENABLED
        IAPhander.BuyNonConsumable();
#endif
    }

    /// <summary>
    /// Retore IAP purchases on resintall.
    /// </summary>
    public void RestorePurchases()
    {
#if IAPENABLED
        IAPhander.RestorePurchases();
#endif
    }

}


#if IAPENABLED
public class IAPManagerHandler : IStoreListener
{

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public string m_NonConsumableProductID = ""; // non-consumable product ID from the store

    public UnityEvent m_OnPurchaseSuccessEvent, m_OnPurchaseFailedEvent; //to handle successful and failed purchase


    /// <summary>
    /// Init IAP .
    /// </summary>
    public void Init(string gameIdentifier, string nonConsumableProductID, UnityEvent onSuccessUnityEvent, UnityEvent onFailedUnityEvent)
    {
        m_NonConsumableProductID = gameIdentifier + "." + nonConsumableProductID;
        m_OnPurchaseSuccessEvent = onSuccessUnityEvent;
        m_OnPurchaseFailedEvent = onFailedUnityEvent;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder and add product.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(m_NonConsumableProductID, ProductType.NonConsumable);

        // Initialize UnityPurchasing. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }



    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(m_NonConsumableProductID);
    }



    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, m_NonConsumableProductID, StringComparison.Ordinal))
        {
            m_OnPurchaseSuccessEvent.Invoke();
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            m_OnPurchaseFailedEvent.Invoke();
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        m_OnPurchaseFailedEvent.Invoke();
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}

#endif
