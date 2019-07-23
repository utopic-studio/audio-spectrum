using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRenderOptionFactory
{
    JRenderOption[] BuildRenderOptions(JResource.ContentOption[] Options);
}

public abstract class JRenderOption : MonoBehaviour {

    //public abstract void Assign(Recurso.OpcionContenido Option);
    public abstract IRenderOptionFactory GetFactory();

    //TODO: GetData deberia devolver un objeto tipo Respuesta
}
