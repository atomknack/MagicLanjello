using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UKnack.Preconcrete.UI.SimpleToolkit;
using UnityEngine.UIElements;
using UKnack.UI;

public class UIElementSetTextFromAsset : EffortlessUIElement_VisualElement
{
    [SerializeField]
    private TextAsset _text;

    protected override void LayoutReadyAndElementFound(VisualElement layout)
    {
        if (_text == null)
            throw new System.ArgumentNullException(nameof(_text));
        _visualElement.TryAssignTextWithoutNotification(_text.text);
    }
    protected override void LayoutCleanupBeforeDestruction()
    {

    }
}
