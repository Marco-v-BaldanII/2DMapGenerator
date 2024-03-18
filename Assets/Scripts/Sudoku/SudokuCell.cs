using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SudokuCell : MonoBehaviour
{


    public bool filled = false;


    public TMP_InputField[] inputField;

    // Start is called before the first frame update

    public Vector2Int[] positions;
    void Awake()
    {
        inputField = new TMP_InputField[9];


        int i = 0;
        foreach (Transform childTransform in transform)
        {
            // Check if the child has a TMP_InputField component
            TMP_InputField childInputField = childTransform.GetComponent<TMP_InputField>();
            if (childInputField != null)
            {
                // Assign the TMP_InputField component to the array
                inputField[i] = childInputField;

                // add ValidateInput as a listener on input validation
                inputField[i].onValidateInput += ValidateInput;

                ++i;
            }
        }

    }


    char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Convert the added character to a digit
        int digit;
        bool isDigit = int.TryParse(addedChar.ToString(), out digit);

        // Check if the character is a digit and within the range of 1 to 9
        if (isDigit && digit >= 1 && digit <= 9)
        {
            // Allow the character
            return addedChar;
        }
        else
        {
            // Disallow the character
            return '\0'; // Return null character to indicate disallowing the input
        }
    }

    // Update is called once per frame
    void Update()
    {

        bool complete = true;
       for(int i = 0; i < 9; ++i)
        {
            if(inputField[i].text == null || inputField[i].text == "")
            {
                complete = false;
            }
            

        }
        if (complete)
        {
            filled = true;
        }


        if (Input.GetKeyDown(KeyCode.K))
        {
            for(int i = 0; i < 9; ++i)
            {
                inputField[i].text = Random.Range(1, 9).ToString();
            }
        }

    }
}
