using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Transform currentTarget;
    public SphereCollider sphereColl;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringConstants.npc))
        {
            currentTarget = other.transform;
            IsometricPlayerControls.Instance.ToggleTalkBtn(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(currentTarget != null)
        {
            currentTarget = null;
            IsometricPlayerControls.Instance.ToggleTalkBtn(false);
        }
    }
}
