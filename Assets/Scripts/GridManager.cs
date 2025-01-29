using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

/// <summary>
/// Controlador principal para la generación de la rejilla y la carga de datos desde un archivo JSON.
/// </summary>
public class GridManager : MonoBehaviour
{
    /// <summary>
    /// Nombre del archivo JSON que contiene la configuración del juego.
    /// </summary>
    [Tooltip("Nombre del archivo JSON dentro de la carpeta StreamingAssets.")]
    public string jsonFileName = "GameConfig.json";

    /// <summary>
    /// Configuración del juego deserializada desde el archivo JSON.
    /// </summary>
    private GameConfig gameConfig;

    /// <summary>
    /// Instancia única del GridManager (patrón Singleton).
    /// </summary>
    public static GridManager Instance;

    /// <summary>
    /// Bloques seleccionados actualmente para emparejar.
    /// </summary>
    public List<Block> selectedBlocks = new List<Block>();
        
    [Header("Configuración de la rejilla")]
    public float spacing = 1.2f;  // Espaciado entre los bloques (puede ser ajustado en el Inspector)

    /// <summary>
    /// Indica si el numero de clicks hechos.
    /// </summary>
    private int totalClicks = 0;

    /// <summary>
    /// Indica si el numero de parejas encontradas.
    /// </summary>
    private int pairs = 0;

    /// <summary>
    /// Tiempo de inicio del temporizador en segundos.
    /// </summary>
    private float startTime;

    /// <summary>
    /// Tiempo transcurrido en el juego, calculado durante el tiempo de ejecución.
    /// </summary>
    private float elapsedTime;

    /// <summary>
    /// Indica si el temporizador ha comenzado.
    /// </summary>
    private bool timerStarted = false;

    /// <summary>
    /// Indica si el juego ha terminado.
    /// </summary>
    private bool gameEnded = false;

    /// <summary>
    /// referencia al texto que muestra el tiempo.
    /// </summary>
    public TMP_Text timeText;

    /// <summary>
    /// referencia al texto que muestra el numero de clicks.
    /// </summary>
    public TMP_Text clicksText;


    /// <summary>
    /// referencia al objeto que muestra el final score
    /// </summary>
    public GameObject finalScoreObject;

    /// <summary>
    /// referencia al objeto que muestra el final score
    /// </summary>
    public TMP_Text finalScoreText;

    /// <summary>
    /// referencia a la lista de sprites para el matching game
    /// </summary>
    [Header("Sprites")]
    public List<Sprite> spriteList; // Lista de sprites asignada desde el editor.

    /// <summary>
    /// boolean para saber si se esta revisando un match
    /// </summary>
    private bool isCheckingMatch = false;

    /// <summary>
    /// Clip de sonido que se reproduce cuando el jugador selecciona un par correcto.
    /// </summary>
    public AudioClip correctSound;

    /// <summary>
    /// Clip de sonido que se reproduce cuando el jugador selecciona un par incorrecto.
    /// </summary>
    public AudioClip incorrectSound;

    /// <summary>
    /// Clip de sonido que se reproduce cuando el juego termina.
    /// </summary>
    public AudioClip gameEndSound;

    /// <summary>
    /// Clip de sonido que se reproduce cuando el juego termina.
    /// </summary>
    public AudioClip blockClickedSound;

    /// <summary>
    /// Componente AudioSource que se usa para reproducir los sonidos.
    /// </summary>
    private AudioSource audioSource;


    /// <summary>
    /// Indica si el juego fue cargado exitosamente.
    /// </summary>
    private bool gameLoadedSuccesfully;

    /// <summary>
    /// Nombre del archivo JSON que contiene el leaderboard.
    /// </summary>
    public string leaderboardFileName = "Leaderboard.json";

    /// <summary>
    /// FilePath del LeadeBoard.
    /// </summary>
    private string leaderboardFilePath;

    /// <summary>
    /// Objeto para representar el leaderBoard.
    /// </summary>
    private LeaderboardData leaderboardData;

    /// <summary>
    /// Objeto para representar la UI del leaderBoard.
    /// </summary>
    [SerializeField] private LeaderboardUI leaderboardUI;

    /// <summary>
    /// hace Referencia al nombre del jugador.
    /// </summary>
    private string playerName;

