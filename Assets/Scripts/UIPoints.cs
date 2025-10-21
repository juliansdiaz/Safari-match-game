using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPoints : MonoBehaviour
{
    int displayedPoints = 0;
    public TextMeshProUGUI pointsLabel;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onPointsUpdated.AddListener(UpdatePoints);
    }

    void UpdatePoints()
    {
        StartCoroutine(UpdatePointCount());
    }

    IEnumerator UpdatePointCount()
    {
        while (displayedPoints < GameManager.Instance.points)
        {
            displayedPoints++;
            pointsLabel.text = displayedPoints.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
}
