using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CinematicPicController
{
    private VisualElement _rootElement;

    public void SetPic(Sprite newPic)
    {
        if (newPic == null)
        {
            _rootElement.style.backgroundImage = new StyleBackground(StyleKeyword.None);
        }
        else
        {
            _rootElement.style.backgroundImage = new StyleBackground(newPic);
        }
    }
}
