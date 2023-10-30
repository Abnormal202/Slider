using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JungleRecipeBookUI : MonoBehaviour
{
    public RecipeList recipeList;

    public Image displayImage;
    public TextMeshProUGUI displayNameText;
    public TextMeshProUGUI displayNumberText;
    public TextMeshProUGUI recipeCompletionText;
    public List<JungleRecipeWidget> recipeWidgets; // There are three of these

    private int currentShapeIndex; // Horizontal
    private int currentRecipePageIndex; // Vertical
    private Shape currentShape;

    public void SetCurrentShape(int index)
    {
        currentShapeIndex = index;
        currentShape = recipeList.list[index].result;
        
        displayImage.sprite = currentShape.GetDisplaySprite();
        displayNameText.text = currentShape.GetDisplayName();
        displayNumberText.text = (index + 1).ToString().PadLeft(2, '0');

        int totalRecipes = recipeList.list[index].combinations.Count;
        int completedRecipes = totalRecipes - 1;
        recipeCompletionText.text = $"{completedRecipes}/{totalRecipes}";

        SetCurrentRecipeDisplay(0);
    }

    public void SetCurrentRecipeDisplay(int index)
    {
        currentRecipePageIndex = index;

        for (int i = 0; i < 3; i++)
        {
            Recipe.Shapes shapes = null;
            if (index + i < recipeList.list[currentShapeIndex].combinations.Count)
            {
                shapes = recipeList.list[currentShapeIndex].combinations[index + i];
            }
            recipeWidgets[i].SetIngredientsOrNull(shapes, recipeList.list[currentShapeIndex].result);
        }
    }

    public void IncrementCurrentShape()
    {
        int index = (currentShapeIndex + 1) % recipeList.list.Count;
        SetCurrentShape(index);
    }

    public void DecrementCurrentShape()
    {
        int index = (currentShapeIndex + recipeList.list.Count - 1) % recipeList.list.Count;
        SetCurrentShape(index);
    }

    public void IncrementRecipeDisplay()
    {
        int count = recipeList.list[currentShapeIndex].combinations.Count / 3;
        int index = (currentRecipePageIndex + 1) % count;
        SetCurrentRecipeDisplay(index);
    }

    public void DecrementRecipeDisplay()
    {
        int count = recipeList.list[currentShapeIndex].combinations.Count / 3;
        int index = (currentRecipePageIndex + count - 1) % count;
        SetCurrentRecipeDisplay(index);
    }
}
