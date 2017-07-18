namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    public class CEquipLineSet
    {
        private const int c_rowCount = 12;
        public CanvasGroup[][] m_horizontalLines = new CanvasGroup[12][];
        public CanvasGroup[] m_verticalLines;

        public CEquipLineSet()
        {
            for (int i = 0; i < 12; i++)
            {
                this.m_horizontalLines[i] = new CanvasGroup[Enum.GetValues(typeof(enHorizontalLineType)).Length];
            }
            this.m_verticalLines = new CanvasGroup[11];
        }

        public void Clear()
        {
            for (int i = 0; i < this.m_horizontalLines.Length; i++)
            {
                for (int k = 0; k < this.m_horizontalLines[i].Length; k++)
                {
                    this.m_horizontalLines[i][k] = null;
                }
            }
            for (int j = 0; j < this.m_verticalLines.Length; j++)
            {
                this.m_verticalLines[j] = null;
            }
        }

        public void DisplayHorizontalLine(int row, enHorizontalLineType type)
        {
            if (this.m_horizontalLines[row][(int) type] != null)
            {
                this.m_horizontalLines[row][(int) type].set_alpha(1f);
            }
        }

        public void DisplayVerticalLine(int startRow, int endRow)
        {
            if (startRow != endRow)
            {
                for (int i = startRow; i < endRow; i++)
                {
                    if (this.m_verticalLines[i] != null)
                    {
                        this.m_verticalLines[i].set_alpha(1f);
                    }
                }
            }
        }

        public void HideAllLines()
        {
            for (int i = 0; i < this.m_horizontalLines.Length; i++)
            {
                for (int k = 0; k < this.m_horizontalLines[i].Length; k++)
                {
                    if (this.m_horizontalLines[i][k] != null)
                    {
                        this.m_horizontalLines[i][k].set_alpha(0f);
                    }
                }
            }
            for (int j = 0; j < this.m_verticalLines.Length; j++)
            {
                if (this.m_verticalLines[j] != null)
                {
                    this.m_verticalLines[j].set_alpha(0f);
                }
            }
        }

        public void InitializeHorizontalLine(int row, enHorizontalLineType type, GameObject gameObject)
        {
            CanvasGroup component = gameObject.GetComponent<CanvasGroup>();
            if (component == null)
            {
                component = gameObject.AddComponent<CanvasGroup>();
            }
            component.set_alpha(0f);
            this.m_horizontalLines[row][(int) type] = component;
        }

        public void InitializeVerticalLine(int startRow, int endRow, GameObject gameObject)
        {
            if ((endRow - startRow) == 1)
            {
                CanvasGroup component = gameObject.GetComponent<CanvasGroup>();
                if (component == null)
                {
                    component = gameObject.AddComponent<CanvasGroup>();
                }
                component.set_alpha(0f);
                this.m_verticalLines[startRow] = component;
            }
        }

        public enum enHorizontalLineType
        {
            Left,
            Right
        }
    }
}

