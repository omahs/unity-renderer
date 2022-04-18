using DCL.Models;
using DCL.Components;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DCL;

public class InteractionHoverCanvasController : MonoBehaviour
{
    public static InteractionHoverCanvasController i { get; private set; }
    public Canvas canvas;
    public RectTransform backgroundTransform;
    public TextMeshProUGUI text;
    public GameObject[] icons;

    bool isHovered = false;
    Camera mainCamera;
    GameObject hoverIcon;
    Vector3 meshCenteredPos;
    IDCLEntity entity;

    const string ACTION_BUTTON_POINTER = "POINTER";
    const string ACTION_BUTTON_PRIMARY = "PRIMARY";
    const string ACTION_BUTTON_SECONDARY = "SECONDARY";

    void Awake()
    {
        i = this;
        mainCamera = Camera.main;
        backgroundTransform.gameObject.SetActive(false);
    }

    public void Setup(string button, string feedbackText, IDCLEntity entity)
    {
        text.text = feedbackText;
        this.entity = entity;

        ConfigureIcon(button);

        canvas.enabled = enabled && isHovered;
    }

    void ConfigureIcon(string button)
    {
        hoverIcon?.SetActive(false);

        switch (button)
        {
            case ACTION_BUTTON_POINTER:
                hoverIcon = icons[0];
                break;
            case ACTION_BUTTON_PRIMARY:
                hoverIcon = icons[1];
                break;
            case ACTION_BUTTON_SECONDARY:
                hoverIcon = icons[2];
                break;
            default: // ANY
                hoverIcon = icons[3];
                break;
        }

        hoverIcon.SetActive(true);
        backgroundTransform.gameObject.SetActive(true);
    }

    public void SetHoverState(bool hoverState)
    {
        if (!enabled || hoverState == isHovered)
            return;

        isHovered = hoverState;

        canvas.enabled = isHovered;
    }

    public GameObject GetCurrentHoverIcon() { return hoverIcon; }
}