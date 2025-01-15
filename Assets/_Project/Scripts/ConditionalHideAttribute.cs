using UnityEngine;

public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public bool HideInInspector;

    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
    }
}
