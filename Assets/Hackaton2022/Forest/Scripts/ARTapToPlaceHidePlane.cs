using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Hackaton2022.Skogen
{

    public class ARTapToPlaceHidePlane : MonoBehaviour
    {
        [SerializeField]
        private GameObject cursorIndicator;

        [SerializeField]
        private GameObject prefabToInstantiate;

        [SerializeField]
        private ARRaycastManager raycastManager;

        [SerializeField]
        private ARPlaneManager planeManager;

        public bool useIndicator = true;

        private bool isPlaneToggled = true;

        private GameObject spawnedObject = null;

        private void OnEnable()
        {
            planeManager.planesChanged += PlaneManager_planesChanged;
        }

        private void OnDisable()
        {
            planeManager.planesChanged -= PlaneManager_planesChanged;
        }

        private void PlaneManager_planesChanged(ARPlanesChangedEventArgs obj)
        {
            if (obj.added != null)
            {

            }
        }

        // Start is called before the first frame update
        void Start()
        {
            cursorIndicator.SetActive(useIndicator);
        }

        /// <summary>
        /// Kollar om n�gra av v�ra fingrar tryckte p� sk�rmen.
        /// OBS detta utg�r / baserar sig p� det gamla input systemet
        /// </summary>
        /// <param name="touchPosition"></param>
        /// <returns></returns>
        bool TryGetTouchPosition(out Vector2 touchPos)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchPos = Input.GetTouch(0).position; // kolla bara vilket som var det f�rsta fingret
                return true;
            }

            touchPos = default;
            return false;
        }


        // Update is called once per frame
        void Update()
        {

            if (spawnedObject != null)
                return;

            // Ska vi visa indikatorn (cursor) ? is�fall uppdatera dess position p� sk�rmen
            if (useIndicator)
                UpdateCursorIndicator();

            // L�s av om vi touchat med finger p� sk�rmen, is�fall returnera dess position (touchPosition)
            if (!TryGetTouchPosition(out Vector2 touchPosition))
            {
                return;
            }

            

            // Ska vi skapa ett objekt p� den position d�r indikatorn finns ?
            if (useIndicator)
                spawnedObject = Instantiate(prefabToInstantiate, transform.position, transform.rotation);
            else
            {
                // Annars placera den annars vart man tryckte p� sk�rmen
                // (oavsett vart indicatorns position befann sig)

                DebugManager.Instance.AddDebugMessage("OK TOUCH");

                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                raycastManager.Raycast(touchPosition, hits, TrackableType.Planes);

               
                Pose pose = hits[0].pose;

                DebugManager.Instance.AddDebugMessage("Placing out : " + pose.position);

                spawnedObject = Instantiate(prefabToInstantiate, pose.position, pose.rotation);
                

            }

            TogglePlaneDetection();

        }

        /// <summary>
        /// Uppdaterar indikatorns position p� mobilsk�rmen. 
        /// </summary>
        private void UpdateCursorIndicator()
        {
            Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            raycastManager.Raycast(screenPosition, hits, TrackableType.Planes);

            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;

            }
        }



        public void TogglePlaneDetection()
        {
            // Toggle
            isPlaneToggled = !isPlaneToggled;

            // # alternativ 1)
            planeManager.enabled = isPlaneToggled;

            // # alternativ 2) annat alternativ p� planeManager.enabled = !planeManager.enabled;
            //planeManager.SetTrackablesActive(isPlaneToggled);
            //planeManager.planePrefab.SetActive(isPlaneToggled);

            // Troligen kan vi �ven dra nytt av : planeManager.gameObject.SetActive(isPlaneToggled);

            foreach (ARPlane plane in planeManager.trackables)
            {
                // plane.trackingState == TrackingState.
                plane.gameObject.SetActive(isPlaneToggled);
            }

        }

        public void ActivatePlaneDetection(bool ch)
        {
            isPlaneToggled = ch;

            // # alternativ 1)
            planeManager.enabled = isPlaneToggled;

            // # alternativ 2) annat alternativ p� planeManager.enabled = !planeManager.enabled;
            //planeManager.SetTrackablesActive(isPlaneToggled);
            //planeManager.planePrefab.SetActive(isPlaneToggled);

            // Troligen kan vi �ven dra nytt av : planeManager.gameObject.SetActive(isPlaneToggled);

            foreach (ARPlane plane in planeManager.trackables)
            {
                // plane.trackingState == TrackingState.
                plane.gameObject.SetActive(isPlaneToggled);
            }
        }
    }
}