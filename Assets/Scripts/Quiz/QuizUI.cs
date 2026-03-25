using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Texts")]
    public TMP_Text questionText;
    public TMP_Text optionAText;
    public TMP_Text optionBText;

    [Header("Buttons")]
    public Button optionAButton;
    public Button optionBButton;

    private QuestionManager questionManager;

    void Awake()
    {
        questionManager = FindFirstObjectByType<QuestionManager>();

        optionAButton.onClick.RemoveAllListeners();
        optionBButton.onClick.RemoveAllListeners();

        optionAButton.onClick.AddListener(() => Submit(0));
        optionBButton.onClick.AddListener(() => Submit(1));

        Hide();
    }

    public void Show(string question, string optionA, string optionB)
    {
        if (panel != null) panel.SetActive(true);

        if (questionText != null) questionText.text = question;
        if (optionAText != null) optionAText.text = optionA;
        if (optionBText != null) optionBText.text = optionB;
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Submit(int selectedIndex)
    {
        if (questionManager != null)
            questionManager.SubmitAnswerFromUI(selectedIndex);
        else
            Debug.LogWarning("QuizUI: QuestionManager bulunamadý.");
    }
}