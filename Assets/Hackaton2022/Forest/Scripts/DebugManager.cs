using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hackaton2022
{
    /// <summary>
    /// Klassen är en singeltonklass som endast användas för att skriva ut meddelanden om vilka
    /// värden/tillstånd som appen använder eller felmeddelanden. 
    /// </summary>
    [DefaultExecutionOrder(-2)]
    public class DebugManager : Singleton<DebugManager>
    {
        [SerializeField]
        private TextMeshProUGUI debugInfo;

        [SerializeField]
        private bool isActivate = true;

        private string tmpStatus = "";

        public int maxLines = 30;

        private void Start()
        {
            if (!isActivate)
            {
                debugInfo.text = "";
                //this.gameObject.SetActive(false);
            }
        }

        public void WriteInfoMessage(string msg)
        {
            if (isActivate)
                debugInfo.text = msg;
        }

        public void AddDebugMessage(string msg)
        {
            if (isActivate)
            {
                if (tmpStatus.Split('\n').Length > maxLines)
                {
                    ClearDebugMessage();
                }
                tmpStatus += msg + '\n';
                debugInfo.text = tmpStatus;
            }
        }

        public void ClearDebugMessage()
        {
            tmpStatus = "";
            debugInfo.text = tmpStatus;
        }
    }
}
