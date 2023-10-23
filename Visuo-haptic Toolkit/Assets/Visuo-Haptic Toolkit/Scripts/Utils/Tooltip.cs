using UnityEngine;

public class TooltipCustom : PropertyAttribute
{
    public string text;

    public TooltipCustom(string tooltipText)
    {
        this.text = tooltipText;
    }
}