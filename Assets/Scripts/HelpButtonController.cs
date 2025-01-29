using UnityEngine;

public class HelpButtonController : MonoBehaviour
{
    // Referencia al objeto de texto de ayuda que queremos mostrar u ocultar.
    public GameObject helpText;

    // Un flag para saber si el texto de ayuda est� visible.
    private bool isHelpTextVisible = false;

    /// <summary>
    /// Este m�todo se llama cuando el bot�n de ayuda es presionado.
    /// Muestra u oculta el texto de ayuda dependiendo de su estado actual.
    /// </summary>
    public void ToggleHelpText()
    {
        // Cambiar el estado de visibilidad del texto de ayuda
        isHelpTextVisible = !isHelpTextVisible;

        // Hacer visible u ocultar el objeto de texto de ayuda
        helpText.SetActive(isHelpTextVisible);
    }

    /// <summary>
    /// Este m�todo se llama para verificar si el clic ocurri� fuera del texto de ayuda.
    /// Si el texto est� visible, se ocultar� cuando se haga clic en cualquier otra parte de la pantalla.
    /// </summary>
    void Update()
    {
        // Verificar si el texto de ayuda est� visible y si se hizo clic fuera de �l.
        if (isHelpTextVisible && Input.GetMouseButtonDown(0) && !IsClickInsideHelpText())
        {
            // Si el clic fue fuera del �rea del texto de ayuda, ocultarlo
            helpText.SetActive(false);
            isHelpTextVisible = false;
        }
    }

    /// <summary>
    /// Verifica si el clic ocurri� dentro del �rea del objeto de texto de ayuda.
    /// </summary>
    /// <returns>Devuelve true si el clic fue dentro del �rea del texto de ayuda.</returns>
    private bool IsClickInsideHelpText()
    {
        // Obtener el �rea del rect�ngulo que contiene el texto de ayuda.
        RectTransform rectTransform = helpText.GetComponent<RectTransform>();
        Vector2 mousePosition = Input.mousePosition;

        // Convertir las coordenadas del mouse a coordenadas del mundo
        Vector2 localPosition = rectTransform.InverseTransformPoint(mousePosition);

        // Verificar si la posici�n del mouse est� dentro del �rea del RectTransform
        return rectTransform.rect.Contains(localPosition);
    }
}
