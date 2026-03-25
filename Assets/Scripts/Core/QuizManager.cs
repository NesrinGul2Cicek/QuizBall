using UnityEngine;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public ActionType actionType;
        [TextArea] public string question;
        public string optionA;
        public string optionB;
        public int correctIndex; // 0=A, 1=B
    }

    public Question[] questions;

    void Awake()
    {
        if (questions == null || questions.Length == 0)
            Debug.LogWarning("QuizManager: soru listesi boþ.");
    }

    public Question GetQuestionForAction(ActionType actionType)
    {
        if (questions == null || questions.Length == 0)
            return null;

        foreach (var q in questions)
        {
            if (q != null && q.actionType == actionType)
                return q;
        }

        return null;
    }

    public bool IsCorrect(Question question, int selectedIndex)
    {
        if (question == null) return false;
        return selectedIndex == question.correctIndex;
    }
}