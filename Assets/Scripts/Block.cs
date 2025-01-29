using System.Collections;
using UnityEngine;

/// <summary>
/// Representa la lógica y las propiedades de un bloque en el juego.
/// </summary>
public class Block : MonoBehaviour
{
    /// <summary>
    /// Número asociado al bloque para emparejar.
    /// </summary>
    public int number;

    /// <summary>
    /// Indica si el bloque ya ha sido emparejado.
    /// </summary>
    private bool isMatched = false;

    /// <summary>
    /// Material del bloque para cambiar su color.
    /// </summary>
    public Renderer blockRenderer;

    /// <summary>
    /// Color original del bloque antes de la interacción.
    /// </summary>
    private Color originalColor;

    /// <summary>
    /// Color al hacer clic en el bloque.
    /// </summary>
    [Tooltip("Color que se muestra al seleccionar el bloque.")]
    public Color clickedColor = Color.blue;

    /// <summary>
    /// Velopcidad de  rotacion al hacer click en un bloque
    /// </summary>
    public float rotationSpeed = 2f;

    /// <summary>
    /// Para evitar que se gire mientras ya se está animando
    /// </summary>
    private bool isFlipping = false;

    /// <summary>
    /// Sistema de particulas para arrojar estrellas en un match
    /// </summary>
    public ParticleSystem starParticles;


    /// <summary>
    /// Inicializa el bloque asignando el material y guardando el color original.
    /// </summary>
    private void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        originalColor = blockRenderer.material.color;
    }

    /// <summary>
    /// Lógica para manejar el clic en el bloque.
    /// </summary>
    private void OnMouseDown()
    {
        if (isMatched)
        {
            Debug.Log("Este bloque ya está emparejado.");
            return;
        }


        // Verifica si el bloque ya está en la lista de bloques seleccionados.
        if (GridManager.Instance.selectedBlocks.Contains(this))
        {
            return;  // No hace nada si ya está seleccionado.
        }

        // Notifica al GridManager para manejar el emparejamiento.
        GridManager.Instance.OnBlockClicked(this);
    }

    /// <summary>
    /// Marca el bloque como emparejado y desactiva su interacción.
    /// </summary>
    public void SetMatched()
    {
        isMatched = true;
        Debug.Log("¡Reproduciendo sistema de partículas!");
        starParticles.Play();
        blockRenderer.material.color = Color.green; // Color que indica que está emparejado.
    }

    /// <summary>
    /// Restaura el bloque a su estado original.
    /// </summary>
    public void ResetBlock()
    {
        isMatched = false;
        blockRenderer.material.color = originalColor;
    }

    // Método para girar el bloque al seleccionarlo
    public void FlipBlock()
    {
        if (!isFlipping)  // Evita que se gire mientras ya se está girando
        {
            StartCoroutine(RotateBlock(180f));  // Gira 180 grados
        }
    }

    // Método para restablecer la rotación del bloque
    public void ResetFlip()
    {
        if (!isFlipping)  // Evita que se gire mientras ya se está girando
        {
            StartCoroutine(RotateBlock(-180f));  // Gira -180 grados para volver a la posición original
        }
    }

    // Coroutine para girar el bloque
    private IEnumerator RotateBlock(float targetRotation)
    {
        isFlipping = true;  // Marcar como que está girando
        float startRotation = transform.rotation.eulerAngles.z;  // Obtener la rotación inicial en el eje Z
        float targetRotationZ = (startRotation + targetRotation) % 360;  // Calcular la rotación final en el eje Z

        float currentRotation = startRotation;
        float timeElapsed = 0f;

        // Animar la rotación
        while (timeElapsed < 1f)
        {
            currentRotation = Mathf.Lerp(startRotation, targetRotationZ, timeElapsed);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);  // Rotar en el eje Z
            timeElapsed += Time.deltaTime * rotationSpeed;
            yield return null;  // Esperar el siguiente frame
        }

        transform.rotation = Quaternion.Euler(0f, 0f, targetRotationZ);  // Asegurarse de que la rotación final es exacta
        isFlipping = false;  // Marcar que ya ha terminado de girar
    }

}
