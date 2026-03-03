using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GuidanceUI : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    private TextMeshProUGUI guidanceText;
    private bool hasStartedFading = false;

    void Start()
    {
        guidanceText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        bool keyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool mouseClicked = Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame);

        if (!hasStartedFading && (keyPressed || mouseClicked))
        {
            hasStartedFading = true;
            StartCoroutine(FadeOutRoutine());
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        if (guidanceText == null) yield break;

        Color originalColor = guidanceText.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            guidanceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            yield return null;
        }

        guidanceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}