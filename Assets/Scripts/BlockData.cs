using System.Collections.Generic;

/// <summary>
/// Representa un bloque en la rejilla del juego.
/// </summary>
[System.Serializable]
public class BlockData
{
    /// <summary>
    /// Fila en la que se encuentra el bloque.
    /// </summary>
    public int R;

    /// <summary>
    /// Columna en la que se encuentra el bloque.
    /// </summary>
    public int C;

    /// <summary>
    /// Número que identifica al bloque para emparejar.
    /// </summary>
    public int number;
}

/// <summary>
/// Configuración general del juego, incluyendo la lista de bloques.
/// </summary>
[System.Serializable]
public class GameConfig
{
    /// <summary>
    /// Lista de bloques en la rejilla del juego.
    /// </summary>
    public List<BlockData> blocks;
}
