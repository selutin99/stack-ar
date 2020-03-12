using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For self disabling instantiated and pooled items
/// </summary>
public class SelfDisable : MonoBehaviour
{
    [Tooltip("Will make the Gameobject's Rigidbody kinematic after this times")]
    [SerializeField]
    private int makeKinematicTime = default;

    [Tooltip("Will disable the Gameobject afer this time")]
    [SerializeField]
    private int disableTime = default;

    /// <summary>
    /// Invokes the makekinematic/ disable or both when gameobjects enable
    /// </summary>

    private Rigidbody rbody;


    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //Not disabling fallen objects in AR as they are visible
        if (GameManager.Instance.gameMode.isARModeOn && rbody)
        {
            rbody.isKinematic = false;
            Invoke("UnParent", 2);
            return;
        }

        if (rbody)
        {
            rbody.isKinematic = false;
            //Making the leftover objects kinematic because some fall on top of other blocks and directly disabling them doesn't look g sood
            Invoke("MakeKinematic", makeKinematicTime);
        }

        //Disabling them after these seconds. block must have moved out of frame by now.
        Invoke("Disable", disableTime);

    }

    /// <summary>
    /// Makes the gameobject's rigidbody kinematic.
    /// </summary>
    void MakeKinematic()
    {
        rbody.isKinematic = true;
    }

    /// <summary>
    /// Disable this gameobject.
    /// </summary>
    void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// If case of force disable. Cancel invokes and make kinematic
    /// </summary>
    private void OnDisable()
    {
        //Cancel invokes as the object might be disabled by Object pooler before invoke executes
        CancelInvoke();
        if (rbody)
        {
            rbody.isKinematic = true;
        }
    }

    private void UnParent()
    {
        transform.parent = null;
    }

}
