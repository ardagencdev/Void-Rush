using UnityEngine;

public class ResponsivePanelScaler : MonoBehaviour
{
    [SerializeField] private RectTransform panelContent;
    [SerializeField] private float maxWidthPercent = 0.92f;
    [SerializeField] private float maxHeightPercent = 0.82f;
    [SerializeField] private float minScale = 0.65f;

    private void Start()
    {
        ApplyScale();
    }

    private void OnRectTransformDimensionsChange()
    {
        ApplyScale();
    }

    private void ApplyScale()
    {
        if (panelContent == null) return;

        RectTransform canvasRect = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
        if (canvasRect == null) return;

        panelContent.localScale = Vector3.one;

        float maxWidth = canvasRect.rect.width * maxWidthPercent;
        float maxHeight = canvasRect.rect.height * maxHeightPercent;

        float widthScale = maxWidth / panelContent.rect.width;
        float heightScale = maxHeight / panelContent.rect.height;

        float finalScale = Mathf.Min(1f, widthScale, heightScale);
        finalScale = Mathf.Max(finalScale, minScale);

        panelContent.localScale = Vector3.one * finalScale;
    }
}