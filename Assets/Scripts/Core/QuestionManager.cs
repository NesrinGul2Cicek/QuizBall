using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    [Header("Refs")]
    public QuizManager quizManager;
    public QuizUI quizUI;

    private bool waitingAnswer;
    private ActionType currentActionType;
    private QuizManager.Question currentQuestion;

    public bool IsWaitingAnswer => waitingAnswer;

    void Awake()
    {
        if (quizManager == null) quizManager = FindFirstObjectByType<QuizManager>();
        if (quizUI == null) quizUI = FindFirstObjectByType<QuizUI>();
    }

    public void ShowQuestion(ActionType actionType)
    {
        currentActionType = actionType;
        waitingAnswer = true;

        if (quizManager == null)
        {
            Debug.LogWarning("QuestionManager: QuizManager yok.");
            waitingAnswer = false;
            GameManager.Instance.OnQuestionAnswered(false);
            return;
        }

        currentQuestion = quizManager.GetQuestionForAction(actionType);

        if (currentQuestion == null)
        {
            Debug.LogWarning($"QuestionManager: {actionType} için soru bulunamadý.");
            waitingAnswer = false;
            GameManager.Instance.OnQuestionAnswered(false);
            return;
        }

        if (quizUI != null)
        {
            quizUI.Show(
                currentQuestion.question,
                currentQuestion.optionA,
                currentQuestion.optionB
            );
        }
        else
        {
            Debug.Log($"Soru açýldý: {currentActionType}");
            Debug.Log($"A: {currentQuestion.optionA}");
            Debug.Log($"B: {currentQuestion.optionB}");
        }
    }

    public void SubmitAnswerFromUI(int selectedIndex)
    {
        if (!waitingAnswer) return;

        bool isCorrect = quizManager != null && quizManager.IsCorrect(currentQuestion, selectedIndex);

        waitingAnswer = false;

        if (quizUI != null)
            quizUI.Hide();

        GameManager.Instance.OnQuestionAnswered(isCorrect);
    }

    public void CancelQuestion()
    {
        if (!waitingAnswer) return;

        waitingAnswer = false;

        if (quizUI != null)
            quizUI.Hide();

        GameManager.Instance.CancelPendingAction();
    }

    void Update()
    {
        if (!waitingAnswer) return;
        if (GameManager.Instance == null) return;

        // UI kapalýysa test amaçlý klavye desteði
        if (quizUI == null || quizUI.panel == null || !quizUI.panel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A))
                SubmitAnswerFromUI(0);
            else if (Input.GetKeyDown(KeyCode.B))
                SubmitAnswerFromUI(1);
            else if (Input.GetKeyDown(KeyCode.Escape))
                CancelQuestion();
        }
    }
}