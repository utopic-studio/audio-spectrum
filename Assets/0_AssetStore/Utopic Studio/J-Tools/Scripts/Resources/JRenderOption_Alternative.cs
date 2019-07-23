using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JRenderOption_AlternativeFactory : IRenderOptionFactory
{
    private GameObject Blueprint;

    public JRenderOption_AlternativeFactory(GameObject InBlueprint)
    {
        if(!InBlueprint.GetComponent<JRenderOption_Alternative>())
        {
            Debug.LogError("Expected URenderOption_Alternative component on Blueprint GameObject, not found");
        }
        else
        {
            Blueprint = InBlueprint;
        }
    }

    public JRenderOption[] BuildRenderOptions(JResource.ContentOption[] Options)
    {
        List<JRenderOption> RenderOptions = new List<JRenderOption>();

        for(int i = 0; i < Options.Length; i++)
        {
            JResource.ContentOption Opt = Options[i];
            GameObject instantiated = GameObject.Instantiate(Blueprint);
            JRenderOption_Alternative alternative = instantiated.GetComponent<JRenderOption_Alternative>();

            alternative.Assign(Opt, i);
            RenderOptions.Add(alternative);
        }

        return RenderOptions.ToArray();
    }
}

[AddComponentMenu("J/Resources/RenderOptions/RenderOption_Alternative")]
public class JRenderOption_Alternative : JRenderOption {

    public UnityEngine.UI.Toggle Toggle;
    public UnityEngine.UI.Text Label;
    public UnityEngine.UI.Text IndexLabel;

    public void Assign(JResource.ContentOption Option, int index)
    {
        //We should be assigned to a toggle, so we can search it and init the values
        Toggle.isOn = false;
        Label.text = Option.Data;
        int asciiValue = (int)'A' + index;
        IndexLabel.text = ((char)asciiValue).ToString();
    }

    public override IRenderOptionFactory GetFactory()
    {
        return new JRenderOption_AlternativeFactory(this.gameObject);
    }

}
