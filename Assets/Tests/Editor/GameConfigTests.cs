using NUnit.Framework;
using System.Collections.Generic;

public class GameConfigValidationTests
{
    private GameConfig gameConfig;


    [SetUp]
    public void Setup()
    {
        gameConfig = new GameConfig
        {
            blocks = new List<Block>()
        };
    }

    [Test]
    public void ValidateGameConfig_EmptyBlocks_ReturnsFalse()
    {
        gameConfig.blocks = new List<Block>();

        Assert.IsFalse(ValidateGameConfig(), "La validación debería fallar cuando no hay bloques en la configuración.");
    }

    [Test]
    public void ValidateGameConfig_RowAndColumnDifference_OutOfBounds_ReturnsFalse()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 2, number = 1 },
            new Block { R = 2, C = 1, number = 1 }, // Es una grilla de 2 x 1
        };

        Assert.IsFalse(ValidateGameConfig(), "La validación debería fallar cuando la diferencia de R o C está fuera del rango permitido (2-8).");
    }

    [Test]
    public void ValidateGameConfig_RowAndColumnDifference_WithinBounds_ReturnsTrue()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 1, number = 1 },
            new Block { R = 1, C = 2, number = 1 }, // Es una grilla de 2 x 2
            new Block { R = 2, C = 1, number = 2 },
            new Block { R = 2, C = 2, number = 2 }
        };

        Assert.IsTrue(ValidateGameConfig(), "La validación debería pasar cuando las diferencias de R y C están dentro del rango permitido (2-8).");
    }

    [Test]
    public void ValidateGameConfig_NumberOutOfRange_ReturnsFalse()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 1, number = 1 },
            new Block { R = 1, C = 2, number = 10 }, // numero fuera del rango
            new Block { R = 2, C = 1, number = 2 },
            new Block { R = 2, C = 2, number = 2 }
        };

        Assert.IsFalse(ValidateGameConfig(), "La validación debería fallar cuando hay números fuera del rango permitido (0-9).");
    }

    [Test]
    public void ValidateGameConfig_NumberWithoutPair_ReturnsFalse()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 1, number = 1 },// numero sin pareja
            new Block { R = 1, C = 2, number = 3}, // numero sin pareja
            new Block { R = 2, C = 1, number = 2 },
            new Block { R = 2, C = 2, number = 2 }
        };

        Assert.IsFalse(ValidateGameConfig(), "La validación debería fallar cuando hay un número sin pareja.");
    }

    [Test]
    public void ValidateGameConfig_NumberWithPairs_ReturnsTrue()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 1, number = 1 },
            new Block { R = 1, C = 2, number = 1 },
            new Block { R = 2, C = 1, number = 2 },
            new Block { R = 2, C = 2, number = 2 }
        };

        Assert.IsTrue(ValidateGameConfig(), "La validación debería pasar cuando todos los números tienen al menos una pareja.");
    }

    [Test]
    public void ValidateGameConfig_NoGapsInGrid_ReturnsTrue()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 1, number = 1 },
            new Block { R = 1, C = 2, number = 1 },
            new Block { R = 2, C = 1, number = 2 },
            new Block { R = 2, C = 2, number = 2 },
            new Block { R = 3, C = 1, number = 3 },
            new Block { R = 3, C = 2, number = 3 }

        };

        Assert.IsTrue(ValidateGameConfig(), "La validación debería pasar cuando no hay huecos en la grilla.");
    }

    [Test]
    public void ValidateGameConfig_WithGapsInGrid_ReturnsFalse()
    {
        gameConfig.blocks = new List<Block>
        {
            new Block { R = 1, C = 1, number = 1 },
            new Block { R = 1, C = 2, number = 2 },
            // Falta el bloque en (2, 1)
            new Block { R = 2, C = 2, number = 1 }
        };

        Assert.IsFalse(ValidateGameConfig(), "La validación debería fallar cuando hay huecos en la grilla.");
    }

    /// <summary>
    /// Simula el método de validación del juego.
    /// </summary>
    /// <returns>True si todas las validaciones se cumplen.</returns>
    private bool ValidateGameConfig()
    {
        if (gameConfig == null || gameConfig.blocks == null || gameConfig.blocks.Count == 0)
        {
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
        int rowDifference = (maxRow - minRow) + 1;
        int colDifference = (maxCol - minCol) + 1;

        // Verificar que las diferencias estén entre 2 y 8.
        if (rowDifference < 2 || rowDifference > 8)
        {       
            isValid = false;
        }

        if (colDifference < 2 || colDifference > 8)
        {
            isValid = false;
        }

        // Caso 2: Validar que el valor (number) esté entre 0 y 9.
        foreach (var block in gameConfig.blocks)
        {
            if (block.number < 0 || block.number > 9)
            {
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
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    // Clases necesarias para las pruebas
    private class GameConfig
    {
        public List<Block> blocks;
    }

    private class Block
    {
        public int R;
        public int C;
        public int number;
    }
}
