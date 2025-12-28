using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsometricPlayerControls : MonoBehaviour
{
    public static IsometricPlayerControls Instance;
    public Button talkBtn;

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

    public void Start()
    {
        talkBtn.onClick.AddListener(Interact);
    }

    public void ToggleTalkBtn(bool isActive)
    {
        talkBtn.gameObject.SetActive(isActive);
    }

    private void Interact()
    {
        if (PlayerController.Instance.currentTarget != null)
        {
            Interactable interactable = PlayerController.Instance.currentTarget.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }
    }
}
