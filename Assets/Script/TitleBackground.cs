using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBackground : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    private GoalManager goalManager;
    // Start is called before the first frame update
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hitPoints <= 0)
        {
            if(goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoal();
            }
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        // take the current color
        Color color = sprite.color;
        // Get current color's alpha value
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
