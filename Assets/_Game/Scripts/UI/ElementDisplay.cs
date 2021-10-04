using System;
using System.Collections.Generic;
using _Game.Scripts.Classes;
using UnityEngine;
using UnityEngine.UI;

public class ElementDisplay : MonoBehaviour
{
    public Image Background;
    public Image Icon;
    public List<Image> statusIcons;
    public Element element;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        foreach (var icon in statusIcons) icon.gameObject.SetActive(false);

        if (element != null)
        {
            if (element.elementStatus != Element.ElementStatus.Default)
            {
                statusIcons[0].sprite =
                    ElementSpawner.Instance.StatusIcons.Find(stat => stat.status == element.elementStatus).icon;
                statusIcons[0].gameObject.SetActive(true);
            }
        }
        else
        {
            statusIcons[0].gameObject.SetActive(false);
        }

    }
}
