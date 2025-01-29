using UnityEngine;

public class HelpButtonController : MonoBehaviour
{
    // Referencia al objeto de texto de ayuda que queremos mostrar u ocultar.
    public GameObject helpText;

    // Un flag para saber si el texto de ayuda está visible.
    private bool isHelpTextVisible = false;

    /// <summary>
    /// Este método se llama cuando el botón de ayuda es presionado.
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
    /// Este método se llama para verificar si el clic ocurrió fuera del texto de ayuda.
    /// Si el texto está visible, se ocultará cuando se haga clic en cualquier otra parte de la pantalla.
    /// </summary>
    void Update()
    {
        // Verificar si el texto de ayuda está visible y si se hizo clic fuera de él.
        if (isHelpTextVisible && Input.GetMouseButtonDown(0) && !IsClickInsideHelpText())
        {
            // Si el clic fue fuera del área del texto de ayuda, ocultarlo
            helpText.SetActive(false);
            isHelpTextVisible = false;
        }
    }

    /// <summary>
    /// Verifica si el clic ocurrió dentro del área del objeto de texto de ayuda.
    /// </summary>
    /// <returns>Devuelve true si el clic fue dentro del área del texto de ayuda.</returns>
    private bool IsClickInsideHelpText()
    {
        // Obtener el área del rectángulo que contiene el texto de ayuda.
        RectTransform rectTransform = helpText.GetComponent<RectTransform>();
        Vector2 mousePosition = Input.mousePosition;

        // Convertir las coordenadas del mouse a coordenadas del mundo
        Vector2 localPosition = rectTransform.InverseTransformPoint(mousePosition);

        // Verificar si la posición del mouse está dentro del área del RectTransform
        return rectTransform.rect.Contains(localPosition);
    }
}
