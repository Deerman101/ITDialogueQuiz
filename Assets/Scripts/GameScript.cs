using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class QuestionList
{
    public string question;
    public string[] answers = new string[3];
}

public class GameScript : MonoBehaviour
{
    public QuestionList[] questionHeads;
    public TextMeshProUGUI[] answersText;
    public TextMeshProUGUI questionText;
    public GameObject gamePanel;
    public Button[] answerButtons = new Button[3];
    public Sprite[] TFIcons = new Sprite[2];
    public Image TFIcon;
    public TextMeshProUGUI TFText;

    List<object> qList;
    QuestionList currentQuestion;
    int randQuestion;
    bool defaultColor = false, trueColor = false, falseColor = false;

    void Update()
    {/*!!!!!*/
        if (defaultColor) gamePanel.GetComponent<Image>().color = Color.Lerp(gamePanel.GetComponent<Image>().color, Color.blue, 8 * Time.deltaTime);
        if (trueColor) gamePanel.GetComponent<Image>().color = Color.Lerp(gamePanel.GetComponent<Image>().color, Color.green, 8 * Time.deltaTime);
        if (falseColor) gamePanel.GetComponent<Image>().color = Color.Lerp(gamePanel.GetComponent<Image>().color, Color.red, 8 * Time.deltaTime);
    }

    public void OnClickPlay()
    {
        qList = new(questionHeads);
        QuestionsGenerate();

        if (!gamePanel.GetComponent<Animator>().enabled) gamePanel.GetComponent<Animator>().enabled = true;
        else gamePanel.GetComponent<Animator>().SetTrigger("In");
    }

    void QuestionsGenerate()
    {
        if (qList.Count > 0)
        {
            randQuestion = Random.Range(0, qList.Count);
            currentQuestion = qList[randQuestion] as QuestionList;
            questionText.text = currentQuestion.question;
            questionText.GetComponent<Animator>().SetTrigger("In");
            List<string> answers = new(currentQuestion.answers);
            for (int i = 0; i < currentQuestion.answers.Length; i++)
            {
                int rand = Random.Range(0, answers.Count);
                answersText[i].text = answers[rand];
                answers.RemoveAt(rand);
            }
            StartCoroutine(AnimButtons());
        }
        else
        {
            gamePanel.gameObject.GetComponent<Animator>().SetTrigger("Out");
            print("Вопросы кончились, увынск(");
        }
    }

    public void AnswersButtons(int index)
    {
        if (answersText[index].text.ToString() == currentQuestion.answers[0]) StartCoroutine(TrueOrFalseAnswer(true));
        else StartCoroutine(TrueOrFalseAnswer(false));
        //qList.RemoveAt(randQuestion);
        //QuestionsGenerate();
    }

    IEnumerator AnimButtons()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
            answerButtons[i].interactable = false;
        }
        yield return new WaitForSeconds(1);

        int count = 0;
        while (count < answerButtons.Length)
        {
            if (!answerButtons[count].gameObject.activeSelf) answerButtons[count].gameObject.SetActive(true);
            else answerButtons[count].gameObject.GetComponent<Animator>().SetTrigger("In");
            count++;
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < answerButtons.Length; i++) answerButtons[i].interactable = true;
        yield break;
    }

    IEnumerator TrueOrFalseAnswer(bool check)
    {
        for (int i = 0; i < answerButtons.Length; i++) answerButtons[i].interactable = false;
        yield return new WaitForSeconds(1);

        defaultColor = false;

        /*важная темка*/
        for (int i = 0; i < answerButtons.Length; i++) answerButtons[i].gameObject.GetComponent<Animator>().SetTrigger("Out");
        questionText.gameObject.GetComponent<Animator>().SetTrigger("Out");

        yield return new WaitForSeconds(0.5f);

        if (!TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(true);
        else TFIcon.gameObject.GetComponent<Animator>().SetTrigger("In");

        if (check)
        {
            trueColor = true;
            TFIcon.sprite = TFIcons[0];
            TFText.text = "Правильный ответ!";
            yield return new WaitForSeconds(2);
            TFIcon.GetComponent<Animator>().SetTrigger("Out");
            qList.RemoveAt(randQuestion);
            QuestionsGenerate();
            trueColor = false;
            defaultColor = true;
            yield break;
        }
        else
        {
            falseColor = true;
            TFIcon.sprite = TFIcons[1];
            TFText.text = "Неправильный ответ!";
            yield return new WaitForSeconds(2);
            TFIcon.GetComponent<Animator>().SetTrigger("Out");
            gamePanel.GetComponent<Animator>().SetTrigger("Out");
            falseColor = false;
            defaultColor = true;
            yield break;
        }
    }
}