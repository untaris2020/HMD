using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StyleSheet : MonoBehaviour { 

        public Font HeadingFont;
        public Font BodyFont;
        public Font ErrorFont;

        public int HeadingFontSize;
        public int BodyFontSize;
        public int ErrorFontSize;

        public Color HeadingFontColor;
        public Color BodyFontColor;
        public Color ErrorFontColor;
        public Color GreenFontColor;
        public Color YellowFontColor;
        public Color RedFontColor;

        public FontStyle HeadingFontStyle;
        public FontStyle BodyFontStyle;
        public FontStyle ErrorFontStyle;
}

//Assignment Process

//declare variables
//public GameObject styleObject;
//public StyleSheet style;

//connect to external game object
//style = styleObject.GetComponent<StyleSheet>();

//Set Font Values
//HEADING
//myText.font = style.HeadingFont;
//myText.color = style.HeadingFontColor;
//myText.fontSize = style.HeadingFontSize;
//myText.fontStyle = style.HeadingFontStyle; 

//BODY
//myText.font = style.BodyFont;
//myText.color = style.BodyFontColor;
//myText.fontSize = style.BodyFontSize;
//myText.fontStyle = style.BodyFontStyle; 