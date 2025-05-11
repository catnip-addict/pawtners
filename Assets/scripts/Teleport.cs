using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Teleport : MonoBehaviour
{
    TMP_Dropdown dropdownSelect;
    void Start()
    {
        dropdownSelect = GetComponent<TMP_Dropdown>();
        dropdownSelect.onValueChanged.AddListener(delegate { DropDownValueChanged(dropdownSelect); });
        InitializeQualityDropdown();
    }
    public void Tp(int id)
    {
        Debug.Log("Teleporting to scene: " + id);
        SceneManager.LoadScene(id);
    }
    public void InitializeQualityDropdown()
    {
        if (dropdownSelect == null) return;

        dropdownSelect.ClearOptions();

        List<string> qualityOptions = new List<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            qualityOptions.Add(i.ToString());
        }

        dropdownSelect.AddOptions(qualityOptions);

        dropdownSelect.value = SceneManager.GetActiveScene().buildIndex;
        dropdownSelect.RefreshShownValue();
    }
    void DropDownValueChanged(TMP_Dropdown change)
    {
        if (dropdownSelect.value == SceneManager.GetActiveScene().buildIndex)
        {
            return;
        }
        int selectedIndex = dropdownSelect.value;
        if (selectedIndex >= 0 && selectedIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            Tp(selectedIndex);
        }
    }
}