    /// <summary>
    /// Input field para tomar el nombre del jugador.
    /// </summary>
    public TMP_InputField playerNameInputField;


    /// <summary>
    /// Inicializa la instancia única y genera la rejilla.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Inicializar la ruta completa del archivo
        leaderboardFilePath = Path.Combine(Application.persistentDataPath, leaderboardFileName);

        // Cargar el leaderboard al iniciar
        LoadLeaderboard();
    }

    /// <summary>
    /// Inicia el temporizador del juego y genera la rejilla de bloques.
    /// </summary>
    private void Start()
    {
        gameLoadedSuccesfully = false;
        LoadGameConfig();
        GenerateGrid();
        // Obtiene el componente AudioSource del GameObject en el que está el script.
        audioSource = GetComponent<AudioSource>();
        timeText.text = "000";   
    }

    /// <summary>
    /// Actualiza el tiempo total del juego y comprueba si se ha emparejado la última pareja.
    /// </summary>
    private void Update()
    {
        if (gameLoadedSuccesfully)
        {
            if (!gameEnded && pairs == gameConfig.blocks.Count / 2)
            {
                EndGame();
            }

            // Detectar la tecla R para reiniciar el juego
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }

            // Solo contar el tiempo si el temporizador ha comenzado.
            if (timerStarted && !gameEnded)
            {
                elapsedTime = Time.time - startTime; // Calcula el tiempo transcurrido en segundos.
                timeText.text = $"{Mathf.FloorToInt(elapsedTime):D3}";  // Actualiza el texto con el tiempo actual.
            }
        }

        // Verifica si se presionó la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Cierra el juego
            Debug.Log("Juego cerrado.");
            Application.Quit();

            // Si estás en el editor de Unity, esto no cerrará el editor, pero mostrará un mensaje en la consola
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    /// <summary>
    /// Carga el leaderboard desde un archivo JSON.
    /// </summary>
    private void LoadLeaderboard()
    {
        if (File.Exists(leaderboardFilePath))
        {
            string json = File.ReadAllText(leaderboardFilePath);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
            Debug.Log("El leaderBoard existe");
        }
        else
        {
            leaderboardData = new LeaderboardData();

            // Crear el leaderboard con puntajes predeterminados
            leaderboardData.leaderboard.Add(new PlayerScore("AAA", 300));
            leaderboardData.leaderboard.Add(new PlayerScore("BBB", 200));
            leaderboardData.leaderboard.Add(new PlayerScore("CCC", 100));

            SaveLeaderboard(); // Crear el archivo si no existe.
            Debug.Log("El leaderBoard no existe");
        }
        TrimLeaderboard();
        leaderboardUI.DisplayLeaderboard();
    }

    /// <summary>
    /// Guarda el leaderboard en un archivo JSON.
    /// </summary>
    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(leaderboardFilePath, json);
    }

    /// <summary>
    /// Agrega un nuevo puntaje al leaderboard y verifica si debe incluirse en el top 3.
    /// </summary>
    public void AddScore(string playerName, int score)
    {
        // Agregar el nuevo puntaje
        leaderboardData.leaderboard.Add(new PlayerScore(playerName, score));

        // Ordenar el leaderboard por puntaje (de mayor a menor)
        leaderboardData.leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        TrimLeaderboard();

        // Guardar los cambios
        SaveLeaderboard();
    }

    /// <summary>
    /// Devuelve el leaderboard completo.
    /// </summary>
    public List<PlayerScore> GetLeaderboard()
    {
        return leaderboardData.leaderboard;
    }

    /// <summary>
    /// Elimina los puntajes fuera del top 3.
    /// </summary>
    private void TrimLeaderboard()
    {
        if (leaderboardData.leaderboard.Count > 3)
        {
            leaderboardData.leaderboard = leaderboardData.leaderboard.GetRange(0, 3);
        }
    }

    /// <summary>
    /// Verifica si el puntaje supera a alguno del leaderboard.
    /// </summary>
    public bool IsNewHighScore(int score)
    {
        // Comparar con el puntaje más bajo en el top 3
        return score > leaderboardData.leaderboard[leaderboardData.leaderboard.Count - 1].score;
    }

    /// <summary>
    /// Carga la configuración del juego desde el archivo JSON.
    /// </summary>
    private void LoadGameConfig()
    {
        // Construye la ruta completa del archivo JSON.
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(filePath))
        {
            // Lee el contenido del archivo JSON.
            string jsonData = File.ReadAllText(filePath);

            // Deserializa los datos en un objeto GameConfig.
            gameConfig = JsonUtility.FromJson<GameConfig>(jsonData);

            Debug.Log("Archivo JSON cargado exitosamente.");

            // Realiza las validaciones del archivo JSON.
            if (ValidateGameConfig())
            {
                gameLoadedSuccesfully = true;
                Debug.Log("Validaciones del JSON pasaron correctamente.");
            }
            else
            {
                Debug.LogError("Se encontraron errores en el archivo JSON. No se generará la grilla.");
                gameConfig = null; // Invalidamos la configuración si no pasa las validaciones.
            }
        }
        else
        {
            // Maneja el error si el archivo no existe.
            Debug.LogError($"No se encontró el archivo JSON en la ruta: {filePath}");
        }
    }

    /// <summary>
    /// Valida la configuración del juego cargada desde el archivo JSON.
    /// </summary>
    /// <returns>True si todas las validaciones se cumplen, de lo contrario, False.</returns>
    private bool ValidateGameConfig()
    {
        if (gameConfig == null || gameConfig.blocks == null || gameConfig.blocks.Count == 0)
        {
            Debug.LogError("La configuración del juego está vacía o no es válida.");
            return false;
        }

        bool isValid = true;
        // Caso 1: Validar que la diferencia entre el valor máximo y mínimo de R y C no sea menor a 2 ni mayor a 8.
        int minRow = int.MaxValue, maxRow = int.MinValue;
        int minCol = int.MaxValue, maxCol = int.MinValue;

        // Determinar los valores mínimos y máximos de R y C.
        foreach (var block in gameConfig.blocks)
        {
            if (block.R < minRow) minRow = block.R;
            if (block.R > maxRow) maxRow = block.R;
            if (block.C < minCol) minCol = block.C;
            if (block.C > maxCol) maxCol = block.C;
        }

        // Calcular las diferencias entre los valores máximos y mínimos.
        int rowDifference = (maxRow - minRow)+1;
        int colDifference = (maxCol - minCol)+1;

        // Verificar que las diferencias estén entre 2 y 8.
        if (rowDifference < 2 || rowDifference > 8)
        {
            Debug.LogError($"La diferencia entre el mayor y menor valor de R ({rowDifference}) está fuera del rango permitido (2-8).");
            isValid = false;
        }

        if (colDifference < 2 || colDifference > 8)
        {
            Debug.LogError($"La diferencia entre el mayor y menor valor de C ({colDifference}) está fuera del rango permitido (2-8).");
            isValid = false;
        }

        // Caso 2: Validar que el valor (number) esté entre 0 y 9.
        foreach (var block in gameConfig.blocks)
        {
            if (block.number < 0 || block.number > 9)
            {
                Debug.LogError($"El número {block.number} en R={block.R}, C={block.C} está fuera del rango permitido (0-9).");
                isValid = false;
            }
        }

        // Caso 3: Validar que cada número tenga al menos una pareja.
        Dictionary<int, int> numberCounts = new Dictionary<int, int>();
        foreach (var block in gameConfig.blocks)
        {
            if (!numberCounts.ContainsKey(block.number))
            {
                numberCounts[block.number] = 0;
            }
            numberCounts[block.number]++;
        }

        foreach (var pair in numberCounts)
        {
            if (pair.Value % 2 != 0)
            {
                Debug.LogError($"El número {pair.Key} tiene una cantidad impar de bloques ({pair.Value}), lo que impide encontrar parejas.");
                isValid = false;
            }
        }

        // Caso 4: Validar que no haya huecos en la grilla (debe ser un rectángulo o cuadrado completo).
        HashSet<(int, int)> blockPositions = new HashSet<(int, int)>();
        int minGridRow = int.MaxValue, maxGridRow = int.MinValue;
        int minGridCol = int.MaxValue, maxGridCol = int.MinValue;

        foreach (var block in gameConfig.blocks)
        {
            blockPositions.Add((block.R, block.C));
            if (block.R < minGridRow) minGridRow = block.R;
            if (block.R > maxGridRow) maxGridRow = block.R;
            if (block.C < minGridCol) minGridCol = block.C;
            if (block.C > maxGridCol) maxGridCol = block.C;
        }

        for (int r = minGridRow; r <= maxGridRow; r++)
        {
            for (int c = minGridCol; c <= maxGridCol; c++)
            {
                if (!blockPositions.Contains((r, c)))
                {
                    Debug.LogError($"Falta un bloque en la posición R={r}, C={c}, lo que genera un hueco en la grilla.");
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    /// <summary>
    /// Genera la rejilla del juego basada en los datos del JSON y ajusta la cámara para que mire al centro de la rejilla.
    /// </summary>
    private void GenerateGrid()
    {
        // Verifica si los datos están correctamente cargados.
        if (gameConfig == null || gameConfig.blocks == null)
        {
            Debug.LogError("Los datos del juego no están cargados correctamente.");
            return;
        }

        // Variables para determinar el tamaño de la rejilla.
        float minRow = float.MaxValue;
        float maxRow = float.MinValue;
        float minColumn = float.MaxValue;
        float maxColumn = float.MinValue;

        // Itera por cada bloque en la configuración y genera un cubo en la escena.
        foreach (var block in gameConfig.blocks)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Ajusta la posición del cubo en la rejilla. Usamos el eje Y para la altura y X/Z para la posición en la grilla.
            tile.transform.position = new Vector3(block.C * spacing, 0, block.R * spacing);

            // Asigna un nombre al objeto para identificarlo fácilmente.
            tile.name = $"Tile ({block.R}, {block.C})";

            // Añadir el script Block y configurar sus propiedades.
            Block blockComponent = tile.AddComponent<Block>();
            blockComponent.number = block.number;

            // Configura el color inicial del bloque (opcional, para diferenciar los bloques).
            tile.GetComponent<Renderer>().material.color = Color.gray;

            // Crear un objeto hijo vacío para contener el SpriteRenderer.
            GameObject spriteObject = new GameObject("TileSprite");
            spriteObject.transform.SetParent(tile.transform);  // Hacerlo hijo del Tile
            spriteObject.transform.localPosition = new Vector3(0f, -0.6f, 0f); // Posición ajustada
            spriteObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Rotación ajustada
            spriteObject.transform.localScale = new Vector3(2.6f, 2.6f, 2.6f); // Escala ajustada

            // Añadir un SpriteRenderer al objeto hijo.
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();

            // Asigna un sprite según el número del bloque.
            if (block.number >= 0 && block.number < spriteList.Count)
            {
                spriteRenderer.sprite = spriteList[block.number];
            }
            else
            {
                Debug.LogWarning($"El número del bloque ({block.number}) está fuera del rango de la lista de sprites.");
            }

            // Actualiza las posiciones mínimas y máximas de las filas y columnas
            minRow = Mathf.Min(minRow, block.R);
            maxRow = Mathf.Max(maxRow, block.R);
            minColumn = Mathf.Min(minColumn, block.C);
            maxColumn = Mathf.Max(maxColumn, block.C);

            // *** Creación del sistema de partículas como hijo del Tile ***

            GameObject particleSystemPrefab = Resources.Load<GameObject>("StarsPrefab");

            if (particleSystemPrefab != null)
            {
                // Instancia el sistema de partículas.
                GameObject particleSystemInstance = Instantiate(particleSystemPrefab);

                // Haz que el sistema de partículas sea hijo del objeto "tile".
                particleSystemInstance.transform.SetParent(tile.transform);

                // Ajusta la posición local si es necesario para que se alinee con el tile
                particleSystemInstance.transform.localPosition = Vector3.zero;

                blockComponent.starParticles = particleSystemInstance.GetComponent<ParticleSystem>();
            }

            // Cargar el material desde la carpeta Resources
            Material material = Resources.Load<Material>("blockDefault");

            if (material != null)
            {
                // Asignar el material al Renderer del objeto
                tile.GetComponent<Renderer>().material = material;
            }
            else
            {
                Debug.LogWarning("Material no encontrado.");
            }
        }

        // Calcula el centro de la rejilla.
        Vector3 gridCenter = new Vector3((minColumn + maxColumn) * spacing / 2, 0, (minRow + maxRow) * spacing / 2);

        // Calcula el tamaño de la rejilla para ajustarlo a la cámara.
        float gridHeight = (maxRow - minRow + 1) * spacing;
        float gridWidth = (maxColumn - minColumn + 1) * spacing;

        // Ajusta la posición de la cámara para que esté directamente encima del centro de la rejilla y mirando hacia abajo
        Camera.main.transform.position = new Vector3(gridCenter.x, gridHeight + 2f, gridCenter.z); // Posiciona la cámara un poco por encima de la rejilla

        // Asegura que la cámara mire hacia abajo, es decir, hacia el centro de la rejilla.
        Camera.main.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Debug.Log("Rejilla generada exitosamente.");
    }

    /// <summary>
    /// Método llamado al hacer clic en un bloque.
    /// </summary>
    /// <param name="clickedBlock">El bloque que fue clicado.</param>
    public void OnBlockClicked(Block clickedBlock)
    {
        if (gameEnded) return;

        // Si el temporizador no ha comenzado, iniciarlo al hacer clic en el primer bloque.
        if (!timerStarted)
        {
            StartTimer();
        }

        if (!isCheckingMatch)
        {
            // Reproducir sonido de acierto
            PlaySound(blockClickedSound);
            // Añade el bloque seleccionado a la lista.
            selectedBlocks.Add(clickedBlock);
            totalClicks++;
            clicksText.text = $"{totalClicks:D3}";
            clickedBlock.FlipBlock();  // Gira el bloque al hacer clic
            clickedBlock.blockRenderer.material.color = clickedBlock.clickedColor;  // Cambia el color para indicar la selección.
                                                                                 
            Debug.Log($"Número del bloque: {clickedBlock.number}");   // Revela el número en la consola

            // Comprueba si hay dos bloques seleccionados para intentar emparejar.
            if (selectedBlocks.Count == 2)
            {
                isCheckingMatch = true;
                // Llama a la corrutina para esperar 2 segundos antes de comprobar la coincidencia
                StartCoroutine(DelayedCheckMatch());
            }
        }

    }

    /// <summary>
    /// Corrutina que retrasa la llamada a CheckMatch() por 2 segundos.
    /// </summary>
    private IEnumerator DelayedCheckMatch()
    {
        // Espera 2 segundos
        yield return new WaitForSeconds(2f);

        // Ahora que ha pasado el tiempo, ejecuta CheckMatch
        CheckMatch();

        yield return new WaitForSeconds(0.6f);
        isCheckingMatch = false;
    }

    /// <summary>
    /// Comprueba si los bloques seleccionados tienen el mismo número.
    /// </summary>
    private void CheckMatch()
    {
        if (selectedBlocks[0].number == selectedBlocks[1].number)
        {
            Debug.Log("¡Emparejamiento correcto!");
            // Reproducir sonido de acierto
            if (!gameEnded)
            {
                PlaySound(correctSound);
            }
            pairs++;
            // Marca ambos bloques como emparejados.
            foreach (Block block in selectedBlocks)
            {
                block.SetMatched();
            }
        }
        else
        {
            Debug.Log("Emparejamiento incorrecto. Restableciendo bloques...");
            // Reproducir sonido de acierto
            PlaySound(incorrectSound);
            // Restaura ambos bloques a su estado original.
            foreach (Block block in selectedBlocks)
            {
                block.ResetBlock();
                block.FlipBlock();  // Gira el bloque al hacer clic
            }
        }

        // Limpia la lista de bloques seleccionados.
        selectedBlocks.Clear();
    }

    /// <summary>
    /// Inicia el temporizador cuando el primer bloque es clicado.
    /// </summary>
    private void StartTimer()
    {
        timerStarted = true; // Marca que el temporizador ha comenzado.
        startTime = Time.time; // Registra el tiempo de inicio.
    }
    /// <summary>
    /// Finaliza el juego cuando todas las parejas se han encontrado.
    /// </summary>
    private void EndGame()
    {
        gameEnded = true;
        Debug.Log("¡Juego terminado!");
        PlaySound(gameEndSound);

        int score = CalculateScore();

        SaveResults(score);
        // Mostrar la puntuación final en la UI
        finalScoreObject.SetActive(true);  // Habilita el objeto para que se vea


        if (IsNewHighScore(score))
        {
            // Actualizar el texto con la puntuación final
            finalScoreText.text = $"NEW HIGH SCORE! \nPLEASE ENTER A NAME \nFINAL SCORE: {score:D4} \nPRESS R TO RESTART";
            // Pedir el nombre del jugador si es un nuevo récord
            StartCoroutine(RequestPlayerName(score));
        }
        else
        {
            // Actualizar el texto con la puntuación final
            finalScoreText.text = $"FINAL SCORE: {score:D4} \nPRESS R TO RESTART";
        }
    }

    /// <summary>
    /// Solicita al jugador ingresar su nombre si se alcanza un nuevo récord.
    /// </summary>
    private IEnumerator RequestPlayerName(int score)
    {
        // Muestra una ventana de entrada o un InputField para capturar el nombre del jugador
        playerNameInputField.gameObject.SetActive(true);  // Habilita el InputField para el nombre
        playerNameInputField.text = "";  // Limpiar el campo de entrada

        // Esperar hasta que el jugador ingrese un nombre válido (3 caracteres)
        while (playerNameInputField.text.Length != 3)
        {
            yield return null;
        }

        // Guardar el nombre del jugador ingresado
        playerName = playerNameInputField.text;

        // Agregar el puntaje al leaderboard
        AddScore(playerName, score);

        // Actualizar la interfaz del leaderboard
        leaderboardUI.DisplayLeaderboard();

        // Ocultar el InputField después de que se haya ingresado el nombre
        playerNameInputField.gameObject.SetActive(false);

        // Actualizar el texto con la puntuación final
        finalScoreText.text = $"FINAL SCORE: {score:D4} \nPRESS R TO RESTART";
    }


    /// <summary>
    /// Calcula el puntaje del jugador en base a las parejas encontradas y los clics realizados.
    /// </summary>
    /// <returns>El puntaje calculado.</returns>
    private int CalculateScore()
    {
        // cálculo de puntaje: (pares * 100) - (clics * 10) - (tiempo en segundos * 2)
        int timeBonus = Mathf.Max(0, 1000 - Mathf.RoundToInt(elapsedTime) * 2);  // Penaliza por tiempo transcurrido
        return (pairs * 100) - (totalClicks * 10) + timeBonus;
    }

    /// <summary>
    /// Guarda los resultados del juego en un archivo JSON, agregando nuevas entradas.
    /// </summary>
    /// <param name="score">El puntaje final del jugador.</param>
    private void SaveResults(int score)
    {
        // Crear una nueva entrada de resultados
        GameResults newResult = new GameResults
        {
            total_clicks = totalClicks,
            total_time = Mathf.FloorToInt(elapsedTime),
            pairs = pairs,
            score = score
        };

        // Ruta del archivo JSON
        string filePath = Path.Combine(Application.streamingAssetsPath, "game_results.json");

        // Verificar si el archivo existe
        if (File.Exists(filePath))
        {
            // Leer los datos existentes
            string json = File.ReadAllText(filePath);

            // Deserializar el archivo en un wrapper de resultados
            ResultsWrapper resultsWrapper = JsonUtility.FromJson<ResultsWrapper>(json);

            // Agregar la nueva entrada a la lista de resultados
            resultsWrapper.results.Add(newResult);

            // Convertir nuevamente a JSON
            json = JsonUtility.ToJson(resultsWrapper, true);

            // Escribir los datos actualizados en el archivo
            File.WriteAllText(filePath, json);
        }
        else
        {
            // Si el archivo no existe, crear uno con la nueva entrada
            ResultsWrapper resultsWrapper = new ResultsWrapper();
            resultsWrapper.results = new List<GameResults> { newResult };

            // Convertir a JSON
            string json = JsonUtility.ToJson(resultsWrapper, true);

            // Escribir el nuevo archivo
            File.WriteAllText(filePath, json);
        }

        Debug.Log("Resultados guardados en: " + filePath);
    }


    /// <summary>
    /// Reinicia el juego, recargando la escena actual.
    /// </summary>
    private void RestartGame()
    {
        // Restablecer las variables del juego
        totalClicks = 0;
        pairs = 0;
        startTime = 0;
        elapsedTime = 0;
        timerStarted = false;
        gameEnded = false;

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Juego reiniciado.");
    }

    /// <summary>
    /// Reproduce un sonido específico usando el AudioSource.
    /// </summary>
    /// <param name="clip">El clip de sonido a reproducir.</param>
    private void PlaySound(AudioClip clip)
    {
        // Reproduce el clip de sonido utilizando el método PlayOneShot
        audioSource.PlayOneShot(clip);
    }
}