using System;
using Wsh.Mathematics;

namespace Wsh.GridSystem {

    public class Grid<TGridObject> where TGridObject : BaseGridObject {

        public event Action<int, int, TGridObject> onGridValueChanged;

        public int Width { get { return m_gridInfo.Row; } }
        public int Height { get { return m_gridInfo.Column; } }
        public int Row { get { return m_gridInfo.Row; } }
        public int Column { get { return m_gridInfo.Column; } }
        public float CellSize { get { return m_gridInfo.CellSize; } }
        public Vect2 OriginPosition { get { return m_gridInfo.OriginPosition; } }

        private TGridObject[,] m_gridArray;
        private GridInfo m_gridInfo;
        private float m_offset;

        public Grid(GridInfo gridInfo, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
            m_gridInfo = gridInfo;
            m_offset = CellSize / 2f;
            m_gridArray = new TGridObject[Row, Column];
            for(int x = 0; x < Row; x++) {
                for(int y = 0; y < Column; y++) {
                    m_gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        public void GetWorldPosition(Vect2Int coordinate, ref Vect2 worldPosition) {
            GetWorldPosition(coordinate.X, coordinate.Y, ref worldPosition);
        }

        public void GetWorldPosition(int x, int y, ref Vect2 worldPosition) {
            worldPosition.Set(x * CellSize + m_offset + OriginPosition.X, y * CellSize + m_offset + OriginPosition.Y);
        }

        private bool CheckLimit(int x, int y) {
            return x >= 0 && y >= 0 && x < Row && y < Column;
        }

        public void GetXY(Vect2 worldPosition, out int x, out int y) {
            x = GetX(worldPosition.X);
            y = GetY(worldPosition.Y);
        }

        public void GetXY(Vect2 worldPosition, ref Vect2Int vect2) {
            vect2.Set(GetX(worldPosition.X), GetY(worldPosition.Y));
        }

        private int GetX(float x) {
            return MathCalculator.FloorToInt((x - OriginPosition.X) / CellSize);
        }

        private int GetY(float y) {
            return MathCalculator.FloorToInt((y - OriginPosition.Y) / CellSize);
        }
        
        public void SetGridObject(int x, int y, TGridObject value) {
            if(CheckLimit(x, y)) {
                m_gridArray[x, y] = value;
                onGridValueChanged?.Invoke(x, y, value);
            }
        }

        public void ForeachGridObject(Action<TGridObject> onForeachHandler) {
            for(int x = 0; x < Row; x++) {
                for(int y = 0; y < Column; y++) {
                    onForeachHandler?.Invoke(m_gridArray[x, y]);
                }
            }
        }

        public void SetGridObject(Vect2 worldPosition, TGridObject value) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetGridObject(x, y, value);
        }

        public TGridObject GetGridObject(Vect2Int coordinate) {
            return GetGridObject(coordinate.X, coordinate.Y);
        }

        public TGridObject GetGridObject(int x, int y) {
            if(CheckLimit(x, y)) {
                return m_gridArray[x, y];
            } else {
                return default(TGridObject);
            }
        }

        public TGridObject GetGridObject(Vect2 worldPosition) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetGridObject(x, y);
        }
    
        public void Dispose() {
            for(int x = 0;x < Row; x++) {
                for( int y = 0; y < Column; y++) {
                    m_gridArray[x, y].Dispose();
                }
            }
            m_gridArray = null;
            m_gridInfo.Dispose();
            m_gridInfo = null;
        }

    }
}