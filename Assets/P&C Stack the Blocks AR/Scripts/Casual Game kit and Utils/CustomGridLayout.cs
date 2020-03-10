using System.Collections;
using UnityEngine;

namespace PnCCasualGameKit
{
    /// <summary>
    /// 1. Static : Does not change with resizing of container
    /// 2. Fits all the child elements according to given no of coloumns, padding and spacing
    /// 3. Sets the height for container.
    /// </summary>
    public class CustomGridLayout : MonoBehaviour
    {
        RectTransform container;
        public int coloumns;
        public float padding;
        public float spacing;

        IEnumerator Start()
        {
            container = GetComponent<RectTransform>();
            if (coloumns <= 0)
            {
                Debug.Log("Number of coloums in zero or invalid");
                yield break;
            }

            if (spacing < 0 || padding < 0)
            {
                Debug.Log("Invalid spacing/padding");
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            float alreayUsedSpace = padding * 2 + (coloumns - 1) * spacing;

            float cellWidth = (container.rect.width - alreayUsedSpace) / coloumns;
            int currentRow = 0;
            int currentColoumn = 0;

            //Iterate through all children and set the anchors, anchoredPosition, and sizeDelta from left and top of parent
            foreach (Transform trans in container.transform)
            {
                trans.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding + currentColoumn * (cellWidth + spacing), cellWidth);
                trans.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding + currentRow * (cellWidth + spacing), cellWidth);

                currentColoumn++;
                if (currentColoumn % coloumns == 0)
                {
                    currentRow++;
                    currentColoumn = 0;
                }
            }

            yield return new WaitForEndOfFrame();

            //Calculate the height by adding the height of all cells with the spacing and padding.
            int noOfRows = (int)Mathf.Ceil((float)container.transform.childCount / (float)coloumns);
            container.SetSizeWithCurrentAnchors((RectTransform.Axis)1, cellWidth * (noOfRows) + spacing * (noOfRows - 1) + padding * 2);
        }
    }
}