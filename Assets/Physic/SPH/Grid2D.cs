using System.Collections.Generic;
using UnityEngine;

public class Grid2D
{
    Dictionary<int, List<SPHParticle>> cellDict;

    private int colNum;

    private int rowNum;

    private float cellSize;

    private Vector2 startPos;

    public Grid2D(Vector2 startPos, Vector2 size, float h) {
        this.startPos = startPos;
        colNum = Mathf.CeilToInt(size.x / h);
        rowNum = Mathf.CeilToInt(size.y / h);

        cellSize = h;
        cellDict = new Dictionary<int, List<SPHParticle>>(colNum * rowNum);
        for (int i = 0; i < colNum * rowNum; i++) {
            cellDict.Add(i, new List<SPHParticle>());
        }
        Debug.Log(" col = " + colNum + " row = " + rowNum + " cell dict count = " + cellDict.Count);
    }

    public void Clear() {
        for (int i = 0; i < colNum * rowNum; i++)
        {
            cellDict[i].Clear();
        }
    }

    public void Add(SPHParticle particle) {
        int col = Mathf.CeilToInt((particle.Position.x - this.startPos.x) / cellSize);
        int row = Mathf.CeilToInt((this.startPos.y  - particle.Position.y)/ cellSize);

        int cellIndex = GetCellIndex(col, row);

        if (!cellDict[cellIndex].Contains(particle)) {
            cellDict[cellIndex].Add(particle);
        }
    }

    List<SPHParticle> nearParticles = new List<SPHParticle>();

    public List<SPHParticle> GetNearby(SPHParticle p) {
        nearParticles.Clear();

        int col = Mathf.CeilToInt((p.Position.x - this.startPos.x) / cellSize);
        int row = Mathf.CeilToInt((this.startPos.y - p.Position.y) / cellSize);

        int cellIndex = GetCellIndex(col, row);

        nearParticles.AddRange(cellDict[cellIndex]);

        if (col - 1 > 0) {
            cellIndex = GetCellIndex(col - 1, row);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        if (col + 1 < colNum)
        {
            cellIndex = GetCellIndex(col + 1, row);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        if (row - 1 > 0)
        {
            cellIndex = GetCellIndex(col, row - 1);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        if (row + 1 < rowNum)
        {
            cellIndex = GetCellIndex(col, row + 1);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        if (col - 1 > 0 && row - 1 > 0)
        {
            cellIndex = GetCellIndex(col - 1, row - 1);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        if (col + 1 < colNum && row + 1 < rowNum)
        {
            cellIndex = GetCellIndex(col + 1, row + 1);
            nearParticles.AddRange(cellDict[cellIndex]);
        }


        if (col - 1 > 0 && row + 1 < rowNum)
        {
            cellIndex = GetCellIndex(col - 1, row + 1);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        if (col + 1 < colNum && row - 1 > 0)
        {
            cellIndex = GetCellIndex(col + 1, row - 1);
            nearParticles.AddRange(cellDict[cellIndex]);
        }

        return nearParticles;
    }

    private int GetCellIndex(int c,int r) {
        return c + rowNum * r;
    }
   
}
