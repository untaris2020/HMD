using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


public class CamerasManager : MonoBehaviour
{
    public static CamerasManager Instance;

    // To Display Data on Screen
    Texture2D thisTexture;
    public GameObject ImageHolder;
    private string prevFrame; 
    private string defaultTransfer = "/9j/4AAQSkZJRgABAQEASABIAAD/4QMsaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA1LjYtYzE0MCA3OS4xNjA0NTEsIDIwMTcvMDUvMDYtMDE6MDg6MjEgICAgICAgICI+IDxyZGY6UkRGIHhtbG5zOnJkZj0iaHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyI+IDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSIiIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCBDQyAoTWFjaW50b3NoKSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo2RjkzNkEwRDdCQzYxMUU4OTQyRkE5MUZBODVDODExMCIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDo2RjkzNkEwRTdCQzYxMUU4OTQyRkE5MUZBODVDODExMCI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOkFERTM4MUEwN0JCRTExRTg5NDJGQTkxRkE4NUM4MTEwIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjZGOTM2QTBDN0JDNjExRTg5NDJGQTkxRkE4NUM4MTEwIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+/+EAGEV4aWYAAElJKgAIAAAAAAAAAAAAAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCADwAUADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD5/ooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD0H4NaDpniPx6thq1ol1am2kfy3JAyMYPFcbrkEdrr+pW8KBIorqVEUdlDEAV6J+z/8A8lNT/rzl/pXn/iP/AJGjVv8Ar9m/9DNACaHoGp+JNR+waTaNdXWwv5akA4HU81Y8Q+Etc8KvAmtafJaNcAmIOQdwGM9D7ivRfDlrH4H+F9p8Q9J3NrUkzWrLOd0OwsQflGDnCjvV62upPjB4S1/WfEuEutAtmezFmPLUllZjuBznlB6UAeSaHoGp+JNR+waTaNdXWwv5akA7R1PNb158LPGmn2U95daFPHbwRtJI5ZcKoGSetdh4ctY/A/wvtPiHpO5takma1ZZzui2FiD8owc/KO9dh4W8f6z47+HHjaTV1tla0smWPyIyv3o3znJPoKAPAdD0DU/Emo/YNItGurrYX8tSAdo6nmvWPC/wtl0/wP4svPFeheXdwWrSWTytypCMSRg+u3rVXw5ax+B/hfafETSctrUkzWrLOd0OwsQflGDn5R3rsPC3j/WPHfw48bSautsptLJlj8iMr96N85yT6CgDyq4/4RD/hUVt5Xk/8JV9p/eY3b/L3N+HTFV4PhR43ureK4h0Gd4pUDowZeVIyD1rQufBWlRfBS18XK1x/aUt35LAuPL27mHTHXAHevUfiB8Rtb8CaD4Tj0hbVlurBWk8+Mt91UxjBHqaAPnrWdF1Dw/qcmnapbNbXcYBaNiCQCMjp7Ve8PeDdf8VJO+i6dJdrAQJShA2k5x1Psa77xjp0Hij4ZR/Ea/LjW7u5W3kWI4h2qSowvJzhR3rW1jUZ/gtpWjP4XCudctVuLr7aPMwyquNuMYHzn1oA8z1z4f8Aijw5px1DVtJltbUOE8xmUjJ6Dg1zVe/eNfEt94t/Z6ttY1ERC5mv1VhCpVflZgOMn0rwGgAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooA9S/Z/IHxNQk4/wBDl/pXWeLvgVZ2tjrfiAa/Kzok155PkDBPLbc5/CvBIppYH3wyvG+MbkYg/pUzaheuhRry4ZSMEGViCPzoA9S8BXn/AAn3hu2+GkyizgiL3n25TuYlWJ27TgfxevatTxJar8FdDvdDtZP7VXxHbyI8sn7swbRt4Azn7/6V4nFNLA++GV43xjcjEH9KdNcz3BHnzyS7em9y2PzoA9X8BXv/AAn3hu2+GkyizhjL3f25TuYlWJ27TgfxevavSbL4dW/w9+Hfi+KHU2vftljIxLRhNu2NvQn1r5dimlgffDK8b4xuRiD+lTNqF66lWvLhlIwQZWII/OgD1LwFef8ACfeG7b4aTKLOCMvefblO5iVYnbtOB/F69q9Ksvh1B8Pfh34vih1Nr37ZYyMS0YTbtjf0J9a+XYppYH3wyvG+MbkYg/pUzaheupVry4ZWGCDKxBH50Aeu3pH/AAy7YDIz/aH/ALUek+OZB0XwRg5/4lx/9Bjrx43ExgEBmk8oHIj3Hbn6US3E04USzSSBBhQ7E4HtQB7DfEf8Mu2AyM/2h0/7aPWjo2nJ8dNMto7qb+yP7AhS2Tyx5vnBlHJzjH3P1rw03ExgEBmk8oHIj3Hbn6V2/wAPfh9rfjeG/k0jU4rMWrIsgd2Xduzj7v0NAHqHxK8Lx+DfgVDosV210kN8jCVlCk7mY9PxrzvXvhlb6NqfhG0TVHlGvbN7GMDydxTpzz9/9K1fHerjw34DHw41Ay3GrWlwtw90pzGysS4AJ5zhh2ryhrq4cxl55WMf3CXJ2/T0oA9Mt/hPbTfFa48G/wBryCKK28/7V5QyTtBxjPvXnGp2gsNVvLNXLi3neIMRjdtYjP6VGLy5E5nFxMJSMGTed2PrURJZizEkk5JPegBKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigArW0bxRrnh5Jl0jVLmyWYgyCF9u7HTP5msmigC5qeq3+s3z32pXUt1dOAGllOWIAwKp0UUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFAH//2Q==";
    private string frame = "";
    private bool newFrame = false; 
    public void spawnCam()
    {
        ImageHolder.SetActive(true);
    }
    public void destroyCam()
    {
        ImageHolder.SetActive(false);
    }


    private void Awake()
    { 
        Instance = this;
    }

    public void Start()
    {
        updateFrame(defaultTransfer);
    }

    public void Update()
    {
        if(newFrame)
        {
            newFrame = false; 
            try
            {
                if (thisTexture = new Texture2D(2, 2))
                {
                    byte[] bytesConv = System.Convert.FromBase64String(frame);
                    thisTexture.LoadImage(bytesConv);
                    ImageHolder.GetComponent<RawImage>().texture = thisTexture;
                    prevFrame = frame;
                    Resources.UnloadUnusedAssets();
                }
                else
                {
                    byte[] bytesConv = System.Convert.FromBase64String(prevFrame);
                    thisTexture.LoadImage(bytesConv);
                    ImageHolder.GetComponent<RawImage>().texture = thisTexture;
                    Resources.UnloadUnusedAssets();
                }
            }
            catch (FormatException e)
            {
                // Catches the Exceptions
                Debug.Log("Hit exception rendering frame: " + e); 
            }
        }
    }

    public void updateFrame(string transfer)
    {
        frame = transfer;
        newFrame = true;
    }
}