using UnityEngine;
using System.Collections;

public class UIWarningController : MonoBehaviour
{
    [Header("UI Tanda Seru")]
    public GameObject leftWarning;
    public GameObject centerWarning;
    public GameObject rightWarning;

    [Header("Settings")]
    public float warningDuration = 2f;

    public void ShowWarning(int lane)
    {
        HideAllWarnings();

        GameObject warningToShow = null;

        if (lane == 0) warningToShow = leftWarning;
        else if (lane == 1) warningToShow = centerWarning;
        else if (lane == 2) warningToShow = rightWarning;

        if (warningToShow != null)
        {
            warningToShow.SetActive(true);
            StartCoroutine(HideWarningAfterDelay(warningToShow));
        }
    }

    private IEnumerator HideWarningAfterDelay(GameObject warning)
    {
        yield return new WaitForSeconds(warningDuration);
        if (warning != null)
            warning.SetActive(false);
    }

    public void HideAllWarnings()
    {
        if (leftWarning != null) leftWarning.SetActive(false);
        if (centerWarning != null) centerWarning.SetActive(false);
        if (rightWarning != null) rightWarning.SetActive(false);
    }
}
