using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InverseMaskImage : Image {
    private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");

    public override Material materialForRendering {
        get {
            Material renderingMaterial = new Material(base.materialForRendering);
            renderingMaterial.SetInt(StencilComp, (int) CompareFunction.NotEqual);
            return renderingMaterial;
        }
    }
}