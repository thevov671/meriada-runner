using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraOccluderFader : MonoBehaviour
{
    [SerializeField] private Transform _target; // персонаж, за которым следим
    [SerializeField] private LayerMask _occluderLayerMask;
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private float _minFadeAlpha = 0.3f;
    [SerializeField] private float _targetHeightOffset = 1.5f; // центр персонажа по Y

    private Dictionary<Renderer, Material[]> _originalMaterials = new();
    private List<Renderer> _fadedRenderers = new();

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 camPos = transform.position; // позиция камеры — сам объект, к которому этот скрипт прикреплен
        Vector3 targetPos = _target.position + Vector3.up * _targetHeightOffset;

        RaycastHit[] hits = Physics.RaycastAll(camPos, targetPos - camPos, Vector3.Distance(camPos, targetPos), _occluderLayerMask);

        HashSet<Renderer> currentOccluders = new();

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                currentOccluders.Add(rend);
                if (!_fadedRenderers.Contains(rend))
                {
                    FadeOutRenderer(rend);
                    _fadedRenderers.Add(rend);
                }
            }
        }

        // Возвращаем в норму тех, кто перестал заслонять
        for (int i = _fadedRenderers.Count - 1; i >= 0; i--)
        {
            Renderer rend = _fadedRenderers[i];
            if (!currentOccluders.Contains(rend))
            {
                FadeInRenderer(rend);
                _fadedRenderers.RemoveAt(i);
            }
        }
    }

    private void FadeOutRenderer(Renderer rend)
    {
        if (!_originalMaterials.ContainsKey(rend))
            _originalMaterials[rend] = rend.materials;

        foreach (var mat in rend.materials)
        {
            SetupMaterialForTransparency(mat);
            DOTween.ToAlpha(() => mat.color, c => mat.color = c, _minFadeAlpha, _fadeDuration);
        }
    }

    private void FadeInRenderer(Renderer rend)
    {
        if (!_originalMaterials.ContainsKey(rend)) return;

        foreach (var mat in rend.materials)
        {
            DOTween.ToAlpha(() => mat.color, c => mat.color = c, 1f, _fadeDuration)
                .OnComplete(() => SetupMaterialForOpaque(mat));
        }
    }

    private void SetupMaterialForTransparency(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    private void SetupMaterialForOpaque(Material mat)
    {
        mat.SetFloat("_Mode", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}
