using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Hackaton2022.Skogen
{
    public class PortalManager : MonoBehaviour
    {
        //This materials matter needs to be optimizated!
        public Material[] materials;

        private Vector3 camPostionInPortalSpace;

        [SerializeField]
        private AudioSource audioSource;

        bool audioIsActive = false;

        bool wasInFront;
        bool inOtherWorld;

        bool hasCollided;

        // Start is called before the first frame update
        void Start()
        {
            SetMaterials(false);
        }

        /// <summary>
        /// S�tter en egenskap hos shader-materialet som avg�r om de ska "maskas" bort
        /// eller visas helt. Det �r denna funktion som simulerar att du tr�tt in i 
        /// portalen och befinner dig i den andra v�rlden eller st�r utanf�r den. 
        /// </summary>
        /// <param name="fullRender"></param>
        void SetMaterials(bool fullRender)
        {
            var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;

            foreach (var mat in materials)
            {
                mat.SetInt("_StencilComp", (int)stencilTest);
            }
        }

        //Set bidirectional function
        bool GetIsInFront()
        {
            // H�mta referens till huvudkameran (AR kameran)
            GameObject MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            // Ber�kna om dess "worldposition" vart den befinner sig 
            Vector3 worldPos = MainCamera.transform.position + MainCamera.transform.forward * Camera.main.nearClipPlane;
            camPostionInPortalSpace = transform.InverseTransformPoint(worldPos);
            // och returnerar om kameran befinner sig framf�r Portalen" eller p� andra sidan om portalen
            return camPostionInPortalSpace.y >= 0 ? true : false;
        }

        private void OnTriggerEnter(Collider collider)
        {
            DebugManager.Instance.AddDebugMessage("You have collided with the portal!");

            GameObject MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (collider.transform != MainCamera.transform)
                return;

            wasInFront = GetIsInFront();
            hasCollided = true;

            audioIsActive = !audioIsActive;

            // V�xla till att spela eller inte spela. 
            if (audioIsActive)
                audioSource.Play();
            else
                audioSource.Stop();

        }

        // Update is called once per frame
        void OnTriggerExit(Collider collider)
        {
            GameObject MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (collider.transform != MainCamera.transform)
                return;
            hasCollided = false;
        }

        /// <summary>
        /// Detta sker undertiden du kolliderar med box-collider
        /// </summary>
        void whileCameraColliding()
        {
            if (!hasCollided)
                return;
            bool isInFront = GetIsInFront();
            if ((isInFront && !wasInFront) || (wasInFront && !isInFront))
            {
                inOtherWorld = !inOtherWorld;
                SetMaterials(inOtherWorld); // Byt material
            }
            wasInFront = isInFront;
        }

        private void OnDestroy()
        {
            SetMaterials(true);
        }

        private void Update()
        {
            whileCameraColliding();
        }

    }

}