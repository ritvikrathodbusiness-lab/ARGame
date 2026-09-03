using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneVisibilityController : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private float visibleDuration = 2f;   // how long it stays fully visible
    [SerializeField] private float fadeDuration = 1.5f;    // how long the fade-out takes

    // Tracks the running fade coroutine per plane so we don't double-start it
    private readonly Dictionary<ARPlane, Coroutine> activeFades = new Dictionary<ARPlane, Coroutine>();

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            // Fresh plane: make sure it's fully visible, then start its fade lifecycle
            SetPlaneAlpha(plane, 1f);
            var routine = StartCoroutine(ShowThenFade(plane));
            activeFades[plane] = routine;
        }

        foreach (var plane in args.removed)
        {
            if (activeFades.TryGetValue(plane, out var routine))
            {
                if (routine != null) StopCoroutine(routine);
                activeFades.Remove(plane);
            }
        }
    }

    private IEnumerator ShowThenFade(ARPlane plane)
    {
        yield return new WaitForSeconds(visibleDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            if (plane == null) yield break; // plane was removed mid-fade

            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetPlaneAlpha(plane, alpha);
            yield return null;
        }

        if (plane != null) SetPlaneAlpha(plane, 0f);
        activeFades.Remove(plane);
    }

    private void SetPlaneAlpha(ARPlane plane, float alpha)
    {
        var meshRenderer = plane.GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            // .material (not sharedMaterial) instances the material per-plane automatically
            var mat = meshRenderer.material;
            if (mat.HasProperty("_BaseColor")) // URP
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = alpha;
                mat.SetColor("_BaseColor", c);
            }
            else if (mat.HasProperty("_Color")) // Built-in RP
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }

        var lineRenderer = plane.GetComponent<LineRenderer>();
        if (lineRenderer)
        {
            Color startC = lineRenderer.startColor;
            Color endC = lineRenderer.endColor;
            startC.a = alpha;
            endC.a = alpha;
            lineRenderer.startColor = startC;
            lineRenderer.endColor = endC;
        }
    }
}